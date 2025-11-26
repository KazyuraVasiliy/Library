using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DataAccess.Services;
using Library.Messages;
using Library.Services;
using System.Collections.ObjectModel;

namespace Library.Pages.Author
{
    public partial class AuthorsViewModel : ObservableObject, IRecipient<AuthorsChanged>
    {
        private readonly AuthorService _authorService;
        private readonly ImageService _imageService;

        [ObservableProperty]
        private string _searchAuthorsString =
            string.Empty;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private ObservableCollection<AuthorsModel> _authors =
           new();

        [ObservableProperty]
        private ObservableCollection<AuthorsModel> _filteredAuthors =
           new();

        public AuthorsViewModel(AuthorService authorService, ImageService imageService)
        {
            _authorService = authorService;
            _imageService = imageService;

            WeakReferenceMessenger.Default.Register<AuthorsChanged>(this);
            Initialize();
        }

        private void Initialize()
        {
            Authors = _authorService.GetAuthorsAsync().Result
                .Select(x => new AuthorsModel(_authorService, x))
                .ToObservableCollection();

            SearchAuthors(SearchAuthorsString);
        }

        [RelayCommand]
        private void Refresh()
        {
            Initialize();
            IsRefreshing = false;
        }

        public async void Receive(AuthorsChanged message) =>
            await ChangeAuthors(message.Value);

        private async Task ChangeAuthors(Guid id)
        {
            var removeItem = Authors.FirstOrDefault(x => x.Author.Id == id);
            var index = 0;

            if (removeItem != null)
            {
                index = Authors.IndexOf(removeItem);
                Authors.Remove(removeItem);
            }

            var addItem = await _authorService.GetAuthorAsync(id);
            if (addItem != null)
                Authors.Insert(index, new AuthorsModel(_authorService, addItem));

            SearchAuthors(SearchAuthorsString);
        }

        [RelayCommand]
        private void SearchAuthors(string text) =>
            FilteredAuthors = Authors
                .Where(x =>
                    StringService.StringContains(x.Author.Name, text))
                .OrderBy(x => x.Author.Name)
                .ToObservableCollection();

        [RelayCommand]
        private async Task EditAuthor(AuthorsModel? author = null)
        {
            try
            {
                var selectedAuthor = author != null
                    ? await _authorService.GetAuthorAsync(author.Author.Id)
                    : null;

                var page = new AuthorEditPage();
                page.BindingContext = new AuthorEditViewModel(_authorService, _imageService, new AuthorEditModel(_authorService, _imageService, selectedAuthor));

                await Shell.Current.Navigation.PushAsync(page);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Ошибка", ex.Message + ex?.InnerException?.Message, "Ok");
            }
        }

        [RelayCommand]
        private async Task RemoveAuthor(AuthorsModel author)
        {
            try
            {
                bool question = await Shell.Current.DisplayAlert(
                    "Удалить?", "Вы уверены, что хотите удалить запись?", "Да", "Нет");

                if (!question)
                    return;

                await _authorService.RemoveAuthorAsync(author.Author);
                await ChangeAuthors(author.Author.Id);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Ошибка", ex.Message + ex?.InnerException?.Message, "Ok");
            }
        }
    }
}
