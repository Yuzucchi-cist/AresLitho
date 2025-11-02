using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Input;

namespace AresLitho.Behaviors
{

    public class BackgroundDropCommandBehavior : Behavior<UIElement>
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                nameof(Command),
                typeof(ICommand),
                typeof(BackgroundDropCommandBehavior),
                new PropertyMetadata(null));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AllowDrop = true;
            AssociatedObject.PreviewDrop += OnDrop;
            AssociatedObject.PreviewDragOver += OnDragOver;
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewDrop -= OnDrop;
            AssociatedObject.PreviewDragOver -= OnDragOver;
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop)
                      ? DragDropEffects.Copy
                      : DragDropEffects.None;
            e.Handled = true;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (Command != null && Command.CanExecute(e))
            {
                Command.Execute(e);
            }
        }
    }
}
