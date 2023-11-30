using CampusCoin.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusCoin.Services
{
    public class IncomeService
    {
        private IDbContextFactory<CampusCoinContext> _context;
        public string DbPath { get; }
        public IncomeService(IDbContextFactory<CampusCoinContext> context)
        {
            _context = context;
        }

        public async Task SubmitIncome(UserIncomeData userData)
        {
            var dbContext = await _context.CreateDbContextAsync(); // Create database context
            dbContext.UserIncomeData.Add(userData); // Add user data to the database
            dbContext.SaveChanges(); // Save the changes to the database
        }
    }
}
