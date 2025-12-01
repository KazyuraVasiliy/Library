using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DataAccess.Services;
using Library.Messages;
using Library.Pages.Content.Content;
using Library.Services;
using System.Collections.ObjectModel;

namespace Library.Pages.Book.Content
{
    public partial class ContentEditViewModel : ObservableObject
    {
        private readonly BookService _bookService;
        private readonly AuthorService _authorService;
        private readonly SeriesService _seriesService;
        private readonly CollectionService _collectionService;

        private const int _takeFilteredItems = 10;

        [ObservableProperty]
        private ContentEditModel _editContent;

        [ObservableProperty]
        private ObservableCollection<DataAccess.Models.Status> _statuses =
            new();

        [ObservableProperty]
        private ObservableCollection<DataAccess.Models.Language> _languages =
            new();

        [ObservableProperty]
        private ObservableCollection<DataAccess.Models.Publisher> _publishers =
            new();

        [ObservableProperty]
        private ObservableCollection<DataAccess.Models.Series> _serieses =
            new();

        public ContentEditViewModel(BookService bookService, AuthorService authorService, SeriesService seriesService, CollectionService collectionService, CollectionsModel collections, ContentEditModel content)
        {
            _bookService = bookService;
            _authorService = authorService;
            _seriesService = seriesService;
            _collectionService = collectionService;

            _statuses = collections.Statuses ?? new ObservableCollection<DataAccess.Models.Status>();
            _languages = collections.Languages ?? new ObservableCollection<DataAccess.Models.Language>();
            _publishers = collections.Publishers ?? new ObservableCollection<DataAccess.Models.Publisher>();
            _serieses = collections.Serieses ?? new ObservableCollection<DataAccess.Models.Series>();
            _authors = collections.Authors ?? new ObservableCollection<DataAccess.Models.Author>();
            _translators = collections.Translators ?? new ObservableCollection<DataAccess.Models.Translator>();

            _editContent = content;
            _editContent.Status = _statuses.First(x => x.Id == _editContent.Content.StatusId);
        }

        [RelayCommand]
        private async Task SaveContent()
        {
            try
            {
                _bookService.CheckContentBeforeSave(EditContent.Content);

                WeakReferenceMessenger.Default.Send(new ContentsChanged(EditContent.Content));
                await Shell.Current.Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Ошибка", ex.Message + ex?.InnerException?.Message, "Ok");
            }
        }

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
            EditContent.Authors.Add(author);
            FilteredAuthors.Remove(author);
        }

        [RelayCommand]
        private void RemoveAuthor(DataAccess.Models.Author author)
        {
            EditContent.Authors.Remove(author);
            SearchAuthors(SearchAuthorsString);
        }

        [RelayCommand]
        private void SearchAuthors(string text) =>
            FilteredAuthors = string.IsNullOrWhiteSpace(text)
                ? new ObservableCollection<DataAccess.Models.Author>()
                : Authors
                    .Where(x =>
                        StringService.StringContains(x.Name, text) &&
                        !EditContent.Authors.Contains(x))
                    .OrderBy(x => x.Name)
                    .Take(_takeFilteredItems)
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
                    EditContent.Authors.Add(newAuthor);

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
            EditContent.Translators.Add(translator);
            FilteredTranslators.Remove(translator);
        }

        [RelayCommand]
        private void RemoveTranslator(DataAccess.Models.Translator translator)
        {
            EditContent.Translators.Remove(translator);
            SearchTranslators(SearchTranslatorsString);
        }

        [RelayCommand]
        private void SearchTranslators(string text) =>
            FilteredTranslators = string.IsNullOrWhiteSpace(text)
                ? new ObservableCollection<DataAccess.Models.Translator>()
                : Translators
                    .Where(x =>
                        StringService.StringContains(x.Name, text) &&
                        !EditContent.Translators.Contains(x))
                    .OrderBy(x => x.Name)
                    .Take(_takeFilteredItems)
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
                    EditContent.Translators.Add(newTranslator);

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
                    EditContent.Series = newSeries;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Ошибка", ex.Message + ex?.InnerException?.Message, "Ok");
            }
        }
    }
}
