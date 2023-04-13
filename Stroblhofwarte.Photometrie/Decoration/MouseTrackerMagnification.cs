using Stroblhofwarte.FITS.DataObjects;
using Stroblhofwarte.Image;
using Stroblhofwarte.Photometrie.ViewModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using Stroblhofwarte.Photometrie.ViewModel;
using System;
using Stroblhofwarte.FITS.DataObjects;
using Stroblhofwarte.Image;

namespace Stroblhofwarte.Photometrie
{
    public class MouseTrackerMagnification : Decorator
    {
        static readonly DependencyProperty MousePositionProperty;
        static MouseTrackerMagnification()
        {
            MousePositionProperty = DependencyProperty.Register("MousePosition", typeof(Point), typeof(MouseTrackerMagnification));
        }

        public override UIElement Child
        {
            get
            {
                return base.Child;
            }
            set
            {
                if (base.Child != null)
                    base.Child.MouseMove -= _controlledObject_MouseMove;
                base.Child = value;
                base.Child.MouseMove += _controlledObject_MouseMove;
            }
        }

        public Point MousePosition
        {
            get
            {
                return (Point)GetValue(MouseTrackerMagnification.MousePositionProperty);
            }
            set
            {
                SetValue(MouseTrackerMagnification.MousePositionProperty, value);
            }
        }

        void _controlledObject_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(base.Child);

            var imageControl = this.Child as System.Windows.Controls.Image;
            MagnificationViewModel model = this.DataContext as MagnificationViewModel;

            // Here you can add some validation logic
            MousePosition = p;
        }
    }
}