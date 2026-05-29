using CommunityToolkit.Mvvm.Messaging.Messages;
using Library.Pages.Book.Filter;

namespace Library.Messages
{
    public class FilterChanged : ValueChangedMessage<FilterModel>
    {
        public FilterChanged(FilterModel value) : base(value)
        {
        }
    }
}
