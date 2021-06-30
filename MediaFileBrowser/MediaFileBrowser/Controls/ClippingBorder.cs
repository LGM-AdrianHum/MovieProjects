using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MediaFileBrowser.Controls
{
    public class ClippingBorder : Border
    {
        private readonly RectangleGeometry _clipRect = new RectangleGeometry();
        private object _oldClip;

        public override UIElement Child
        {
            get { return base.Child; }
            set
            {
                if (Equals(Child, value)) return;
                // Restore original clipping
                Child?.SetValue(ClipProperty, _oldClip);

                _oldClip = value?.ReadLocalValue(ClipProperty);

                base.Child = value;
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            OnApplyChildClip();
            base.OnRender(dc);
        }

        protected virtual void OnApplyChildClip()
        {
            var child = Child;
            if (child == null) return;
            _clipRect.RadiusX = _clipRect.RadiusY = Math.Max(0.0, CornerRadius.TopLeft - BorderThickness.Left * 0.5);
            _clipRect.Rect = new Rect(Child.RenderSize);
            child.Clip = _clipRect;
        }
    }
}