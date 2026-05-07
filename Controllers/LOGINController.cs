using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FeedBack_APP.Data;
using FeedBack_APP.Models;
using FeedBack_APP.Models.Entities;
using FeedBack_APP.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FeedBack_APP.Controllers
{
    public class LOGINController : Controller
    {
        private const string DefaultPassword = "RTStrc2026";
        private const string PendingLoginUserIdKey = "PendingLogin.UserId";
        private const string PendingLoginReturnUrlKey = "PendingLogin.ReturnUrl";
        private const string PendingLoginQrEmailSentKey = "PendingLogin.QrEmailSent";
        private const string PendingLoginShouldSendQrEmailKey = "PendingLogin.ShouldSendQrEmail";

        private readonly FeedbackDbContext _dbContext;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;

        public LOGINController(FeedbackDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _passwordHasher = new PasswordHasher<User>();
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Home", "DASHBOARD");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _dbContext.Users.AsNoTracking().SingleOrDefaultAsync(item => item.Username == model.Username && item.Active == 1);

            if (user is null || !PasswordMatches(user, model.Password))
            {
                ModelState.AddModelError(string.Empty, "Incorrect username or password.");
                return View(model);
            }

            SetPendingLogin(user, returnUrl);

            if (PasswordMatches(user, DefaultPassword))
            {
                return RedirectToAction(nameof(ChangeDefaultPassword));
            }

            return RedirectToAction(nameof(TwoFactor));
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ChangeDefaultPassword()
        {
            var user = await GetPendingLoginUserAsync();
            if (user is null)
            {
                ClearPendingLogin();
                return RedirectToAction(nameof(Index));
            }

            if (!PasswordMatches(user, DefaultPassword))
            {
                return RedirectToAction(nameof(TwoFactor));
            }

            return View(new ChangeDefaultPasswordViewModel
            {
                Email = user.Username
            });
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeDefaultPassword(ChangeDefaultPasswordViewModel model)
        {
            var user = await GetPendingLoginUserAsync();
            if (user is null)
            {
                ClearPendingLogin();
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                model.Email = user.Username;
                return View(model);
            }

            if (!PasswordMatches(user, DefaultPassword))
            {
                return RedirectToAction(nameof(TwoFactor));
            }

            if (string.Equals(model.NewPassword, DefaultPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError(nameof(model.NewPassword), "The new password cannot be the default password.");
                model.Email = user.Username;
                return View(model);
            }

            user.Pass = _passwordHasher.HashPassword(user, model.NewPassword);
            await _dbContext.SaveChangesAsync();

            HttpContext.Session.SetString(PendingLoginShouldSendQrEmailKey, "1");
            HttpContext.Session.Remove(PendingLoginQrEmailSentKey);

            return RedirectToAction(nameof(TwoFactor));
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> TwoFactor()
        {
            var user = await GetPendingLoginUserAsync(asNoTracking: true);
            if (user is null)
            {
                ClearPendingLogin();
                return RedirectToAction(nameof(Index));
            }

            if (PasswordMatches(user, DefaultPassword))
            {
                return RedirectToAction(nameof(ChangeDefaultPassword));
            }

            var viewModel = BuildTwoFactorViewModel(user);

            var shouldSendQrEmail = string.Equals(HttpContext.Session.GetString(PendingLoginShouldSendQrEmailKey), "1", StringComparison.Ordinal);
            if (shouldSendQrEmail && !string.Equals(HttpContext.Session.GetString(PendingLoginQrEmailSentKey), "1", StringComparison.Ordinal))
            {
                await TrySendTotpQrEmailAsync(user, viewModel);
                HttpContext.Session.SetString(PendingLoginQrEmailSentKey, "1");
                HttpContext.Session.Remove(PendingLoginShouldSendQrEmailKey);
            }

            return View(viewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TwoFactor(TwoFactorViewModel model)
        {
            var user = await GetPendingLoginUserAsync(asNoTracking: true);
            if (user is null)
            {
                ClearPendingLogin();
                return RedirectToAction(nameof(Index));
            }

            if (PasswordMatches(user, DefaultPassword))
            {
                return RedirectToAction(nameof(ChangeDefaultPassword));
            }

            if (!ModelState.IsValid)
            {
                return View(BuildTwoFactorViewModel(user, model));
            }

            if (!VerifyTotpCode(user, model.Code))
            {
                ModelState.AddModelError(nameof(model.Code), "The authentication code is not valid.");
                return View(BuildTwoFactorViewModel(user, model));
            }

            await SignInUserAsync(user);
            var pendingReturnUrl = HttpContext.Session.GetString(PendingLoginReturnUrlKey);
            ClearPendingLogin();

            if (!string.IsNullOrWhiteSpace(pendingReturnUrl) && Url.IsLocalUrl(pendingReturnUrl))
            {
                return Redirect(pendingReturnUrl);
            }

            return RedirectToAction("Home", "DASHBOARD");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            ClearPendingLogin();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private bool PasswordMatches(User user, string plainPassword)
        {
            var verification = _passwordHasher.VerifyHashedPassword(user, user.Pass, plainPassword);
            return verification != PasswordVerificationResult.Failed;
        }

        private void SetPendingLogin(User user, string? returnUrl)
        {
            HttpContext.Session.SetInt32(PendingLoginUserIdKey, user.IdUser);

            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                HttpContext.Session.Remove(PendingLoginReturnUrlKey);
            }
            else
            {
                HttpContext.Session.SetString(PendingLoginReturnUrlKey, returnUrl);
            }

            HttpContext.Session.Remove(PendingLoginQrEmailSentKey);
            HttpContext.Session.Remove(PendingLoginShouldSendQrEmailKey);
        }

        private void ClearPendingLogin()
        {
            HttpContext.Session.Remove(PendingLoginUserIdKey);
            HttpContext.Session.Remove(PendingLoginReturnUrlKey);
            HttpContext.Session.Remove(PendingLoginQrEmailSentKey);
        }

        private async Task<User?> GetPendingLoginUserAsync(bool asNoTracking = false)
        {
            var userId = HttpContext.Session.GetInt32(PendingLoginUserIdKey);
            if (!userId.HasValue)
            {
                return null;
            }

            var query = _dbContext.Users.Where(item => item.IdUser == userId.Value && item.Active == 1);
            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.SingleOrDefaultAsync();
        }

        private async Task SignInUserAsync(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.IdUser.ToString()),
                new(ClaimTypes.Name, user.Name)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = false,
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                });
        }

        private TwoFactorViewModel BuildTwoFactorViewModel(User user, TwoFactorViewModel? model = null)
        {
            var provisioningUri = BuildProvisioningUri(user);
            var secret = GetTotpSecret(user);

            model ??= new TwoFactorViewModel();
            model.Email = user.Username;
            model.MaskedEmail = MaskEmail(user.Username);
            model.ManualEntryKey = SplitSecret(secret);
            model.QrCodeImageUrl = BuildQrCodeUrl(provisioningUri);
            model.Issuer = GetTotpIssuer();
            return model;
        }

        private async Task<bool> TrySendTotpQrEmailAsync(User user, TwoFactorViewModel model)
        {
            var smtpHost = _configuration["Smtp:Host"];
            var fromEmail = _configuration["Smtp:FromEmail"];
            if (string.IsNullOrWhiteSpace(smtpHost) || string.IsNullOrWhiteSpace(fromEmail) || string.IsNullOrWhiteSpace(user.Username))
            {
                return false;
            }

            var smtpPort = int.TryParse(_configuration["Smtp:Port"], out var parsedPort) ? parsedPort : 587;
            var enableSsl = !bool.TryParse(_configuration["Smtp:EnableSsl"], out var parsedSsl) || parsedSsl;
            var smtpUsername = _configuration["Smtp:Username"];
            var smtpPassword = _configuration["Smtp:Password"] ?? string.Empty;
            var fromName = _configuration["Smtp:FromName"] ?? model.Issuer;

            try
            {
                using var message = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = $"{model.Issuer} - Two-step authentication setup",
                    IsBodyHtml = true,
                    Body = BuildTotpEmailBody(user, model)
                };

                message.To.Add(new MailAddress(user.Username));

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    EnableSsl = enableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                if (!string.IsNullOrWhiteSpace(smtpUsername))
                {
                    client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                }

                await client.SendMailAsync(message);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string BuildTotpEmailBody(User user, TwoFactorViewModel model)
        {
            var encodedName = WebUtility.HtmlEncode(user.Name);
            var encodedQrCodeUrl = WebUtility.HtmlEncode(model.QrCodeImageUrl);
            var encodedManualEntryKey = WebUtility.HtmlEncode(model.ManualEntryKey);

            return $"""
                <div style="font-family: Arial, sans-serif; color: #1f2937; line-height: 1.5;">
                    <h2 style="margin-bottom: 12px;">2FA Authentication</h2>
                    <p>Hello {encodedName}, scan this QR code in your authenticator app to generate your TOTP code.</p>
                    <p style="margin: 20px 0;">
                        <img src="{encodedQrCodeUrl}" alt="QR TOTP" style="max-width: 260px; width: 100%; height: auto;" />
                    </p>
                    
                </div>
                """;

            //<p>If you prefer to enter it manually, use this key:</p>
            //        <p style="font-size: 18px; font-weight: bold; letter-spacing: 2px;">{encodedManualEntryKey}</p>
            //        <p>Once the app is set up, enter the 6-digit code on the login screen.</p>
        }

        private string GetTotpIssuer()
        {
            return _configuration["TwoFactor:Issuer"] ?? "FeedBack APP";
        }

        private string BuildProvisioningUri(User user)
        {
            var issuer = GetTotpIssuer();
            var secret = GetTotpSecret(user);
            var label = $"{issuer}:{user.Username}";

            return $"otpauth://totp/{Uri.EscapeDataString(label)}?secret={secret}&issuer={Uri.EscapeDataString(issuer)}&digits=6&period=30";
        }

        private string BuildQrCodeUrl(string provisioningUri)
        {
            return $"https://quickchart.io/qr?text={Uri.EscapeDataString(provisioningUri)}&size=280";
        }

        private string GetTotpSecret(User user)
        {
            return Base32Encode(GetTotpSecretBytes(user));
        }

        private byte[] GetTotpSecretBytes(User user)
        {
            var seed = $"{user.IdUser}|{user.Username}|{user.Pass}";
            return SHA256.HashData(Encoding.UTF8.GetBytes(seed))[..20];
        }

        private bool VerifyTotpCode(User user, string code)
        {
            var normalizedCode = NormalizeTotpCode(code);
            if (normalizedCode.Length != 6)
            {
                return false;
            }

            var secretBytes = GetTotpSecretBytes(user);
            var currentUtc = DateTime.UtcNow;

            for (var offset = -1; offset <= 1; offset++)
            {
                var candidate = GenerateTotpCode(secretBytes, currentUtc.AddSeconds(offset * 30));
                if (FixedTimeEquals(normalizedCode, candidate))
                {
                    return true;
                }
            }

            return false;
        }

        private static string NormalizeTotpCode(string code)
        {
            return new string(code.Where(char.IsDigit).ToArray());
        }

        private static string GenerateTotpCode(byte[] secret, DateTime timestampUtc)
        {
            var counter = (long)Math.Floor((timestampUtc - DateTime.UnixEpoch).TotalSeconds / 30d);
            var counterBytes = BitConverter.GetBytes(counter);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(counterBytes);
            }

            using var hmac = new HMACSHA1(secret);
            var hash = hmac.ComputeHash(counterBytes);
            var offset = hash[^1] & 0x0F;
            var binaryCode =
                ((hash[offset] & 0x7F) << 24) |
                ((hash[offset + 1] & 0xFF) << 16) |
                ((hash[offset + 2] & 0xFF) << 8) |
                (hash[offset + 3] & 0xFF);

            var otp = binaryCode % 1_000_000;
            return otp.ToString("D6", CultureInfo.InvariantCulture);
        }

        private static bool FixedTimeEquals(string left, string right)
        {
            var leftBytes = Encoding.UTF8.GetBytes(left);
            var rightBytes = Encoding.UTF8.GetBytes(right);

            return leftBytes.Length == rightBytes.Length &&
                   CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
        }

        private static string Base32Encode(byte[] data)
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            var output = new StringBuilder((int)Math.Ceiling(data.Length / 5d) * 8);
            var bitBuffer = 0;
            var bitsLeft = 0;

            foreach (var value in data)
            {
                bitBuffer = (bitBuffer << 8) | value;
                bitsLeft += 8;

                while (bitsLeft >= 5)
                {
                    var index = (bitBuffer >> (bitsLeft - 5)) & 31;
                    bitsLeft -= 5;
                    output.Append(alphabet[index]);
                }
            }

            if (bitsLeft > 0)
            {
                var index = (bitBuffer << (5 - bitsLeft)) & 31;
                output.Append(alphabet[index]);
            }

            return output.ToString();
        }

        private static string SplitSecret(string secret)
        {
            var groups = Enumerable.Range(0, (int)Math.Ceiling(secret.Length / 4d))
                .Select(index => secret.Substring(index * 4, Math.Min(4, secret.Length - (index * 4))));

            return string.Join(" ", groups);
        }

        private static string MaskEmail(string email)
        {
            var separatorIndex = email.IndexOf('@');
            if (separatorIndex <= 1)
            {
                return email;
            }

            var localPart = email[..separatorIndex];
            var domain = email[separatorIndex..];
            var visibleStart = localPart[..1];
            var visibleEnd = localPart.Length > 2 ? localPart[^1].ToString() : string.Empty;
            var hiddenLength = Math.Max(localPart.Length - visibleStart.Length - visibleEnd.Length, 1);

            return $"{visibleStart}{new string('*', hiddenLength)}{visibleEnd}{domain}";
        }


        public string EncPass(string pass)
        {
            var user = _dbContext.Users.Find(1);
            return _passwordHasher.HashPassword(user, pass);



        }





    }
}
