using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace SpotifyWPF.View.Extension
{
    public static class UiHelper
    {
        public static IList<T> FindChildren<T>(DependencyObject element) where T : FrameworkElement
        {
            var retval = new List<T>();

            for (var counter = 0; counter < VisualTreeHelper.GetChildrenCount(element); counter++)
            {
                if (!(VisualTreeHelper.GetChild(element, counter) is FrameworkElement toAdd)) continue;

                if (toAdd is T correctlyTyped)
                    retval.Add(correctlyTyped);

                else
                    retval.AddRange(FindChildren<T>(toAdd));
            }

            return retval;
        }

        public static T FindParent<T>(DependencyObject element) where T : FrameworkElement
        {
            var parent = VisualTreeHelper.GetParent(element) as FrameworkElement;

            while (parent != null)
            {
                if (parent is T correctlyTyped) return correctlyTyped;

                return FindParent<T>(parent);
            }

            return null;
        }
    }
}
