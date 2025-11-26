namespace DataAccess.Models.Import
{
    public class General
    {
        public List<Tag> Tags { get; set; } = new();
        public List<Genre> Genres { get; set; } = new();
        public List<Translator> Translators { get; set; } = new();
        public List<Language> Languages { get; set; } = new();
        public List<Publisher> Publishers { get; set; } = new();
        public List<Author> Authors { get; set; } = new();
        public List<Series> Serieses { get; set; } = new();
        public List<Person> Persons { get; set; } = new();
        public List<Book> Books { get; set; } = new();
    }
}
