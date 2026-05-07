namespace FeedBack_APP.Models.Entities
{
    public class FeedbackResponseD
    {
        public int IdFeedbackResponseD { get; set; }
        public int IdFeedbackResponseG { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public byte Active { get; set; }
        public virtual FeedbackResponseG FeedbackResponseG { get; set; } = null!;
    }
}
