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
        private void SearchBooks(string text) =>
            FilteredBooks = Books
                .Where(x =>
                    StringService.StringContains(x.Book.Name, text))
                .OrderByDescending(x => x.Book.CreatedAt)
                .ToObservableCollection();

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
    }
}
