using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows;

namespace SortableListView
{
	public static class Utility
	{
		public static T GetLogicalParent<T>(this DependencyObject depObj)
			where T : DependencyObject
		{
			DependencyObject oParent = depObj;
			Type oTargetType = typeof(T);
			do
			{
				oParent = LogicalTreeHelper.GetParent(oParent);
			}
			while (
				!(
					oParent == null
					|| oParent.GetType() == oTargetType
					|| oParent.GetType().IsSubclassOf(oTargetType)
				)
			);

			return oParent as T;
		}
		public static T FindAncestor<T>(DependencyObject obj)
	where T : DependencyObject
		{
			if (obj != null)
			{
				var dependObj = obj;
				do
				{
					dependObj = GetParent(dependObj);
					if (dependObj is T)
						return dependObj as T;
				}
				while (dependObj != null);
			}

			return null;
		}

		public static DependencyObject GetParent(DependencyObject obj)
		{
			if (obj == null)
				return null;
			if (obj is ContentElement)
			{
				var parent = ContentOperations.GetParent(obj as ContentElement);
				if (parent != null)
					return parent;
				if (obj is FrameworkContentElement)
					return (obj as FrameworkContentElement).Parent;
				return null;
			}

			return VisualTreeHelper.GetParent(obj);
		}
		public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
		{
			if (depObj != null)
			{
				for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
				{
					DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
					if (child is T)
					{
						yield return (T)child;
					}

					foreach (T childOfChild in FindVisualChildren<T>(child))
					{
						yield return childOfChild;
					}
				}
			}
		}
	}
}
