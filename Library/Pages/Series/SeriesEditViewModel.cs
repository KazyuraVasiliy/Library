using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DataAccess.Services;
using Library.Messages;

namespace Library.Pages.Series
{
    public partial class SeriesEditViewModel : ObservableObject
    {
        private readonly SeriesService _seriesService;

        [ObservableProperty]
        private SeriesEditModel _editSeries;

        public SeriesEditViewModel(SeriesService seriesService, SeriesEditModel series)
        {
            _seriesService = seriesService;
            _editSeries = series;
        }

        [RelayCommand]
        private async Task SaveSeries()
        {
            try
            {
                await _seriesService.CheckSeriesBeforeSave(EditSeries.Series);
                await _seriesService.EditSeriesAsync(EditSeries.Series, DateTime.Now);

                WeakReferenceMessenger.Default.Send(new SeriesesChanged(EditSeries.Series.Id));
                await Shell.Current.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Ошибка", ex.Message + ex?.InnerException?.Message, "Ok");
            }
        }
    }
}
