using FeedBack_APP.Models.Entities;

namespace FeedBack_APP.Models
{
    public class SurveyDTO
    {
        public int IdFormG { get; set; }
        public string FormName { get; set; }
        public int  IdFormType { get; set; }
        public string FormTypeName { get; set; }
        public string FormTypeDescription { get; set; }
        public DateTime FormDateCreateed { get; set; }
        public string FormUserCreated { get; set; }
        public DateTime FormDateUpdated { get; set; }
        public string FormUserUpdated { get; set; }


        public List<QuestionDTO> Questions { get; set; }
    }
}
