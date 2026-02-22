namespace ExpenseReimbursmentSaaS.Models
{
    public class ExpenseItem
    {
        public int Id { get; set; }

        public int ExpenseReportId { get; set; }
        public int UploaderId { get; set; }

        public double Amount { get; set; }

        public required string Category { get; set; }
        public DateOnly UploadDate { get; set; }

        public string Description { get; set; } 
    }
}
