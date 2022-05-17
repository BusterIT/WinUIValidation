using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace ValidationExamples
{
    public class Person : INotifyPropertyChanged
    {
        private string _name;
        [Required]
        public string Name { get => _name; set { _name = value; NotifyPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
