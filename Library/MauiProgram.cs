using CommunityToolkit.Maui;
using DataAccess.Contexts;
using DataAccess.Services;
using Library.Pages.Author;
using Library.Pages.Book;
using Library.Pages.Person;
using Library.Pages.Series;
using Library.Services;
using Library.Services.ProgressService;
using Maui.NullableDateTimePicker;

namespace Library
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()                
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("FontAwesome-Regular.otf", "FontAwesomeRegular");
                    fonts.AddFont("FontAwesome-Solid.otf", "FontAwesomeSolid");
                })
                .UseMauiCommunityToolkit()
                .ConfigureNullableDateTimePicker();

            // Services
            builder.Services.AddSingleton<PermissionService>();
            builder.Services.AddSingleton<ImageService>();

            // Database
            string databasePath = Path.Combine(FileSystem.AppDataDirectory, "library.db");
            Initialize.InitAsync(databasePath).Wait();

            builder.Services.AddSingleton(x => new BookService(databasePath, FileSystem.AppDataDirectory));
            builder.Services.AddSingleton(x => new SeriesService(databasePath));
            builder.Services.AddSingleton(x => new AuthorService(databasePath, FileSystem.AppDataDirectory));
            builder.Services.AddSingleton(x => new PersonService(databasePath, FileSystem.AppDataDirectory));
            builder.Services.AddSingleton(x => new CollectionService(databasePath));
            builder.Services.AddSingleton(x => new ImportService(databasePath));
            builder.Services.AddSingleton<ProgressService>();

            // ShellContent
            builder.Services.AddSingleton(x => new AppShellViewModel(
                FileSystem.AppDataDirectory,
                x.GetRequiredService<ImportService>(),
                x.GetRequiredService<ImageService>(),
                x.GetRequiredService<ProgressService>()));

            builder.Services.AddSingleton(x => new BooksPage()
            {
                BindingContext = new BooksViewModel(
                    x.GetRequiredService<BookService>(),
                    x.GetRequiredService<AuthorService>(),
                    x.GetRequiredService<SeriesService>(),
                    x.GetRequiredService<PersonService>(),
                    x.GetRequiredService<ImageService>(),
                    x.GetRequiredService<CollectionService>())
            });

            builder.Services.AddSingleton(x => new AuthorsPage()
            {
                BindingContext = new AuthorsViewModel(
                    x.GetRequiredService<AuthorService>(),
                    x.GetRequiredService<ImageService>())
            });

            builder.Services.AddSingleton(x => new SeriesesPage()
            {
                BindingContext = new SeriesesViewModel(
                    x.GetRequiredService<SeriesService>())
            });

            builder.Services.AddSingleton(x => new PersonsPage()
            {
                BindingContext = new PersonsViewModel(
                    x.GetRequiredService<PersonService>(),
                    x.GetRequiredService<ImageService>())
            });

            return builder.Build();
        }
    }
}
