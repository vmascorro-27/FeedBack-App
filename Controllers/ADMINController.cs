using FeedBack_APP.Data;
using FeedBack_APP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace FeedBack_APP.Controllers
{
    [Authorize]
    public class ADMINController : Controller
    {
        private readonly FeedbackDbContext db;

        public ADMINController(FeedbackDbContext _db){
            db = _db;
        }


        public IActionResult ManageFeedBack()
        {
            ViewData["Title"] = "Admin Feedback";
            return View("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult GetFeedBacks(string date_start, string date_end /*, int[] client_remoties*/)
        {
            //string cli_rem = string.Join(",", client_remoties);

            date_end += " 23:59";
            var data = db.Database.SqlQuery<SurveysGeneralInfo>(@"SELECT 
                resp_g.id_feedback_response_g AS IdFeedbackResponseG,
                resp_g.token AS Token,
                resp_g.name_form AS NameForm,
                resp_g.remotie_client AS RemotieClient,
                resp_g.active AS Active,
                resp_g.date_survey AS DateSurvey,
                pend.remotie_submitted,
                pend.client_submitted,
                emp.id AS IdEmployee,
                emp.First_Name AS FirstName,
                emp.Last_Name AS LastName,
                emp.Email AS Email,
                depa.department_name AS DepartmentName,
                puesto.designation_name AS DesignationName,
                cli.customer_name AS CustomerName FROM rts_feedback_db.feedback_responses_g resp_g
                JOIN rts_feedback_db.pending_feedback pend ON resp_g.token = pend.token
                JOIN rts_master_entities_db.employees emp ON pend.remotie_id = emp.id
                JOIN rts_master_entities_db.departments depa ON emp.department_id = depa.id
                JOIN rts_master_entities_db.designation puesto ON emp.designation_id = puesto.id
                JOIN rts_master_entities_db.customer cli ON emp.Client = cli.customer_pk_id
                WHERE resp_g.active = 1 AND resp_g.date_survey >= @date_start AND resp_g.date_survey <= @date_end 
                ;",
                new MySqlParameter("@date_start", date_start),
                new MySqlParameter("@date_end", date_end)
                ).ToList();

            return PartialView("_FeedBack_g_table", data);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult GetSurveyDetail(int id_feedback_response_g)
        {
            var data = db.Database.SqlQuery<SurveysDetailInfo>(@"SELECT 
                resp_g.id_feedback_response_g AS IdFeedbackResponseG,
                resp_g.token AS Token,
                resp_g.name_form AS NameForm,
                resp_g.remotie_client AS RemotieClient,
                resp_g.active AS Active,
                resp_g.date_survey AS DateSurvey,
                resp_g.score_survey as ScoreSurvey,
                resp_g.summary_AI as Summary_AI,
                resp_d.id_feedback_response_d AS IdFeedbackResponseD,
                resp_d.question AS QuestionSurvey,
                resp_d.response AS ResponseSurvey,
                emp.id AS IdEmployee,
                emp.First_Name AS FirstName,
                emp.Last_Name AS LastName,
                emp.Email AS Email,
                depa.department_name AS DepartmentName,
                puesto.designation_name AS DesignationName,
                cli.customer_name AS CustomerName FROM rts_feedback_db.feedback_responses_g resp_g
                JOIN rts_feedback_db.feedback_responses_d resp_d ON resp_g.id_feedback_response_g = resp_d.id_feedback_response_g
                JOIN rts_feedback_db.pending_feedback pend ON resp_g.token = pend.token
                JOIN rts_master_entities_db.employees emp ON pend.remotie_id = emp.id
                JOIN rts_master_entities_db.departments depa ON emp.department_id = depa.id
                JOIN rts_master_entities_db.designation puesto ON emp.designation_id = puesto.id
                JOIN rts_master_entities_db.customer cli ON emp.Client = cli.customer_pk_id
                WHERE resp_d.active = 1 AND resp_g.id_feedback_response_g = @id_feedback_response_g", new MySqlParameter("@id_feedback_response_g", id_feedback_response_g)).ToList();
            return PartialView("_FeedBack_d", data);
        }







    }
}
