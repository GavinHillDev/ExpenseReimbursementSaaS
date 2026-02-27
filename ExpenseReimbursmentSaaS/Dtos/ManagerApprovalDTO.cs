namespace ExpenseReimbursmentSaaS.Dtos
{
    public class ManagerApprovalDTO
    {
        public string managerStatement { get; set; }

        public bool revisionNeeded { get; set; }

        public string? revisionComment { get; set; }
    }
}
