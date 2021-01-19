using System.Collections.Generic;
using System.Threading.Tasks;
using FinancialChat.Data;
using FinancialChat.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FinancialChat.DB
{
    public static class DbService
    {
        public static async Task SaveMessage(Message message)
        {
            await using var db = new ApplicationDbContext(OptionsBuilder.Instance.Options);
            await db.Messages.AddAsync(message);
            await db.SaveChangesAsync();
        }

        public static async Task<List<Message>> GetMessages()
        {
            await using var db = new ApplicationDbContext(OptionsBuilder.Instance.Options);
            return await db.Messages.OrderByDescending(m => m.Date).Take(50).OrderBy(m => m.Date).ToListAsync();
        }
    }
}
