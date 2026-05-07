namespace FeedBack_APP.Models.MasterEntities
{
    public class Designation
    {
        public Designation()
        {
            Employees = new HashSet<Employee>();
        }

        public int Id { get; set; }
        public string ZohoId { get; set; } = string.Empty;
        public string? DesignationName { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
