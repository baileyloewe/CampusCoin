using CampusCoin.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusCoin.Services
{
    class ExpensesService
    {
        private IDbContextFactory<CampusCoinContext> _context;
        public ExpensesService(IDbContextFactory<CampusCoinContext> context)
        {
            _context = context;
        }
    }
}
