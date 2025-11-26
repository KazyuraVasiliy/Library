using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Library.Messages
{
    public class ContentsChanged : ValueChangedMessage<DataAccess.Models.Content>
    {
        public ContentsChanged(DataAccess.Models.Content value) : base(value)
        {
        }
    }
}
