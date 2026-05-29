using DataAccess.Contexts;
using DataAccess.Models.Statistic;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Services
{
    public class StatisticService
    {
        private readonly string _databasePath;

        public StatisticService(string databasePath) =>
            _databasePath = databasePath;

        public async Task<List<HaveReadPerPeriod>> GetHaveReadPerPeriodStatistic()
        {
            using var db = new ApplicationContext(_databasePath);
            var books = await db.Books
                .Where(x => x.StatusId == Constants.Statuses.HaveRead && x.DateRead != null)
                .Select(x => x.DateRead!.Value)
                .ToListAsync();

            var statistic = new List<HaveReadPerPeriod>();

            statistic.AddRange(books
                .GroupBy(x => new DateOnly(x.Year, x.Month, 1))
                .Select(x => new HaveReadPerPeriod()
                {
                    PeriodType = PerioTypes.Month,
                    Period = x.Key,
                    Count = x.Count()
                })
                .ToList());

            statistic.AddRange(books
                .GroupBy(x => new DateOnly(x.Year, 1, 1))
                .Select(x => new HaveReadPerPeriod()
                {
                    PeriodType = PerioTypes.Year,
                    Period = x.Key,
                    Count = x.Count()
                })
                .ToList());

            return statistic;
        }
    }
}
