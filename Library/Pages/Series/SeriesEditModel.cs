using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using Library.Pages.Book;
using Library.Pages.Book.Content;
using System.Collections.ObjectModel;

namespace Library.Pages.Series
{
    public partial class SeriesEditModel : ObservableObject
    {
        [ObservableProperty]
        public DataAccess.Models.Series _series;

        [ObservableProperty]
        private ObservableCollection<BooksModel> _books;

        [ObservableProperty]
        private ObservableCollection<ContentsModel> _contents;

        public SeriesEditModel(DataAccess.Models.Series? series)
        {
            _series = series ?? new DataAccess.Models.Series();

            _books = _series.Books
                .Select(x => new BooksModel(null, x))
                .OrderBy(x => x.Book.SeriesNumber ?? double.MinValue)
                .ToObservableCollection();

            _contents = _series.Contents
                .Select(x => new ContentsModel(x))
                .OrderBy(x => x.Content.SeriesNumber ?? double.MinValue)
                .ToObservableCollection();
        }
    }
}
