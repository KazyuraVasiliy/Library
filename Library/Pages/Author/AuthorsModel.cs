using CommunityToolkit.Mvvm.ComponentModel;
using DataAccess.Services;

namespace Library.Pages.Author
{
    public partial class AuthorsModel : ObservableObject
    {
        private readonly AuthorService _authorService;

        [ObservableProperty]
        private DataAccess.Models.Author _author;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ImagePath))]
        private bool _isImageExist;

        private string _imagePath;
        public string ImagePath =>
            IsImageExist
                ? _imagePath
                : "no_photo.png";

        public AuthorsModel(AuthorService authorService, DataAccess.Models.Author author)
        {
            _authorService = authorService;
            _author = author;

            _imagePath = _authorService.GetAuthorThumbImagePath(_author.ImageName);
            _isImageExist = File.Exists(_imagePath);
        }
    }
}
