using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DataAccess.Services;
using Library.Messages;
using Library.Services;

namespace Library.Pages.Person
{
    public partial class PersonEditViewModel : ObservableObject
    {
        private readonly PersonService _personService;
        private readonly ImageService _imageService;

        [ObservableProperty]
        private PersonEditModel _editPerson;

        public PersonEditViewModel(PersonService personService, ImageService imageService, PersonEditModel person)
        {
            _personService = personService;
            _imageService = imageService;
            _editPerson = person;

            _imageMenu.Add("Добавить из файла", GetImageFromFile);
            _imageMenu.Add("Добавить по Uri", GetImageFromUri);
            _imageMenu.Add("Удалить", RemoveImage);
        }

        private async Task SavePersonImage()
        {
            var sourceImagePath = _personService.GetPersonImagePath(EditPerson.Person.ImageName);
            var sourceThumbImagePath = _personService.GetPersonThumbImagePath(EditPerson.Person.ImageName);

            if (EditPerson.Person.ImageName != string.Empty)
            {
                await _imageService.RemoveImage(sourceImagePath);
                await _imageService.RemoveImage(sourceThumbImagePath);
            }

            if (EditPerson.TempImagePath != string.Empty)
            {
                var newImageName = Path.GetFileName(EditPerson.TempImagePath);

                sourceImagePath = _personService.GetPersonImagePath(newImageName);
                sourceThumbImagePath = _personService.GetPersonThumbImagePath(newImageName);

                await _imageService.CopyImage(EditPerson.TempImagePath, sourceImagePath);
                await _imageService.CreateThumbImageAsync(EditPerson.TempImagePath, sourceThumbImagePath);

                EditPerson.Person.ImageName = newImageName;
                await _imageService.RemoveImage(EditPerson.TempImagePath);
            }
        }

        [RelayCommand]
        private async Task SavePerson()
        {
            try
            {
                _personService.CheckPersonBeforeSave(EditPerson.Person);

                await SavePersonImage();
                await _personService.EditPersonAsync(EditPerson.Person, DateTime.Now);

                WeakReferenceMessenger.Default.Send(new PersonsChanged(EditPerson.Person.Id));
                await Shell.Current.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Ошибка", ex.Message + ex?.InnerException?.Message, "Ok");
            }
        }

        private Dictionary<string, Func<Task>> _imageMenu =
            new();

        private async Task GetImageFromFile()
        {
            var tempImagePath = _personService.GetPersonTempImagePath();
            var file = await _imageService.GetTempImageFromStorageAsync(tempImagePath);

            if (file != null)
            {
                EditPerson.TempImagePath = tempImagePath;
                EditPerson.IsImageExist = true;
            }
        }

        private async Task GetImageFromUri()
        {
            var uri = await Shell.Current.DisplayPromptAsync(
                "Скачивание файла", "Укажите адрес файла");

            if (!string.IsNullOrWhiteSpace(uri))
            {
                var tempImagePath = _personService.GetPersonTempImagePath();
                await _imageService.GetTempImageFromUriAsync(uri, tempImagePath);

                EditPerson.TempImagePath = tempImagePath;
                EditPerson.IsImageExist = true;
            }
        }

        private async Task RemoveImage()
        {
            bool question = await Shell.Current.DisplayAlert(
                "Удалить?", "Вы уверены, что хотите удалить изображение?", "Да", "Нет");

            if (question)
            {
                await _imageService.RemoveImage(EditPerson.TempImagePath);

                EditPerson.IsImageExist = false;
                EditPerson.TempImagePath = string.Empty;
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
    }
}
