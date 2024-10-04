using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;

namespace SortableListView
{
	public partial class GridViewColumnDictionary
	{
		private ListView _lastSortedListView = null;
		private GridViewColumnHeader _lastHeaderClicked = null;
		private ListSortDirection _lastDirection = ListSortDirection.Ascending;
		private GridViewSortAdorner listViewSortAdorner = null;
		private GridViewColumnCollection baseGridViewRowPres = null;
		private GridViewColumnHeader lastClickedHeader = null;

		public void SortGridViewColumnHeader(object sender, RoutedEventArgs e)
		{
			try
			{
				var headerClicked = e.OriginalSource as GridViewColumnHeader;
				ListSortDirection direction;

				if (headerClicked != null)
				{
					var currentListView = Utility.FindAncestor<ListView>(headerClicked);

					if (_lastSortedListView != null && _lastSortedListView != currentListView)
					{
						ClearSortingAdorners(_lastSortedListView);
						ClearSortingAdorners(currentListView);
					}

					if (lastClickedHeader != null && lastClickedHeader != headerClicked &&
						baseGridViewRowPres == ((System.Windows.Controls.Primitives.GridViewRowPresenterBase)headerClicked.Parent).Columns)
					{
						ClearHeaderAdorner(lastClickedHeader);
					}
					else if (lastClickedHeader != null && baseGridViewRowPres != ((System.Windows.Controls.Primitives.GridViewRowPresenterBase)headerClicked.Parent).Columns)
					{
						foreach (GridViewColumn head in ((System.Windows.Controls.Primitives.GridViewRowPresenterBase)headerClicked.Parent).Columns)
						{
							var header = head.Header as GridViewColumnHeader;
							if (header != null)
							{
								ClearHeaderAdorner(header);
							}
						}
					}

					lastClickedHeader = headerClicked;

					if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
					{
						if (headerClicked != _lastHeaderClicked)
						{
							direction = ListSortDirection.Ascending;
						}
						else
						{
							direction = _lastDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
						}

						ClearHeaderAdorner(headerClicked);

						listViewSortAdorner = new GridViewSortAdorner(headerClicked, direction);
						AdornerLayer.GetAdornerLayer(headerClicked)?.Add(listViewSortAdorner);

						if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
						{
							_lastHeaderClicked.Column.HeaderTemplate = null;
						}

						_lastHeaderClicked = headerClicked;
						_lastDirection = direction;
						baseGridViewRowPres = ((System.Windows.Controls.Primitives.GridViewRowPresenterBase)headerClicked.Parent).Columns;

						if (currentListView?.ItemsSource != null)
						{
							var gridViewCol = (GridViewColumn)headerClicked.Column;

							string sortBy = gridViewCol.SortBy;

							if (!string.IsNullOrEmpty(sortBy))
							{
								var collectionView = CollectionViewSource.GetDefaultView(currentListView.ItemsSource);
								SetSortingView(collectionView, sortBy, _lastDirection);
								collectionView.Refresh();
							}
						}

						_lastSortedListView = currentListView;
					}
				}
			}
			catch (Exception ex)
			{
			}
		}

		private void ClearSortingAdorners(ListView listView)
		{
			if (listView.View is GridView)
			{
				var headers = Utility.FindVisualChildren<GridViewColumnHeader>(listView);
				foreach (var header in headers)
				{
					ClearHeaderAdorner(header);
				}
			}
		}

		private void ClearHeaderAdorner(GridViewColumnHeader header)
		{
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(header);
			if (adornerLayer != null)
			{
				Adorner[] adorners = adornerLayer.GetAdorners(header);
				if (adorners != null && adorners.Length > 0)
				{
					adornerLayer.Remove(adorners.FirstOrDefault());
				}
			}
		}

		public void SetSortingView(ICollectionView collView, string sortName, ListSortDirection direction)
		{
			collView.SortDescriptions.Clear();
			collView.SortDescriptions.Add(new SortDescription(sortName, direction));
		}

		public static string GetPropertyName(DependencyObject obj)
		{
			return (string)obj.GetValue(PropertyNameProperty);
		}

		public static readonly DependencyProperty PropertyNameProperty =
	DependencyProperty.RegisterAttached(
		"PropertyName",
		typeof(string),
		typeof(GridViewSort),
		new UIPropertyMetadata(null)
	);
	}

	public class GridViewColumn : System.Windows.Controls.GridViewColumn
	{
		public static readonly DependencyProperty WidthUnitProperty = DependencyProperty.Register(
			nameof(WidthUnit),
			typeof(GridLength),
			typeof(GridViewColumn),
			new PropertyMetadata(GridLength.Auto));

		public GridLength WidthUnit
		{
			get => (GridLength)GetValue(WidthUnitProperty);
			set => SetValue(WidthUnitProperty, value);
		}

		public static readonly DependencyProperty MinWidthProperty = DependencyProperty.Register(
			nameof(MinWidth),
			typeof(double),
			typeof(GridViewColumn),
			new PropertyMetadata((double)0));

		public double MinWidth
		{
			get => (double)GetValue(MinWidthProperty);
			set => SetValue(MinWidthProperty, value);
		}

		public static readonly DependencyProperty MaxWidthProperty = DependencyProperty.Register(
			nameof(MaxWidth),
			typeof(double),
			typeof(GridViewColumn),
			new PropertyMetadata(double.PositiveInfinity));

		public double MaxWidth
		{
			get => (double)GetValue(MaxWidthProperty);
			set => SetValue(MaxWidthProperty, value);
		}

		public static readonly DependencyProperty SortByProperty = DependencyProperty.Register(
			nameof(SortBy),
			typeof(string),
			typeof(GridViewColumn),
			new PropertyMetadata(string.Empty));

		public string SortBy
		{
			get => (string)GetValue(SortByProperty);
			set => SetValue(SortByProperty, value);
		}

	}

	public class GridViewSort
	{
		public static DataTemplate GetDescendingHeaderTemplate(DependencyObject obj)
		{
			return (DataTemplate)obj.GetValue(DescendingHeaderTemplateProperty);
		}
		public static void SetDescendingHeaderTemplate(DependencyObject obj, DataTemplate value)
		{
			obj.SetValue(DescendingHeaderTemplateProperty, value);
		}

		public static readonly DependencyProperty DescendingHeaderTemplateProperty = DependencyProperty.RegisterAttached(
			"DescendingHeaderTemplate", typeof(DataTemplate), typeof(GridViewSort));

		public static DataTemplate GetAscendingHeaderTemplate(DependencyObject obj)
		{
			return (DataTemplate)obj.GetValue(AscendingHeaderTemplateProperty);
		}

		public static void SetAscendingHeaderTemplate(DependencyObject obj, DataTemplate value)
		{
			obj.SetValue(AscendingHeaderTemplateProperty, value);
		}

		public static readonly DependencyProperty AscendingHeaderTemplateProperty = DependencyProperty.RegisterAttached(
			"AscendingHeaderTemplate", typeof(DataTemplate), typeof(GridViewSort));

		public static ICommand GetCommand(DependencyObject obj)
		{
			return (ICommand)obj.GetValue(CommandProperty);
		}

		public static void SetCommand(DependencyObject obj, ICommand value)
		{
			obj.SetValue(CommandProperty, value);
		}

		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.RegisterAttached(
				"Command",
				typeof(ICommand),
				typeof(GridViewSort),
				new UIPropertyMetadata(
					null,
					(o, e) =>
					{
						ItemsControl listView = o as ItemsControl;
						if (listView != null)
						{
							if (!GetAutoSort(listView)) 
							{
								if (e.OldValue != null && e.NewValue == null)
								{
									listView.RemoveHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
								}
								if (e.OldValue == null && e.NewValue != null)
								{
									listView.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
								}
							}
						}
					}
				)
			);

		public static bool GetAutoSort(DependencyObject obj)
		{
			return (bool)obj.GetValue(AutoSortProperty);
		}

		public static void SetAutoSort(DependencyObject obj, bool value)
		{
			obj.SetValue(AutoSortProperty, value);
		}

		// Using a DependencyProperty as the backing store for AutoSort.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty AutoSortProperty =
			DependencyProperty.RegisterAttached(
				"AutoSort",
				typeof(bool),
				typeof(GridViewSort),
				new UIPropertyMetadata(false, AutoSort_PropertyChanged));

		private static void AutoSort_PropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			ListView listView = sender as ListView;
			if (listView != null)
			{
				if (GetCommand(listView) == null) // Don't change click handler if a command is set
				{
					bool oldValue = (bool)e.OldValue;
					bool newValue = (bool)e.NewValue;
					if (oldValue && !newValue)
					{
						listView.RemoveHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
					}
					if (!oldValue && newValue)
					{
						listView.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
					}
				}
			}
		}

		public static string GetPropertyName(DependencyObject obj)
		{
			return (string)obj.GetValue(PropertyNameProperty);
		}

		public static void SetPropertyName(DependencyObject obj, string value)
		{
			obj.SetValue(PropertyNameProperty, value);
		}

		// Using a DependencyProperty as the backing store for PropertyName.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty PropertyNameProperty =
			DependencyProperty.RegisterAttached(
				"PropertyName",
				typeof(string),
				typeof(GridViewSort),
				new UIPropertyMetadata(null)
			);

		private static readonly Dictionary<ListView, LastClickData> _lastClick = new Dictionary<ListView, LastClickData>();

		private static void ColumnHeader_Click(object sender, RoutedEventArgs e)
		{
			GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
			if (headerClicked != null)
			{
				ListView listView = Utility.FindAncestor<ListView>(headerClicked);

				LastClickData lastData = null;

				if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
				{
					string propertyName = GetPropertyName(headerClicked.Column);

					ListSortDirection direction = ListSortDirection.Ascending;
					bool applyedSort = false;

					if (!string.IsNullOrEmpty(propertyName))
					{
						if (listView != null)
						{
							ICommand command = GetCommand(listView);
							if (command != null)
							{
								if (command.CanExecute(propertyName))
								{
									command.Execute(propertyName);
								}
							}
							else if (GetAutoSort(listView))
							{
								applyedSort = ApplySort(listView.Items, propertyName, out direction);
							}
						}
					}

					lastData = new LastClickData(headerClicked, direction);

					if (applyedSort)
					{
						if (!_lastClick.TryGetValue(listView, out LastClickData previousData)
							|| !(previousData.Header == headerClicked && previousData.Direction == lastData.Direction))
						{
							if (direction == ListSortDirection.Ascending)
							{
								headerClicked.Column.HeaderTemplate = GetAscendingHeaderTemplate(listView);
							}
							else
							{
								headerClicked.Column.HeaderTemplate = GetDescendingHeaderTemplate(listView);
							}

							if (previousData != null && previousData.Header != lastData.Header)
							{
								previousData.Header.Column.HeaderTemplate = null;
							}
						}
					}
				}

				if (_lastClick.ContainsKey(listView))
				{
					_lastClick[listView] = lastData;
				}
				else
				{
					_lastClick.Add(listView, lastData);
				}
			}
		}

		public static bool ApplySort(ICollectionView view, string propertyName, out ListSortDirection direction)
		{
			bool applyedSort = false;
			direction = ListSortDirection.Ascending;
			if (view.SortDescriptions.Count > 0)
			{
				SortDescription currentSort = view.SortDescriptions[0];
				if (currentSort.PropertyName == propertyName)
				{
					if (currentSort.Direction == ListSortDirection.Ascending)
						direction = ListSortDirection.Descending;
					else
						direction = ListSortDirection.Ascending;
				}
				view.SortDescriptions.Clear();
			}
			if (!string.IsNullOrEmpty(propertyName))
			{
				try
				{
					view.SortDescriptions.Add(new SortDescription(propertyName, direction));
					applyedSort = true;
				}
				catch (ArgumentException)
				{

				}
			}

			return applyedSort;
		}

		class LastClickData
		{
			public readonly GridViewColumnHeader Header;
			public readonly ListSortDirection Direction;

			public LastClickData(GridViewColumnHeader header, ListSortDirection direction)
			{
				Header = header;
				Direction = direction;
			}
		}
	}

	public class GridViewSortAdorner : System.Windows.Documents.Adorner
	{
		private static Geometry ascGeometry =
			Geometry.Parse("M 0 4 L 3.5 0 L 7 4 Z");

		private static Geometry descGeometry =
			Geometry.Parse("M 0 0 L 3.5 4 L 7 0 Z");

		public ListSortDirection Direction { get; private set; }

		public GridViewSortAdorner(UIElement element, ListSortDirection dir)
			: base(element)
		{
			this.Direction = dir;
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);

			if (AdornedElement.RenderSize.Width < 20)
				return;

			TranslateTransform transform = new TranslateTransform
				(
					AdornedElement.RenderSize.Width - 15,
					(AdornedElement.RenderSize.Height - 5) / 2
				);
			drawingContext.PushTransform(transform);


			Geometry geometry = ascGeometry;
			if (this.Direction == ListSortDirection.Descending)
				geometry = descGeometry;

			Brush newColor = Brushes.Blue;
			SolidColorBrush newBrush = (SolidColorBrush)newColor;
			drawingContext.DrawGeometry(newBrush, null, geometry);

			drawingContext.Pop();
		}
	}
}
