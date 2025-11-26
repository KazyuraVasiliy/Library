using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Library.Messages
{
    public class AuthorsChanged : ValueChangedMessage<Guid>
    {
        public AuthorsChanged(Guid value) : base(value)
        {
        }
    }
}
