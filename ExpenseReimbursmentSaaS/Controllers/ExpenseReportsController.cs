using ExpenseReimbursmentSaaS.Data;
using ExpenseReimbursmentSaaS.Dtos;
using ExpenseReimbursmentSaaS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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

        // GET: api/ExpenseReports
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseReport>>> GetExpenseReport()
        {
            return await _context.ExpenseReport.ToListAsync();
        }

        // GET: api/ExpenseReports/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExpenseReport>> GetExpenseReport(int id)
        {
            var expenseReport = await _context.ExpenseReport.FindAsync(id);

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
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> CreateReport([FromBody] RegisterDto register)
        {
            var context = _context.ExpenseReport;
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (id == null) return Unauthorized();
            var user = await _context.Employee.FindAsync(int.Parse(id));
            //User is the uploader
            //Date is upload Date
            //Use ID to upload Expense Items
            //Status = Pending

            var expenseReport = new ExpenseReport()
            {
                Uploader = user,
                UploaderId = user.Id,
                UploadDate = new DateOnly(),
                Status = ExpenseStatus.Started,
            };

            return Ok(new { message = "Report Started" });
        }
        [HttpPut("id")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> AddToReport([FromBody] int id, ExpenseReport expenseReport, string managerComment)
        {
            var context = _context.ExpenseReport;
            var Employeeid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (Employeeid == null) return Unauthorized();
            var user = await _context.Employee.FindAsync(int.Parse(Employeeid));

             if (id != expenseReport.Id)
            {
                return BadRequest();
            }
            var report = await _context.ExpenseReport.FindAsync(id);
            if (managerComment != null) { 
                report.managerComment = managerComment;
                report.Status = ExpenseStatus.Pending;
            
            }

            //If Receipt exists or ExpenseReport exists manager comment or finance
            //commnet. Add Expense Item and Receipt in individual controller instead of adding here
            //Update this to be a comment
             

            return Ok(new { message = "Report Submitted" });
        }


        // POST: api/ExpenseReports
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ExpenseReport>> PostExpenseReport(ExpenseReport expenseReport)
        {
            _context.ExpenseReport.Add(expenseReport);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExpenseReport", new { id = expenseReport.Id }, expenseReport);
        }

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
