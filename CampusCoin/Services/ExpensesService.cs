using CampusCoin.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusCoin.Services
{
    public class ExpensesService
    {
        private IDbContextFactory<CampusCoinContext> _context;
        public string DbPath { get; }
        public ExpensesService(IDbContextFactory<CampusCoinContext> context)
        {
            _context = context;
        }

        public async Task SubmitExpense(UserExpenseData userData)
        {
            var dbContext = await _context.CreateDbContextAsync(); // Create database context
            dbContext.UserExpenseData.Add(userData); // Add user data to the database
            dbContext.SaveChanges(); // Save the changes to the database
        }
    }
}
