using CommunityToolkit.Mvvm.ComponentModel;

namespace SapphireXR_App.Models
{
    public class Member : ObservableObject
    {
        public int Id { get; set; }
        private string? _name;
        public string? Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private string? _phone;
        public string? Phone
        {
            get { return _phone; }
            set { SetProperty(ref _phone, value); }
        }
        public DateTime RegDate { get; set; }
        private bool _isUse;
        public bool IsUse
        {
            get { return _isUse; }
            set { SetProperty(ref _isUse, value); }
        }
    }
}
