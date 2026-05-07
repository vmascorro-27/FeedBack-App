using FeedBack_APP.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;

namespace FeedBack_APP.Controllers
{
    [Authorize]
    public class CATController : Controller
    {
        private readonly FeedbackDbContext _dbContext;

        public CATController(FeedbackDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult GetFormTypesSelect()
        {
            var data =  _dbContext.CatFormTypes.AsNoTracking().ToList();
            return PartialView("../CATALOG/_SelectFormTypes", data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult GetRolesSelect()
        {
            var data = _dbContext.Roles.Where(x => x.Activo == 1).AsNoTracking().ToList();
            return PartialView("../CATALOG/_SelectRolesUsers", data);
        }





    }
}
