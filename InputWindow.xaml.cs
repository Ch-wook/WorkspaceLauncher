using System.Windows;

namespace WorkspaceLauncher
{
    public partial class InputWindow : Window
    {
        public string InputText { get; private set; } = string.Empty;

        public InputWindow(string prompt, string defaultValue = "")
        {
            InitializeComponent();
            txtPrompt.Text = prompt;
            txtInput.Text = defaultValue;
            txtInput.SelectAll();
            txtInput.Focus();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            InputText = txtInput.Text;
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
