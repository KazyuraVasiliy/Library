using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DataAccess.Services;
using Library.Messages;
using Library.Services;
using System.Collections.ObjectModel;

namespace Library.Pages.Person
{
    public partial class PersonsViewModel : ObservableObject, IRecipient<PersonsChanged>
    {
        private readonly PersonService _personService;
        private readonly ImageService _imageService;

        [ObservableProperty]
        private string _searchPersonsString =
            string.Empty;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private ObservableCollection<PersonsModel> _persons =
           new();

        [ObservableProperty]
        private ObservableCollection<PersonsModel> _filteredPersons =
           new();

        public PersonsViewModel(PersonService personService, ImageService imageService)
        {
            _personService = personService;
            _imageService = imageService;

            WeakReferenceMessenger.Default.Register<PersonsChanged>(this);
            Initialize();
        }

        private void Initialize()
        {
            Persons = _personService.GetPersonsAsync().Result
                .Select(x => new PersonsModel(_personService, x))
                .ToObservableCollection();

            SearchPersons(SearchPersonsString);
        }

        [RelayCommand]
        private void Refresh()
        {
            Initialize();
            IsRefreshing = false;
        }

        public async void Receive(PersonsChanged message) =>
        await ChangePersons(message.Value);

        private async Task ChangePersons(Guid id)
        {
            var removeItem = Persons.FirstOrDefault(x => x.Person.Id == id);
            var index = 0;

            if (removeItem != null)
            {
                index = Persons.IndexOf(removeItem);
                Persons.Remove(removeItem);
            }

            var addItem = await _personService.GetPersonAsync(id);
            if (addItem != null)
                Persons.Insert(index, new PersonsModel(_personService, addItem));

            SearchPersons(SearchPersonsString);
        }

        [RelayCommand]
        private void SearchPersons(string text) =>
            FilteredPersons = Persons
                .Where(x =>
                    StringService.StringContains(x.Person.Name, text))
                .OrderBy(x => x.Person.Name)
                .ToObservableCollection();

        [RelayCommand]
        private async Task EditPerson(PersonsModel? person = null)
        {
            try
            {
                var selectedPerson = person != null
                    ? await _personService.GetPersonAsync(person.Person.Id)
                    : null;

                var page = new PersonEditPage();
                page.BindingContext = new PersonEditViewModel(_personService, _imageService, new PersonEditModel(_personService, _imageService, selectedPerson));

                await Shell.Current.Navigation.PushAsync(page);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Ошибка", ex.Message + ex?.InnerException?.Message, "Ok");
            }
        }

        [RelayCommand]
        private async Task RemovePerson(PersonsModel person)
        {
            try
            {
                bool question = await Shell.Current.DisplayAlert(
                    "Удалить?", "Вы уверены, что хотите удалить запись?", "Да", "Нет");

                if (!question)
                    return;

                await _personService.RemovePersonAsync(person.Person);
                await ChangePersons(person.Person.Id);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Ошибка", ex.Message + ex?.InnerException?.Message, "Ok");
            }
        }
    }
}
