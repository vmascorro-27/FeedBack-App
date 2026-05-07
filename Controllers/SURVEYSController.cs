using FeedBack_APP.Data;
using FeedBack_APP.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Data.Entity;
using FeedBack_APP.Models;
using Microsoft.AspNetCore.Authorization;

namespace FeedBack_APP.Controllers
{
    [Authorize]
    public class SURVEYSController : Controller
    {

        private readonly FeedbackDbContext _dbContext;
        public SURVEYSController(FeedbackDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        #region MANAGE FORMS
        public IActionResult Forms(string? msj)
        {
            ViewData["Title"] = "Manage forms";
            ViewBag.msjmsj = msj;
            return View("Forms/Index");
        }

        public IActionResult NewSurvey()
        {
            ViewData["Title"] = "New Survey";
            return View("Forms/NewForm");
        }

        public IActionResult EditSurvey(int id_form_g)
        {
            ViewData["Title"] = "Edit Survey";
            var data = GetInfoForm(id_form_g);
            return View("Forms/EditForm", data);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<PartialViewResult> GetForms()
        {
            var data = await _dbContext.FeedbackFormsG.Include(x => x.FormType).Include(x => x.UserCreated).Include(x => x.UserUpdated).Include(x => x.FeedbackFormsD)
                .Where(x => x.Active == 1).AsNoTracking().ToListAsync();
            return PartialView("Forms/_SurveysTable", data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<PartialViewResult> ShowSurvey(int id_form_g)
        {
            var data = GetInfoForm(id_form_g);
            return PartialView("Forms/_SurveyPreview", data);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public int SaveForm([FromBody] SurveySaveRequest model)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    //VALID ONLY  TYPE FORM PER SURVEY
                    var valid_type_form = _dbContext.FeedbackFormsG.Where(x => x.IdFormType == model.IdTypeForm && x.Active == 1 );
                    if (valid_type_form.Count() > 0)
                    {
                        if (valid_type_form.FirstOrDefault().IdFormG != model.id_form_g ) { return 1; }
                    }

                    // INACTIVE ALL QUESTIONS AND THEIR RESPONSES
                    if (model.id_form_g != null)
                    {
                        _dbContext.FeedbackFormsD.Where(x => x.IdFormG == model.id_form_g).ForEachAsync(x => x.Active = 0);
                        _dbContext.FeedbackFormsDResponses.Include(x => x.FeedbackFormD).Where(x => x.FeedbackFormD.IdFormG == model.id_form_g).ForEachAsync(x => x.Active = 0);
                        _dbContext.SaveChanges();
                    }

                    int id_form_g = 0;
                    if (model.id_form_g != null)  // UPDATE FORM
                    {
                        FeedbackFormG form_g = _dbContext.FeedbackFormsG.Find(model.id_form_g);
                        form_g.NameForm = model.NameForm;
                        form_g.IdFormType = model.IdTypeForm;
                        form_g.Active = 1;
                        form_g.IdUserUpdated = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                        form_g.DateUpdated = DateTime.Now;
                        id_form_g = form_g.IdFormG;
                        _dbContext.SaveChanges();
                    }
                    else  //REGISTER FORM
                    {
                        FeedbackFormG form_g = new FeedbackFormG();
                        form_g.NameForm = model.NameForm;
                        form_g.IdFormType = model.IdTypeForm;
                        form_g.IdUserCreated = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                        form_g.Active = 1;
                        form_g.DateCreated = DateTime.Now;
                        _dbContext.FeedbackFormsG.Add(form_g);
                        _dbContext.SaveChanges();
                        id_form_g = form_g.IdFormG;
                    }
                    

                    for (int i = 0; i < model.Questions.Length; i++)
                    {
                        FeedbackFormD new_question = new FeedbackFormD();
                        new_question.Active = 1;
                        new_question.Question = model.Questions[i];
                        new_question.WrittenOptionable = byte.Parse(model.TypeQuestion[i].ToString());
                        new_question.IdFormG = id_form_g;
                        _dbContext.FeedbackFormsD.Add(new_question);
                        _dbContext.SaveChanges();
                        int id_form_d = new_question.IdFormD;

                        if (new_question.WrittenOptionable == 1)  //MULTIPLE
                        {
                            for (int z = 0; z < model.Responses[i].Length; z++)
                            {
                                FeedbackFormDResponse new_reponse = new FeedbackFormDResponse();
                                new_reponse.IdFormD = id_form_d;
                                new_reponse.ResponseText = model.Responses[i][z];
                                new_reponse.Active = 1;
                                _dbContext.FeedbackFormsDResponses.Add(new_reponse);
                            }
                            _dbContext.SaveChanges();
                        }
                    }
                    
                    _dbContext.SaveChanges();
                    transaction.Commit();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public int OnOffForm(int id_form_g)
        {
            try
            {
                var form = _dbContext.FeedbackFormsG.Find(id_form_g);
                if (form == null) return 1;

                form.Active = 0;
                _dbContext.SaveChanges();

                return 0;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }



        private  SurveyDTO GetInfoForm(int? id_form_g)
        {
            if (id_form_g == null) return null;
            var data = _dbContext.FeedbackFormsG.Where(x => x.IdFormG == id_form_g && x.Active == 1).Include(x => x.FormType).Select(z => new SurveyDTO
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
            })
            .FirstOrDefault();
            return data;
        }

        #endregion

    }
}
