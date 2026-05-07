namespace FeedBack_APP.Models.Entities
{
    public class User
    {
        public User()
        {
            FeedbackFormsCreated = new HashSet<FeedbackFormG>();
            FeedbackFormsUpdated = new HashSet<FeedbackFormG>();
        }

        public int IdUser { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Pass { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public int IdRol { get; set; }
        public byte Active { get; set; }
        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<FeedbackFormG> FeedbackFormsCreated { get; set; }
        public virtual ICollection<FeedbackFormG> FeedbackFormsUpdated { get; set; }
    }
}
