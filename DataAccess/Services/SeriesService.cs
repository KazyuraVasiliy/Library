using DataAccess.Contexts;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Services
{
    public class SeriesService
    {
        private readonly string _databasePath;

        public SeriesService(string databasePath) =>
            _databasePath = databasePath;

        public virtual async Task CheckSeriesBeforeSave(Series series)
        {
            if (string.IsNullOrWhiteSpace(series.Name))
                throw new ArgumentException("Укажите имя серии");

            using var db = new ApplicationContext(_databasePath);
            var sameName = await db.Serieses
                .FirstOrDefaultAsync(x =>
                    x.Id != series.Id &&
                    x.Name == series.Name);

            if (sameName != null)
                throw new ArgumentException("Серия с таким именем уже существует, измените имя серии");
        }

        public virtual async Task<int> EditSeriesAsync(Series series, DateTime createdAt)
        {
            using var db = new ApplicationContext(_databasePath);
            var entity = await db.Serieses
                .FirstOrDefaultAsync(x => x.Id == series.Id);

            if (entity == null)
                entity = (await db.Serieses.AddAsync(new Series()
                {
                    Id = series.Id
                })).Entity;

            entity.Name = series.Name;
            entity.Comment = series.Comment;

            entity.CreatedAt = createdAt;
            return await db.SaveChangesAsync();
        }

        public virtual async Task<int> RemoveSeriesAsync(Series series)
        {
            using var db = new ApplicationContext(_databasePath);

            db.Entry(series).State = EntityState.Deleted;
            return await db.SaveChangesAsync();
        }

        public virtual async Task<Series?> GetSeriesAsync(Guid id)
        {
            using var db = new ApplicationContext(_databasePath);

            return await db.Serieses
                .Where(x => x.Id == id)
                .Include(x => x.Books).ThenInclude(x => x.Authors)
                .Include(x => x.Contents).ThenInclude(x => x.Authors)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
        }

        public virtual async Task<List<Series>> GetSeriesesAsync()
        {
            using var db = new ApplicationContext(_databasePath);

            return await db.Serieses
                .ToListAsync();
        }
    }
}
