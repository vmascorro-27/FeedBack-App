using FeedBack_APP.Data;
using FeedBack_APP.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FeedBack_APP.Controllers
{
    [Authorize]
    public class USERSController : Controller
    {
        private const string DefaultPassword = "RTStrc2026";
        private readonly FeedbackDbContext db;
        private readonly PasswordHasher<User> _passwordHasher;

        public USERSController(FeedbackDbContext _db)
        {
            db = _db;
            _passwordHasher = new PasswordHasher<User>();
        }


        public IActionResult Index()
        {
            ViewData["Title"] = "Users";
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult GetUsers()
        {
            var data = db.Users.AsNoTracking().ToList();
            return PartialView("_UsersTable", data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public string GetDataUser(int id_user)
        {
            var data = from user in db.Users
                       where user.IdUser == id_user
                       select new { user.IdUser, user.Username, user.Name, user.IdRol };
            return Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public int SaveUpdateUser(int id_user, string name, string username, int id_rol)
        {
            try
            {
                var valid_username = db.Users.Where(x => x.Username == username).FirstOrDefault();
                if (valid_username != null)
                {
                    if (valid_username.IdUser != id_user)
                    {
                        return 2;
                    }
                }

                User obj_user = new Models.Entities.User();
                if (id_user != 0)
                {
                    obj_user = db.Users.Find(id_user);
                    obj_user.Name = name;
                    obj_user.Username = username;
                    obj_user.IdRol = id_rol;
                    obj_user.Active = 1;
                    db.SaveChanges();
                }
                else
                {
                    obj_user = new User
                    {
                        Name = name,
                        Username = username,
                        IdRol = id_rol,
                        Active = 1,
                        DateCreated = DateTime.Now
                    };
                    string encrypt_pass = _passwordHasher.HashPassword(obj_user, DefaultPassword);
                    obj_user.Pass = encrypt_pass;
                    db.Users.Add(obj_user);
                    db.SaveChanges();
                }

                return 0;
            }
            catch (Exception)
            {
                return 1;
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public int OnOffUser(int id_user, int modo)
        {
            try
            {
                var user = db.Users.Find(id_user); if (user == null) { return 2; }
                user.Active = Convert.ToByte(modo);
                db.SaveChanges();
                return 0;
            }
            catch (Exception)
            {
                return 1;
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public int ResetUserPassword(int id_user)
        {
            try
            {
                var obj_user = db.Users.Find(id_user);
                string encrypt_pass = _passwordHasher.HashPassword(obj_user, DefaultPassword);
                obj_user.Pass = encrypt_pass;
                db.SaveChanges();
                return 0;
            }
            catch (Exception)
            {
                return 1;
            }
        }


    }
}
