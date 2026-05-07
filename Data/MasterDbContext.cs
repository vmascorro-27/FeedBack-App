using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using FeedBack_APP.Models.MasterEntities;
using MySql.Data.EntityFramework;

namespace FeedBack_APP.Data
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class MasterDbContext : DbContext
    {
        public MasterDbContext(string connectionString)
            : base(connectionString)
        {
        }

        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<Designation> Designations { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>().ToTable("customer");
            modelBuilder.Entity<Customer>().HasKey(item => item.CustomerPkId);
            modelBuilder.Entity<Customer>().Property(item => item.CustomerPkId).HasColumnName("customer_pk_id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Customer>().Property(item => item.CustomerId).HasColumnName("customer_id").IsRequired().HasMaxLength(64);
            modelBuilder.Entity<Customer>().Property(item => item.ZohoPeopleId).HasColumnName("zoho_people_id").IsOptional().HasMaxLength(50);
            modelBuilder.Entity<Customer>().Property(item => item.CustomerName).HasColumnName("customer_name").IsRequired().HasMaxLength(155);
            modelBuilder.Entity<Customer>().Property(item => item.Status).HasColumnName("status").IsRequired().HasMaxLength(10);
            modelBuilder.Entity<Customer>().Property(item => item.ActiveRecurringInvoice).HasColumnName("active_recurring_invoice").IsOptional().HasMaxLength(5);
            modelBuilder.Entity<Customer>().Property(item => item.BillingType).HasColumnName("billing_type").IsOptional().HasMaxLength(20);
            modelBuilder.Entity<Customer>().Property(item => item.BillingPeriodStartDay).HasColumnName("billing_period_start_day").IsOptional();
            modelBuilder.Entity<Customer>().Property(item => item.BillingPeriodEndDay).HasColumnName("billing_period_end_day").IsOptional();
            modelBuilder.Entity<Customer>().Property(item => item.InvoiceSendDay).HasColumnName("invoice_send_day").IsOptional();
            modelBuilder.Entity<Customer>().Property(item => item.BiweeklyFactor).HasColumnName("biweekly_factor").IsRequired();
            modelBuilder.Entity<Customer>().Property(item => item.PaymentTerms).HasColumnName("payment_terms").IsOptional();

            modelBuilder.Entity<Department>().ToTable("departments");
            modelBuilder.Entity<Department>().HasKey(item => item.Id);
            modelBuilder.Entity<Department>().Property(item => item.Id).HasColumnName("id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Department>().Property(item => item.ZohoId).HasColumnName("zoho_id").IsOptional().HasMaxLength(25);
            modelBuilder.Entity<Department>().Property(item => item.DepartmentName).HasColumnName("department_name").IsOptional().HasMaxLength(75);

            modelBuilder.Entity<Designation>().ToTable("designation");
            modelBuilder.Entity<Designation>().HasKey(item => item.Id);
            modelBuilder.Entity<Designation>().Property(item => item.Id).HasColumnName("id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Designation>().Property(item => item.ZohoId).HasColumnName("zoho_id").IsRequired().HasMaxLength(25);
            modelBuilder.Entity<Designation>().Property(item => item.DesignationName).HasColumnName("designation_name").IsOptional().HasMaxLength(155);

            modelBuilder.Entity<Employee>().ToTable("employees");
            modelBuilder.Entity<Employee>().HasKey(item => item.Id);
            modelBuilder.Entity<Employee>().Property(item => item.Id).HasColumnName("id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Employee>().Property(item => item.ZohoId).HasColumnName("zoho_id").IsRequired().HasMaxLength(25);
            modelBuilder.Entity<Employee>().Property(item => item.EmployeeId).HasColumnName("Employee_ID").IsOptional().HasMaxLength(20);
            modelBuilder.Entity<Employee>().Property(item => item.FirstName).HasColumnName("First_Name").IsOptional().HasMaxLength(75);
            modelBuilder.Entity<Employee>().Property(item => item.LastName).HasColumnName("Last_Name").IsOptional().HasMaxLength(75);
            modelBuilder.Entity<Employee>().Property(item => item.EmployeeName).HasColumnName("Employee_Name").IsRequired().HasMaxLength(75);
            modelBuilder.Entity<Employee>().Property(item => item.Email).HasColumnName("Email").IsOptional().HasMaxLength(100);
            modelBuilder.Entity<Employee>().Property(item => item.DateOfJoining).HasColumnName("Date_of_Joining").IsRequired();
            modelBuilder.Entity<Employee>().Property(item => item.DateOfExit).HasColumnName("Date_of_Exit").IsOptional();
            modelBuilder.Entity<Employee>().Property(item => item.EmployeeStatus).HasColumnName("Employee_Status").IsRequired().HasMaxLength(25);
            modelBuilder.Entity<Employee>().Property(item => item.DepartmentId).HasColumnName("department_id").IsOptional();
            modelBuilder.Entity<Employee>().Property(item => item.DesignationId).HasColumnName("designation_id").IsOptional();
            modelBuilder.Entity<Employee>().Property(item => item.ManagerName).HasColumnName("Manager_Name").IsOptional().HasMaxLength(75);
            modelBuilder.Entity<Employee>().Property(item => item.ManagerEmail).HasColumnName("Manager_Email").IsOptional().HasMaxLength(150);
            modelBuilder.Entity<Employee>().Property(item => item.CustomerId).HasColumnName("Client").IsOptional();
            modelBuilder.Entity<Employee>().Property(item => item.Rate).HasColumnName("Rate").IsOptional().HasPrecision(12, 2);
            modelBuilder.Entity<Employee>().Property(item => item.CreatedAt).HasColumnName("created_at").IsOptional();
            modelBuilder.Entity<Employee>().Property(item => item.UpdatedAt).HasColumnName("updated_at").IsOptional();

            modelBuilder.Entity<Employee>()
                .HasOptional(item => item.Customer)
                .WithMany(item => item!.Employees)
                .HasForeignKey(item => item.CustomerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasOptional(item => item.Department)
                .WithMany(item => item!.Employees)
                .HasForeignKey(item => item.DepartmentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasOptional(item => item.Designation)
                .WithMany(item => item!.Employees)
                .HasForeignKey(item => item.DesignationId)
                .WillCascadeOnDelete(false);
        }
    }
}
