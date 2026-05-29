using CommunityToolkit.Mvvm.ComponentModel;

namespace Library.Pages.Book.Filter
{
    public partial class FilterEditModel : ObservableObject
    {
        [ObservableProperty]
        private DataAccess.Models.Status? _status;

        [ObservableProperty]
        private DataAccess.Models.Language? _language;

        [ObservableProperty]
        private DataAccess.Models.Publisher? _publisher;
    }
}
