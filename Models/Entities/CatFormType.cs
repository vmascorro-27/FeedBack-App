namespace FeedBack_APP.Models.Entities
{
    public class CatFormType
    {
        public CatFormType()
        {
            FeedbackFormsG = new HashSet<FeedbackFormG>();
        }

        public int Id { get; set; }
        public string FormType { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public byte ClientRemotie { get; set; }
        public int? IdPeriod { get; set; }
        public virtual CatPeriod? Period { get; set; }
        public virtual ICollection<FeedbackFormG> FeedbackFormsG { get; set; }
    }
}
