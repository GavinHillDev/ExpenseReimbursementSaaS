using ExpenseReimbursmentSaaS.Data;
using ExpenseReimbursmentSaaS.Dtos;
using ExpenseReimbursmentSaaS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ExpenseReimbursmentSaaS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly ExpenseReimbursmentSaaSContext _context;
        private readonly JwtService _jwtService;
        private readonly IPasswordHasher<Employee> _passwordHasher;
        public EmployeesController(ExpenseReimbursmentSaaSContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordHasher = new PasswordHasher<Employee>();
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployee()
        {
            return await _context.Employee.ToListAsync();
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employee.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest();
            }

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
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


        //LOGIN ENDPOINT
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var employee = await _context.Employee.FirstOrDefaultAsync(e => e.Email == login.Email);
            if (employee == null)
            {
                return Unauthorized(new { message = "Not Found" });
            }
            var result = _passwordHasher.VerifyHashedPassword(employee, employee.PasswordHash, login.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                Console.WriteLine(employee.PasswordHash);
                Console.WriteLine(_passwordHasher.HashPassword(null,login.Password));
                Console.WriteLine("Failed Password");
                return Unauthorized(new { message = "Invalid Credentials" });
            }
            var token = _jwtService.GenerateToken(employee);
            return Ok(new { token });
        }


        // POST: api/Employees/Register
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("registeremployee")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Register([FromBody] RegisterDto register)
        {

            //Create Employee and define their role
            var employee = new Employee
            {
                Email = register.Email,
                Name = register.Name,
                PasswordHash = _passwordHasher.HashPassword(null, register.Password),
                Role = Roles.Employee

            };
            
            employee.EmployeeId = employee.Id;
            _context.Employee.Add(employee);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Employee registered successfully" });
        }
        [HttpPost("registeradmin")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDto register)
        {

            //Create Employee and define their role
            var employee = new Employee
            {
                Email = register.Email,
                Name = register.Name,
                PasswordHash = _passwordHasher.HashPassword(null, register.Password),
                Role = Roles.Admin

            };

            employee.EmployeeId = employee.Id;
            _context.Employee.Add(employee);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Employee registered successfully" });
        }
        [HttpPost("registerfinance")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> RegisterFinance([FromBody] RegisterDto register)
        {

            //Create Employee and define their role
            var employee = new Employee
            {
                Email = register.Email,
                Name = register.Name,
                PasswordHash = _passwordHasher.HashPassword(null, register.Password),
                Role = Roles.Finance

            };

            employee.EmployeeId = employee.Id;
            _context.Employee.Add(employee);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Employee registered successfully" });
        }



        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employee.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employee.Any(e => e.Id == id);
        }
    }
}
