using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Library.Pages.Content.Content
{
    public partial class ContentEditModel : ObservableObject
    {
        [ObservableProperty]
        public DataAccess.Models.Content _content;

        public DataAccess.Models.Status? Status
        {
            get => Content.Status;
            set
            {
                if (Content.Status != value)
                {
                    Content.Status = value;
                    OnPropertyChanged();
                }
            }
        }

        public DataAccess.Models.Series? Series
        {
            get => Content.Series;
            set
            {
                if (Content.Series != value)
                {
                    Content.Series = value;
                    OnPropertyChanged();
                }
            }
        }

        public DataAccess.Models.Language? Language
        {
            get => Content.Language;
            set
            {
                if (Content.Language != value)
                {
                    Content.Language = value;
                    OnPropertyChanged();
                }
            }
        }

        [ObservableProperty]
        public ObservableCollection<DataAccess.Models.Translator> _translators;

        [ObservableProperty]
        public ObservableCollection<DataAccess.Models.Author> _authors;

        public ContentEditModel(DataAccess.Models.Content content)
        {
            _content = content;

            _translators = _content.Translators.OrderBy(x => x.Name).ToObservableCollection();
            _authors = _content.Authors.OrderBy(x => x.Name).ToObservableCollection();

            _translators.CollectionChanged += (x, y) =>
            {
                if (y.OldItems != null)
                    _content.Translators.RemoveAll(z => y.OldItems.Cast<DataAccess.Models.Translator>().Contains(z));

                if (y.NewItems != null)
                    _content.Translators.AddRange(y.NewItems.Cast<DataAccess.Models.Translator>());
            };

            _authors.CollectionChanged += (x, y) =>
            {
                if (y.OldItems != null)
                    _content.Authors.RemoveAll(z => y.OldItems.Cast<DataAccess.Models.Author>().Contains(z));

                if (y.NewItems != null)
                    _content.Authors.AddRange(y.NewItems.Cast<DataAccess.Models.Author>());
            };
        }
    }
}
