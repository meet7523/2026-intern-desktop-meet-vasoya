using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Behaviors
{
    
        public static class RenameCommitBehavior
        {
            public static readonly DependencyProperty CommandProperty =
                DependencyProperty.RegisterAttached(
                    "Command",
                    typeof(ICommand),
                    typeof(RenameCommitBehavior),
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
                TextBox textBox = dependencyObject as TextBox;

                if (textBox == null)
                    return;

                if (e.OldValue == null && e.NewValue != null)
                {
                    textBox.Loaded += TextBox_Loaded;
                    textBox.Unloaded += TextBox_Unloaded;
                }
            }

            private static void TextBox_Loaded(
                object sender,
                RoutedEventArgs e)
            {
                TextBox textBox = sender as TextBox;

                if (textBox == null)
                    return;

                Window window = Window.GetWindow(textBox);

                if (window != null)
                {
                    window.PreviewMouseDown +=
                        Window_PreviewMouseDown;
                }

                textBox.Focus();
                textBox.SelectAll();
            }

            private static void TextBox_Unloaded(
                object sender,
                RoutedEventArgs e)
            {
                TextBox textBox = sender as TextBox;

                if (textBox == null)
                    return;

                Window window = Window.GetWindow(textBox);

                if (window != null)
                {
                    window.PreviewMouseDown -=
                        Window_PreviewMouseDown;
                }
            }

            private static void Window_PreviewMouseDown(
                object sender,
                MouseButtonEventArgs e)
            {
                Window window = sender as Window;

                if (window == null)
                    return;

                TextBox textBox = FindVisibleTextBox(window);

                if (textBox == null)
                    return;

                DependencyObject clicked =
                    e.OriginalSource as DependencyObject;

                if (clicked == null)
                    return;

                if (IsInsideTextBox(clicked, textBox))
                    return;

                ICommand command =
                    GetCommand(textBox);

                if (command == null)
                    return;

                object parameter =
                    textBox.DataContext;

                if (command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
            }

            private static TextBox FindVisibleTextBox(
                DependencyObject parent)
            {
                int count =
                    VisualTreeHelper.GetChildrenCount(parent);

                for (int i = 0; i < count; i++)
                {
                    DependencyObject child =
                        VisualTreeHelper.GetChild(parent, i);

                    TextBox textBox =
                        child as TextBox;

                    if (textBox != null &&
                        textBox.IsVisible)
                    {
                        return textBox;
                    }

                    TextBox result =
                        FindVisibleTextBox(child);

                    if (result != null)
                        return result;
                }

                return null;
            }

            private static bool IsInsideTextBox(
                DependencyObject element,
                TextBox textBox)
            {
                DependencyObject current = element;

                while (current != null)
                {
                    if (current == textBox)
                        return true;

                    current =
                        VisualTreeHelper.GetParent(current);
                }

                return false;
            }
        }
    }
