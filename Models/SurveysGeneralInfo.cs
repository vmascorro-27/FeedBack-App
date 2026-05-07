namespace FeedBack_APP.Models
{
    public class SurveysGeneralInfo
    {
        public int IdFeedbackResponseG { get; set; }
        public string Token { get; set; }
        public string NameForm { get; set; }
        public int RemotieClient { get; set; }
        public byte Active { get; set; }
        public DateTime DateSurvey { get; set; }
        public int remotie_submitted { get; set; }
        public int client_submitted { get; set; }
        public int IdEmployee { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public string CustomerName { get; set; }
    }
}
