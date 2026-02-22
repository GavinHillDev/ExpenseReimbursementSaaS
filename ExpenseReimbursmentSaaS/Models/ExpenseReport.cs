namespace ExpenseReimbursmentSaaS.Models
{
    public class ExpenseReport
    {
        public int Id { get; set; }

        public DateOnly UploadDate { get; set;}

        public string Status { get; set; }

        public int totalAmount { get; set; }

        public string? managerComment { get; set; }
        public int? managerId { get; set; }
        public string? financeComment { get; set; }
        public int? FinanceId { get; set; }
        public ICollection<ExpenseItem>? ExpenseItems { get; set; }
        public ICollection<Receipt>? ExpenseReceipts { get; set; }
        public int UploaderId { get; set; }
    }
}
