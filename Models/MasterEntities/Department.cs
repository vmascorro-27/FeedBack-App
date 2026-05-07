namespace FeedBack_APP.Models.MasterEntities
{
    public class Department
    {
        public Department()
        {
            Employees = new HashSet<Employee>();
        }

        public int Id { get; set; }
        public string? ZohoId { get; set; }
        public string? DepartmentName { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
