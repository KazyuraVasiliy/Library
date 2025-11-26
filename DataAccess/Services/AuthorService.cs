using DataAccess.Contexts;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Services
{
    public class AuthorService
    {
        private readonly string _databasePath;
        private readonly string _appDataDirectory;

        public AuthorService(string databasePath, string appDataDirectory)
        {
            _databasePath = databasePath;
            _appDataDirectory = appDataDirectory;
        }

        public virtual void CheckAuthorBeforeSave(Author author)
        {
            if (string.IsNullOrWhiteSpace(author.Name))
                throw new ArgumentException("Укажите имя автора");

            if (author.DateBirth != null && author.DateDeath != null)
            {
                if (author.DateBirth >= author.DateDeath)
                    throw new ArgumentException("Дата рождения превосходит дату смерти, укажите корректные даты.");
            }
        }

        public virtual async Task<int> EditAuthorAsync(Author author, DateTime createdAt)
        {
            using var db = new ApplicationContext(_databasePath);
            var entity = await db.Authors
                .FirstOrDefaultAsync(x => x.Id == author.Id);

            if (entity == null)
                entity = (await db.Authors.AddAsync(new Author()
                {
                    Id = author.Id
                })).Entity;

            entity.Name = author.Name;
            entity.DateBirth = author.DateBirth;
            entity.DateDeath = author.DateDeath;
            entity.Comment = author.Comment;

            entity.CreatedAt = createdAt;
            return await db.SaveChangesAsync();
        }

        public virtual async Task<int> RemoveAuthorAsync(Author author)
        {
            using var db = new ApplicationContext(_databasePath);
            var entity = await db.Authors
                .FirstAsync(x => x.Id == author.Id);

            db.Authors.Remove(entity);
            return await db.SaveChangesAsync();
        }

        public virtual async Task<Author?> GetAuthorAsync(Guid id)
        {
            using var db = new ApplicationContext(_databasePath);

            return await db.Authors
                .Where(x => x.Id == id)
                .Include(x => x.Books).ThenInclude(x => x.Authors)
                .Include(x => x.Contents).ThenInclude(x => x.Authors)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
        }

        public virtual async Task<List<Author>> GetAuthorsAsync()
        {
            using var db = new ApplicationContext(_databasePath);

            return await db.Authors
                .ToListAsync();
        }

        public string GetAuthorImagePath(string imageName) =>
            Path.Combine(_appDataDirectory, "authors", "images", imageName);

        public string GetAuthorThumbImagePath(string imageName) =>
            Path.Combine(_appDataDirectory, "authors", "images", "thumbs", imageName);

        public string GetAuthorTempImagePath() =>
            Path.Combine(_appDataDirectory, "authors", "images", "temp", $"{Guid.NewGuid()}.jpg");
    }
}
