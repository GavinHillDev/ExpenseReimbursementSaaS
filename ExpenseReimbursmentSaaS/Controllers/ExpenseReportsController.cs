using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseReimbursmentSaaS.Data;
using ExpenseReimbursmentSaaS.Models;

namespace ExpenseReimbursmentSaaS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseReportsController : ControllerBase
    {
        private readonly ExpenseReimbursmentSaaSContext _context;

        public ExpenseReportsController(ExpenseReimbursmentSaaSContext context)
        {
            _context = context;
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
