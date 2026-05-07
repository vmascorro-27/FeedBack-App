namespace FeedBack_APP.Models
{
    public class SurveySaveRequest
    {
        public int? id_form_g { get; set; }
        public string NameForm { get; set; }
        public int IdTypeForm { get; set; }
        public string[] Questions { get; set; }
        public int[] TypeQuestion { get; set; }
        public string[][] Responses { get; set; }
    }
}
