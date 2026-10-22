using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Behaviors
{
    public static class ContextMenuBehavior
    {

        public static readonly DependencyProperty OpenOnClickProperty =
           DependencyProperty.RegisterAttached(
               "OpenOnClick",
               typeof(bool),
               typeof(ContextMenuBehavior),
               new PropertyMetadata(false, OnOpenOnClickChanged));

        public static void SetOpenOnClick(
            DependencyObject element,
            bool value)
        {
            element.SetValue(OpenOnClickProperty, value);
        }

        public static bool GetOpenOnClick(
            DependencyObject element)
        {
            return (bool)element.GetValue(OpenOnClickProperty);
        }

        private static void OnOpenOnClickChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            Button button = d as Button;

            if (button == null)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                button.Click += Button_Click;
            }
            else
            {
                button.Click -= Button_Click;
            }
        }

        private static void Button_Click(
            object sender,
            RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button == null)
            {
                return;
            }

            if (button.ContextMenu == null)
            {
                return;
            }

            // Position the ContextMenu relative to the 3-dots button
            button.ContextMenu.PlacementTarget = button;

            // Pass the current Chat item to the ContextMenu
            button.ContextMenu.DataContext = button.DataContext;

            // Open menu
            button.ContextMenu.IsOpen = true;
        }
    }
}

