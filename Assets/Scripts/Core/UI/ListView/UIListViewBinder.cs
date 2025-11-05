using Loxodon.Framework.Binding;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Views;
using System;
using System.Collections.Specialized;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ProjectBase.UI
{
    public class UIListViewBinder<T> : IDisposable where T : DIViewModelBase
    {
        private ObservableList<T> _items;

        private readonly Transform _content;

        private readonly GameObject _itemTemplate;

        public UIListViewBinder(Transform content, GameObject itemTemplate)
        {
            _content = content;
            _itemTemplate = itemTemplate;
        }

        public ObservableList<T> Items
        {
            get { return this._items; }
            set
            {
                if (this._items == value)
                    return;

                if (this._items != null)
                    this._items.CollectionChanged -= OnCollectionChanged;
          
                this._items = value;
                this.OnItemsChanged();

                if (this._items != null)
                    this._items.CollectionChanged += OnCollectionChanged;
            }
        }

        protected void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            switch (eventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.AddItem(eventArgs.NewStartingIndex, eventArgs.NewItems[0]);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.RemoveItem(eventArgs.OldStartingIndex, eventArgs.OldItems[0]);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    this.ReplaceItem(eventArgs.OldStartingIndex, eventArgs.OldItems[0], eventArgs.NewItems[0]);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.ResetItem();
                    break;
                case NotifyCollectionChangedAction.Move:
                    this.MoveItem(eventArgs.OldStartingIndex, eventArgs.NewStartingIndex, eventArgs.NewItems[0]);
                    break;
            }
        }

        protected virtual void OnItemsChanged()
        {
            int count = this._content.childCount;
            for(int i = count - 1; i >= 0; i--)
            {
                Transform child = this._content.GetChild(i);
                GameObject.Destroy(child.gameObject);
            }

            for (int i = 0; i < this._items.Count; i++)
            {
                this.AddItem(i, _items[i]);
            }
        }

        protected virtual void AddItem(int index, object item)
        {
            var itemViewGo = GameObject.Instantiate(_itemTemplate);
            itemViewGo.transform.SetParent(this._content, false);
            itemViewGo.transform.SetSiblingIndex(index);
            itemViewGo.SetActive(true);

            var itemView = itemViewGo.GetComponent<VMSubView<T>>();
            itemView.SetViewModel((T)item);
            itemView.Create();
        }

        protected virtual void RemoveItem(int index, object item)
        {
            Transform transform = this._content.GetChild(index);
            var itemView = transform.GetComponent<VMSubView<T>>();
            if (itemView.GetDataContext() == item)
            {
                itemView.gameObject.SetActive(false);
                GameObject.Destroy(itemView.gameObject);
            }
        }

        protected virtual void ReplaceItem(int index, object oldItem, object item)
        {
            Transform transform = this._content.GetChild(index);
            var itemView = transform.GetComponent<VMSubView<T>>();
            if (itemView.GetDataContext() == oldItem)
            {
                itemView.SetDataContext(item);
            }
        }

        protected virtual void MoveItem(int oldIndex, int index, object item)
        {
            Transform transform = this._content.GetChild(oldIndex);
            var itemView = transform.GetComponent<VMSubView<T>>();
            itemView.transform.SetSiblingIndex(index);
        }

        protected virtual void ResetItem()
        {
            for (int i = this._content.childCount - 1; i >= 0; i--)
            {
                Transform transform = this._content.GetChild(i);
                GameObject.Destroy(transform.gameObject);
            }
        }

        public void Dispose()
        {
            if (_items != null)
                _items.CollectionChanged -= OnCollectionChanged;
        }
    }

}