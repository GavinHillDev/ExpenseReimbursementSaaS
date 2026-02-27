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
using System.Security.Claims;
using System.Threading.Tasks;

namespace ExpenseReimbursmentSaaS.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/reports/{reportId}/items")]
    [ApiController]
    public class ExpenseItemsController : ControllerBase
    {
        private readonly ExpenseReimbursmentSaaSContext _context;
        private readonly JwtService _jwtService;

        public ExpenseItemsController(ExpenseReimbursmentSaaSContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // GET: api/ExpenseItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseItem>>> GetExpenseItem()
        {
            return await _context.ExpenseItem.ToListAsync();
        }

        // GET: api/ExpenseItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExpenseItem>> GetExpenseItem(int id)
        {
            var expenseItem = await _context.ExpenseItem.FindAsync(id);

            if (expenseItem == null)
            {
                return NotFound();
            }

            return expenseItem;
        }

        // PUT: api/ExpenseItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExpenseItem(int id, ExpenseItem expenseItem)
        {
            if (id != expenseItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(expenseItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExpenseItemExists(id))
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


        [HttpPost("add")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = Roles.Employee + "," + Roles.Admin + "," + Roles.Manager + "," + Roles.Finance)]
        public async Task<IActionResult> AddReportItem([FromRoute] int reportId, [FromBody] ReportItemDTO item)
        {

            var context = _context.ExpenseReport;
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (id == null) return Unauthorized();
            var user = await _context.Employee.FindAsync(int.Parse(id));

           

            var newitem = new ExpenseItem()
            {
                UploaderId = user.Id,
                UploadDate = DateOnly.FromDateTime(DateTime.Now),
                ExpenseReportId = reportId,
                Category = item.Category,
                Description = item.Description,
                Amount = item.itemAmount
            };
            var report = await _context.ExpenseReport.FirstOrDefaultAsync(r => r.Id == reportId);
            _context.ExpenseItem.Add(newitem);
            _context.SaveChangesAsync();

            return Ok(new { message = report });
        }
        // DELETE: api/ExpenseItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpenseItem(int id)
        {
            var expenseItem = await _context.ExpenseItem.FindAsync(id);
            if (expenseItem == null)
            {
                return NotFound();
            }

            _context.ExpenseItem.Remove(expenseItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExpenseItemExists(int id)
        {
            return _context.ExpenseItem.Any(e => e.Id == id);
        }
    }
}
