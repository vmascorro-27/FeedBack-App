namespace FeedBack_APP.Models.Entities
{
    public class FeedbackFormDResponse
    {
        public int IdFormsDResponse { get; set; }
        public int IdFormD { get; set; }
        public string ResponseText { get; set; } = string.Empty;
        public byte Active { get; set; }
        public virtual FeedbackFormD FeedbackFormD { get; set; } = null!;
    }
}
