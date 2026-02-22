using ExpenseReimbursmentSaaS.Models;

namespace ExpenseReimbursmentSaaS.Dtos
{
    public class ExpenseReportDto
    {
        //public DateOnly UploadDate { get; set; }

        public int totalAmount { get; set; }

        public string? managerComment { get; set; }

        public string? financeComment { get; set; }
    }
}
