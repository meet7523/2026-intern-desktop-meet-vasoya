using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Behaviors
{
    public static class EnterToSendBehavior
    {

            public static readonly DependencyProperty CommandProperty =
                DependencyProperty.RegisterAttached(
                    "Command",
                    typeof(ICommand),
                    typeof(EnterToSendBehavior),
                    new PropertyMetadata(null, OnCommandChanged));

            public static void SetCommand(
                DependencyObject element,
                ICommand value)
            {
                element.SetValue(CommandProperty, value);
            }

            public static ICommand GetCommand(
                DependencyObject element)
            {
                return (ICommand)element.GetValue(CommandProperty);
            }

            private static void OnCommandChanged(
                DependencyObject dependencyObject,
                DependencyPropertyChangedEventArgs e)
            {
                TextBox textBox =
                    dependencyObject as TextBox;

                if (textBox == null)
                    return;

                if (e.OldValue == null &&
                    e.NewValue != null)
                {
                    textBox.PreviewKeyDown +=
                        TextBox_PreviewKeyDown;
                }
                else if (e.OldValue != null &&
                         e.NewValue == null)
                {
                    textBox.PreviewKeyDown -=
                        TextBox_PreviewKeyDown;
                }
            }

            private static void TextBox_PreviewKeyDown(
                object sender,
                KeyEventArgs e)
            {
                TextBox textBox =
                    sender as TextBox;

                if (textBox == null)
                    return;

                // ==========================================
                // SHIFT + ENTER
                // ==========================================
                // Let TextBox handle Enter normally.
                // This creates a new line.
                // ==========================================

                if (e.Key == Key.Enter &&
                    Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    return;
                }

                // ==========================================
                // ENTER
                // ==========================================

                if (e.Key == Key.Enter &&
                    Keyboard.Modifiers == ModifierKeys.None)
                {
                    ICommand command =
                        GetCommand(textBox);

                    if (command == null)
                        return;

                    object parameter =
                        textBox.DataContext;

                    if (command.CanExecute(parameter))
                    {
                        e.Handled = true;

                        command.Execute(parameter);
                    }
                }
            }
        }
    }
