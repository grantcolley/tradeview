using Microsoft.Win32;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DevelopmentInProgress.Wpf.Configuration.Controls
{
    /// <summary>
    /// Interaction logic for SelectFileDialog.xaml
    /// </summary>
    public partial class FileDialog : UserControl
    {
        private static readonly DependencyProperty FilesProperty;
        private static readonly DependencyProperty MultiSelectProperty;

        static FileDialog()
        {
            FilesProperty = DependencyProperty.Register("Files", typeof(IEnumerable<string>), typeof(FileDialog));
            MultiSelectProperty = DependencyProperty.Register("MultiSelect", typeof(bool), typeof(FileDialog), new PropertyMetadata(false));
        }

        public FileDialog()
        {
            InitializeComponent();
        }

        public IEnumerable<string> Files
        {
            get { return (IEnumerable<string>)GetValue(FilesProperty); }
            set { SetValue(FilesProperty, value); }
        }

        public bool MultiSelect
        {
            get { return (bool)GetValue(MultiSelectProperty); }
            set { SetValue(MultiSelectProperty, value); }
        }

        private void SelectFiles(object sender, RoutedEventArgs e)
        {
            var files = new List<string>();

            var dialog = new OpenFileDialog
            {
                Title = "Select",
                Multiselect = this.MultiSelect,
                CheckFileExists = true,
                CheckPathExists = true                
            };

            var result = dialog.ShowDialog();
            if(result.HasValue
                && result.Value.Equals(true))
            {
                files.AddRange(dialog.FileNames);
            }

            Files = files;
        }
    }
}