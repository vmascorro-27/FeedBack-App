namespace FeedBack_APP.Models.MasterEntities
{
    public class Employee
    {
        public int Id { get; set; }
        public string ZohoId { get; set; } = string.Empty;
        public string? EmployeeId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public DateTime DateOfJoining { get; set; }
        public DateTime? DateOfExit { get; set; }
        public string EmployeeStatus { get; set; } = string.Empty;
        public int? DepartmentId { get; set; }
        public int? DesignationId { get; set; }
        public string? ManagerName { get; set; }
        public string? ManagerEmail { get; set; }
        public int? CustomerId { get; set; }
        public decimal? Rate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public virtual Customer? Customer { get; set; }
        public virtual Department? Department { get; set; }
        public virtual Designation? Designation { get; set; }
    }
}
