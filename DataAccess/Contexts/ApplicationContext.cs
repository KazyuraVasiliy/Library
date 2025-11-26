using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Contexts
{
    public class ApplicationContext : DbContext
    {
        private readonly string? databasePath;

        public ApplicationContext(string? databasePath = null) =>
            this.databasePath = databasePath;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlite($"Filename={databasePath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .HasMany(b => b.MyBooks)
                .WithOne(p => p.Owner);

            modelBuilder.Entity<Person>()
                .HasMany(b => b.LocationBooks)
                .WithOne(p => p.Location);

            modelBuilder.Entity<Content>()
                .HasMany(c => c.Authors)
                .WithMany(s => s.Contents)
                .UsingEntity<ContentAuthor>();

            modelBuilder.Entity<Content>()
                .HasMany(c => c.Translators)
                .WithMany(s => s.Contents)
                .UsingEntity<ContentTranslator>();

            modelBuilder.Entity<Book>()
                .HasMany(c => c.Authors)
                .WithMany(s => s.Books)
                .UsingEntity<BookAuthor>();

            modelBuilder.Entity<Book>()
                .HasMany(c => c.Tags)
                .WithMany(s => s.Books)
                .UsingEntity<BookTag>();

            modelBuilder.Entity<Book>()
                .HasMany(c => c.Genres)
                .WithMany(s => s.Books)
                .UsingEntity<BookGenre>();

            modelBuilder.Entity<Book>()
                .HasMany(c => c.Translators)
                .WithMany(s => s.Books)
                .UsingEntity<BookTranslator>();

            modelBuilder.Entity<Series>()
                .HasIndex(x => x.Name)
                .IsUnique();

            modelBuilder.Entity<Status>()
                .HasIndex(x => x.Name)
                .IsUnique();

            modelBuilder.Entity<Genre>()
                .HasIndex(x => x.Name)
                .IsUnique();

            modelBuilder.Entity<Language>()
                .HasIndex(x => x.Name)
                .IsUnique();

            modelBuilder.Entity<Publisher>()
                .HasIndex(x => x.Name)
                .IsUnique();

            modelBuilder.Entity<Tag>()
                .HasIndex(x => x.Name)
                .IsUnique();

            modelBuilder.Entity<Translator>()
                .HasIndex(x => x.Name)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Series> Serieses { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Translator> Translators { get; set; }

        public DbSet<BookAuthor> BookAuthors { get; set; }
        public DbSet<BookTag> BookTags { get; set; }
        public DbSet<BookTranslator> BookTranslators { get; set; }
        public DbSet<BookGenre> BookGenres { get; set; }

        public DbSet<ContentAuthor> ContentAuthors { get; set; }
        public DbSet<ContentTranslator> ContentTranslators { get; set; }
    }
}
