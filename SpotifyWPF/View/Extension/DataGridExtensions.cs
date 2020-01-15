using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SpotifyWPF.View.Extension
{
    public class DataGridExtensions
    {
        public static readonly DependencyProperty StartLoadingCommand =
            DependencyProperty.RegisterAttached("StartLoadingCommand", typeof(ICommand), typeof(DataGridExtensions),
                new PropertyMetadata(default(ICommand), OnStartLoadingCommandChanged));

        public static readonly DependencyProperty StopLoadingCommandProperty =
            DependencyProperty.RegisterAttached("StopLoadingCommand", typeof(ICommand), typeof(DataGridExtensions),
                new PropertyMetadata(default(ICommand), OnStopLoadingCommandChanged));

        public static readonly DependencyProperty LoadMoreCommandProperty =
            DependencyProperty.RegisterAttached("LoadMoreCommand", typeof(ICommand), typeof(DataGridExtensions),
                new PropertyMetadata(default(ICommand), OnLoadMoreCommandChanged));

        private static readonly IDictionary<DataGrid, double>
            PrevScrollableHeights = new Dictionary<DataGrid, double>();

        private static void OnStartLoadingCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Init(d, e);
        }

        private static void OnStopLoadingCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Init(d, e);
        }

        private static void OnLoadMoreCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Init(d, e);
        }

        private static void Init(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DataGrid dataGrid)) return;

            if (e.NewValue != null)
            {
                dataGrid.Loaded -= DataGridOnLoaded;
                dataGrid.Loaded += DataGridOnLoaded;
                PrevScrollableHeights[dataGrid] = 0;
            }

            else if (e.OldValue != null)
            {
                dataGrid.Loaded -= DataGridOnLoaded;
            }
        }

        private static void DataGridOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!(sender is DataGrid dataGrid)) return;

            var scrollViewer = UiHelper.FindChildren<ScrollViewer>(dataGrid).FirstOrDefault();

            if (scrollViewer == null) return;

            scrollViewer.ScrollChanged -= ScrollViewerOnScrollChanged;
            scrollViewer.ScrollChanged += ScrollViewerOnScrollChanged;
            dataGrid.IsVisibleChanged -= DataGridIsVisibleChanged;
            dataGrid.IsVisibleChanged += DataGridIsVisibleChanged;

            // Kinda hacky, but visibilty doesn't fire when it's first loaded
            GetStartLoadingCommand(dataGrid)?.Execute(dataGrid);
        }

        private static void DataGridIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is DataGrid dataGrid)) return;

            if (dataGrid.IsVisible)
            {
                var command = GetStartLoadingCommand(dataGrid);
                GetStartLoadingCommand(dataGrid)?.Execute(dataGrid);
            }
            else
            {
                GetStopLoadingCommand(dataGrid)?.Execute(dataGrid);
            }
        }

        private static void ScrollViewerOnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!(sender is ScrollViewer scrollViewer)) return;

            var dataGrid = UiHelper.FindParent<DataGrid>(scrollViewer);
            if (dataGrid == null) return;

            // This means that the scrollbar just showed up
            if (scrollViewer.ScrollableHeight > 0 && PrevScrollableHeights[dataGrid] <= 0)
            {
                GetStopLoadingCommand(dataGrid)?.Execute(dataGrid);
                PrevScrollableHeights[dataGrid] = scrollViewer.ScrollableHeight;

                return;
            }

            PrevScrollableHeights[dataGrid] = scrollViewer.ScrollableHeight;

            // If we have a scrollbar and the scroller is >= 75% from the top, load more
            if (e.VerticalChange > 0 && scrollViewer.ScrollableHeight > 0 && e.VerticalOffset / scrollViewer.ScrollableHeight >= 0.75)
                GetLoadMoreCommand(dataGrid)?.Execute(dataGrid);
        }

        public static void SetStopLoadingCommand(DependencyObject element, ICommand value)
        {
            element.SetValue(StopLoadingCommandProperty, value);
        }

        public static ICommand GetStopLoadingCommand(DependencyObject element)
        {
            return (ICommand) element.GetValue(StopLoadingCommandProperty);
        }

        public static void SetLoadMoreCommand(DependencyObject element, ICommand value)
        {
            element.SetValue(LoadMoreCommandProperty, value);
        }

        public static ICommand GetLoadMoreCommand(DependencyObject element)
        {
            return (ICommand) element.GetValue(LoadMoreCommandProperty);
        }

        public static void SetStartLoadingCommand(DependencyObject element, ICommand value)
        {
            element.SetValue(StartLoadingCommand, value);
        }

        public static ICommand GetStartLoadingCommand(DependencyObject element)
        {
            return (ICommand) element.GetValue(StartLoadingCommand);
        }
    }
}