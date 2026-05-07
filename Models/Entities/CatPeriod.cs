namespace FeedBack_APP.Models.Entities
{
    public class CatPeriod
    {
        public CatPeriod()
        {
            CatFormTypes = new HashSet<CatFormType>();
        }

        public int Id { get; set; }
        public string Period { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public virtual ICollection<CatFormType> CatFormTypes { get; set; }
    }
}
