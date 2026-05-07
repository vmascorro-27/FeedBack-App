namespace FeedBack_APP.Models.Entities
{
    public class FeedbackResponse
    {
        public int Id { get; set; }
        public int FormTypeId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public string Data { get; set; } = string.Empty;
    }
}
