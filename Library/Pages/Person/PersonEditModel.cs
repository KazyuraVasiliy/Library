using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using DataAccess.Services;
using Library.Pages.Book;
using Library.Services;
using System.Collections.ObjectModel;

namespace Library.Pages.Person
{
    public partial class PersonEditModel : ObservableObject
    {
        private readonly PersonService _personService;
        private readonly ImageService _imageService;

        [ObservableProperty]
        public DataAccess.Models.Person _person;

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

        [ObservableProperty]
        private ObservableCollection<BooksModel> _myBooksInMyLibrary;

        [ObservableProperty]
        private ObservableCollection<BooksModel> _myBooksInOtherPersonsLibrary;

        [ObservableProperty]
        private ObservableCollection<BooksModel> _otherPersonsBooksInMyLibrary;

        public PersonEditModel(PersonService personService, ImageService imageService, DataAccess.Models.Person? person)
        {
            _personService = personService;
            _imageService = imageService;

            _person = person ?? new DataAccess.Models.Person();

            _myBooksInMyLibrary = _person.MyBooks
                .Where(x => x.LocationId == _person.Id || x.LocationId == null)
                .Select(x => new BooksModel(null, x))
                .OrderBy(x => x.Book.Name)
                .ToObservableCollection();

            _myBooksInOtherPersonsLibrary = _person.MyBooks
                .Where(x => x.LocationId != _person.Id)
                .Select(x => new BooksModel(null, x))
                .OrderBy(x => x.Book.Name)
                .ToObservableCollection();

            _otherPersonsBooksInMyLibrary = _person.LocationBooks
                .Where(x => x.OwnerId != _person.Id || x.OwnerId == null)
                .Select(x => new BooksModel(null, x))
                .OrderBy(x => x.Book.Name)
                .ToObservableCollection();

            var sourceImagePath = _personService.GetPersonImagePath(_person.ImageName);
            _isImageExist = File.Exists(sourceImagePath);

            if (_isImageExist)
            {
                _tempImagePath = _personService.GetPersonTempImagePath();
                _imageService.GetTempImageFromFileAsync(sourceImagePath, _tempImagePath);
            }
        }
    }
}
