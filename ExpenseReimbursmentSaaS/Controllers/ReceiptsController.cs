using ExpenseReimbursmentSaaS.Data;
using ExpenseReimbursmentSaaS.Dtos;
using ExpenseReimbursmentSaaS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ExpenseReimbursmentSaaS.Controllers
{
    [Route("api/reports/{reportId}/receipts")]
    [ApiController]
    public class ReceiptsController : ControllerBase
    {
        private readonly ExpenseReimbursmentSaaSContext _context;
        private readonly JwtService _jwtService;
        
        public ReceiptsController(ExpenseReimbursmentSaaSContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // GET: api/Receipts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Receipt>>> GetReceipt()
        {
            return await _context.Receipt.ToListAsync();
        }

        // GET: api/Receipts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Receipt>> GetReceipt(int id)
        {
            var receipt = await _context.Receipt.FindAsync(id);

            if (receipt == null)
            {
                return NotFound();
            }

            return receipt;
        }

        // PUT: api/Receipts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReceipt(int id, Receipt receipt)
        {
            if (id != receipt.Id)
            {
                return BadRequest();
            }

            _context.Entry(receipt).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReceiptExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        [HttpPost("addReceipt")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = Roles.Employee)]
        public async Task<IActionResult> AddReceipt([FromRoute] int reportId, [FromForm] ReceiptUploadDTO receipt)
        {

            var context = _context.ExpenseReport;
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (id == null) return Unauthorized();
            var user = await _context.Employee.FindAsync(int.Parse(id));
            //var parentReport = await _context.ExpenseReport.FindAsync(reportId);

            if (receipt.ReceiptFile == null || receipt.ReceiptFile.Length == 0)
            {
                return BadRequest("No File");
            }
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "receipts");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            var fileExtension = Path.GetExtension(receipt.ReceiptFile.FileName);
            var uniqueFileName = $"{ Guid.NewGuid()}{fileExtension}";

            var filePath = Path.Combine(folderPath, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await receipt.ReceiptFile.CopyToAsync(fileStream);
            }
            var relativePath = Path.Combine("receipts", uniqueFileName);



            var newreceipt = new Receipt()
            {
                UploaderId = user.Id,
                UploadDate = new DateOnly(),
                ExpenseReportId = reportId,   
                FilePath = relativePath,
                Category = receipt.Category,
            };
            var report = await _context.ExpenseReport.FirstOrDefaultAsync(r => r.Id == reportId);
            _context.Receipt.Add(newreceipt);
            _context.SaveChangesAsync();
            //report.ExpenseReceipts.Add(newreceipt);

            return Ok(new { message = report });
        }

        //User Adds Receipt to existing report - Optional
        //User adds Report Item to existing report
        //Manager Comments on Report > Can mark that a follow up is needed
        //Once Manager Approves it will go to finance and they can comment or say follow up is needed

        // DELETE: api/Receipts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReceipt(int id)
        {
            var receipt = await _context.Receipt.FindAsync(id);
            if (receipt == null)
            {
                return NotFound();
            }

            _context.Receipt.Remove(receipt);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReceiptExists(int id)
        {
            return _context.Receipt.Any(e => e.Id == id);
        }
    }
}
