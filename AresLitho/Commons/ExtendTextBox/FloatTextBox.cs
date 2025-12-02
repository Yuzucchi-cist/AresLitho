using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AresLitho.Commons.ExtendTextBox
{
    public class FloatTextBox : TextBox
    {
        public FloatTextBox()
        {
            // Disable IME and context menu
            InputMethod.SetIsInputMethodEnabled(this, false);
            ContextMenu = null;
            // Handle paste command
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, ExecutePatste));
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.Space)
                e.Handled = true; // Disallow space
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            float result = 0;
            if (!float.TryParse(e.Text, out result))
            {
                e.Handled = true; // Disallow non-float input
            }
        }

        private void ExecutePatste(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                string text = Clipboard.GetText();

                float result = 0;
                if (float.TryParse(text, out result))
                {
                    textBox.Paste();
                }
            }
        }

    }
}
