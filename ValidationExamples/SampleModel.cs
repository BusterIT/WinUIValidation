using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ValidationExamples
{
    public class SampleModel : INotifyPropertyChanged
    {
        private string _requiredText;
        private string _result;

        public ICommand SampleCommand => new RelayCommand((o) => Result = $"Form successful submitted!\n{Guid.NewGuid()}", (p) => true);

        public ICommand CreatePersonCommand => new RelayCommand((o) => Nested = new() { Person = new() { Name = "Martin" } }, (p) => true);
        public ICommand DeleteCommand => new RelayCommand((o) => Nested = null, (p) => true);

        [Required]
        [MaxLength(5)]
        public string RequiredText { get => _requiredText; set { _requiredText = value; NotifyPropertyChanged(); } }

        public string Result { get => _result; set { _result = value; NotifyPropertyChanged(); } }

        private NestedModel _nested;
        public NestedModel Nested { get => _nested; set { _nested = value; NotifyPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
