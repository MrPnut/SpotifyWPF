using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SpotifyWPF.View.Extension
{
    public class DataGridExtensions
    {
        public static readonly DependencyProperty VisibleChangedCommandProperty =
            DependencyProperty.RegisterAttached("VisibleChangedCommand", typeof(ICommand), typeof(DataGridExtensions),
                new PropertyMetadata(default(ICommand), OnVisibleCommandChanged));

        public static readonly DependencyProperty ScrollChangedCommandProperty =
            DependencyProperty.RegisterAttached("ScrollChangedCommand", typeof(ICommand), typeof(DataGridExtensions),
                new PropertyMetadata(default(ICommand), OnScrollChangedCommandChanged));

        private static readonly HashSet<DataGrid>
            AppliedToDataGrids = new HashSet<DataGrid>();

        private static void OnVisibleCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Init(d, e);
        }

        private static void OnScrollChangedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Init(d, e);
        }

        private static void Init(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DataGrid dataGrid)) return;

            if (AppliedToDataGrids.Contains(dataGrid)) return;

            if (e.NewValue != null)
            {
                AppliedToDataGrids.Add(dataGrid);
                dataGrid.Loaded += DataGridOnLoaded;
            }

            else if (e.OldValue != null)
            {
                AppliedToDataGrids.Remove(dataGrid);
                dataGrid.Loaded -= DataGridOnLoaded;
            }
        }

        private static void DataGridOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!(sender is DataGrid dataGrid)) return;

            var scrollViewer = UiHelper.FindChildren<ScrollViewer>(dataGrid).FirstOrDefault();

            if (scrollViewer == null) return;

            scrollViewer.ScrollChanged += ScrollViewerOnScrollChanged;
            dataGrid.IsVisibleChanged += DataGridIsVisibleChanged;

            // Kinda hacky, but visibilty doesn't fire when it's first loaded
            GetVisibleChangedCommand(dataGrid)?.Execute(true);
        }

        private static void DataGridIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is DataGrid dataGrid)) return;

            GetVisibleChangedCommand(dataGrid)?.Execute(dataGrid.IsVisible);
        }

        private static void ScrollViewerOnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!(sender is ScrollViewer scrollViewer)) return;

            var dataGrid = UiHelper.FindParent<DataGrid>(scrollViewer);
            if (dataGrid == null) return;

            GetScrollChangedCommand(dataGrid).Execute(new ScrollInfo(scrollViewer, e));
        }

        public static void SetVisibleChangedCommand(DependencyObject element, ICommand value)
        {
            element.SetValue(VisibleChangedCommandProperty, value);
        }

        public static ICommand GetVisibleChangedCommand(DependencyObject element)
        {
            return (ICommand) element.GetValue(VisibleChangedCommandProperty);
        }

        public static void SetScrollChangedCommand(DependencyObject element, ICommand value)
        {
            element.SetValue(ScrollChangedCommandProperty, value);
        }

        public static ICommand GetScrollChangedCommand(DependencyObject element)
        {
            return (ICommand) element.GetValue(ScrollChangedCommandProperty);
        }
    }

    public class ScrollInfo
    {
        public bool IsScrollable { get; }

        public double ScrollPercentage { get; }

        public ScrollInfo(ScrollViewer scrollViewer, ScrollChangedEventArgs args)
        {
            if (scrollViewer.ScrollableHeight > 0)
            {
                IsScrollable = true;
            }

            if (args.VerticalChange > 0 && IsScrollable)
            {
                ScrollPercentage = args.VerticalOffset / scrollViewer.ScrollableHeight;
            }
        }
    }
}