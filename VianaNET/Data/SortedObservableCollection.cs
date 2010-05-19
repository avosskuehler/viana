
namespace VianaNET
{
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.ComponentModel;

  /// <summary>
  /// SortedCollection which implements INotifyCollectionChanged interface and so can be used
  /// in WPF applications as the source of the binding.
  /// </summary>
  /// <author>consept</author>
  public class SortedObservableCollection<TValue> : SortedCollection<TValue>, INotifyPropertyChanged, INotifyCollectionChanged
  {
    public SortedObservableCollection() : base() { }

    public SortedObservableCollection(IComparer<TValue> comparer) : base(comparer) { }

    // Events
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      if (this.CollectionChanged != null)
      {
        this.CollectionChanged(this, e);
      }
    }

    private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
    {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
    }

    private void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index)
    {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
    }

    private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
    {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
    }

    private void OnCollectionReset()
    {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, e);
      }
    }

    private void OnPropertyChanged(string propertyName)
    {
      this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    public override void Insert(int index, TValue value)
    {
      base.Insert(index, value);
      this.OnPropertyChanged("Count");
      this.OnPropertyChanged("Item[]");
      this.OnCollectionChanged(NotifyCollectionChangedAction.Add, value, index);
    }

    public override void RemoveAt(int index)
    {
      var item = this[index];
      base.RemoveAt(index);
      this.OnPropertyChanged("Item[]");
      this.OnPropertyChanged("Count");
      this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
    }

    public override TValue this[int index]
    {
      get
      {
        return base[index];
      }
      set
      {
        var oldItem = base[index];
        base[index] = value;
        this.OnPropertyChanged("Item[]");
        this.OnCollectionChanged(NotifyCollectionChangedAction.Replace, oldItem, value, index);
      }
    }

    public override void Clear()
    {
      base.Clear();
      OnCollectionReset();
    }
  }
}
