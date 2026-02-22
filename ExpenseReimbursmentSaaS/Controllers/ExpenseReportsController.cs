using ExpenseReimbursmentSaaS.Data;
using ExpenseReimbursmentSaaS.Dtos;
using ExpenseReimbursmentSaaS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ExpenseReimbursmentSaaS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseReportsController : ControllerBase
    {
        private readonly ExpenseReimbursmentSaaSContext _context;
        private readonly JwtService _jwtService;
        
        public ExpenseReportsController(ExpenseReimbursmentSaaSContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        //// GET: api/ExpenseReports
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<ExpenseReport>>> GetExpenseReport()
        //{
        //    return await _context.ExpenseReport.ToListAsync();
        //}

        // GET: api/ExpenseReports/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExpenseReport>> GetExpenseReport(int id)
        {
            var expenseReport = await _context.ExpenseReport
                //.FindAsync(id);
            .Include(r => r.ExpenseReceipts)
            .Include(r => r.ExpenseItems)
            .FirstOrDefaultAsync(r => r.Id == id);

            if (expenseReport == null)
            {
                return NotFound();
            }

            return expenseReport;
        }

 


        // PUT: api/ExpenseReports/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExpenseReport(int id, ExpenseReport expenseReport)
        {
            if (id != expenseReport.Id)
            {
                return BadRequest();
            }

            _context.Entry(expenseReport).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExpenseReportExists(id))
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
        //Create Expense Report
        [HttpPost("createReport")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = Roles.Employee)]
        public async Task<IActionResult> CreateReport([FromBody] ExpenseReportDto report)
        {
            var context = _context.ExpenseReport;
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (id == null) return Unauthorized();
            var user = await _context.Employee.FindAsync(int.Parse(id));
            //User is the uploader
            //Date is upload Date
            //Use ID to upload Expense Items
            //Status = Pending
            //Employee puts in totalAmount of items
            var expenseReport = new ExpenseReport()
            {
                UploaderId = user.Id,
                UploadDate = new DateOnly(),
                Status = ExpenseStatus.Started,
                totalAmount = report.totalAmount
            };
            _context.ExpenseReport.Add(expenseReport);
            _context.SaveChangesAsync();
            return Ok(new { message = expenseReport.Id, expenseReport});
        }

        //Manager and Finance get approval request, if revision is needed it will 
        //Go back to the employee
        [HttpPut("id")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = Roles.Manager)]
        public async Task<IActionResult> ManagerApproval([FromBody] int id, string managerComment, bool revisionNeeded)
        {
            var context = _context.ExpenseReport;
            var Employeeid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (Employeeid == null) return Unauthorized();
            var user = await _context.Employee.FindAsync(int.Parse(Employeeid));
 
            var report = await _context.ExpenseReport.FindAsync(id);

            if (report == null)
            {
                return BadRequest();
            }
            if (managerComment != null) {
                if (revisionNeeded)
                {
                    report.managerComment = managerComment;
                    report.Status = ExpenseStatus.RevisionNeeded;
                }
                report.managerComment = managerComment;
                report.Status = ExpenseStatus.UnderReview;
            }

            return Ok(new { message = "Report Needs Revised" });
        }
        [HttpPut("id")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = Roles.Finance)]
        public async Task<IActionResult> FinanceApproval([FromBody] int id, string financeComment, bool revisionNeeded)
        {
            var context = _context.ExpenseReport;
            var Employeeid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (Employeeid == null) return Unauthorized();
            var user = await _context.Employee.FindAsync(int.Parse(Employeeid));

            var report = await _context.ExpenseReport.FindAsync(id);

            if (report == null)
            {
                return BadRequest();
            }
            if (financeComment != null)
            {
                if (revisionNeeded)
                {
                    report.financeComment = financeComment;
                    report.Status = ExpenseStatus.RevisionNeeded;
                }
                report.financeComment = financeComment;
                report.Status = ExpenseStatus.Completed;
            }
            return Ok(new { message = "Report Submitted" });
        }

        // POST: api/ExpenseReports
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<ExpenseReport>> PostExpenseReport(ExpenseReport expenseReport)
        //{
        //    _context.ExpenseReport.Add(expenseReport);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetExpenseReport", new { id = expenseReport.Id }, expenseReport);
        //}
        //[HttpGet("id")]
        //public async Task<IActionResult> GetReport(int id)
        //{
        
            
        
        
        //}
        // DELETE: api/ExpenseReports/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpenseReport(int id)
        {
            var expenseReport = await _context.ExpenseReport.FindAsync(id);
            if (expenseReport == null)
            {
                return NotFound();
            }

            _context.ExpenseReport.Remove(expenseReport);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExpenseReportExists(int id)
        {
            return _context.ExpenseReport.Any(e => e.Id == id);
        }
    }
}
