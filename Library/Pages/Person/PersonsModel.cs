using CommunityToolkit.Mvvm.ComponentModel;
using DataAccess.Services;

namespace Library.Pages.Person
{
    public partial class PersonsModel : ObservableObject
    {
        private readonly PersonService _personService;

        [ObservableProperty]
        private DataAccess.Models.Person _person;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ImagePath))]
        private bool _isImageExist;

        private string _imagePath;
        public string ImagePath =>
            IsImageExist
                ? _imagePath
                : "no_photo.png";

        public PersonsModel(PersonService personService, DataAccess.Models.Person person)
        {
            _personService = personService;
            _person = person;

            _imagePath = _personService.GetPersonThumbImagePath(_person.ImageName);
            _isImageExist = File.Exists(_imagePath);
        }
    }
}
