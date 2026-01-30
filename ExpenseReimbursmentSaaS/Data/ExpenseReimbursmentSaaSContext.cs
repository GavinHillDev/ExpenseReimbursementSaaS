using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ExpenseReimbursmentSaaS.Models;

namespace ExpenseReimbursmentSaaS.Data
{
    public class ExpenseReimbursmentSaaSContext : DbContext
    {
        public ExpenseReimbursmentSaaSContext (DbContextOptions<ExpenseReimbursmentSaaSContext> options)
            : base(options)
        {
        }

        public DbSet<ExpenseReimbursmentSaaS.Models.Employee> Employee { get; set; } = default!;
        public DbSet<ExpenseReimbursmentSaaS.Models.ExpenseReport> ExpenseReport { get; set; } = default!;
        public DbSet<ExpenseReimbursmentSaaS.Models.ExpenseItem> ExpenseItem { get; set; } = default!;
        public DbSet<ExpenseReimbursmentSaaS.Models.Receipt> Receipt { get; set; } = default!;
        public DbSet<ExpenseReimbursmentSaaS.Models.ExpenseCategory> ExpenseCategory { get; set; } = default!;
    }
}
