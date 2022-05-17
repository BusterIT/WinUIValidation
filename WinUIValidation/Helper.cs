using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace WinUIValidation
{
    internal static class Helper
    {
        internal static T TryFindParent<T>(DependencyObject current) where T : class
        {
            DependencyObject parent = VisualTreeHelper.GetParent(current);
            return parent == null ? null : parent is T ? parent as T : TryFindParent<T>(parent);
        }

        internal static SubmitButton GetSubmitButton(DependencyObject @object)
        {
            List<SubmitButton> submitButtons = new();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(@object); i++)
            {
                DependencyObject uiElement = VisualTreeHelper.GetChild(@object, i);
                SubmitButton button;
                if (uiElement is SubmitButton)
                {
                    button = uiElement as SubmitButton;
                }
                else
                {
                    button = GetSubmitButton(uiElement);
                }

                if (button != null)
                {
                    submitButtons.Add(button);
                }
            }

            if (submitButtons.Count > 1)
            {
                throw new Exception("A Validation.From cannot inherits more than one Validation.SubmitButton.");
            }

            return submitButtons.FirstOrDefault();
        }

        internal static object GetPropertyBaseModel(object model, string[] property)
        {
            if (model != null)
            {
                var prop = model.GetType().GetProperty(property[0]);
                return property.Length == 1 ? model : GetPropertyBaseModel(prop.GetValue(model), property.Skip(1).ToArray());
            }
            else
            {
                return null;
            }
        }

        internal static List<ValidationFor> GetValidationFors(DependencyObject @object)
        {
            List<ValidationFor> messages = new();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(@object); i++)
            {
                DependencyObject uiElement = VisualTreeHelper.GetChild(@object, i);
                if (uiElement is ValidationFor)
                {
                    messages.Add((ValidationFor)VisualTreeHelper.GetChild(@object, i));
                }
                else
                {
                    messages.AddRange(GetValidationFors(uiElement));
                }
            }
            return messages;
        }
    }
}
