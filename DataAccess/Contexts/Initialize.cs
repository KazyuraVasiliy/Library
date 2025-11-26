using Microsoft.EntityFrameworkCore;

namespace DataAccess.Contexts
{
    public static class Initialize
    {
        public static async Task InitAsync(string databasePath)
        {
            using var db = new ApplicationContext(databasePath);
            await db.Database.MigrateAsync();
        }
    }
}
