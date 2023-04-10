using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using Stroblhofwarte.Photometrie.ViewModel;
using System;
using Stroblhofwarte.FITS.DataObjects;
using Stroblhofwarte.Image;

namespace Stroblhofwarte.Photometrie
{
    public class MouseTrackerDecorator : Decorator
    {
        static readonly DependencyProperty MousePositionProperty;
        static MouseTrackerDecorator()
        {
            MousePositionProperty = DependencyProperty.Register("MousePosition", typeof(Point), typeof(MouseTrackerDecorator));
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
                return (Point)GetValue(MouseTrackerDecorator.MousePositionProperty);
            }
            set
            {
                SetValue(MouseTrackerDecorator.MousePositionProperty, value);
            }
        }

        void _controlledObject_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(base.Child);

            var imageControl = this.Child as System.Windows.Controls.Image;
            ImageViewModel model = this.DataContext as ImageViewModel;
            if (imageControl != null && imageControl.Source != null)
            {
                // Convert from control space to image space
                var x = Math.Floor(p.X * imageControl.Source.Width / imageControl.ActualWidth);
                var y = Math.Floor(p.Y * imageControl.Source.Height / imageControl.ActualHeight);
                WorldCoordinateSystem wcs = StroblhofwarteImage.Instance.WCS;
                if(wcs != null)
                {
                    // WCS is valid
                    double xRel = x+  imageControl.Source.Width/2.0;// imageControl.Source.Width-x;
                    double yRel = imageControl.Source.Height-y;
                    Coordinates c = wcs.GetCoordinates(xRel, yRel);
                    model.CoordinateText = c.ToString();
                }
                
            }

            // Here you can add some validation logic
            MousePosition = p;
        }
    }
}