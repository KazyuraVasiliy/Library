using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Library.Messages
{
    public class PersonsChanged : ValueChangedMessage<Guid>
    {
        public PersonsChanged(Guid value) : base(value)
        {
        }
    }
}
