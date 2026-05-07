using FeedBack_APP.Data;
using FeedBack_APP.Models;
using FeedBack_APP.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using System.Text;
using System.Text.Json;
using System.Transactions;

namespace FeedBack_APP.Controllers
{
    public class FEEDBACKController : Controller
    {
        private readonly FeedbackDbContext _dbContext;
        private readonly MasterDbContext _dbMaster;

        public FEEDBACKController(FeedbackDbContext dbContext, MasterDbContext dbMaster)
        {
            _dbContext = dbContext;
            _dbMaster = dbMaster;
        }


        //0: REMOTIE    |   1: CLIENT
        public IActionResult Survey(string token, int? remotie_client)
        {
            try
            {
                //------- VALID TOKEN
                PendingFeedback pending_feedback = null;
                pending_feedback = _dbContext.PendingFeedback.Where(x => x.Token == token).FirstOrDefault();
                if (pending_feedback == null)
                {
                    ViewBag.error_code = "Invalid token";
                    return View("Survey/ErrorRequestSurvey");
                }

                //------- VALID IF SUVEY IS FOR REMOTIR OR CLIENT
                if (remotie_client == 0 && pending_feedback.RemotieSubmitted == 1) { return View("AnsweredViews/AnsweredSurveyClient", null); }
                else if (remotie_client == 1 && pending_feedback.ClientSubmitted == 1) { return View("AnsweredViews/AnsweredSurveyRemotie", null); }
                else if (remotie_client != 0 && remotie_client != 1)
                {
                    ViewBag.error_code = "Recipient not found";
                    return View("Survey/ErrorRequestSurvey");
                }


                if (pending_feedback == null)
                {
                    ViewBag.error_code = "Invalid token";
                    return View("Survey/ErrorRequestSurvey");
                }

                int id_period = pending_feedback.PeriodId;
                var type_form = _dbContext.CatFormTypes.Include(x => x.Period).FirstOrDefault(x => x.IdPeriod == id_period && x.ClientRemotie == remotie_client);
                if (type_form == null)
                {
                    ViewBag.error_code = "No type form found";
                    return View("Survey/ErrorRequestSurvey");
                }

                var survey = GetInfoForm(type_form.Id);

                if (survey == null) {
                    ViewBag.error_code = "No surveys with this type of form were found.";
                    return View("Survey/ErrorRequestSurvey");
                }

                var empleado = _dbMaster.Employees.Include(x => x.Customer).FirstOrDefault(x => x.Id == pending_feedback.RemotieId);
                ViewBag.name_remotie = $"{empleado.FirstName} {empleado.LastName}";
                ViewBag.client = empleado.Customer.CustomerName;
                ViewBag.remotie_client = remotie_client;
                ViewBag.periodo = type_form.Period.Period;
                ViewBag.id_feedback = pending_feedback.Id;
                return View("Survey/SurveyForm", survey);
            }
            catch (Exception)
            {
                ViewBag.error_code = "There was an error in the request";
                return View("Survey/ErrorRequestSurvey");
            }


        }


        private SurveyDTO GetInfoForm(int id_type_form)
        {
            var survey = _dbContext.FeedbackFormsG.Where(x => x.IdFormType == id_type_form && x.Active == 1).Include(x => x.FormType).Select(z => new SurveyDTO
            {
                IdFormG = z.IdFormG,
                FormName = z.NameForm,
                IdFormType = z.IdFormType,
                FormTypeName = z.FormType.FormType ?? "",
                FormTypeDescription = z.FormType.Description ?? "",
                FormDateCreateed = z.DateCreated,
                FormUserCreated = z.UserCreated.Name ?? "",
                FormDateUpdated = z.DateUpdated ?? DateTime.MinValue,
                FormUserUpdated = z.UserUpdated.Name,
                Questions = z.FeedbackFormsD.Where(x => x.Active == 1).Select(q => new QuestionDTO
                {
                    IdQuestionD = q.IdFormD,
                    IdQuestionG = q.IdFormG,
                    Question = q.Question,
                    Written_Optionable = q.WrittenOptionable,
                    Responses = q.FeedbackFormDResponses.Where(x => x.Active == 1).Select(r => new ResponseDTO
                    {
                        IdResponse = r.IdFormsDResponse,
                        Response = r.ResponseText
                    }).ToList()
                }).ToList()
            }).FirstOrDefault();
            return survey;
        }


        [HttpPost]
        public async Task<int> AnswerSurvey(string token, int id_feedback, string[] questions, string[] responses, int? remotie_client, int score_survey)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    if (responses.Length != questions.Length)
                    {
                        return 1;
                    }
                    PendingFeedback pending_feedback = null;
                    if (remotie_client == 1) { pending_feedback = _dbContext.PendingFeedback.Where(x => x.Token == token && x.ClientSubmitted == 0).FirstOrDefault(); }
                    else if (remotie_client == 0) { pending_feedback = _dbContext.PendingFeedback.Where(x => x.Token == token && x.RemotieSubmitted == 0).FirstOrDefault(); }
                    else {
                        return 2;
                    }

                    if (pending_feedback != null)
                    {
                        if (id_feedback != pending_feedback.Id)
                        {
                            return 3;
                        }
                    }

                    if (remotie_client == 1)
                    {
                        pending_feedback.ClientSubmitted = 1;
                    }
                    else if (remotie_client == 0)
                    {
                        pending_feedback.RemotieSubmitted = 1;
                    }

                    int id_period = pending_feedback.PeriodId;
                    var type_form = _dbContext.CatFormTypes.Where(x => x.IdPeriod == id_period && x.ClientRemotie == remotie_client).FirstOrDefault();
                    if (type_form == null) return 4;

                    FeedbackResponseG response_g = new FeedbackResponseG();
                    response_g.Token = token;
                    response_g.NameForm = type_form.FormType;
                    response_g.RemotieClient = Convert.ToByte(remotie_client);
                    response_g.Active = 1;
                    response_g.DateSurvey = DateTime.Now;
                    response_g.ScoreSurvey = score_survey;
                    _dbContext.FeedbackResponsesG.Add(response_g);
                    _dbContext.SaveChanges();
                    int id_response_g = response_g.IdFeedbackResponseG;

                    for (int i = 0; i < questions.Length; i++)
                    {
                        FeedbackResponseD response_d = new FeedbackResponseD();
                        response_d.IdFeedbackResponseG = id_response_g;
                        response_d.Question = questions[i];
                        response_d.Response = (responses[i] == null ? "" : responses[i]);
                        response_d.Active = 1;
                        _dbContext.FeedbackResponsesD.Add(response_d);
                    }



                    var empleado = _dbMaster.Employees.Include(x => x.Customer).Include(x => x.Department).FirstOrDefault(x => x.EmployeeId == pending_feedback.RemotieId.ToString());
                    var webhookUrl = "https://ai.remoteteamsolutions.com/webhook/feedback-ai";
                    var Respuestas = new Dictionary<string, string>();
                    for (int i = 0; i < questions.Length; i++) { Respuestas.Add(questions[i], responses[i]); }

                    var payload = new
                    {
                        token = token,
                        name = $"{empleado.FirstName} {empleado.LastName}",
                        email = empleado.Email,
                        manager = empleado.ManagerName,
                        manager_email = empleado.ManagerEmail,
                        client = empleado.Customer.CustomerName == null ? "" : empleado.Customer.CustomerName,
                        departament = empleado.Department.DepartmentName,
                        name_form = type_form.Description,
                        date_survey = DateTime.Now.ToString("MM/dd/yyyy HH:mm tt"),
                        Respuestas = Respuestas
                    };

                    using var client = new HttpClient();
                    var json = JsonSerializer.Serialize(payload);
                    using var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(webhookUrl, content);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var result = JsonSerializer.Deserialize<SurveySumaryAI>(responseBody);
                        string summary = result?.summary_AI;
                        response_g.SummaryAI = summary;

                        _dbContext.SaveChanges();
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                        return -1;
                    }

                }
                catch (Exception e)
                {
                    string msj = e.ToString();
                    transaction.Rollback();
                    return -1;
                }
            }

            return 0;
        }



        public bool SYNC_RESPONSES()
        {
            try
            {
                JsonElement root;
                var type_forms = _dbContext.CatFormTypes.AsNoTracking().ToList();
                var responses = _dbContext.FeedbackResponses.ToList();
                foreach (var resp in responses)
                {
                    using (var transaction = _dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            var info_form = type_forms.Where(x => x.Id == resp.FormTypeId).FirstOrDefault();
                            string name_form = info_form.FormType;
                            string token = resp.Token;
                            byte remotie_client = info_form.ClientRemotie;
                            DateTime date_survey = resp.SubmittedAt;
                            List<string> questions_data = new List<string>();
                            List<string> responses_data = new List<string>();

                            var json = resp.Data;
                            var doc = JsonDocument.Parse(json);
                            root = doc.RootElement;
                            foreach (var prop in root.EnumerateObject())
                            {
                                try
                                {
                                    if (prop.Name == "token" || prop.Name == "formMode" || prop.Name == "submittedAt" || prop.Name == "formQueryParameters" ||
                                        prop.Name == "score" || prop.Name == "description")
                                    {
                                        continue;
                                    }
                                    questions_data.Add(prop.Name);
                                    responses_data.Add(prop.Value.GetString());
                                }
                                catch (Exception)
                                {
                                    string error = resp.Id.ToString();
                                }
                            }
                            //AQUI HAGO EL INSERT CON EL HEADER Y LAS PREGUNTAS Y RESPUESTAS

                            if (questions_data.Count != responses_data.Count())
                            {
                                string errror = resp.Token;
                            }
                            else
                            {
                                FeedbackResponseG resp_g = new FeedbackResponseG();
                                resp_g.Token = token;
                                resp_g.NameForm = name_form;
                                resp_g.RemotieClient = remotie_client;
                                resp_g.Active = 1;
                                resp_g.DateSurvey = date_survey;
                                resp_g.SummaryAI = "N/A";
                                resp_g.ScoreSurvey = 0;
                                _dbContext.FeedbackResponsesG.Add(resp_g);
                                _dbContext.SaveChanges();
                                int id_response_g = resp_g.IdFeedbackResponseG;
                                for (int i = 0; i < questions_data.Count; i++)
                                {
                                    FeedbackResponseD resp_d = new FeedbackResponseD();
                                    resp_d.IdFeedbackResponseG = id_response_g;
                                    resp_d.Question = questions_data[i];
                                    resp_d.Response = responses_data[i];
                                    resp_d.Active = 1;
                                    _dbContext.FeedbackResponsesD.Add(resp_d);
                                }
                                _dbContext.SaveChanges();
                                transaction.Commit();
                            }
                        }
                        catch (Exception)
                        {
                            string error = resp.Id.ToString();
                            transaction.Rollback();
                        }
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                string msj = ex.ToString();
                return false;
            }
        }



    }
}
