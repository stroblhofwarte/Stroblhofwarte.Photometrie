using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using Stroblhofwarte.Photometrie.ViewModel;
using System;
using Stroblhofwarte.FITS.DataObjects;
using Stroblhofwarte.Image;
using Stroblhofwarte.Photometrie.View;

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
                {
                    base.Child.MouseMove -= _controlledObject_MouseMove;
                    base.Child.MouseMove -= _controlledObject_MouseUp;
                }
                base.Child = value;
                base.Child.MouseMove += _controlledObject_MouseMove;
                base.Child.MouseUp += _controlledObject_MouseUp;
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
                var x = Math.Floor(p.X);// * (imageControl.Source.Width / imageControl.ActualWidth));
                var y = Math.Floor(p.Y);// * (imageControl.Source.Height / imageControl.ActualHeight));
                WorldCoordinateSystem wcs = StroblhofwarteImage.Instance.WCS;
                if (wcs != null)
                {
                    // WCS is valid
                    double xRel = x;// imageControl.Source.Width-x;
                    double yRel = y;
                    StroblhofwarteImage.Instance.CursorPosition = new System.Drawing.Point((int)x, (int)y);
                    Coordinates c = wcs.GetCoordinates(xRel, yRel);
                    model.CoordinateText = c.ToString();
                }

            }
            if(model != null)
            {
                FrameworkElement f = sender as FrameworkElement;
                while((f.Parent as ImageView) == null)
                {
                    f = f.Parent as FrameworkElement;
                    if (f == null)
                        break;
                }
                var point = Mouse.GetPosition(f);
                model.FloatingPanelX = (int)point.X + 20;
                model.FloatingPanelY = (int)point.Y + 20;
            }
            // Here you can add some validation logic
            MousePosition = p;
            
        }

        void _controlledObject_MouseUp(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(base.Child);
            // Transfer screen click coordinate to entire image click position
            var imageControl = this.Child as System.Windows.Controls.Image;
            var x = Math.Floor(p.X);// * imageControl.Source.Width / imageControl.ActualWidth);
            var y = Math.Floor(p.Y);// * imageControl.Source.Height / imageControl.ActualHeight);
            StroblhofwarteImage.Instance.CursorClickPosition = new System.Drawing.Point((int)x, (int)y);
        }
    }
}