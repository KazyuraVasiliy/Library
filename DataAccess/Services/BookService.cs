using DataAccess.Contexts;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Services
{
    public class BookService
    {
        private readonly string _databasePath;
        private readonly string _appDataDirectory;

        public BookService(string databasePath, string appDataDirectory)
        {
            _databasePath = databasePath;
            _appDataDirectory = appDataDirectory;
        }

        public virtual void CheckBookBeforeSave(Book book)
        {
            if (string.IsNullOrWhiteSpace(book.Name))
                throw new ArgumentException("Укажите название книги");

            if (book.Status == null)
                throw new ArgumentException("Укажите статус книги");
        }

        public virtual void CheckContentBeforeSave(Content content)
        {
            if (string.IsNullOrWhiteSpace(content.Name))
                throw new ArgumentException("Укажите название пункта содержания");

            if (content.Status == null)
                throw new ArgumentException("Укажите статус пункта содержания");

            if (content.Level < 0)
                throw new ArgumentException("Уровень пункта содержания не может быть отрицательным числом");

            if (content.PageNumber <= 0)
                throw new ArgumentException("Страница пункта содержания должна быть положительным числом");
        }

        public virtual async Task<int> EditBookAsync(Book book, DateTime createdAt)
        {
            using var db = new ApplicationContext(_databasePath);
            var entity = await db.Books
                .FirstOrDefaultAsync(x => x.Id == book.Id);

            if (entity == null)
                entity = (await db.Books.AddAsync(new Book()
                {
                    Id = book.Id
                })).Entity;

            entity.Name = book.Name;
            entity.PublisherId = book.Publisher?.Id;
            entity.PublishYear = book.PublishYear;
            entity.WriteYear = book.WriteYear;
            entity.LanguageId = book.Language?.Id;
            entity.IsPaperBook = book.IsPaperBook;
            entity.IsEBook = book.IsEBook;
            entity.IsAudioBook = book.IsAudioBook;
            entity.StatusId = book.Status!.Id;
            entity.DateRead = book.DateRead;
            entity.SeriesId = book.Series?.Id;
            entity.SeriesNumber = book.SeriesNumber;
            entity.ISBN = book.ISBN;
            entity.IndexCode = book.IndexCode;
            entity.UDC = book.UDC;
            entity.LBC = book.LBC;
            entity.OwnerId = book.Owner?.Id;
            entity.LocationId = book.Location?.Id;
            entity.Annotation = book.Annotation;
            entity.Comment = book.Comment;
            entity.ImageName = book.ImageName;

            // Authors
            var authors = await db.BookAuthors
                .Where(x => x.BookId == entity.Id)
                .ToListAsync();

            var bookAuthorsId = book.Authors.Select(y => y.Id).ToList();
            var entityAuthorsId = authors.Select(x => x.AuthorId).ToList();

            db.BookAuthors.RemoveRange(authors.Where(x => !bookAuthorsId.Contains(x.AuthorId)));
            await db.BookAuthors.AddRangeAsync(book.Authors
                .Where(x => !entityAuthorsId.Contains(x.Id))
                .Select(x =>
                    new BookAuthor()
                    {
                        BookId = entity.Id,
                        AuthorId = x.Id
                    }));

            // Tags
            var tags = await db.BookTags
                .Where(x => x.BookId == entity.Id)
                .ToListAsync();

            var bookTagsId = book.Tags.Select(y => y.Id).ToList();
            var entityTagsId = tags.Select(x => x.TagId).ToList();

            db.BookTags.RemoveRange(tags.Where(x => !bookTagsId.Contains(x.TagId)));
            await db.BookTags.AddRangeAsync(book.Tags
                .Where(x => !entityTagsId.Contains(x.Id))
                .Select(x =>
                    new BookTag()
                    {
                        BookId = entity.Id,
                        TagId = x.Id
                    }));

            // Genres
            var genres = await db.BookGenres
                .Where(x => x.BookId == entity.Id)
                .ToListAsync();

            var bookGenresId = book.Genres.Select(y => y.Id).ToList();
            var entityGenresId = genres.Select(x => x.GenreId).ToList();

            db.BookGenres.RemoveRange(genres.Where(x => !bookGenresId.Contains(x.GenreId)));
            await db.BookGenres.AddRangeAsync(book.Genres
                .Where(x => !entityGenresId.Contains(x.Id))
                .Select(x =>
                    new BookGenre()
                    {
                        BookId = entity.Id,
                        GenreId = x.Id
                    }));

            // Translators
            var translators = await db.BookTranslators
                .Where(x => x.BookId == entity.Id)
                .ToListAsync();

            var bookTranslatorsId = book.Translators.Select(y => y.Id).ToList();
            var entityTranslatorsId = translators.Select(x => x.TranslatorId).ToList();

            db.BookTranslators.RemoveRange(translators.Where(x => !bookTranslatorsId.Contains(x.TranslatorId)));
            await db.BookTranslators.AddRangeAsync(book.Translators
                .Where(x => !entityTranslatorsId.Contains(x.Id))
                .Select(x =>
                    new BookTranslator()
                    {
                        BookId = entity.Id,
                        TranslatorId = x.Id
                    }));

            // Contents
            var contents = await db.Contents
                .Where(x => x.BookId == entity.Id)
                .ToListAsync();

            var bookContentsId = book.Contents.Select(y => y.Id).ToList();
            db.Contents.RemoveRange(contents.Where(x => !bookContentsId.Contains(x.Id)));

            var contentAuthors = await db.ContentAuthors
                .Where(x => bookContentsId.Contains(x.ContentId))
                .ToListAsync();

            var contentTranslators = await db.ContentTranslators
                .Where(x => bookContentsId.Contains(x.ContentId))
                .ToListAsync();

            foreach (var content in book.Contents)
            {
                var entityContent = contents
                    .FirstOrDefault(x => x.Id == content.Id);

                if (entityContent == null)
                    entityContent = (await db.Contents.AddAsync(new Content()
                    {
                        Id = content.Id
                    })).Entity;

                entityContent.BookId = entity.Id;
                entityContent.CreatedAt = createdAt;

                entityContent.Name = content.Name;
                entityContent.SeriesId = content.Series?.Id;
                entityContent.SeriesNumber = content.SeriesNumber;
                entityContent.PageNumber = content.PageNumber;
                entityContent.StatusId = content.Status!.Id;
                entityContent.LanguageId = content.Language?.Id;
                entityContent.Level = content.Level;
                entityContent.WriteYear = content.WriteYear;

                // ...Authors
                var contentAuthorsId = content.Authors.Select(y => y.Id).ToList();
                var entityContentAuthorsId = contentAuthors.Where(x => x.ContentId == entityContent.Id).Select(x => x.AuthorId).ToList();

                db.ContentAuthors.RemoveRange(contentAuthors
                    .Where(x => 
                        x.ContentId == entityContent.Id && 
                        !contentAuthorsId.Contains(x.AuthorId)));

                await db.ContentAuthors.AddRangeAsync(content.Authors
                    .Where(x => !entityContentAuthorsId.Contains(x.Id))
                    .Select(x =>
                        new ContentAuthor()
                        {
                            ContentId = entityContent.Id,
                            AuthorId = x.Id
                        }));

                // ...Translators
                var contentTranslatorsId = content.Translators.Select(y => y.Id).ToList();
                var entityContentTranslatorsId = contentTranslators.Where(x => x.ContentId == entityContent.Id).Select(x => x.TranslatorId).ToList();

                db.ContentTranslators.RemoveRange(contentTranslators
                    .Where(x =>
                        x.ContentId == entityContent.Id &&
                        !contentTranslatorsId.Contains(x.TranslatorId)));

                await db.ContentTranslators.AddRangeAsync(content.Translators
                    .Where(x => !entityContentTranslatorsId.Contains(x.Id))
                    .Select(x =>
                        new ContentTranslator()
                        {
                            ContentId = entityContent.Id,
                            TranslatorId = x.Id
                        }));
            }

            // ...
            entity.CreatedAt = createdAt;
            return await db.SaveChangesAsync();
        }

        public virtual async Task<int> RemoveBookAsync(Book book)
        {
            using var db = new ApplicationContext(_databasePath);
            var entity = await db.Books
                .FirstAsync(x => x.Id == book.Id);

            db.Books.Remove(entity);
            return await db.SaveChangesAsync();
        }

        public virtual async Task<Book?> GetBookAsync(Guid id)
        {
            using var db = new ApplicationContext(_databasePath);

            return await db.Books
                .Where(x => x.Id == id)
                .Include(x => x.Authors)
                .Include(x => x.Tags)
                .Include(x => x.Translators)
                .Include(x => x.Genres)
                .Include(x => x.Status)
                .Include(x => x.Series)
                .Include(x => x.Language)
                .Include(x => x.Publisher)
                .Include(x => x.Owner)
                .Include(x => x.Location)
                .Include(x => x.Contents).ThenInclude(x => x.Authors)
                .Include(x => x.Contents).ThenInclude(x => x.Translators)
                .Include(x => x.Contents).ThenInclude(x => x.Status)
                .Include(x => x.Contents).ThenInclude(x => x.Language)
                .Include(x => x.Contents).ThenInclude(x => x.Series)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
        }

        public virtual async Task<List<Book>> GetBooksAsync()
        {
            using var db = new ApplicationContext(_databasePath);

            return await db.Books
                .Include(x => x.Status)
                .Include(x => x.Authors)
                .Include(x => x.Series)
                .AsSplitQuery()
                .ToListAsync();
        }

        public virtual Content CopyContent(Content content)
        {
            var copy = new Content();

            copy.Id = content.Id;
            copy.Name = content.Name;
            copy.PageNumber = content.PageNumber;
            copy.Status = content.Status;
            copy.StatusId = content.StatusId;
            copy.Series = content.Series;
            copy.SeriesId = content.SeriesId;
            copy.SeriesNumber = content.SeriesNumber;
            copy.Language = content.Language;
            copy.LanguageId = content.LanguageId;
            copy.Level = content.Level;
            copy.WriteYear = content.WriteYear;
            copy.Book = content.Book;
            copy.BookId = content.BookId;

            copy.Authors = [.. content.Authors];
            copy.Translators = [.. content.Translators];

            return copy;
        }

        public string GetBookImagePath(string imageName) =>
            Path.Combine(_appDataDirectory, "books", "images", imageName);

        public string GetBookThumbImagePath(string imageName) =>
            Path.Combine(_appDataDirectory, "books", "images", "thumbs", imageName);

        public string GetBookTempImagePath() =>
            Path.Combine(_appDataDirectory, "books", "images", "temp", $"{Guid.NewGuid()}.jpg");
    }
}
