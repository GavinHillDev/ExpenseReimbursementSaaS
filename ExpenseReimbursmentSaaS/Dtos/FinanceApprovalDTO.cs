namespace ExpenseReimbursmentSaaS.Dtos
{
    public class FinanceApprovalDTO
    {
        public string financeStatement { get; set; }

        public bool revisionNeeded { get; set; }

        public string? revisionComment { get; set; }
    }
}
