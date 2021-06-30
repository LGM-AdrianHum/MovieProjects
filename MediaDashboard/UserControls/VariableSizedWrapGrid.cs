using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace UserControls
{
    /// <summary>
    /// </summary>
    public class VariableSizedWrapGrid : Panel, IScrollInfo
    {
        private Size _extent;
        private IList<Rect> _finalRects;
        private double _itemHeight;
        private double _itemWidth;
        private Vector _offset;
        private Size _viewport;

        private IEnumerable<Rect> ReserveAcreage(Rect acreage, Rect plot)
        {
            if (acreage.IntersectsWith(plot))
            {
                // Above?
                if (plot.Top < acreage.Top)
                {
                    var rest = new Rect(plot.Location, new Size(plot.Width, acreage.Top - plot.Top));
                    yield return rest;
                }

                // Below?
                if (plot.Bottom > acreage.Bottom)
                {
                    var rest = new Rect(new Point(plot.Left, acreage.Bottom),
                        new Size(plot.Width, plot.Bottom - acreage.Bottom));
                    yield return rest;
                }

                // Left?
                if (plot.Left < acreage.Left)
                {
                    var rest = new Rect(plot.Location, new Size(acreage.Left - plot.Left, plot.Height));
                    yield return rest;
                }

                // Right?
                if (plot.Right > acreage.Right)
                {
                    var rest = new Rect(new Point(acreage.Right, plot.Top),
                        new Size(plot.Right - acreage.Right, plot.Height));
                    yield return rest;
                }
            }
            else
            {
                yield return plot;
            }
        }

        private Point PlaceElement(Size requiredSize, ref List<Rect> plots,
            double itemWidth, double itemHeight)
        {
            var location = new Point();

            foreach (var plot in plots)
            {
                if (!(plot.Height >= requiredSize.Height) || !(plot.Width >= requiredSize.Width)) continue;
                var acreage = new Rect(plot.Location, requiredSize);

                Rect innerRect;
                Rect outerRect;
                IComparer<Rect> plotSorter;

                if (Orientation == Orientation.Vertical)
                {
                    innerRect = new Rect(0, 0, acreage.X + itemWidth, acreage.Y);
                    outerRect = new Rect(0, 0, acreage.X, double.MaxValue);
                    plotSorter = new PlotSorterVertical();
                }
                else
                {
                    innerRect = new Rect(0, 0, acreage.X, acreage.Y + itemHeight);
                    outerRect = new Rect(0, 0, double.MaxValue, acreage.Y);
                    plotSorter = new PlotSorterHorizontal();
                }

                List<Rect> localPlots;

                if (StrictItemOrder)
                {
                    localPlots = plots.SelectMany(p => ReserveAcreage(acreage, p))
                        .SelectMany(p => ReserveAcreage(outerRect, p))
                        .SelectMany(p => ReserveAcreage(innerRect, p)).Distinct().ToList();
                }
                else
                {
                    localPlots = plots.SelectMany(p => ReserveAcreage(acreage, p)).Distinct().ToList();
                }

                localPlots.RemoveAll(x => localPlots.Any(y => y.Contains(x) && !y.Equals(x)));
                localPlots.Sort(plotSorter);
                plots = localPlots;

                location = acreage.Location;
                break;
            }

            return location;
        }

        private Rect ArrangeElement(Rect acreage, Size desiredSize, Vector offset)
        {
            var rect = acreage;

            // Adjust horizontal location and size for alignment
            switch (HorizontalChildrenAlignment)
            {
                case HorizontalAlignment.Center:
                    rect.X = rect.X + Math.Max(0, (acreage.Width - desiredSize.Width)/2);
                    rect.Width = desiredSize.Width;
                    break;
                case HorizontalAlignment.Left:
                    rect.Width = desiredSize.Width;
                    break;
                case HorizontalAlignment.Right:
                    rect.X = rect.X + Math.Max(0, acreage.Width - desiredSize.Width);
                    rect.Width = desiredSize.Width;
                    break;
                // ReSharper disable once RedundantCaseLabel
                case HorizontalAlignment.Stretch:
                    break;
            }

            // Adjust vertical location and size for alignment
            switch (VerticalChildrenAlignment)
            {
                case VerticalAlignment.Bottom:
                    rect.Y = rect.Y + Math.Max(0, acreage.Height - desiredSize.Height);
                    rect.Height = desiredSize.Height;
                    break;
                case VerticalAlignment.Center:
                    rect.Y = rect.Y + Math.Max(0, (acreage.Height - desiredSize.Height)/2);
                    rect.Height = desiredSize.Height;
                    break;
                case VerticalAlignment.Top:
                    rect.Height = desiredSize.Height;
                    break;
                // ReSharper disable once RedundantCaseLabel
                case VerticalAlignment.Stretch:
                    // ReSharper disable once RedundantEmptyDefaultSwitchBranch

                    break;
            }

            // Adjust location for scrolling offset
            rect.Location = rect.Location - offset;

            return rect;
        }

        private void SetViewport(Size size)
        {
            if (_viewport == size) return;
            _viewport = size;
            ScrollOwner?.InvalidateScrollInfo();
        }

        private void SetExtent(Size size)
        {
            if (_extent == size) return;
            _extent = size;
            ScrollOwner?.InvalidateScrollInfo();
        }

        /// <summary>When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement" />-derived class. </summary>
        /// <returns>The size that this element determines it needs during layout, based on its calculations of child element sizes.</returns>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        protected override Size MeasureOverride(Size availableSize)
        {
            var desiredSizeMin = new Size();
            var elementSizes = new List<Size>(InternalChildren.Count);

            _itemHeight = ItemHeight;
            _itemWidth = ItemWidth;

            foreach (UIElement element in InternalChildren)
            {
                var elementSize = LatchItemSize
                    ? new Size(double.IsNaN(_itemWidth) ? double.MaxValue : _itemWidth*GetColumnSpan(element),
                        double.IsNaN(_itemHeight) ? double.MaxValue : _itemHeight*GetRowSpan(element))
                    : new Size(double.IsNaN(ItemWidth) ? double.MaxValue : _itemWidth*GetColumnSpan(element),
                        double.IsNaN(ItemHeight) ? double.MaxValue : _itemHeight*GetRowSpan(element));

                // Measure each element providing allocated plot size.
                element.Measure(elementSize);

                // Use the elements desired size as item size in the undefined dimension(s)
                if (double.IsNaN(_itemHeight) || (!LatchItemSize && double.IsNaN(ItemHeight)))
                {
                    elementSize.Height = element.DesiredSize.Height;
                }

                if (double.IsNaN(_itemWidth) || (!LatchItemSize && double.IsNaN(ItemWidth)))
                {
                    elementSize.Width = element.DesiredSize.Width;
                }

                if (double.IsNaN(_itemHeight))
                {
                    _itemHeight = element.DesiredSize.Height/GetRowSpan(element);
                }

                if (double.IsNaN(_itemWidth))
                {
                    _itemWidth = element.DesiredSize.Width/GetColumnSpan(element);
                }

                // The minimum size of the panel is equal to the largest element in each dimension.
                desiredSizeMin.Height = Math.Max(desiredSizeMin.Height, elementSize.Height);
                desiredSizeMin.Width = Math.Max(desiredSizeMin.Width, elementSize.Width);

                elementSizes.Add(elementSize);
            }

            // Always use at least the available size for the panel unless infinite.
            var desiredSize = new Size
            {
                Height = double.IsPositiveInfinity(availableSize.Height) ? 0 : availableSize.Height,
                Width = double.IsPositiveInfinity(availableSize.Width) ? 0 : availableSize.Width
            };

            // Available plots on the panel real estate
            var plots = new List<Rect>();

            // Calculate maximum size
            var maxSize = MaximumRowsOrColumns > 0
                ? new Size(_itemWidth*MaximumRowsOrColumns, _itemHeight*MaximumRowsOrColumns)
                : new Size(double.MaxValue, double.MaxValue);

            // Add the first plot covering the entire estate.
            var bigPlot = new Rect(new Point(0, 0), Orientation == Orientation.Vertical
                ? new Size(double.MaxValue,
                    Math.Max(Math.Min(availableSize.Height, maxSize.Height), desiredSizeMin.Height))
                : new Size(Math.Max(Math.Min(availableSize.Width, maxSize.Width), desiredSizeMin.Width), double.MaxValue));

            plots.Add(bigPlot);

            _finalRects = new List<Rect>(InternalChildren.Count);

            using (var sizeEnumerator = elementSizes.GetEnumerator())
            {
                // ReSharper disable once UnusedVariable
                foreach (UIElement element in InternalChildren)
                {
                    sizeEnumerator.MoveNext();
                    var elementSize = sizeEnumerator.Current;

                    // Find a plot able to hold this element.
                    var acreage = new Rect(
                        PlaceElement(elementSize, ref plots, _itemWidth, _itemHeight), elementSize);

                    _finalRects.Add(acreage);

                    // Keep track of panel size...
                    desiredSize.Height = Math.Max(desiredSize.Height, acreage.Bottom);
                    desiredSize.Width = Math.Max(desiredSize.Width, acreage.Right);
                }
            }

            SetViewport(availableSize);
            SetExtent(desiredSize);

            return desiredSize;
        }

        /// <summary>When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement" /> derived class. </summary>
        /// <returns>The actual size used.</returns>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        protected override Size ArrangeOverride(Size finalSize)
        {
            var actualSize = new Size
            {
                Height = double.IsPositiveInfinity(finalSize.Height) ? 0 : finalSize.Height,
                Width = double.IsPositiveInfinity(finalSize.Width) ? 0 : finalSize.Width
            };

            using (var rectEnumerator = _finalRects.GetEnumerator())
            {
                foreach (UIElement element in InternalChildren)
                {
                    rectEnumerator.MoveNext();
                    var acreage = rectEnumerator.Current;

                    // Keep track of panel size...
                    actualSize.Height = Math.Max(actualSize.Height, acreage.Bottom);
                    actualSize.Width = Math.Max(actualSize.Width, acreage.Right);

                    // Arrange each element using allocated plot location and size.
                    element.Arrange(ArrangeElement(acreage, element.DesiredSize, _offset));
                }
            }

            // Adjust offset when the viewport size changes
            SetHorizontalOffset(Math.Max(0, Math.Min(HorizontalOffset, ExtentWidth - ViewportWidth)));
            SetVerticalOffset(Math.Max(0, Math.Min(VerticalOffset, ExtentHeight - ViewportHeight)));

            return actualSize;
        }

        private class PlotSorterVertical : IComparer<Rect>
        {
            public int Compare(Rect x, Rect y)
            {
                if (x.Left < y.Left)
                    return -1;
                if (x.Left > y.Left)
                    return 1;
                if (x.Top < y.Top)
                    return -1;
                if (x.Top > y.Top)
                    return 1;
                return 0;
            }
        }

        private class PlotSorterHorizontal : IComparer<Rect>
        {
            public int Compare(Rect x, Rect y)
            {
                if (x.Top < y.Top)
                    return -1;
                if (x.Top > y.Top)
                    return 1;
                if (x.Left < y.Left)
                    return -1;
                if (x.Left > y.Left)
                    return 1;
                return 0;
            }
        }

        #region HorizontalAlignment HorizontalChildrenAlignment

        /// <summary>
        /// </summary>
        public HorizontalAlignment HorizontalChildrenAlignment
        {
            get { return (HorizontalAlignment) GetValue(HorizontalChildrenAlignmentProperty); }
            set { SetValue(HorizontalChildrenAlignmentProperty, value); }
        }

        /// <summary>
        /// </summary>
        public static readonly DependencyProperty HorizontalChildrenAlignmentProperty =
            DependencyProperty.Register("HorizontalChildrenAlignment", typeof(HorizontalAlignment),
                typeof(VariableSizedWrapGrid),
                new FrameworkPropertyMetadata(HorizontalAlignment.Left, FrameworkPropertyMetadataOptions.AffectsArrange));

        #endregion

        #region double ItemHeight


        /// <summary>
        /// 
        /// </summary>
        public double ItemHeight
        {
            get { return (double) GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        /// <summary>
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(VariableSizedWrapGrid),
                new FrameworkPropertyMetadata(double.NaN,
                    FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        #endregion

        #region double ItemWidth

        /// <summary>
        /// </summary>
        public double ItemWidth
        {
            get { return (double) GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        /// <summary>
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(VariableSizedWrapGrid),
                new FrameworkPropertyMetadata(double.NaN,
                    FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        #endregion

        #region bool LatchIemSize

        /// <summary>
        /// </summary>
        public bool LatchItemSize
        {
            get { return (bool) GetValue(LatchItemSizeProperty); }
            set { SetValue(LatchItemSizeProperty, value); }
        }

        /// <summary>
        /// </summary>
        public static readonly DependencyProperty LatchItemSizeProperty =
            DependencyProperty.Register("LatchItemSize", typeof(bool), typeof(VariableSizedWrapGrid),
                new FrameworkPropertyMetadata(true,
                    FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        #endregion

        #region int MaximumRowsOrColumns

        /// <summary>
        /// </summary>
        public int MaximumRowsOrColumns
        {
            get { return (int) GetValue(MaximumRowsOrColumnsProperty); }
            set { SetValue(MaximumRowsOrColumnsProperty, value); }
        }

        /// <summary>
        /// </summary>
        public static readonly DependencyProperty MaximumRowsOrColumnsProperty =
            DependencyProperty.Register("MaximumRowsOrColumns", typeof(int), typeof(VariableSizedWrapGrid),
                new FrameworkPropertyMetadata(-1,
                    FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        #endregion

        #region Orientation Orientation

        /// <summary>
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation) GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(VariableSizedWrapGrid),
                new FrameworkPropertyMetadata(Orientation.Vertical,
                    FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        #endregion

        #region bool StrictItemOrder

        /// <summary>
        /// </summary>
        public bool StrictItemOrder
        {
            get { return (bool) GetValue(StrictItemOrderProperty); }
            set { SetValue(StrictItemOrderProperty, value); }
        }

        /// <summary>
        /// </summary>
        public static readonly DependencyProperty StrictItemOrderProperty =
            DependencyProperty.Register("StrictItemOrder", typeof(bool), typeof(VariableSizedWrapGrid),
                new FrameworkPropertyMetadata(true,
                    FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        #endregion

        #region VerticalAlignment VerticalChildrenAlignment

        /// <summary>
        /// </summary>
        public VerticalAlignment VerticalChildrenAlignment
        {
            get { return (VerticalAlignment) GetValue(VerticalChildrenAlignmentProperty); }
            set { SetValue(VerticalChildrenAlignmentProperty, value); }
        }

        /// <summary>
        /// </summary>
        public static readonly DependencyProperty VerticalChildrenAlignmentProperty =
            DependencyProperty.Register("VerticalChildrenAlignment", typeof(VerticalAlignment),
                typeof(VariableSizedWrapGrid),
                new FrameworkPropertyMetadata(VerticalAlignment.Top, FrameworkPropertyMetadataOptions.AffectsArrange));

        #endregion

        #region int ColumnSpan

        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetColumnSpan(DependencyObject obj)
        {
            return (int) obj.GetValue(ColumnSpanProperty);
        }

        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetColumnSpan(DependencyObject obj, int value)
        {
            obj.SetValue(ColumnSpanProperty, value);
        }

        /// <summary>
        /// </summary>
        public static readonly DependencyProperty ColumnSpanProperty =
            DependencyProperty.RegisterAttached("ColumnSpan", typeof(int), typeof(VariableSizedWrapGrid),
                new FrameworkPropertyMetadata(1,
                    FrameworkPropertyMetadataOptions.AffectsParentArrange |
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        #endregion

        #region int RowSpan

        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetRowSpan(DependencyObject obj)
        {
            return (int) obj.GetValue(RowSpanProperty);
        }

        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetRowSpan(DependencyObject obj, int value)
        {
            obj.SetValue(RowSpanProperty, value);
        }

        /// <summary>
        /// </summary>
        public static readonly DependencyProperty RowSpanProperty =
            DependencyProperty.RegisterAttached("RowSpan", typeof(int), typeof(VariableSizedWrapGrid),
                new FrameworkPropertyMetadata(1,
                    FrameworkPropertyMetadataOptions.AffectsParentArrange |
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        #endregion

        #region IScrollInfo

        // This property is not intended for use in your code. It is exposed publicly to fulfill an interface contract (IScrollInfo). Setting this property has no effect.
        public bool CanVerticallyScroll
        {
            get { return false; }
            set { }
        }

        // This property is not intended for use in your code. It is exposed publicly to fulfill an interface contract (IScrollInfo). Setting this property has no effect.
        public bool CanHorizontallyScroll
        {
            get { return false; }
            set { }
        }

        public double ExtentWidth => _extent.Width;

        public double ExtentHeight => _extent.Height;

        public double ViewportWidth => _viewport.Width;

        public double ViewportHeight => _viewport.Height;

        public double HorizontalOffset => _offset.X;

        public double VerticalOffset => _offset.Y;

        public ScrollViewer ScrollOwner { get; set; }

        public void LineUp()
        {
            SetVerticalOffset(VerticalOffset - _itemHeight);
        }

        public void LineDown()
        {
            SetVerticalOffset(VerticalOffset + _itemHeight);
        }

        public void LineLeft()
        {
            SetHorizontalOffset(HorizontalOffset - _itemWidth);
        }

        public void LineRight()
        {
            SetHorizontalOffset(HorizontalOffset + _itemWidth);
        }

        public void PageUp()
        {
            SetVerticalOffset(VerticalOffset - Math.Max(_itemHeight, Math.Max(0, _viewport.Height - _itemHeight)));
        }

        public void PageDown()
        {
            SetVerticalOffset(VerticalOffset + Math.Max(_itemHeight, Math.Max(0, _viewport.Height - _itemHeight)));
        }

        public void PageLeft()
        {
            SetHorizontalOffset(HorizontalOffset - Math.Max(_itemWidth, Math.Max(0, _viewport.Width - _itemWidth)));
        }

        public void PageRight()
        {
            SetHorizontalOffset(HorizontalOffset + Math.Max(_itemWidth, Math.Max(0, _viewport.Width - _itemWidth)));
        }

        public void MouseWheelUp()
        {
            LineUp();
        }

        public void MouseWheelDown()
        {
            LineDown();
        }

        public void MouseWheelLeft()
        {
            LineLeft();
        }

        public void MouseWheelRight()
        {
            LineRight();
        }

        /// <summary>Sets the amount of horizontal offset.</summary>
        /// <param name="offset">The degree to which content is horizontally offset from the containing viewport.</param>
        public void SetHorizontalOffset(double offset)
        {
            offset = Math.Max(0, Math.Min(offset, ExtentWidth - ViewportWidth));
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (offset == _offset.X) return;
            _offset.X = offset;
            ScrollOwner?.InvalidateScrollInfo();
            InvalidateArrange();
        }

        /// <summary>Sets the amount of vertical offset.</summary>
        /// <param name="offset">The degree to which content is vertically offset from the containing viewport.</param>
        public void SetVerticalOffset(double offset)
        {
            offset = Math.Max(0, Math.Min(offset, ExtentHeight - ViewportHeight));
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (offset == _offset.Y) return;
            _offset.Y = offset;
            ScrollOwner?.InvalidateScrollInfo();
            InvalidateArrange();
        }

        /// <summary>
        ///     Forces content to scroll until the coordinate space of a <see cref="T:System.Windows.Media.Visual" /> object
        ///     is visible.
        /// </summary>
        /// <returns>A <see cref="T:System.Windows.Rect" /> that is visible.</returns>
        /// <param name="visual">A <see cref="T:System.Windows.Media.Visual" /> that becomes visible.</param>
        /// <param name="rectangle">A bounding rectangle that identifies the coordinate space to make visible.</param>
        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            if (rectangle.IsEmpty || visual == null || Equals(visual, this) || !IsAncestorOf(visual))
            {
                return Rect.Empty;
            }

            rectangle = visual.TransformToAncestor(this).TransformBounds(rectangle);

            var viewRect = new Rect(HorizontalOffset, VerticalOffset, ViewportWidth, ViewportHeight);

            // Horizontal
            if (rectangle.Right + HorizontalOffset > viewRect.Right)
            {
                viewRect.X = viewRect.X + rectangle.Right + HorizontalOffset - viewRect.Right;
            }
            if (rectangle.Left + HorizontalOffset < viewRect.Left)
            {
                viewRect.X = viewRect.X - (viewRect.Left - (rectangle.Left + HorizontalOffset));
            }

            // Vertical
            if (rectangle.Bottom + VerticalOffset > viewRect.Bottom)
            {
                viewRect.Y = viewRect.Y + rectangle.Bottom + VerticalOffset - viewRect.Bottom;
            }
            if (rectangle.Top + VerticalOffset < viewRect.Top)
            {
                viewRect.Y = viewRect.Y - (viewRect.Top - (rectangle.Top + VerticalOffset));
            }

            SetHorizontalOffset(viewRect.X);
            SetVerticalOffset(viewRect.Y);

            rectangle.Intersect(viewRect);

            return rectangle;
        }

        #endregion
    }
}