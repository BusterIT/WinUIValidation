using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WinUIValidation
{
    public sealed partial class ValidationFor : UserControl
    {
        private object _formModel;

        List<object> _objectTree;
        private readonly Border _border;
        internal bool _isValid = true;

        /// <summary>
        /// Full property name without form model name
        /// </summary>
        public string PropertyName { get; set; }

        public bool HideValidationMessage { get; set; }

        public bool IgnoreUnsetNestedClasses { get; set; }

        public TextBlock ErrorMessage { get; set; } = new TextBlock()
        {
            Foreground = new SolidColorBrush(Colors.Red),
            Visibility = Visibility.Collapsed,
            TextWrapping = TextWrapping.Wrap,
        };

        public event EventHandler PropertyChanged;

        public ValidationFor()
        {
            this.InitializeComponent();
            Loaded += UserControl_Loaded;
            _border = new Border() { BorderBrush = new SolidColorBrush(Colors.Red) };
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Form parent = Helper.TryFindParent<Form>(this);
            _formModel = parent.Model;
            if (_formModel == null)
            {
                throw new Exception("ValidationMessage must be used within a FormValidation.Form control that contains a defined Model object.");
            }

            GetObjectTree(_formModel, null);

            // Wrap content
            var content = Content;
            Content = null;
            StackPanel myPanel = new();

            if (content != null)
            {
                myPanel.Children.Add(_border);
                _border.Child = content;
            }
            myPanel.Children.Add(ErrorMessage);

            Content = myPanel;
            Loaded -= UserControl_Loaded;
        }

        private void GetObjectTree(object model, PropertyChangedEventArgs args)
        {
            if (args != null && !PropertyName.Contains(args.PropertyName)) return;

            // Unregister events to prevent multiple calls from same model
            if (_objectTree != null)
            {
                foreach (object obj in _objectTree.Where(x => x != null && x.GetType().GetInterface(nameof(INotifyPropertyChanged)) != null))
                {
                    (obj as INotifyPropertyChanged).PropertyChanged -= GetObjectTree;
                }
            }

            _objectTree = new();
            for (int i = 0; i < PropertyName.Split('.').Length; i++)
            {
                _objectTree.Add(Helper.GetPropertyBaseModel(_formModel, PropertyName.Split('.').Take(i + 1).ToArray()));
            }

            foreach (var obj in _objectTree.Where(x => x != null && x.GetType().GetInterface(nameof(INotifyPropertyChanged)) != null))
            {
                if (_objectTree.Last() == obj)
                {
                    (obj as INotifyPropertyChanged).PropertyChanged += ValueChanged;
                }
                else
                {
                    // If one part in a complex object tree changed the tree has to be refreshed
                    // and new created objects changes must be tracked
                    (obj as INotifyPropertyChanged).PropertyChanged += GetObjectTree;
                }
            }

            if (args != null) { Validate(); }
        }

        public void ValueChanged(object model, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == PropertyName.Split('.').Last()) { Validate(); }
        }

        internal void Validate()
        {
            object modelBase = _objectTree.Last();
            if (modelBase == null)
            {
                if (IgnoreUnsetNestedClasses)
                {
                    _isValid = true;
                    ErrorMessage.Text = null;
                    ErrorMessage.Visibility = Visibility.Collapsed;
                    _border.BorderThickness = new Thickness(0);
                }
                else
                {
                    if (!HideValidationMessage)
                    {
                        ErrorMessage.Text = "Parent object '" + string.Join(".", PropertyName.Split('.').Take(PropertyName.Split('.').Length - 1)) + "' is NULL.";
                        ErrorMessage.Visibility = Visibility.Visible;
                    }
                    _border.BorderThickness = new Thickness(1);
                    _isValid = false;
                }
            }
            else
            {
                dynamic value = modelBase.GetType().GetProperty(PropertyName.Split('.').Last()).GetValue(modelBase);
                List<ValidationResult> results = new();

                ValidationContext context = new(modelBase) { MemberName = PropertyName.Split('.').Last() };
                Validator.TryValidateProperty(value, context, results);

                if (results.Any())
                {
                    if (!HideValidationMessage)
                    {
                        ErrorMessage.Text = results.First().ErrorMessage;
                        ErrorMessage.Visibility = Visibility.Visible;
                    }
                    _border.BorderThickness = new Thickness(1);
                    _isValid = false;
                }
                else
                {
                    ErrorMessage.Text = null;
                    ErrorMessage.Visibility = Visibility.Collapsed;
                    _border.BorderThickness = new Thickness(0);
                    _isValid = true;
                }
            }

            PropertyChanged?.Invoke(this, null);
        }
    }
}
