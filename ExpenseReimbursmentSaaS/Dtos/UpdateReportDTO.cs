using ExpenseReimbursmentSaaS.Models;

namespace ExpenseReimbursmentSaaS.Dtos
{
    public class UpdateReportDTO
    {
        public double ExpenseItemAmount {  get; set; }

        public string ExpenseItemDescription { get; set; }

        public DateOnly Date { get; set; }

        public ExpenseCategory ExpenseCategory { get; set; }


    }
}
