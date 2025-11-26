using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Library.Messages
{
    public class SeriesesChanged : ValueChangedMessage<Guid>
    {
        public SeriesesChanged(Guid value) : base(value)
        {
        }
    }
}
