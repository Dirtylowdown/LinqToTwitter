﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Windows.ApplicationModel.Core;

namespace Linq2TwitterDemos_WindowsStore.DataModel
{
    abstract class StreamViewModel : INotifyPropertyChanged
    {        
        public StreamViewModel()
        {
            JsonContent = new ObservableCollection<JsonContent>();
            StartCommand = new TwitterCommand<object>(OnStart);
        }

        public ObservableCollection<JsonContent> JsonContent { get; set; }

        public TwitterCommand<object> StartCommand { get; set; }

        public abstract void OnStart(object obj);

        public event PropertyChangedEventHandler PropertyChanged;

        protected void SetPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected async void Show(string content)
        {
            var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                JsonContent.Add(new JsonContent { Content = content });
            });            
        }
    }
}
