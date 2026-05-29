using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataAccess.Services;
using Microcharts;

namespace Library.Pages.Statistic
{
    public partial class StatisticViewModel : ObservableObject
    {
        private readonly StatisticService _statisticService;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private LineChart _haveReadPerYearChart =
            new();

        [ObservableProperty]
        private double _haveReadPerYearWidthRequest;

        [ObservableProperty]
        private LineChart _haveReadPerMonthChart =
            new();

        [ObservableProperty]
        private double _haveReadPerMonthWidthRequest;

        public StatisticViewModel(StatisticService statisticService)
        {
            _statisticService = statisticService;
            _ = InitStatistic();
        }

        [RelayCommand]
        private async Task Refresh()
        {
            await InitStatistic();
            IsRefreshing = false;
        }

        private async Task InitStatistic()
        {
            var statistic = await _statisticService.GetHaveReadPerPeriodStatistic();

            var yearStatistic = statistic
                .Where(x => x.PeriodType == DataAccess.Models.Statistic.PerioTypes.Year)
                .OrderBy(x => x.Period)
                .ToList();

            HaveReadPerYearWidthRequest = yearStatistic.Count * 15;
            HaveReadPerYearChart = new LineChart()
            {
                LineMode = LineMode.Spline,
                LabelTextSize = 24,
                Entries = yearStatistic
                    .Select(x =>
                        new ChartEntry(x.Count)
                        {
                            Label = x.Period.ToString("yyyy"),
                            ValueLabel = x.Count.ToString(),
                            ValueLabelColor = Application.Current!.RequestedTheme == AppTheme.Dark
                                ? SkiaSharp.SKColors.White
                                : SkiaSharp.SKColors.Black,
                            Color = Application.Current!.RequestedTheme == AppTheme.Dark
                                    ? SkiaSharp.SKColors.LightSkyBlue
                                    : SkiaSharp.SKColors.Blue
                        }),
                LabelColor = Application.Current!.RequestedTheme == AppTheme.Dark
                    ? SkiaSharp.SKColors.White
                    : SkiaSharp.SKColors.Black,
                BackgroundColor = Application.Current!.RequestedTheme == AppTheme.Dark
                    ? SkiaSharp.SKColors.Black
                    : SkiaSharp.SKColors.White
            };

            var monthStatistic = statistic
                .Where(x => x.PeriodType == DataAccess.Models.Statistic.PerioTypes.Month)
                .OrderBy(x => x.Period)
                .ToList();

            HaveReadPerMonthWidthRequest = monthStatistic.Count * 15;
            HaveReadPerMonthChart = new LineChart()
            {
                LineMode = LineMode.Spline,
                LabelTextSize = 24,
                Entries = monthStatistic
                    .Select(x =>
                        new ChartEntry(x.Count)
                        {
                            Label = x.Period.ToString("MM.yy"),
                            ValueLabel = x.Count.ToString(),
                            ValueLabelColor = Application.Current!.RequestedTheme == AppTheme.Dark
                                ? SkiaSharp.SKColors.White
                                : SkiaSharp.SKColors.Black,
                            Color = Application.Current!.RequestedTheme == AppTheme.Dark
                                    ? SkiaSharp.SKColors.LightSkyBlue
                                    : SkiaSharp.SKColors.Blue
                        }),
                LabelColor = Application.Current!.RequestedTheme == AppTheme.Dark
                    ? SkiaSharp.SKColors.White
                    : SkiaSharp.SKColors.Black,
                BackgroundColor = Application.Current!.RequestedTheme == AppTheme.Dark
                    ? SkiaSharp.SKColors.Black
                    : SkiaSharp.SKColors.White
            };

            await Shell.Current.Dispatcher.DispatchAsync(async () =>
            {
                if (Shell.Current.CurrentPage is StatisticPage statisticPage)
                    await statisticPage.haveReadPerMonthScrollView.ScrollToAsync(HaveReadPerMonthWidthRequest, 0, true);
            });
        }
    }
}
