using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DataAccess.Services;
using Library.Messages;
using Library.Services;

namespace Library.Pages.Author
{
    public partial class AuthorEditViewModel : ObservableObject
    {
        private readonly AuthorService _authorService;
        private readonly ImageService _imageService;

        [ObservableProperty]
        private AuthorEditModel _editAuthor;

        public AuthorEditViewModel(AuthorService authorService, ImageService imageService, AuthorEditModel author)
        {
            _authorService = authorService;
            _imageService = imageService;
            _editAuthor = author;

            _imageMenu.Add("Добавить из файла", GetImageFromFile);
            _imageMenu.Add("Добавить по Uri", GetImageFromUri);
            _imageMenu.Add("Удалить", RemoveImage);
        }

        private async Task SaveAuthorImage()
        {
            var sourceImagePath = _authorService.GetAuthorImagePath(EditAuthor.Author.ImageName);
            var sourceThumbImagePath = _authorService.GetAuthorThumbImagePath(EditAuthor.Author.ImageName);

            if (EditAuthor.Author.ImageName != string.Empty)
            {
                await _imageService.RemoveImage(sourceImagePath);
                await _imageService.RemoveImage(sourceThumbImagePath);
            }

            if (EditAuthor.TempImagePath != string.Empty)
            {
                var newImageName = Path.GetFileName(EditAuthor.TempImagePath);

                sourceImagePath = _authorService.GetAuthorImagePath(newImageName);
                sourceThumbImagePath = _authorService.GetAuthorThumbImagePath(newImageName);

                await _imageService.CopyImage(EditAuthor.TempImagePath, sourceImagePath);
                await _imageService.CreateThumbImageAsync(EditAuthor.TempImagePath, sourceThumbImagePath);

                EditAuthor.Author.ImageName = newImageName;
                await _imageService.RemoveImage(EditAuthor.TempImagePath);
            }
        }

        [RelayCommand]
        private async Task SaveAuthor()
        {
            try
            {
                _authorService.CheckAuthorBeforeSave(EditAuthor.Author);

                await SaveAuthorImage();
                await _authorService.EditAuthorAsync(EditAuthor.Author, DateTime.Now);

                WeakReferenceMessenger.Default.Send(new AuthorsChanged(EditAuthor.Author.Id));
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
            var tempImagePath = _authorService.GetAuthorTempImagePath();
            var file = await _imageService.GetTempImageFromStorageAsync(tempImagePath);

            if (file != null)
            {
                EditAuthor.TempImagePath = tempImagePath;
                EditAuthor.IsImageExist = true;
            }
        }

        private async Task GetImageFromUri()
        {
            var uri = await Shell.Current.DisplayPromptAsync(
                "Скачивание файла", "Укажите адрес файла");

            if (!string.IsNullOrWhiteSpace(uri))
            {
                var tempImagePath = _authorService.GetAuthorTempImagePath();
                await _imageService.GetTempImageFromUriAsync(uri, tempImagePath);

                EditAuthor.TempImagePath = tempImagePath;
                EditAuthor.IsImageExist = true;
            }
        }

        private async Task RemoveImage()
        {
            bool question = await Shell.Current.DisplayAlert(
                "Удалить?", "Вы уверены, что хотите удалить изображение?", "Да", "Нет");

            if (question)
            {
                await _imageService.RemoveImage(EditAuthor.TempImagePath);

                EditAuthor.IsImageExist = false;
                EditAuthor.TempImagePath = string.Empty;
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
