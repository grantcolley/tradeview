using Microsoft.Win32;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration.Controls
{
    /// <summary>
    /// Interaction logic for SelectFileDialog.xaml
    /// </summary>
    public partial class FileDialog : UserControl
    {
        private static readonly DependencyProperty FilesCommandProperty;
        private static readonly DependencyProperty MultiSelectProperty;

        static FileDialog()
        {
            FilesCommandProperty = DependencyProperty.Register("FilesCommand", typeof(ICommand), typeof(FileDialog));
            MultiSelectProperty = DependencyProperty.Register("MultiSelect", typeof(bool), typeof(FileDialog), new PropertyMetadata(false));
        }

        public FileDialog()
        {
            InitializeComponent();
        }

        public ICommand FilesCommand
        {
            get { return (ICommand)GetValue(FilesCommandProperty); }
            set { SetValue(FilesCommandProperty, value); }
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

            FilesCommand.Execute(files);
        }
    }
}