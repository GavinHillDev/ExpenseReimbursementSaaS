namespace ExpenseReimbursmentSaaS.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public required int EmployeeId { get; set; }
        public required string Name { get; set; }

        public required string Email { get; set; }

        public required string Role { get; set; }
        // Finance, Admin, Manager, Employee
    }
}
//Employee Uploads Report with information including receipt. ExpenseItems refenece Report along with Receipt
//Managers can approve or reject reports and comment/
//Finanance can comment, and move forward with reports