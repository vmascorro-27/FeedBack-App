namespace FeedBack_APP.Models.MasterEntities
{
    public class Customer
    {
        public Customer()
        {
            Employees = new HashSet<Employee>();
        }

        public int CustomerPkId { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public string? ZohoPeopleId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? ActiveRecurringInvoice { get; set; }
        public string? BillingType { get; set; }
        public byte? BillingPeriodStartDay { get; set; }
        public byte? BillingPeriodEndDay { get; set; }
        public byte? InvoiceSendDay { get; set; }
        public byte BiweeklyFactor { get; set; }
        public byte? PaymentTerms { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
