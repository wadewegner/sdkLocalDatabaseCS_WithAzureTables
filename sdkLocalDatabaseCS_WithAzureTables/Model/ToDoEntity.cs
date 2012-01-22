using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.WindowsAzure.Samples.Data.Services.Client;
using Microsoft.WindowsAzure.Samples.Phone.Storage;
using System.Globalization;
using System.ComponentModel;

namespace sdkLocalDatabaseCS.Model
{
    [DataServiceEntity]
    [EntitySet("ToDo")]
    public class ToDoEntity : TableServiceEntity
    {
        public ToDoEntity()
            : base(
                  "PartitionKey",
                  string.Format(
                      CultureInfo.InvariantCulture,
                      "{0:10}_{1}",
                      DateTime.MaxValue.Ticks - DateTime.Now.Ticks,
                      Guid.NewGuid()))
        {
        }

        public ToDoEntity(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {
        }

        public int ToDoItemId { get; set; }

        public string ItemName { get; set; }

        private bool _isComplete;
        public bool IsComplete
        {
            get { return _isComplete; }
            set
            {
                bool loading = ApplicationStateHelpers.Get<bool>("loading");

                if (_isComplete != value && !loading)
                {
                    NotifyPropertyChanging("IsComplete");
                    _isComplete = value;

                    var context = CloudStorageContext.Current.Resolver.CreateTableServiceContext();
                    
                    context.AttachTo("ToDo", this, "*");
                    context.UpdateObject(this);

                    // Send the update to the data service.
                    context.BeginSaveChanges(
                        asyncResult =>
                        {
                            var response = context.EndSaveChanges(asyncResult);

                        },
                        null);

                    NotifyPropertyChanged("IsComplete");
                }
            }
        }

        public string Category { get; set; }

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify that a property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify that a property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }


}
