using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MovieNamer
{
    /// <summary>
    /// When a WPF form is first shown, focus is a tricksy thing.  It's not actually anywhere.
    /// When this attached property is set on the form, it requests a traverse of the focus,
    /// thereby going to the control that has the lowest tab order.
    /// </summary>
    /// <example>
    /// In View xaml
    /// <Window Supporting:FocusBehavior.FocusFirst="true"></window>
    /// </example>
    internal static class FocusBehavior
    {
        internal static readonly DependencyProperty FocusFirstProperty =
            DependencyProperty.RegisterAttached(
                "FocusFirst",
                typeof(bool),
                typeof(Control),
                new PropertyMetadata(false, OnFocusFirstPropertyChanged));

        internal static bool GetFocusFirst(Control control)
        {
            return (bool)control.GetValue(FocusFirstProperty);
        }

        internal static void SetFocusFirst(Control control, bool value)
        {
            control.SetValue(FocusFirstProperty, value);
        }

        internal static void OnFocusFirstPropertyChanged(
            DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var control = obj as Control;
            if (control == null || !(args.NewValue is bool))
            {
                return;
            }

            if ((bool)args.NewValue)
            {
                control.Loaded += (sender, e) =>
                    control.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }
    }
}