namespace FeedBack_APP.Models.Entities
{
    public class FeedbackFormD
    {
        public FeedbackFormD()
        {
            FeedbackFormDResponses = new HashSet<FeedbackFormDResponse>();
        }

        public int IdFormD { get; set; }
        public int IdFormG { get; set; }
        public string Question { get; set; } = string.Empty;
        public byte WrittenOptionable { get; set; }
        public byte Active { get; set; }
        public virtual FeedbackFormG FeedbackFormG { get; set; } = null!;
        public virtual ICollection<FeedbackFormDResponse> FeedbackFormDResponses { get; set; }
    }
}
