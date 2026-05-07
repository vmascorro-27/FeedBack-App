namespace FeedBack_APP.Models.Entities
{
    public class PendingFeedback
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int RemotieId { get; set; }
        public int PeriodId { get; set; }
        public byte ClientSubmitted { get; set; }
        public byte RemotieSubmitted { get; set; }
        public string Token { get; set; } = string.Empty;
        public int Attempt { get; set; }
    }
}
