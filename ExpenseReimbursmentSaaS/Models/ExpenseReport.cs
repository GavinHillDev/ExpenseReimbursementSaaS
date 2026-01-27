namespace ExpenseReimbursmentSaaS.Models
{
    public class ExpenseReport
    {
        public int Id { get; set; }

        public DateOnly UploadDate { get; set;}

        public string Status { get; set; }

        public int totalAmount { get; set; }

        public string? managerComment { get; set; }

        public string? financeComment { get; set; }

        public ICollection<ExpenseItem>? ExpenseItems { get; set; }

        public string Uploader { get; set; }

    }
}
