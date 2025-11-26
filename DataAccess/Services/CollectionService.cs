using DataAccess.Contexts;
using DataAccess.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Services
{
    public class CollectionService
    {
        private readonly string _databasePath;

        public CollectionService(string databasePath) =>
            _databasePath = databasePath;

        public virtual async Task<List<T>> GetCollectionAsync<T>() where T : CollectionBase
        {
            using var db = new ApplicationContext(_databasePath);

            return await db.Set<T>()
                .ToListAsync();
        }

        public virtual async Task CheckCollectionItemBeforeSave<T>(T item) where T : CollectionBase
        {
            if (string.IsNullOrWhiteSpace(item.Name))
                throw new ArgumentException("Укажите имя элемента");

            using var db = new ApplicationContext(_databasePath);
            var sameName = await db.Set<T>()
                .FirstOrDefaultAsync(x =>
                    x.Id != item.Id &&
                    x.Name == item.Name);

            if (sameName != null)
                throw new ArgumentException("Элемент с таким именем уже существует, измените имя элемента");
        }

        public virtual async Task<int> AddItemAsync<T>(T item, DateTime createdAt) where T : CollectionBase
        {
            using var db = new ApplicationContext(_databasePath);

            item.CreatedAt = createdAt;
            var entity = await db.Set<T>().AddAsync(item);

            return await db.SaveChangesAsync();
        }

        public virtual async Task<int> RemoveItemAsync<T>(T item) where T : CollectionBase
        {
            using var db = new ApplicationContext(_databasePath);

            db.Entry(item).State = EntityState.Deleted;
            return await db.SaveChangesAsync();
        }
    }
}
