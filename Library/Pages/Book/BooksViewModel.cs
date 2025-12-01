using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DataAccess.Services;
using Library.Messages;
using Library.Services;
using System.Collections.ObjectModel;

namespace Library.Pages.Book
{
    public partial class BooksViewModel : ObservableObject, IRecipient<BooksChanged>
    {
        private readonly BookService _bookService;
        private readonly AuthorService _authorService;
        private readonly SeriesService _seriesService;
        private readonly PersonService _personService;
        private readonly ImageService _imageService;
        private readonly CollectionService _collectionService;

        [ObservableProperty]
        private string _searchBooksString =
            string.Empty;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private ObservableCollection<BooksModel> _books =
           new();

        [ObservableProperty]
        private ObservableCollection<BooksModel> _filteredBooks =
           new();

        public BooksViewModel(BookService bookService, AuthorService authorService, SeriesService seriesService, PersonService personService, ImageService imageService, CollectionService collectionService)
        {
            _bookService = bookService;
            _authorService = authorService;
            _seriesService = seriesService;
            _personService = personService;
            _imageService = imageService;
            _collectionService = collectionService;

            WeakReferenceMessenger.Default.Register<BooksChanged>(this);

            _selectedSortField = PreferencesService.Book.SortField;
            Initialize();
        }

        private void Initialize()
        {
            Books = _bookService.GetBooksAsync().Result
                .Select(x => new BooksModel(_bookService, x))
                .ToObservableCollection();

            SearchBooks(SearchBooksString);
        }

        [RelayCommand]
        private void Refresh()
        {
            Initialize();
            IsRefreshing = false;
        }

        public async void Receive(BooksChanged message) =>
            await ChangeBooks(message.Value);

        private async Task ChangeBooks(Guid id)
        {
            var removeItem = Books.FirstOrDefault(x => x.Book.Id == id);
            var index = 0;

            if (removeItem != null)
            {
                index = Books.IndexOf(removeItem);
                Books.Remove(removeItem);
            }

            var addItem = await _bookService.GetBookAsync(id);
            if (addItem != null)
                Books.Insert(index, new BooksModel(_bookService, addItem));

            SearchBooks(SearchBooksString);
        }

        [RelayCommand]
        private void SearchBooks(string text)
        {
            var query = Books
                .Where(x =>
                    StringService.StringContains(x.Book.Name, text) ||
                    StringService.StringContains(x.Authors, text) ||
                    StringService.StringContains(x.Book.Series?.Name, text) ||
                    StringService.StringContains(x.Book.ISBN.Replace("-", ""), text.Replace("-", "")));

            if (_selectedSortField == 1)
                query = query.OrderBy(x => x.Book.Name);
            if (_selectedSortField == 2)
                query = query.OrderByDescending(x => x.Book.Name);

            if (_selectedSortField == 3)
                query = query.OrderBy(x => x.Book.CreatedAt);
            if (_selectedSortField == 4)
                query = query.OrderByDescending(x => x.Book.CreatedAt);

            if (_selectedSortField == 5)
                query = query.OrderBy(x => x.Authors);
            if (_selectedSortField == 6)
                query = query.OrderBy(x => x.Book?.Series?.Name).ThenBy(x => x.Book.SeriesNumber);

            FilteredBooks = query
                .ToObservableCollection();
        }

        [RelayCommand]
        private async Task EditBook(BooksModel? book = null)
        {
            try
            {
                var selectedBook = book != null
                    ? await _bookService.GetBookAsync(book.Book.Id)
                    : null;

                var page = new BookEditPage();
                page.BindingContext = new BookEditViewModel(
                    _bookService, 
                    _authorService, 
                    _seriesService, 
                    _personService, 
                    _imageService, 
                    _collectionService, 
                    new BookEditModel(_bookService, _imageService, selectedBook));

                await Shell.Current.Navigation.PushAsync(page);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Ошибка", ex.Message + ex?.InnerException?.Message, "Ok");
            }
        }

        [RelayCommand]
        private async Task RemoveBook(BooksModel book)
        {
            try
            {
                await _bookService.RemoveBookAsync(book.Book);
                await ChangeBooks(book.Book.Id);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Ошибка", ex.Message + ex?.InnerException?.Message, "Ok");
            }
        }

        #region Sort

        private int _selectedSortField;

        private Dictionary<int, string> _sortFields =
            new()
            {
                [1] = "Наименование (возрастание)",
                [2] = "Наименование (убывание)",
                [3] = "Дата добавления (возрастание)",
                [4] = "Дата добавления (убывание)",
                [5] = "Автор",
                [6] = "Серия"
            };

        [RelayCommand]
        private async Task Sort()
        {
            try
            {
                var buttons = _sortFields
                    .Select(x => x.Key == _selectedSortField
                        ? "* " + x.Value
                        : x.Value)
                    .ToArray();
                
                string sortField = await Shell.Current
                    .DisplayActionSheet("Сортировка", "Отмена", null, buttons);

                if (!string.IsNullOrWhiteSpace(sortField) && sortField != "Отмена")
                {
                    sortField = sortField.Trim('*', ' ');

                    _selectedSortField = _sortFields.First(x => x.Value == sortField).Key;
                    PreferencesService.Book.SortField = _selectedSortField;

                    SearchBooks(SearchBooksString);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Ошибка", ex.Message + ex?.InnerException?.Message, "Ok");
            }
        }

        #endregion
    }
}
