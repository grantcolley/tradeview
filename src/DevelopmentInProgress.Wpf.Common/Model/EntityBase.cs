using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace DevelopmentInProgress.Wpf.Common.Model
{
    public abstract class EntityBase : INotifyPropertyChanged
    {
        private int id;
        private bool isVisible;
        private bool isReadOnly;

        protected EntityBase()
        {
            IsVisible = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual T Clone<T>() where T : new()
        {
            throw new NotImplementedException($"Clone<{typeof(T).Name}>() not implemnted.");
        }

        [XmlIgnore]
        public virtual int Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        [XmlIgnore]
        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set
            {
                if (isReadOnly != value)
                {
                    isReadOnly = value;
                    OnPropertyChanged("IsVisible");
                    OnPropertyChanged("CanModify");
                }
            }
        }

        [XmlIgnore]
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (isVisible != value)
                {
                    isVisible = value;
                    OnPropertyChanged("IsVisible");
                }
            }
        }

        [XmlIgnore]
        public bool CanModify
        {
            get { return !IsReadOnly; }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            var propertyChangedHandler = PropertyChanged;
            if (propertyChangedHandler != null)
            {
                propertyChangedHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
