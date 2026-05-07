namespace FeedBack_APP.Models.Entities
{
    public class FeedbackResponseG
    {
        public FeedbackResponseG()
        {
            FeedbackResponsesD = new HashSet<FeedbackResponseD>();
        }

        public int IdFeedbackResponseG { get; set; }
        public string Token { get; set; } = string.Empty;
        public string NameForm { get; set; } = string.Empty;
        public DateTime DateSurvey { get; set; }
        public byte RemotieClient { get; set; }
        public string? SummaryAI { get; set; }
        public int? ScoreSurvey { get; set; }
        public byte Active { get; set; }
        public virtual ICollection<FeedbackResponseD> FeedbackResponsesD { get; set; }
    }
}
