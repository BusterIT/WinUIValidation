using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Windows.Input;

namespace WinUIValidation
{
    public sealed partial class Form : UserControl
    {
        public ICommand OnValidSubmit { get; set; }

        public object CommandParameter { get; set; }

        private SubmitButton _submitButton;

        private List<ValidationFor> _validationItems;

        public INotifyPropertyChanged Model { get; set; }

        public Form()
        {
            this.InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _validationItems = Helper.GetValidationFors(this);
            _submitButton = Helper.GetSubmitButton(this);

            if (_submitButton != null)
            {
                _submitButton.Command = new SubmitCommand(() =>
                {
                    _validationItems.ForEach(x => x.Validate());
                    if (!_validationItems.Any(x => !x._isValid))
                    {
                        OnValidSubmit?.Execute(CommandParameter);
                    }
                });

                // Live control of submit button IsEnabled behaviour during validation state changes
                _validationItems.ForEach(x => x.PropertyChanged += (o, args) => _submitButton.IsEnabled = !_validationItems.Any(v => !v._isValid));
            }
            Loaded -= UserControl_Loaded;
        }
    }
}
