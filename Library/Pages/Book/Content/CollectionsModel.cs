using System.Collections.ObjectModel;

namespace Library.Pages.Book.Content
{
    public class CollectionsModel
    {
        public ObservableCollection<DataAccess.Models.Status>? Statuses;

        public ObservableCollection<DataAccess.Models.Language>? Languages;

        public ObservableCollection<DataAccess.Models.Publisher>? Publishers;

        public ObservableCollection<DataAccess.Models.Series>? Serieses;

        public ObservableCollection<DataAccess.Models.Translator>? Translators;

        public ObservableCollection<DataAccess.Models.Author>? Authors;
    }
}
