namespace ExpenseReimbursmentSaaS.Models
{
    public class Receipt
    {
        public int Id { get; set; }

        public int ExpenseReportId { get; set; }

        public int UploaderId { get; set; }

        public string? FilePath { get; set; }
        public required string Category { get; set; }

        public DateOnly UploadDate { get; set; }
    }
}
