using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using DataAccess.Services;
using Library.Pages.Book;
using Library.Pages.Book.Content;
using Library.Services;
using System.Collections.ObjectModel;

namespace Library.Pages.Author
{
    public partial class AuthorEditModel : ObservableObject
    {
        private readonly AuthorService _authorService;
        private readonly ImageService _imageService;

        [ObservableProperty]
        public DataAccess.Models.Author _author;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ImagePath))]
        private bool _isImageExist;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ImagePath))]
        public string _tempImagePath =
            string.Empty;

        public string ImagePath =>
            IsImageExist
                ? TempImagePath
                : "no_photo.png";

        public DateTime? DateBirth
        {
            get => Author.DateBirth?.ToDateTime(TimeOnly.MinValue);
            set => Author.DateBirth = value != null
                ? DateOnly.FromDateTime(value.Value)
                : null;
        }

        public DateTime? DateDeath
        {
            get => Author.DateDeath?.ToDateTime(TimeOnly.MinValue);
            set => Author.DateDeath = value != null
                ? DateOnly.FromDateTime(value.Value)
                : null;
        }

        [ObservableProperty]
        private ObservableCollection<BooksModel> _books;

        [ObservableProperty]
        private ObservableCollection<ContentsModel> _contents;

        public AuthorEditModel(AuthorService authorService, ImageService imageService, DataAccess.Models.Author? author)
        {
            _authorService = authorService;
            _imageService = imageService;

            _author = author ?? new DataAccess.Models.Author();

            _books = _author.Books
                .Select(x => new BooksModel(null, x))
                .OrderBy(x => x.Book.Name)
                .ToObservableCollection();

            _contents = _author.Contents
                .Select(x => new ContentsModel(x))
                .OrderBy(x => x.Content.Name)
                .ToObservableCollection();

            var sourceImagePath = _authorService.GetAuthorImagePath(_author.ImageName);
            _isImageExist = File.Exists(sourceImagePath);

            if (_isImageExist)
            {
                _tempImagePath = _authorService.GetAuthorTempImagePath();
                _imageService.GetTempImageFromFileAsync(sourceImagePath, _tempImagePath);
            } 
        }
    }
}
