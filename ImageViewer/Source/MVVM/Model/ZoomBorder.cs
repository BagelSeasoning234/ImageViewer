using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ImageViewer.Source.MVVM.Model
{
    public class ZoomBorder : Border
    {
        #region Private Members

        // The internal UIElement for the image.
        private UIElement child = null;
        // The previous location of the image (prior to where it's currently being panned to).
        private Point previousLocation;
        // The starting position when the mouse pans the image.
        private Point start;

        // The amount to zoom when the user zooms in.
        private double zoomIncrement = 0.125;
        // The minimum value that the image can zoom out to.
        private double minZoom = 0.75;
        // The maximum value that the image can zoom in to.
        private double maxZoom = 4;

        // The maximum distance that the image can be panned in any direction.
        private double maxTransformDistance = 400;

        #endregion

        #region Getters

        /// <summary>
        /// Gets the translate transform (pan).
        /// </summary>
        /// <param name="element">The UI element that has been transformed.</param>
        /// <returns>The translate transform.</returns>
        private TranslateTransform GetTranslateTransform(UIElement element)
        {
            return (TranslateTransform)((TransformGroup)element.RenderTransform).Children.First(tr => tr is TranslateTransform);
        }

        /// <summary>
        /// Gets the scale transform (zoom).
        /// </summary>
        /// <param name="element">The UI element that has been transformed.</param>
        /// <returns>The zoom transform.</returns>
        private ScaleTransform GetScaleTransform(UIElement element)
        {
            return (ScaleTransform)((TransformGroup)element.RenderTransform).Children.First(tr => tr is ScaleTransform);
        }

        #endregion

        #region Public Methods

        // The UIElement for the image
        public override UIElement Child 
        {
            get { return base.Child; }
            set
            {
                if (value != null && value != Child)
                    Initialize(value);
                base.Child = value;
            }
        }

        /// <summary>
        /// Initializes the border by creating transforms 
        /// and assigning the mouse movements to events.
        /// </summary>
        /// <param name="element">The UI element to initialize (the image)</param>
        public void Initialize(UIElement element)
        {
            child = element;
            if (child != null)
            {
                // Create and add scale and translate transforms to the transform group.
                TransformGroup group = new TransformGroup();

                ScaleTransform st = new ScaleTransform();
                group.Children.Add(st);
                TranslateTransform tt = new TranslateTransform();
                group.Children.Add(tt);

                child.RenderTransform = group;

                child.RenderTransformOrigin = new Point(0.0, 0.0);

                // Assign the mouse actions to events.
                MouseWheel += child_MouseWheel;
                MouseLeftButtonDown += child_MouseLeftButtonDown;
                MouseLeftButtonUp += child_MouseLeftButtonUp;
                MouseMove += child_MouseMove;
                PreviewMouseRightButtonDown += new MouseButtonEventHandler(child_PreviewMouseRightButtonDown);
            }
        }

        /// <summary>
        /// Resets the transform and scale to default values.
        /// </summary>
        public void Reset()
        {
            if (child != null)
            {
                // Reset zoom
                var st = GetScaleTransform(child);
                st.ScaleX = 1.0;
                st.ScaleY = 1.0;

                // Reset pan
                var tt = GetTranslateTransform(child);
                tt.X = 0.0;
                tt.Y = 0.0;
            }
        }

        #endregion

        #region Child Events

        /// <summary>
        /// The event that fires when the user scrolls the mouse wheel.
        /// </summary>
        private void child_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (child != null)
            {
                var st = GetScaleTransform(child);
                var tt = GetTranslateTransform(child);

                double zoom = e.Delta > 0 ? zoomIncrement : -zoomIncrement;

                // If the zoom's too small and the user tries to zoom out, do nothing.
                if (!(e.Delta > 0) && (st.ScaleX < minZoom || st.ScaleY < minZoom))
                    return;
                // Similarily, ff the zoom's too big and the user tries to zoom in, do nothing.
                if ((e.Delta > 0) && (st.ScaleX > maxZoom || st.ScaleY > maxZoom))
                    return;

                Point relative = e.GetPosition(child);
                double absoluteX;
                double absoluteY;

                absoluteX = relative.X * st.ScaleX + tt.X;
                absoluteY = relative.Y * st.ScaleY + tt.Y;

                // Compensating for the decreased zoom value as the user zooms in.
                // We're assuming that ScaleX and ScaleY are changing at the same rate.
                zoom = zoom * st.ScaleX;

                st.ScaleX += zoom;
                st.ScaleY += zoom;

                tt.X = absoluteX - relative.X * st.ScaleX;
                tt.Y = absoluteY - relative.Y * st.ScaleY;
            }
        }

        /// <summary>
        /// The event that fires when the user holds the left mouse button down.
        /// </summary>
        private void child_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (child != null)
            {
                var tt = GetTranslateTransform(child);
                start = e.GetPosition(this);
                previousLocation = new Point(tt.X, tt.Y);

                Cursor = Cursors.SizeAll;
                child.CaptureMouse();
            }
        }

        /// <summary>
        /// The event that fires when the user lets go of the left mouse button.
        /// </summary>
        private void child_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (child != null)
            {
                child.ReleaseMouseCapture();
                Cursor = Cursors.Arrow;

                //CheckMaxDistance();
            }
        }

        /// <summary>
        /// The event that fires when the user clicks the right mouse button.
        /// </summary>
        void child_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Reset();
        }

        /// <summary>
        /// The event that fires when the user moves the mouse while panning the image.
        /// </summary>
        private void child_MouseMove(object sender, MouseEventArgs e)
        {
            if (child != null)
            {
                if (child.IsMouseCaptured)
                {
                    var tt = GetTranslateTransform(child);
                    Vector newPositionV = start - e.GetPosition(this);
                    tt.X = previousLocation.X - newPositionV.X;
                    tt.Y = previousLocation.Y - newPositionV.Y;
                }
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Check (and compensate if necessary) that the transform doesn't exceed the max distance.
        /// </summary>
        private void CheckMaxDistance()
        {
            var tt = GetTranslateTransform(child);
            var st = GetScaleTransform(child);
            Point origin = GetCenterOfWindow();

            // If the transform is within the acceptable range, do nothing.
            if (!(tt.X < maxTransformDistance && tt.X > -maxTransformDistance)) //Something with the origin.x here
                CheckX();
            if (!(tt.Y < maxTransformDistance && tt.Y > -maxTransformDistance))
                CheckY();
        }

        /// <summary>
        /// Checks to see if the X distance is within the acceptable range, and compensates if necessary.
        /// NOTE: Use CheckMaxDistance if you need to check both X and Y.
        /// </summary>
        private void CheckX()
        {
            var tt = GetTranslateTransform(child);
            var st = GetScaleTransform(child);

            // If the x transform is too far to the left, move it to the right.
            if (tt.X > maxTransformDistance)
            {
                tt.X = maxTransformDistance;
                return;
            }
            // If the x transform is too far to the right, move it to the left.
            if (tt.X < -maxTransformDistance)
            {
                tt.X = -maxTransformDistance;
                return;
            }
        }

        /// <summary>
        /// Checks to see if the Y distance is within the acceptable range, and compensates if necessary.
        /// NOTE: Use CheckMaxDistance if you need to check both X and Y.
        /// </summary>
        private void CheckY()
        {
            var tt = GetTranslateTransform(child);
            var st = GetScaleTransform(child);

            // If the y transform is too far up, move it down.
            if (tt.Y < -maxTransformDistance)
            {
                tt.Y = -maxTransformDistance;
                return;
            }
            // If the y transform is too far down, move it up.
            if (tt.Y > maxTransformDistance)
            {
                tt.Y = maxTransformDistance;
                return;
            }
        }

        private Point GetCenterOfWindow()
        {
            double width = ((Border)Application.Current.MainWindow.Content).ActualWidth;
            double height = ((Border)Application.Current.MainWindow.Content).ActualHeight;
            return new Point(width / 2, height / 2);
        }

        #endregion
    }
}
