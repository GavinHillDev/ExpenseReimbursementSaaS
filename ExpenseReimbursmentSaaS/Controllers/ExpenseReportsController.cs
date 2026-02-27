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

 


       
        //Create Expense Report
        [HttpPost("createReport")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = Roles.Employee + "," + Roles.Admin + "," + Roles.Manager + "," + Roles.Finance)]
        public async Task<IActionResult> CreateReport([FromBody] ExpenseReportDto report)
        {
            var context = _context.ExpenseReport;
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (id == null) return Unauthorized();
            var user = await _context.Employee.FindAsync(int.Parse(id));
 
            var expenseReport = new ExpenseReport()
            {
                UploaderId = user.Id,
                UploadDate = DateOnly.FromDateTime(DateTime.Now),
                Status = ExpenseStatus.Started,
                totalAmount = report.totalAmount
            };
            _context.ExpenseReport.Add(expenseReport);
            _context.SaveChangesAsync();
            return Ok(new { message = expenseReport.Id, expenseReport});
        }

        //Manager and Finance get approval request, if revision is needed it will 
        //Go back to the employee
        [HttpPut("managerapproval/{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = Roles.Manager)]
        public async Task<IActionResult> ManagerApproval(int id, [FromBody] ManagerApprovalDTO approvalDTO)
        {
            var context = _context.ExpenseReport;
            var Employeeid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (Employeeid == null) return Unauthorized();
            var user = await _context.Employee.FindAsync(int.Parse(Employeeid));
 
            var report = await _context.ExpenseReport.FindAsync(id);

            if (report == null)
            {
                return BadRequest(new { message = report });
            }
            if (approvalDTO != null) {
                if (approvalDTO.revisionNeeded)
                {
                    report.managerComment = approvalDTO.managerStatement;
                    report.Status = ExpenseStatus.RevisionNeeded;
                    report.managerId = user.Id;

                }
                report.managerComment = approvalDTO.managerStatement;
                report.Status = ExpenseStatus.UnderReview;
                report.managerId = user.Id;
            }

            return Ok(new { message = "Report Needs Revised" });
        }
        [HttpPut("financeapproval/{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = Roles.Finance)]
        public async Task<IActionResult> FinanceApproval(int id, [FromBody] FinanceApprovalDTO approvalDTO)
        {
            var context = _context.ExpenseReport;
            var Employeeid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (Employeeid == null) return Unauthorized();
            var user = await _context.Employee.FindAsync(int.Parse(Employeeid));

            var report = await _context.ExpenseReport.FindAsync(id);
            if (report == null)
            {
                return BadRequest(new { message = report });
            }
            if (approvalDTO != null)
            {
                if (approvalDTO.revisionNeeded)
                {
                    report.financeComment = approvalDTO.financeStatement;
                    report.Status = ExpenseStatus.RevisionNeeded;
                    report.FinanceId = user.Id;

                }
                report.managerComment = approvalDTO.financeStatement;
                report.Status = ExpenseStatus.Completed;
                report.managerId = user.Id;
            }
            return Ok(new { message = "Report Completed" });
        }

       
        
        
        //}
        // DELETE: api/ExpenseReports/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = Roles.Admin)]
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

       
    }
}
