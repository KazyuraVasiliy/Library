using DataAccess.Contexts;
using DataAccess.Models.Import;

namespace DataAccess.Services
{
    public class ImportService
    {
        private readonly string _databasePath;

        public ImportService(string databasePath) =>
            _databasePath = databasePath;

        public virtual async Task<int> ImportDataAsync(General importData)
        {
            using var db = new ApplicationContext(_databasePath);

            await db.Tags.AddRangeAsync(importData.Tags
                .Select(x =>
                    new Models.Tag()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        CreatedAt = x.CreatedAt
                    }));

            await db.Translators.AddRangeAsync(importData.Translators
                .Select(x =>
                    new Models.Translator()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        CreatedAt = x.CreatedAt
                    }));

            await db.Genres.AddRangeAsync(importData.Genres
                .Select(x =>
                    new Models.Genre()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        CreatedAt = x.CreatedAt
                    }));

            await db.Languages.AddRangeAsync(importData.Languages
                .Select(x =>
                    new Models.Language()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        CreatedAt = x.CreatedAt
                    }));

            await db.Publishers.AddRangeAsync(importData.Publishers
                .Select(x =>
                    new Models.Publisher()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        CreatedAt = x.CreatedAt
                    }));

            await db.Authors.AddRangeAsync(importData.Authors
                .Select(x =>
                    new Models.Author()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        DateBirth = x.DateBirth,
                        DateDeath = x.DateDeath,
                        Comment = x.Comment,
                        ImageName = x.ImageName,
                        CreatedAt = x.CreatedAt
                    }));

            await db.Serieses.AddRangeAsync(importData.Serieses
                .Select(x =>
                    new Models.Series()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Comment = x.Comment,
                        CreatedAt = x.CreatedAt
                    }));

            await db.Persons.AddRangeAsync(importData.Persons
                .Select(x =>
                    new Models.Person()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Comment = x.Comment,
                        ImageName = x.ImageName,
                        CreatedAt = x.CreatedAt
                    }));

            foreach (var book in importData.Books)
            {
                await db.Books.AddAsync(new Models.Book()
                {
                    Id = book.Id,
                    Name = book.Name,
                    StatusId = book.StatusId,
                    DateRead = book.DateRead,
                    IndexCode = book.IndexCode,
                    ISBN = book.ISBN,
                    UDC = book.UDC,
                    LBC = book.LBC,
                    IsAudioBook = book.IsAudioBook,
                    IsEBook = book.IsEBook,
                    IsPaperBook = book.IsPaperBook,
                    PublisherId = book.PublisherId,
                    PublishYear = book.PublishYear,
                    WriteYear = book.WriteYear,
                    LanguageId = book.LanguageId,
                    OwnerId = book.OwnerId,
                    LocationId = book.LocationId,
                    SeriesId = book.SeriesId,
                    SeriesNumber = book.SeriesNumber,
                    Annotation = book.Annotation,
                    Comment = book.Comment,
                    ImageName = book.ImageName,
                    CreatedAt = book.CreatedAt
                });

                if (book.Authors != null)
                    await db.BookAuthors.AddRangeAsync(book.Authors
                        .Select(x =>
                            new Models.BookAuthor()
                            {
                                AuthorId = x,
                                BookId = book.Id
                            }));

                if (book.Genres != null)
                    await db.BookGenres.AddRangeAsync(book.Genres
                        .Select(x =>
                            new Models.BookGenre()
                            {
                                GenreId = x,
                                BookId = book.Id
                            }));

                if (book.Translators != null)
                    await db.BookTranslators.AddRangeAsync(book.Translators
                        .Select(x =>
                            new Models.BookTranslator()
                            {
                                TranslatorId = x,
                                BookId = book.Id
                            }));

                if (book.Tags != null)
                    await db.BookTags.AddRangeAsync(book.Tags
                        .Select(x =>
                            new Models.BookTag()
                            {
                                TagId = x,
                                BookId = book.Id
                            }));

                foreach (var content in book.Contents ?? new List<Content>())
                {
                    await db.Contents.AddAsync(new Models.Content()
                    {
                        Id = content.Id,
                        BookId = book.Id,
                        Name = content.Name,
                        StatusId = content.StatusId,
                        WriteYear = content.WriteYear,
                        LanguageId = content.LanguageId,
                        SeriesId = content.SeriesId,
                        SeriesNumber = content.SeriesNumber,
                        PageNumber = content.PageNumber,
                        Level = content.Level,
                        CreatedAt = content.CreatedAt
                    });

                    if (content.Authors != null)
                        await db.ContentAuthors.AddRangeAsync(content.Authors
                            .Select(x =>
                                new Models.ContentAuthor()
                                {
                                    AuthorId = x,
                                    ContentId = content.Id
                                }));

                    if (content.Translators != null)
                        await db.ContentTranslators.AddRangeAsync(content.Translators
                            .Select(x =>
                                new Models.ContentTranslator()
                                {
                                    TranslatorId = x,
                                    ContentId = content.Id
                                }));
                }
            }

            return await db.SaveChangesAsync();
        }
    }
}
