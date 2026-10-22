using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Behaviors
{
        public static class AutoScrollBehavior
        {
            public static readonly DependencyProperty AutoScrollProperty =
                DependencyProperty.RegisterAttached(
                    "AutoScroll",
                    typeof(bool),
                    typeof(AutoScrollBehavior),
                    new PropertyMetadata(false, OnAutoScrollChanged));

            private static readonly DependencyProperty
                CollectionProperty =
                DependencyProperty.RegisterAttached(
                    "Collection",
                    typeof(INotifyCollectionChanged),
                    typeof(AutoScrollBehavior),
                    new PropertyMetadata(null));

            private static readonly DependencyProperty
                CollectionHandlerProperty =
                DependencyProperty.RegisterAttached(
                    "CollectionHandler",
                    typeof(NotifyCollectionChangedEventHandler),
                    typeof(AutoScrollBehavior),
                    new PropertyMetadata(null));

            public static void SetAutoScroll(
                DependencyObject element,
                bool value)
            {
                element.SetValue(
                    AutoScrollProperty,
                    value);
            }

            public static bool GetAutoScroll(
                DependencyObject element)
            {
                return (bool)element.GetValue(
                    AutoScrollProperty);
            }

            private static void OnAutoScrollChanged(
                DependencyObject dependencyObject,
                DependencyPropertyChangedEventArgs e)
            {
                ScrollViewer scrollViewer =
                    dependencyObject as ScrollViewer;

                if (scrollViewer == null)
                    return;

                bool enabled =
                    (bool)e.NewValue;

                if (enabled)
                {
                    scrollViewer.Loaded +=
                        ScrollViewer_Loaded;

                    scrollViewer.Unloaded +=
                        ScrollViewer_Unloaded;
                }
                else
                {
                    Unsubscribe(scrollViewer);

                    scrollViewer.Loaded -=
                        ScrollViewer_Loaded;

                    scrollViewer.Unloaded -=
                        ScrollViewer_Unloaded;
                }
            }

            private static void ScrollViewer_Loaded(
                object sender,
                RoutedEventArgs e)
            {
                ScrollViewer scrollViewer =
                    sender as ScrollViewer;

                if (scrollViewer == null)
                    return;

                Subscribe(scrollViewer);
            }

            private static void ScrollViewer_Unloaded(
                object sender,
                RoutedEventArgs e)
            {
                ScrollViewer scrollViewer =
                    sender as ScrollViewer;

                if (scrollViewer == null)
                    return;

                Unsubscribe(scrollViewer);
            }

            private static void Subscribe(
                ScrollViewer scrollViewer)
            {
                ItemsControl itemsControl =
                    FindItemsControl(scrollViewer);

                if (itemsControl == null)
                    return;

                INotifyCollectionChanged collection =
                    itemsControl.ItemsSource
                        as INotifyCollectionChanged;

                if (collection == null)
                    return;

                Unsubscribe(scrollViewer);

                NotifyCollectionChangedEventHandler handler =
                    delegate (
                        object sender,
                        NotifyCollectionChangedEventArgs args)
                    {
                        if (args.Action ==
                            NotifyCollectionChangedAction.Add)
                        {
                            ScrollToLatestMessage(
                                scrollViewer,
                                itemsControl,
                                args);
                        }
                    };

                collection.CollectionChanged += handler;

                scrollViewer.SetValue(
                    CollectionProperty,
                    collection);

                scrollViewer.SetValue(
                    CollectionHandlerProperty,
                    handler);
            }

            private static void Unsubscribe(
                ScrollViewer scrollViewer)
            {
                INotifyCollectionChanged collection =
                    scrollViewer.GetValue(
                        CollectionProperty)
                    as INotifyCollectionChanged;

                NotifyCollectionChangedEventHandler handler =
                    scrollViewer.GetValue(
                        CollectionHandlerProperty)
                    as NotifyCollectionChangedEventHandler;

                if (collection != null &&
                    handler != null)
                {
                    collection.CollectionChanged -=
                        handler;
                }

                scrollViewer.ClearValue(
                    CollectionProperty);

                scrollViewer.ClearValue(
                    CollectionHandlerProperty);
            }

            private static void ScrollToLatestMessage(
                ScrollViewer scrollViewer,
                ItemsControl itemsControl,
                NotifyCollectionChangedEventArgs args)
            {
                scrollViewer.Dispatcher.BeginInvoke(
                    new Action(
                        delegate
                        {
                            scrollViewer.UpdateLayout();
                            itemsControl.UpdateLayout();

                            if (args.NewItems == null ||
                                args.NewItems.Count == 0)
                                return;

                            int index =
                                args.NewStartingIndex;

                            if (index < 0)
                                index =
                                    itemsControl.Items.Count - 1;

                            if (index >=
                                itemsControl.Items.Count)
                                index =
                                    itemsControl.Items.Count - 1;

                            if (index < 0)
                                return;

                            object item =
                                itemsControl.Items[index];

                            FrameworkElement container =
                                itemsControl.ItemContainerGenerator
                                    .ContainerFromIndex(index)
                                as FrameworkElement;

                            if (container == null)
                                return;

                            container.BringIntoView();

                            scrollViewer.Dispatcher.BeginInvoke(
                                new Action(
                                    delegate
                                    {
                                        Point position =
                                            container.TranslatePoint(
                                                new Point(0, 0),
                                                scrollViewer);

                                        double targetOffset =
                                            scrollViewer.VerticalOffset
                                            + position.Y;

                                        if (targetOffset < 0)
                                            targetOffset = 0;

                                        if (targetOffset >
                                            scrollViewer.ScrollableHeight)
                                        {
                                            targetOffset =
                                                scrollViewer.ScrollableHeight;
                                        }

                                        scrollViewer.ScrollToVerticalOffset(
                                            targetOffset);
                                    }),
                                DispatcherPriority.ContextIdle);

                        }),
                    DispatcherPriority.ContextIdle);
            }

            private static ItemsControl FindItemsControl(
                DependencyObject parent)
            {
                int count =
                    VisualTreeHelper.GetChildrenCount(parent);

                for (int i = 0; i < count; i++)
                {
                    DependencyObject child =
                        VisualTreeHelper.GetChild(
                            parent,
                            i);

                    ItemsControl itemsControl =
                        child as ItemsControl;

                    if (itemsControl != null)
                        return itemsControl;

                    ItemsControl result =
                        FindItemsControl(child);

                    if (result != null)
                        return result;
                }

                return null;
            }
        }
    }






