using ExpenseReimbursmentSaaS.Models;

namespace ExpenseReimbursmentSaaS.Dtos
{
    public class ReceiptUploadDTO
    {
        public IFormFile? ReceiptFile { get; set; }
        public string Category { get; set; }
    }
}
