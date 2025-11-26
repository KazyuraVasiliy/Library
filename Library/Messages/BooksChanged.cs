using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Library.Messages
{
    public class BooksChanged : ValueChangedMessage<Guid>
    {
        public BooksChanged(Guid value) : base(value)
        {
        }
    }
}
