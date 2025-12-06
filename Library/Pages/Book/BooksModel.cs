using CommunityToolkit.Mvvm.ComponentModel;
using DataAccess.Services;

namespace Library.Pages.Book
{
    public partial class BooksModel : ObservableObject
    {
        private readonly BookService? _bookService;

        [ObservableProperty]
        private DataAccess.Models.Book _book;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ImagePath))]
        private bool _isImageExist;

        private string _imagePath = string.Empty;
        public string ImagePath =>
            IsImageExist
                ? _imagePath
                : "no_photo.png";

        private string _sourceImagePath = string.Empty;
        public string SourceImagePath =>
            IsImageExist
                ? _sourceImagePath
                : "no_photo.png";

        public string Authors =>
            string.Join("; ", Book.Authors.OrderBy(x => x.Name).Select(x => x.Name));

        public string Translators =>
            string.Join("; ", Book.Translators.OrderBy(x => x.Name).Select(x => x.Name));

        public BooksModel(BookService? bookService, DataAccess.Models.Book book)
        {
            _bookService = bookService;
            _book = book;

            if (_bookService != null)
            {
                _imagePath = _bookService.GetBookThumbImagePath(_book.ImageName);
                _sourceImagePath = _bookService.GetBookImagePath(_book.ImageName);
                _isImageExist = File.Exists(_imagePath);
            }
        }
    }
}
