using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DataAccess.Services;
using Library.Messages;
using Library.Services;
using System.Collections.ObjectModel;

namespace Library.Pages.Series
{
    public partial class SeriesesViewModel : ObservableObject, IRecipient<SeriesesChanged>
    {
        private readonly SeriesService _seriesService;

        [ObservableProperty]
        private string _searchSeriesesString =
            string.Empty;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private ObservableCollection<SeriesesModel> _serieses =
           new();

        [ObservableProperty]
        private ObservableCollection<SeriesesModel> _filteredSerieses =
            new();

        public SeriesesViewModel(SeriesService seriesService)
        {
            _seriesService = seriesService;

            WeakReferenceMessenger.Default.Register<SeriesesChanged>(this);
            Initialize();
        }

        private void Initialize()
        {
            Serieses = _seriesService.GetSeriesesAsync().Result
                .Select(x => new SeriesesModel(x))
                .ToObservableCollection();

            SearchSerieses(SearchSeriesesString);
        }

        [RelayCommand]
        private void Refresh()
        {
            Initialize();
            IsRefreshing = false;
        }

        public async void Receive(SeriesesChanged message) =>
            await ChangeSerieses(message.Value);

        private async Task ChangeSerieses(Guid id)
        {
            var removeItem = Serieses.FirstOrDefault(x => x.Series.Id == id);
            var index = 0;

            if (removeItem != null)
            {
                index = Serieses.IndexOf(removeItem);
                Serieses.Remove(removeItem);
            }

            var addItem = await _seriesService.GetSeriesAsync(id);
            if (addItem != null) 
                Serieses.Insert(index, new SeriesesModel(addItem));

            SearchSerieses(SearchSeriesesString);
        }

        [RelayCommand]
        private void SearchSerieses(string text) =>
            FilteredSerieses = Serieses
                .Where(x =>
                    StringService.StringContains(x.Series.Name, text))
                .OrderBy(x => x.Series.Name)
                .ToObservableCollection();

        [RelayCommand]
        private async Task EditSeries(SeriesesModel? series = null)
        {
            try
            {
                var selectedSeries = series != null
                    ? await _seriesService.GetSeriesAsync(series.Series.Id)
                    : null;

                var page = new SeriesEditPage();
                page.BindingContext = new SeriesEditViewModel(_seriesService, new SeriesEditModel(selectedSeries));

                await Shell.Current.Navigation.PushAsync(page);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Ошибка", ex.Message + ex?.InnerException?.Message, "Ok");
            }
        }

        [RelayCommand]
        private async Task RemoveSeries(SeriesesModel series)
        {
            try
            {
                bool question = await Shell.Current.DisplayAlert(
                    "Удалить?", "Вы уверены, что хотите удалить запись?", "Да", "Нет");

                if (!question)
                    return;

                await _seriesService.RemoveSeriesAsync(series.Series);
                await ChangeSerieses(series.Series.Id);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Ошибка", ex.Message + ex?.InnerException?.Message, "Ok");
            }
        }
    }
}
