/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Microsoft.WindowsAzure.Samples.Phone.Storage;
using Microsoft.WindowsAzure.Samples.Data.Services.Client;
using sdkLocalDatabaseCS.Model;
using System;
using System.Globalization;
using System.Collections;


namespace LocalDatabaseSample.ViewModel
{
    public class ToDoViewModel : INotifyPropertyChanged
    {
        private string tableName = "ToDo";

        // Class constructor, create the data context object.
        public ToDoViewModel()
        {
        }

        // All to-do items.
        public void GetAllToDoItems()
        {
            var context = CloudStorageContext.Current.Resolver.CreateTableServiceContext();
            DataServiceCollection<ToDoEntity> toDoEntityCollection = new DataServiceCollection<ToDoEntity>(context);

            toDoEntityCollection.LoadCompleted += (s, e) =>
                {
                    AllToDoItems = toDoEntityCollection;

                    // Query the database and load all associated items to their respective collections.
                    foreach (var category in CategoriesList)
                    {
                        switch (category)
                        {
                            case "Home":
                                HomeToDoItems = Convert(AllToDoItems.Where(w => w.Category == category));
                                break;
                            case "Work":
                                WorkToDoItems = Convert(AllToDoItems.Where(w => w.Category == category));
                                break;
                            case "Hobbies":
                                HobbiesToDoItems = Convert(AllToDoItems.Where(w => w.Category == category));
                                break;
                            default:
                                break;
                        }
                    }
                };

            var tableUri = new Uri(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}/{1}()",
                        context.BaseUri,
                        tableName,
                        DateTime.UtcNow.Ticks),
                UriKind.Absolute);

            toDoEntityCollection.Clear();
            toDoEntityCollection.LoadAsync(tableUri);
        }

        private ObservableCollection<ToDoEntity> _allToDoItems;
        public ObservableCollection<ToDoEntity> AllToDoItems
        {
            get { return _allToDoItems; }
            set
            {
                _allToDoItems = value;
                NotifyPropertyChanged("AllToDoItems");
            }
        }

        // To-do items associated with the home category.
        private ObservableCollection<ToDoEntity> _homeToDoItems;
        public ObservableCollection<ToDoEntity> HomeToDoItems
        {
            get { return _homeToDoItems; }
            set
            {
                _homeToDoItems = value;
                NotifyPropertyChanged("HomeToDoItems");
            }
        }

        // To-do items associated with the work category.
        private ObservableCollection<ToDoEntity> _workToDoItems;
        public ObservableCollection<ToDoEntity> WorkToDoItems
        {
            get { return _workToDoItems; }
            set
            {
                _workToDoItems = value;
                NotifyPropertyChanged("WorkToDoItems");
            }
        }

        // To-do items associated with the hobbies category.
        private ObservableCollection<ToDoEntity> _hobbiesToDoItems;
        public ObservableCollection<ToDoEntity> HobbiesToDoItems
        {
            get { return _hobbiesToDoItems; }
            set
            {
                _hobbiesToDoItems = value;
                NotifyPropertyChanged("HobbiesToDoItems");
            }
        }

        // A list of all categories, used by the add task page.
        private List<string> _categoriesList;
        public List<string> CategoriesList
        {
            get { return _categoriesList; }
            set
            {
                _categoriesList = value;
                NotifyPropertyChanged("CategoriesList");
            }
        }

        // Query the table services and load the collections and list used by the pivot pages.
        public void LoadCollectionsFromDatabase()
        {
            List<string> categories = new List<string>();
            categories.Add("Home");
            categories.Add("Work");
            categories.Add("Hobbies");

            // Load a list of all categories.
            CategoriesList = categories;

            // Load a list of all items.
            GetAllToDoItems();
        }

        public ObservableCollection<ToDoEntity> Convert(IEnumerable original)
        {
            return new ObservableCollection<ToDoEntity>(original.Cast<ToDoEntity>());
        }

        // Add a to-do item to the database and collections.
        public void AddToDoItem(ToDoEntity newToDoItem)
        {

            var tableClient = CloudStorageContext.Current.Resolver.CreateCloudTableClient();

            tableClient.CreateTableIfNotExist(
                tableName,
                p =>
                {
                    var context = CloudStorageContext.Current.Resolver.CreateTableServiceContext();

                    context.AddObject(tableName, newToDoItem);
                    context.BeginSaveChanges(
                        asyncResult =>
                        {
                            var response2 = context.EndSaveChanges(asyncResult);

                            // reload list
                            GetAllToDoItems();
                        },
                        null);
                });
        }

        // Remove a to-do task item from the database and collections.
        public void DeleteToDoItem(ToDoEntity toDoForDelete)
        {
            var context = CloudStorageContext.Current.Resolver.CreateTableServiceContext();

            context.AttachTo("ToDo", toDoForDelete, "*");
            context.DeleteObject(toDoForDelete);
            context.BeginSaveChanges(
                asyncResult =>
                {
                    var response = context.EndSaveChanges(asyncResult);

                    GetAllToDoItems();
                },
                null);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify Silverlight that a property has changed.
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
