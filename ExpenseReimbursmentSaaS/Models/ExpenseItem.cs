namespace ExpenseReimbursmentSaaS.Models
{
    public class ExpenseItem
    {
        public int Id { get; set; }

        public int ExpenseReportId { get; set; }

        public ExpenseReport expenseReport { get; set; }

        public double Amount { get; set; }  

        public ExpenseCategory Category { get; set; } 

        public DateOnly Date { get; set; }

        public string Description { get; set; } 
    }
}
