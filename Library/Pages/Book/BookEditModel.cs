using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using DataAccess;
using DataAccess.Services;
using Library.Pages.Book.Content;
using Library.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Library.Pages.Book
{
    public partial class BookEditModel : ObservableObject
    {
        private readonly BookService _bookService;
        private readonly ImageService _imageService;

        [ObservableProperty]
        public DataAccess.Models.Book _book;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ImagePath))]
        private bool _isImageExist;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ImagePath))]
        public string _tempImagePath =
            string.Empty;

        public string ImagePath =>
            IsImageExist
                ? TempImagePath
                : "no_photo.png";

        public DataAccess.Models.Status? Status
        {
            get => Book.Status;
            set
            {
                if (Book.Status != value)
                {
                    Book.Status = value;
                    OnPropertyChanged();

                    OnPropertyChanged(nameof(IsDateReadEnable));
                }
            }
        }

        public bool IsDateReadEnable =>
            Book.Status?.Id == Constants.Statuses.HaveRead;

        public DateTime? DateRead
        {
            get => Book.DateRead?.ToDateTime(TimeOnly.MinValue);
            set => Book.DateRead = value != null
                ? DateOnly.FromDateTime(value.Value)
                : null;
        }

        public DataAccess.Models.Series? Series
        {
            get => Book.Series;
            set
            {
                if (Book.Series != value)
                {
                    Book.Series = value;
                    OnPropertyChanged();
                }
            }  
        }

        public DataAccess.Models.Publisher? Publisher
        {
            get => Book.Publisher;
            set
            {
                if (Book.Publisher != value)
                {
                    Book.Publisher = value;
                    OnPropertyChanged();
                }
            }
        }

        public DataAccess.Models.Language? Language
        {
            get => Book.Language;
            set
            {
                if (Book.Language != value)
                {
                    Book.Language = value;
                    OnPropertyChanged();
                }
            }
        }

        [ObservableProperty]
        public ObservableCollection<DataAccess.Models.Tag> _tags;

        [ObservableProperty]
        public ObservableCollection<DataAccess.Models.Translator> _translators;

        [ObservableProperty]
        public ObservableCollection<DataAccess.Models.Genre> _genres;

        [ObservableProperty]
        public ObservableCollection<DataAccess.Models.Author> _authors;

        [ObservableProperty]
        public ObservableCollection<ContentsModel> _contents;

        public BookEditModel(BookService bookService, ImageService imageService, DataAccess.Models.Book? book)
        {
            _bookService = bookService;
            _imageService = imageService;

            _book = book ?? new DataAccess.Models.Book();

            _tags = _book.Tags.OrderBy(x => x.Name).ToObservableCollection();
            _translators = _book.Translators.OrderBy(x => x.Name).ToObservableCollection();
            _genres = _book.Genres.OrderBy(x => x.Name).ToObservableCollection();
            _authors = _book.Authors.OrderBy(x => x.Name).ToObservableCollection();
            _contents = _book.Contents.Select(x => new ContentsModel(x)).OrderBy(x => x.Content.PageNumber).ToObservableCollection();

            _tags.CollectionChanged += (x, y) =>
            {
                if (y.OldItems != null)
                    _book.Tags.RemoveAll(z => y.OldItems.Cast<DataAccess.Models.Tag>().Contains(z));

                if (y.NewItems != null)
                    _book.Tags.AddRange(y.NewItems.Cast<DataAccess.Models.Tag>());
            };

            _translators.CollectionChanged += (x, y) =>
            {
                if (y.OldItems != null)
                    _book.Translators.RemoveAll(z => y.OldItems.Cast<DataAccess.Models.Translator>().Contains(z));

                if (y.NewItems != null)
                    _book.Translators.AddRange(y.NewItems.Cast<DataAccess.Models.Translator>());
            };

            _genres.CollectionChanged += (x, y) =>
            {
                if (y.OldItems != null)
                    _book.Genres.RemoveAll(z => y.OldItems.Cast<DataAccess.Models.Genre>().Contains(z));

                if (y.NewItems != null)
                    _book.Genres.AddRange(y.NewItems.Cast<DataAccess.Models.Genre>());
            };

            _authors.CollectionChanged += (x, y) =>
            {
                if (y.OldItems != null)
                    _book.Authors.RemoveAll(z => y.OldItems.Cast<DataAccess.Models.Author>().Contains(z));

                if (y.NewItems != null)
                    _book.Authors.AddRange(y.NewItems.Cast<DataAccess.Models.Author>());
            };

            _contents.CollectionChanged += (x, y) =>
            {
                if (y.OldItems != null)
                    _book.Contents.RemoveAll(z => y.OldItems.Cast<ContentsModel>().Select(x => x.Content).Contains(z));

                if (y.NewItems != null)
                    _book.Contents.AddRange(y.NewItems.Cast<ContentsModel>().Select(x => x.Content));
            };

            var sourceImagePath = _bookService.GetBookImagePath(_book.ImageName);
            _isImageExist = File.Exists(sourceImagePath);

            if (_isImageExist)
            {
                _tempImagePath = _bookService.GetBookTempImagePath();
                _imageService.GetTempImageFromFileAsync(sourceImagePath, _tempImagePath);
            }
        }
    }
}
