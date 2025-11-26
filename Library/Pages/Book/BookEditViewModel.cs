using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DataAccess.Services;
using Library.Messages;
using Library.Pages.Book.Content;
using Library.Pages.Content.Content;
using Library.Services;
using System.Collections.ObjectModel;

namespace Library.Pages.Book
{
    public partial class BookEditViewModel : ObservableObject, IRecipient<ContentsChanged>
    {
        private readonly BookService _bookService;
        private readonly AuthorService _authorService;
        private readonly SeriesService _seriesService;
        private readonly PersonService _personService;
        private readonly ImageService _imageService;
        private readonly CollectionService _collectionService;

        [ObservableProperty]
        private BookEditModel _editBook;

        [ObservableProperty]
        private ObservableCollection<DataAccess.Models.Status> _statuses =
            new();

        [ObservableProperty]
        private ObservableCollection<DataAccess.Models.Person> _persons =
            new();

        public BookEditViewModel(BookService bookService, AuthorService authorService, SeriesService seriesService, PersonService personService, ImageService imageService, CollectionService collectionService, BookEditModel? book = null)
        {
            _bookService = bookService;
            _authorService = authorService;
            _seriesService = seriesService;
            _personService = personService;
            _imageService = imageService;
            _collectionService = collectionService;

            _statuses = _collectionService.GetCollectionAsync<DataAccess.Models.Status>().Result.OrderBy(x => x.Name).ToObservableCollection();
            _languages = _collectionService.GetCollectionAsync<DataAccess.Models.Language>().Result.OrderBy(x => x.Name).ToObservableCollection();
            _publishers = _collectionService.GetCollectionAsync<DataAccess.Models.Publisher>().Result.OrderBy(x => x.Name).ToObservableCollection();
            _tags = _collectionService.GetCollectionAsync<DataAccess.Models.Tag>().Result.ToObservableCollection();
            _genres = _collectionService.GetCollectionAsync<DataAccess.Models.Genre>().Result.ToObservableCollection();
            _translators = _collectionService.GetCollectionAsync<DataAccess.Models.Translator>().Result.ToObservableCollection();

            _authors = _authorService.GetAuthorsAsync().Result.ToObservableCollection();
            _serieses = _seriesService.GetSeriesesAsync().Result.OrderBy(x => x.Name).ToObservableCollection();
            _persons = _personService.GetPersonsAsync().Result.OrderBy(x => x.Name).ToObservableCollection();

            _editBook = book ?? new BookEditModel(_bookService, imageService, book?.Book);
            _editBook.Status = _statuses.First(x => x.Id == _editBook.Book.StatusId);

            _imageMenu.Add("Добавить из файла", GetImageFromFile);
            _imageMenu.Add("Добавить по Uri", GetImageFromUri);
            _imageMenu.Add("Удалить", RemoveImage);

            WeakReferenceMessenger.Default.Register<ContentsChanged>(this);
        }

        private async Task SaveBookImage()
        {
            var sourceImagePath = _bookService.GetBookImagePath(EditBook.Book.ImageName);
            var sourceThumbImagePath = _bookService.GetBookThumbImagePath(EditBook.Book.ImageName);

            if (EditBook.Book.ImageName != string.Empty)
            {
                await _imageService.RemoveImage(sourceImagePath);
                await _imageService.RemoveImage(sourceThumbImagePath);
            }

            if (EditBook.TempImagePath != string.Empty)
            {
                var newImageName = Path.GetFileName(EditBook.TempImagePath);

                sourceImagePath = _bookService.GetBookImagePath(newImageName);
                sourceThumbImagePath = _bookService.GetBookThumbImagePath(newImageName);

                await _imageService.CopyImage(EditBook.TempImagePath, sourceImagePath);
                await _imageService.CreateThumbImageAsync(EditBook.TempImagePath, sourceThumbImagePath);

                EditBook.Book.ImageName = newImageName;
                await _imageService.RemoveImage(EditBook.TempImagePath);
            }
        }

        [RelayCommand]
        private async Task SaveBook()
        {
            try
            {
                _bookService.CheckBookBeforeSave(EditBook.Book);

                await SaveBookImage();
                await _bookService.EditBookAsync(EditBook.Book, DateTime.Now);

                WeakReferenceMessenger.Default.Send(new BooksChanged(EditBook.Book.Id));
                await Shell.Current.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Ошибка", ex.Message + ex?.InnerException?.Message, "Ok");
            }
        }

        #region Image

        private Dictionary<string, Func<Task>> _imageMenu =
            new();

        private async Task GetImageFromFile()
        {
            var tempImagePath = _bookService.GetBookTempImagePath();
            var file = await _imageService.GetTempImageFromStorageAsync(tempImagePath);

            if (file != null)
            {
                EditBook.TempImagePath = tempImagePath;
                EditBook.IsImageExist = true;
            }
        }

        private async Task GetImageFromUri()
        {
            var uri = await Shell.Current.DisplayPromptAsync(
                "Скачивание файла", "Укажите адрес файла");

            if (!string.IsNullOrWhiteSpace(uri))
            {
                var tempImagePath = _bookService.GetBookTempImagePath();
                await _imageService.GetTempImageFromUriAsync(uri, tempImagePath);

                EditBook.TempImagePath = tempImagePath;
                EditBook.IsImageExist = true;
            }
        }

        private async Task RemoveImage()
        {
            bool question = await Shell.Current.DisplayAlert(
                "Удалить?", "Вы уверены, что хотите удалить изображение?", "Да", "Нет");

            if (question)
            {
                await _imageService.RemoveImage(EditBook.TempImagePath);

                EditBook.IsImageExist = false;
                EditBook.TempImagePath = string.Empty;
            }
        }

        [RelayCommand]
        private async Task ImageMenu()
        {
            try
            {
                var menuItems = _imageMenu.Select(x => x.Key).ToArray();
                var selectedItem = await Shell.Current.DisplayActionSheet(
                    "Меню", "Отмена", null, menuItems);

                if (_imageMenu.ContainsKey(selectedItem))
                    await _imageMenu[selectedItem].Invoke();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Ошибка", ex.Message + ex?.InnerException?.Message, "Ok");
            }
        }

        #endregion

        #region Authors

        [ObservableProperty]
        private string _searchAuthorsString =
            string.Empty;

        [ObservableProperty]
        private ObservableCollection<DataAccess.Models.Author> _authors =
            new();

        [ObservableProperty]
        private ObservableCollection<DataAccess.Models.Author> _filteredAuthors =
            new();

        [RelayCommand]
        private void AddAuthor(DataAccess.Models.Author author)
        {
            EditBook.Authors.Add(author);
            FilteredAuthors.Remove(author);
        }

        [RelayCommand]
        private void RemoveAuthor(DataAccess.Models.Author author)
        {
            EditBook.Authors.Remove(author);
            SearchAuthors(SearchAuthorsString);
        }

        [RelayCommand]
        private void SearchAuthors(string text) =>
            FilteredAuthors = string.IsNullOrWhiteSpace(text)
                ? new ObservableCollection<DataAccess.Models.Author>()
                : Authors
                    .Where(x => 
                        StringService.StringContains(x.Name, text) &&
                        !EditBook.Authors.Contains(x))
                    .OrderBy(x => x.Name)
                    .ToObservableCollection();

        [RelayCommand]
        private async Task CreateNewAuthor()
        {
            try
            {
                var authorName = await Shell.Current.DisplayPromptAsync(
                    "Создание автора", "Укажите ФИО автора", initialValue: SearchAuthorsString);

                if (!string.IsNullOrWhiteSpace(authorName))
                {
                    var newAuthor = new DataAccess.Models.Author()
                    {
                        Id = Guid.NewGuid(),
                        Name = authorName
                    };

                    _authorService.CheckAuthorBeforeSave(newAuthor);
                    await _authorService.EditAuthorAsync(newAuthor, DateTime.Now);

                    Authors.Add(newAuthor);
                    EditBook.Authors.Add(newAuthor);

                    SearchAuthors(SearchAuthorsString);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Ошибка", ex.Message + ex?.InnerException?.Message, "Ok");
            }
        }

        #endregion

        #region Tags

        [ObservableProperty]
        private string _searchTagsString =
            string.Empty;

        [ObservableProperty]
        private ObservableCollection<DataAccess.Models.Tag> _tags =
            new();

        [ObservableProperty]
        private ObservableCollection<DataAccess.Models.Tag> _filteredTags =
            new();

        [RelayCommand]
        private void AddTag(DataAccess.Models.Tag tag)
        {
            EditBook.Tags.Add(tag);
            FilteredTags.Remove(tag);
        }

        [RelayCommand]
        private void RemoveTag(DataAccess.Models.Tag tag)
        {
            EditBook.Tags.Remove(tag);
            SearchTags(SearchTagsString);
        }

        [RelayCommand]
        private void SearchTags(string text) =>
            FilteredTags = string.IsNullOrWhiteSpace(text)
                ? new ObservableCollection<DataAccess.Models.Tag>()
                : Tags
                    .Where(x => 
                        StringService.StringContains(x.Name, text) &&
                        !EditBook.Tags.Contains(x))
                    .OrderBy(x => x.Name)
                    .ToObservableCollection();

        [RelayCommand]
        private async Task CreateNewTag()
        {
            try
            {
                var tagName = await Shell.Current.DisplayPromptAsync(
                    "Создание тэга", "Укажите имя тэга", initialValue: SearchTagsString);

                if (!string.IsNullOrWhiteSpace(tagName))
                {
                    var newTag = new DataAccess.Models.Tag()
                    {
                        Id = Guid.NewGuid(),
                        Name = tagName
                    };

                    await _collectionService.CheckCollectionItemBeforeSave(newTag);
                    await _collectionService.AddItemAsync(newTag, DateTime.Now);

                    Tags.Add(newTag);
                    EditBook.Tags.Add(newTag);

                    SearchTags(SearchTagsString);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Ошибка", ex.Message + ex?.InnerException?.Message, "Ok");
            }
        }

        #endregion

        #region Genres

        [ObservableProperty]
        private string _searchGenresString =
            string.Empty;

        [ObservableProperty]
        private ObservableCollection<DataAccess.Models.Genre> _genres =
            new();

        [ObservableProperty]
        private ObservableCollection<DataAccess.Models.Genre> _filteredGenres =
            new();

        [RelayCommand]
        private void AddGenre(DataAccess.Models.Genre genre)
        {
            EditBook.Genres.Add(genre);
            FilteredGenres.Remove(genre);
        }

        [RelayCommand]
        private void RemoveGenre(DataAccess.Models.Genre genre)
        {
            EditBook.Genres.Remove(genre);
            SearchGenres(SearchGenresString);
        }

        [RelayCommand]
        private void SearchGenres(string text) =>
            FilteredGenres = string.IsNullOrWhiteSpace(text)
                ? new ObservableCollection<DataAccess.Models.Genre>()
                : Genres
                    .Where(x =>
                        StringService.StringContains(x.Name, text) &&
                        !EditBook.Genres.Contains(x))
                    .OrderBy(x => x.Name)
                    .ToObservableCollection();

        [RelayCommand]
        private async Task CreateNewGenre()
        {
            try
            {
                var genreName = await Shell.Current.DisplayPromptAsync(
                    "Создание жанра", "Укажите название жанра", initialValue: SearchGenresString);

                if (!string.IsNullOrWhiteSpace(genreName))
                {
                    var newGenre = new DataAccess.Models.Genre()
                    {
                        Id = Guid.NewGuid(),
                        Name = genreName
                    };

                    await _collectionService.CheckCollectionItemBeforeSave(newGenre);
                    await _collectionService.AddItemAsync(newGenre, DateTime.Now);

                    Genres.Add(newGenre);
                    EditBook.Genres.Add(newGenre);

                    SearchGenres(SearchGenresString);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Ошибка", ex.Message + ex?.InnerException?.Message, "Ok");
            }
        }

        #endregion

        #region Translators

        [ObservableProperty]
        private string _searchTranslatorsString =
            string.Empty;

        [ObservableProperty]
        private ObservableCollection<DataAccess.Models.Translator> _translators =
            new();

        [ObservableProperty]
        private ObservableCollection<DataAccess.Models.Translator> _filteredTranslators =
            new();

        [RelayCommand]
        private void AddTranslator(DataAccess.Models.Translator translator)
        {
            EditBook.Translators.Add(translator);
            FilteredTranslators.Remove(translator);
        }

        [RelayCommand]
        private void RemoveTranslator(DataAccess.Models.Translator translator)
        {
            EditBook.Translators.Remove(translator);
            SearchTranslators(SearchTranslatorsString);
        }

        [RelayCommand]
        private void SearchTranslators(string text) =>
            FilteredTranslators = string.IsNullOrWhiteSpace(text)
                ? new ObservableCollection<DataAccess.Models.Translator>()
                : Translators
                    .Where(x => 
                        StringService.StringContains(x.Name, text) &&
                        !EditBook.Translators.Contains(x))
                    .OrderBy(x => x.Name)
                    .ToObservableCollection();

        [RelayCommand]
        private async Task CreateNewTranslator()
        {
            try
            {
                var translatorName = await Shell.Current.DisplayPromptAsync(
                    "Создание переводчика", "Укажите ФИО переводчика", initialValue: SearchTranslatorsString);

                if (!string.IsNullOrWhiteSpace(translatorName))
                {
                    var newTranslator = new DataAccess.Models.Translator()
                    {
                        Id = Guid.NewGuid(),
                        Name = translatorName
                    };

                    await _collectionService.CheckCollectionItemBeforeSave(newTranslator);
                    await _collectionService.AddItemAsync(newTranslator, DateTime.Now);

                    Translators.Add(newTranslator);
                    EditBook.Translators.Add(newTranslator);

                    SearchTranslators(SearchTranslatorsString);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Ошибка", ex.Message + ex?.InnerException?.Message, "Ok");
            }
        }

        #endregion

        #region Language

        [ObservableProperty]
        private ObservableCollection<DataAccess.Models.Language> _languages =
            new();

        [RelayCommand]
        private async Task CreateNewLanguage()
        {
            try
            {
                var seriesName = await Shell.Current.DisplayPromptAsync(
                    "Создание языка", "Укажите язык");

                if (!string.IsNullOrWhiteSpace(seriesName))
                {
                    var newLanguage = new DataAccess.Models.Language()
                    {
                        Id = Guid.NewGuid(),
                        Name = seriesName
                    };

                    await _collectionService.CheckCollectionItemBeforeSave(newLanguage);
                    await _collectionService.AddItemAsync(newLanguage, DateTime.Now);

                    Languages.Insert(0, newLanguage);
                    EditBook.Language = newLanguage;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Ошибка", ex.Message + ex?.InnerException?.Message, "Ok");
            }
        }

        #endregion

        #region Series

        [ObservableProperty]
        private ObservableCollection<DataAccess.Models.Series> _serieses =
            new();

        [RelayCommand]
        private async Task CreateNewSeries()
        {
            try
            {
                var seriesName = await Shell.Current.DisplayPromptAsync(
                    "Создание серии", "Укажите название серии");

                if (!string.IsNullOrWhiteSpace(seriesName))
                {
                    var newSeries = new DataAccess.Models.Series()
                    {
                        Id = Guid.NewGuid(),
                        Name = seriesName
                    };

                    await _seriesService.CheckSeriesBeforeSave(newSeries);
                    await _seriesService.EditSeriesAsync(newSeries, DateTime.Now);

                    Serieses.Insert(0, newSeries);
                    EditBook.Series = newSeries;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Ошибка", ex.Message + ex?.InnerException?.Message, "Ok");
            }
        }

        #endregion

        #region Publisher

        [ObservableProperty]
        private ObservableCollection<DataAccess.Models.Publisher> _publishers =
            new();

        [RelayCommand]
        private async Task CreateNewPublisher()
        {
            try
            {
                var seriesName = await Shell.Current.DisplayPromptAsync(
                    "Создание издателя", "Укажите название издателя");

                if (!string.IsNullOrWhiteSpace(seriesName))
                {
                    var newPublisher = new DataAccess.Models.Publisher()
                    {
                        Id = Guid.NewGuid(),
                        Name = seriesName
                    };

                    await _collectionService.CheckCollectionItemBeforeSave(newPublisher);
                    await _collectionService.AddItemAsync(newPublisher, DateTime.Now);

                    Publishers.Insert(0, newPublisher);
                    EditBook.Publisher = newPublisher;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Ошибка", ex.Message + ex?.InnerException?.Message, "Ok");
            }
        }

        #endregion

        #region Contents

        public void Receive(ContentsChanged message) =>
            ChangeContents(message.Value);

        private void ChangeContents(DataAccess.Models.Content content)
        {
            var removeItem = EditBook.Contents.FirstOrDefault(x => x.Content.Id == content.Id);
            if (removeItem != null)
                EditBook.Contents.Remove(removeItem);

            EditBook.Contents.Add(new ContentsModel(content));
            EditBook.Contents = new ObservableCollection<ContentsModel>(EditBook.Contents.OrderBy(x => x.Content.PageNumber));
        }

        [RelayCommand]
        private async Task EditContent(ContentsModel? content = null)
        {
            try
            {
                var copyContent = content != null
                    ? _bookService.CopyContent(content.Content)
                    : new DataAccess.Models.Content()
                    {
                        Language = EditBook.Language,
                        Status = EditBook.Status,
                        Authors = [.. EditBook.Book.Authors],
                        Translators = [.. EditBook.Book.Translators]
                    };

                copyContent.BookId = EditBook.Book.Id;

                var collectionsModel = new CollectionsModel()
                {
                    Authors = Authors,
                    Translators = Translators,
                    Languages = Languages,
                    Publishers = Publishers,
                    Serieses = Serieses,
                    Statuses = Statuses
                };

                var page = new ContentEditPage();
                page.BindingContext = new ContentEditViewModel(
                    _bookService, 
                    _authorService, 
                    _seriesService, 
                    _collectionService, 
                    collectionsModel, 
                    new ContentEditModel(copyContent));

                await Shell.Current.Navigation.PushModalAsync(page);;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Ошибка", ex.Message + ex?.InnerException?.Message, "Ok");
            }
        }

        [RelayCommand]
        private void RemoveContent(ContentsModel content) =>
            EditBook.Contents.Remove(content);

        #endregion
    }
}
