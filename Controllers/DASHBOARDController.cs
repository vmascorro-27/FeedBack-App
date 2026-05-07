using FeedBack_APP.Data;
using FeedBack_APP.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1;
using System.Configuration;
using System.Data;
using System.Data.Entity;

namespace FeedBack_APP.Controllers
{
    [Authorize]
    public class DASHBOARDController : Controller
    {
        private readonly FeedbackDbContext _dbFeedBack;
        private readonly IConfiguration _configuration;
        public DASHBOARDController(FeedbackDbContext db, IConfiguration configuration)
        {
            _dbFeedBack = db;
            _configuration = configuration;
        }


        public IActionResult Home()
        {
            ViewData["Title"] = "Home";
            return View("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public string GetDASHBOARD_QuestionsRate()
        {
            try
            {
                List<DASH_QuestonsRate> data = new List<DASH_QuestonsRate>();


                using (MySqlConnection conn = new MySqlConnection(_configuration["ConnectionStrings:MySqlConnectionFeedBack"] ?? string.Empty ))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand("DASHBOARD_QuestionsRate", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                data.Add(new DASH_QuestonsRate
                                {
                                    Question = reader["question"].ToString(),
                                    Rate_1 = Convert.ToInt32(reader["rate_1"]),
                                    Rate_2 = Convert.ToInt32(reader["rate_2"]),
                                    Rate_3 = Convert.ToInt32(reader["rate_3"]),
                                    Rate_4 = Convert.ToInt32(reader["rate_4"]),
                                    Rate_5 = Convert.ToInt32(reader["rate_5"])
                                });
                            }
                        }
                    }
                    return Newtonsoft.Json.JsonConvert.SerializeObject(data);
                }
            }
            catch (Exception ex)
            {
                string msj = ex.ToString();
                return "";
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public string GetDASHBOARD_RateCompany()
        {
            var result = _dbFeedBack.Database.SqlQuery<decimal?>("SELECT AVG(score_survey) FROM rts_feedback_db.feedback_responses_g").FirstOrDefault();
            string promedio = result?.ToString("N2") ?? "0";
            return promedio;
        }



    }
}
