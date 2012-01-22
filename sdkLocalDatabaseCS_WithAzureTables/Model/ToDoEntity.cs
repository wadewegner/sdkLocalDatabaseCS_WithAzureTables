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

        public bool IsComplete { get; set; }

        public string Category { get; set; }
    }
}
