namespace FeedBack_APP.Models.Entities
{
    public class FeedbackFormG
    {
        public FeedbackFormG()
        {
            FeedbackFormsD = new HashSet<FeedbackFormD>();
        }

        public int IdFormG { get; set; }
        public int IdFormType { get; set; }
        public string NameForm { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public int IdUserCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int? IdUserUpdated { get; set; }
        public byte Active { get; set; }
        public virtual CatFormType FormType { get; set; } = null!;
        public virtual User UserCreated { get; set; } = null!;
        public virtual User? UserUpdated { get; set; }
        public virtual ICollection<FeedbackFormD> FeedbackFormsD { get; set; }
    }
}
