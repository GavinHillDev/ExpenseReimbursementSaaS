namespace ExpenseReimbursmentSaaS.Models
{
    public class Receipt
    {
        public int Id { get; set; }

        public int ExpentReportId { get; set; }

        public ExpenseReport expenseReport { get; set; }

        public string FilePath { get; set; }

        public string ContentType { get; set; }

        public DateOnly UploadDate { get; set; }
    }
}
