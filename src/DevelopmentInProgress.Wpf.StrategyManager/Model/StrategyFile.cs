using DevelopmentInProgress.Wpf.Common.Model;
using System.IO;

namespace DevelopmentInProgress.Wpf.StrategyManager.Model
{
    public class StrategyFile : EntityBase
    {
        private string file;

        public string File
        {
            get { return file; }
            set
            {
                if (file != value)
                {
                    file = value;
                    var fileInfo = new FileInfo(file);
                    DisplayName = fileInfo.Name;
                    OnPropertyChanged("File");
                    OnPropertyChanged("DisplayName");
                }
            }
        }

        public string DisplayName { get; private set; }
    }
}