using Microsoft.UI.Xaml.Controls;

namespace WinUIValidation
{
    public sealed class SubmitButton : Button
    {
        public SubmitButton()
        {
            Loaded += (s, o) =>
            {
                if (Helper.TryFindParent<Form>(this) == null)
                {
                    throw new Exception("SubmitButton must be used within a FormValidation.Form control that contains a defined Model object.");
                }
            };
        }
    }
}
