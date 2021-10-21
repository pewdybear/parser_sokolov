using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Parser.ComponentModel
{
    public class CollectionModel : INotifyCollectionChanged, IEnumerable
    {
        private ObservableCollection<DBEntity> collection = new ObservableCollection<DBEntity>();
        public event NotifyCollectionChangedEventHandler CollectionChanged;


        public void Add(DBEntity dbEntity)
        {
            collection.Add(dbEntity);
            OnCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, dbEntity));
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)collection).GetEnumerator();
        }

        //public void RemoveAt(int index)
        //{
        //    DBEntity temp = collection[index];
        //    collection.RemoveAt(index);
        //    OnCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, temp, index));
        //}

        public void Remove(DBEntity dBEntity)
        {
            collection.Remove(dBEntity);
        }

        public void Update(int index, DBEntity dbEntity)
        {
            collection[index] = dbEntity;
            OnCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, index));
        }

        protected virtual void OnCollectionChange(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        public int Count()
        {
            return collection.Count();
        }

        public DBEntity ReturnString(int index)
        {
            return collection[index];
        }

        public DBEntity Last()
        {
            return collection.Last();
        }

        public DBEntity this[int index]
        {
            get
            {
                return collection[index];
            }
            set
            {
                collection[index] = value;
            }
        }
    }
}
