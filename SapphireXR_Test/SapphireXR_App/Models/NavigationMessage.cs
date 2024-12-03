using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SapphireXR_App.Models
{
    public class NavigationMessage : ValueChangedMessage<string>
    {
        public NavigationMessage(string value) : base(value) { }
    }
}
