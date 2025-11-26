using DataAccess.Contexts;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Services
{
    public class PersonService
    {
        private readonly string _databasePath;
        private readonly string _appDataDirectory;

        public PersonService(string databasePath, string appDataDirectory)
        {
            _databasePath = databasePath;
            _appDataDirectory = appDataDirectory;
        }

        public virtual void CheckPersonBeforeSave(Person person)
        {
            if (string.IsNullOrWhiteSpace(person.Name))
                throw new ArgumentException("Укажите имя персоны");
        }

        public virtual async Task<int> EditPersonAsync(Person person, DateTime createdAt)
        {
            using var db = new ApplicationContext(_databasePath);
            var entity = await db.Persons
                .FirstOrDefaultAsync(x => x.Id == person.Id);

            if (entity == null)
                entity = (await db.Persons.AddAsync(new Person()
                {
                    Id = person.Id
                })).Entity;

            entity.Name = person.Name;
            entity.Comment = person.Comment;
            entity.ImageName = person.ImageName;

            entity.CreatedAt = createdAt;
            return await db.SaveChangesAsync();
        }

        public virtual async Task<int> RemovePersonAsync(Person series)
        {
            using var db = new ApplicationContext(_databasePath);

            db.Entry(series).State = EntityState.Deleted;
            return await db.SaveChangesAsync();
        }

        public virtual async Task<Person?> GetPersonAsync(Guid id)
        {
            using var db = new ApplicationContext(_databasePath);

            return await db.Persons
                .Where(x => x.Id == id)
                .Include(x => x.MyBooks).ThenInclude(x => x.Authors)
                .Include(x => x.MyBooks).ThenInclude(x => x.Location)
                .Include(x => x.LocationBooks).ThenInclude(x => x.Authors)
                .Include(x => x.LocationBooks).ThenInclude(x => x.Owner)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
        }

        public virtual async Task<List<Person>> GetPersonsAsync()
        {
            using var db = new ApplicationContext(_databasePath);

            return await db.Persons
                .ToListAsync();
        }

        public string GetPersonImagePath(string imageName) =>
            Path.Combine(_appDataDirectory, "persons", "images", imageName);

        public string GetPersonThumbImagePath(string imageName) =>
            Path.Combine(_appDataDirectory, "persons", "images", "thumbs", imageName);

        public string GetPersonTempImagePath() =>
            Path.Combine(_appDataDirectory, "persons", "images", "temp", $"{Guid.NewGuid()}.jpg");
    }
}
