using System;
using System.Windows;
using System.Windows.Input;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Behaviors
{
   
        public static class WindowDragBehavior
        {
            public static readonly DependencyProperty EnableDragProperty =
                DependencyProperty.RegisterAttached(
                    "EnableDrag",
                    typeof(bool),
                    typeof(WindowDragBehavior),
                    new PropertyMetadata(false, EnableDragChanged));

            public static void SetEnableDrag(
                DependencyObject element,
                bool value)
            {
                element.SetValue(EnableDragProperty, value);
            }

            public static bool GetEnableDrag(
                DependencyObject element)
            {
                return (bool)element.GetValue(EnableDragProperty);
            }

            private static void EnableDragChanged(
                DependencyObject dependencyObject,
                DependencyPropertyChangedEventArgs e)
            {
                UIElement element = dependencyObject as UIElement;

                if (element == null)
                {
                    return;
                }

                bool enabled = (bool)e.NewValue;

                if (enabled)
                {
                    element.MouseLeftButtonDown +=
                        OnMouseLeftButtonDown;
                }
                else
                {
                    element.MouseLeftButtonDown -=
                        OnMouseLeftButtonDown;
                }
            }

            private static void OnMouseLeftButtonDown(
                object sender,
                MouseButtonEventArgs e)
            {
                if (e.ChangedButton != MouseButton.Left)
                {
                    return;
                }

                UIElement element = sender as UIElement;

                if (element == null)
                {
                    return;
                }

                Window window = Window.GetWindow(element);

                if (window == null)
                {
                    return;
                }

                window.DragMove();
            }
        }
    }
