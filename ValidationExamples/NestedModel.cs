using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ValidationExamples
{
    public class NestedModel : INotifyPropertyChanged
    {
        private Person _person;
        public Person Person { get => _person; set { _person = value; NotifyPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
