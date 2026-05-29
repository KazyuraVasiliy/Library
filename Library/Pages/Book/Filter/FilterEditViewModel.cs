using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DataAccess.Services;
using Library.Messages;
using System.Collections.ObjectModel;

namespace Library.Pages.Book.Filter
{
    public partial class FilterEditViewModel : ObservableObject
    {
        private readonly CollectionService _collectionService;

        [ObservableProperty]
        private ObservableCollection<DataAccess.Models.Status> _statuses =
            new();

        [ObservableProperty]
        private ObservableCollection<DataAccess.Models.Language> _languages =
            new();

        [ObservableProperty]
        private ObservableCollection<DataAccess.Models.Publisher> _publishers =
            new();

        [ObservableProperty]
        private FilterEditModel _filter;

        public FilterEditViewModel(CollectionService collectionService, FilterModel filter)
        {
            _collectionService = collectionService;

            _statuses = _collectionService.GetCollectionAsync<DataAccess.Models.Status>().Result.OrderBy(x => x.Name).ToObservableCollection();
            _languages = _collectionService.GetCollectionAsync<DataAccess.Models.Language>().Result.OrderBy(x => x.Name).ToObservableCollection();
            _publishers = _collectionService.GetCollectionAsync<DataAccess.Models.Publisher>().Result.OrderBy(x => x.Name).ToObservableCollection();

            _filter = new FilterEditModel()
            {
                Status = _statuses.FirstOrDefault(x => x.Id == filter.StatusId),
                Language = _languages.FirstOrDefault(x => x.Id == filter.LanguageId),
                Publisher = _publishers.FirstOrDefault(x => x.Id == filter.PublisherId)
            };
        }

        private async Task Close(bool isReset)
        {
            try
            {
                FilterModel filter = new();

                if (!isReset)
                {
                    filter.StatusId = Filter.Status?.Id;
                    filter.LanguageId = Filter.Language?.Id;
                    filter.PublisherId = Filter.Publisher?.Id;
                }

                WeakReferenceMessenger.Default.Send(new FilterChanged(filter));
                await Shell.Current.Navigation.ClosePopupAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Ошибка", ex.Message + ex?.InnerException?.Message, "Ok");
            }
        }

        [RelayCommand]
        private async Task Ok() =>
            await Close(false);

        [RelayCommand]
        private async Task Reset() =>
            await Close(true);
    }
}
