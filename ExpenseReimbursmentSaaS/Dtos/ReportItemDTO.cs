namespace ExpenseReimbursmentSaaS.Dtos
{
    public class ReportItemDTO
    {
        public int itemAmount { get; set; }

        public required string Category { get; set; }

        public DateOnly ExpenseDate { get; set; }

        public string Description { get; set; }
    }
}
