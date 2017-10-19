using System.Windows;
using System.Windows.Media;

namespace RemoteMusicPlayerClient
{
    public static class DependencyObjectExtensions
    {
        public static T FindAncestor<T>(this DependencyObject dependencyObject) where T : class
        {
            while (dependencyObject != null && !(dependencyObject is T))
            {
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }
            return dependencyObject as T;
        }
    }
}