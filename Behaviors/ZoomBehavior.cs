using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AresLitho.Behaviors
{
    internal class ZoomBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register(
                nameof(Target),
                typeof(FrameworkElement),
                typeof(ZoomBehavior),
                new PropertyMetadata(null));
        public FrameworkElement? Target
        {
            get => (FrameworkElement)GetValue(TargetProperty);
            set => SetValue(TargetProperty, value);
        }

        private ScaleTransform? scaleTransform;
        private TranslateTransform? translateTransform;

        private (double X, double Y) initialScale;
        private (double X, double Y) initialTranslatePosition;

        protected override void OnAttached()
        {
            base.OnAttached();

            if (Target == null) return;

            AssociatedObject.ClipToBounds = true;
            AssociatedObject.Focusable = true;
            AssociatedObject.Focus();


            AssociatedObject.Loaded += OnLoad;
            AssociatedObject.PreviewMouseWheel += OnMouseWheel;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.Loaded -= OnLoad;
            AssociatedObject.PreviewMouseWheel -= OnMouseWheel;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            if (Target == null) return;
            scaleTransform = new ScaleTransform(1,1);
            translateTransform = new TranslateTransform(0, 0);

            var transformGroup = new TransformGroup()
            {
                Children =
                {
                    scaleTransform,
                    translateTransform,
                    new RotateTransform(),
                }
            };

            Target.RenderTransform = transformGroup;
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (scaleTransform is null || translateTransform is null) return;

            Point mousePosition = e.GetPosition(AssociatedObject);
            double zoomScale = 0 < e.Delta ? 1.1 : 1 / 1.1;

            ChangeScale(mousePosition, zoomScale);

            e.Handled = true;
        }

        private void ChangeScale(Point mousePosition, double zoomScale)
        {
            if (scaleTransform is null || translateTransform is null) return;

            double px = mousePosition.X, py = mousePosition.Y;
            double tx = translateTransform.X, ty = translateTransform.Y;

            translateTransform.X = tx - (px - tx) * (zoomScale - 1);
            translateTransform.Y = ty - (py - ty) * (zoomScale - 1);
            scaleTransform.ScaleX *= zoomScale;
            scaleTransform.ScaleY *= zoomScale;
        }
    }
}
