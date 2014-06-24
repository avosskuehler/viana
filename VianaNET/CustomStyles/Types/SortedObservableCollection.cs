// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SortedObservableCollection.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2012 Dr. Adrian Voßkühler  
//   ------------------------------------------------------------------------
//   This program is free software; you can redistribute it and/or modify it 
//   under the terms of the GNU General Public License as published by the 
//   Free Software Foundation; either version 2 of the License, or 
//   (at your option) any later version.
//   This program is distributed in the hope that it will be useful, 
//   but WITHOUT ANY WARRANTY; without even the implied warranty of 
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
//   See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License 
//   along with this program; if not, write to the Free Software Foundation, 
//   Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//   ************************************************************************
// </copyright>
// <author>Dr. Adrian Voßkühler</author>
// <email>adrian@vosskuehler.name</email>
// <summary>
//   SortedCollection which implements INotifyCollectionChanged interface and so can be used
//   in WPF applications as the source of the binding.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.CustomStyles.Types
{
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.ComponentModel;

  /// <summary>
  /// SortedCollection which implements INotifyCollectionChanged interface and so can be used
  ///   in WPF applications as the source of the binding.
  /// </summary>
  /// <typeparam name="TValue">
  /// </typeparam>
  /// <author>consept</author>
  public class SortedObservableCollection<TValue> : SortedCollection<TValue>, 
                                                    INotifyPropertyChanged, 
                                                    INotifyCollectionChanged
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="SortedObservableCollection{TValue}" /> class.
    /// </summary>
    public SortedObservableCollection()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SortedObservableCollection{TValue}"/> class.
    /// </summary>
    /// <param name="comparer">
    /// The comparer. 
    /// </param>
    public SortedObservableCollection(IComparer<TValue> comparer)
      : base(comparer)
    {
    }

    #endregion

    // Events
    #region Public Events

    /// <summary>
    ///   The collection changed.
    /// </summary>
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    /// <summary>
    ///   The property changed.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region Public Indexers

    /// <summary>
    /// The this.
    /// </summary>
    /// <param name="index">
    /// The index. 
    /// </param>
    /// <returns>
    /// The <see cref="TValue"/> . 
    /// </returns>
    public override TValue this[int index]
    {
      get
      {
        return base[index];
      }

      set
      {
        TValue oldItem = base[index];
        base[index] = value;
        this.OnPropertyChanged("Item[]");
        this.OnCollectionChanged(NotifyCollectionChangedAction.Replace, oldItem, value, index);
      }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   The clear.
    /// </summary>
    public override void Clear()
    {
      base.Clear();
      this.OnCollectionReset();
    }

    /// <summary>
    /// The insert.
    /// </summary>
    /// <param name="index">
    /// The index. 
    /// </param>
    /// <param name="value">
    /// The value. 
    /// </param>
    public override void Insert(int index, TValue value)
    {
      base.Insert(index, value);
      this.OnPropertyChanged("Count");
      this.OnPropertyChanged("Item[]");
      this.OnCollectionChanged(NotifyCollectionChangedAction.Add, value, index);
    }

    /// <summary>
    /// The remove at.
    /// </summary>
    /// <param name="index">
    /// The index. 
    /// </param>
    public override void RemoveAt(int index)
    {
      TValue item = this[index];
      base.RemoveAt(index);
      this.OnPropertyChanged("Item[]");
      this.OnPropertyChanged("Count");
      this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
    }

    #endregion

    #region Methods

    /// <summary>
    /// The on collection changed.
    /// </summary>
    /// <param name="e">
    /// The e. 
    /// </param>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      if (this.CollectionChanged != null)
      {
        this.CollectionChanged(this, e);
      }
    }

    /// <summary>
    /// The on property changed.
    /// </summary>
    /// <param name="e">
    /// The e. 
    /// </param>
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, e);
      }
    }

    /// <summary>
    /// The on collection changed.
    /// </summary>
    /// <param name="action">
    /// The action. 
    /// </param>
    /// <param name="item">
    /// The item. 
    /// </param>
    /// <param name="index">
    /// The index. 
    /// </param>
    private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
    {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
    }

    /// <summary>
    /// The on collection changed.
    /// </summary>
    /// <param name="action">
    /// The action. 
    /// </param>
    /// <param name="oldItem">
    /// The old item. 
    /// </param>
    /// <param name="newItem">
    /// The new item. 
    /// </param>
    /// <param name="index">
    /// The index. 
    /// </param>
    private void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index)
    {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
    }

    /// <summary>
    /// The on collection changed.
    /// </summary>
    /// <param name="action">
    /// The action. 
    /// </param>
    /// <param name="item">
    /// The item. 
    /// </param>
    /// <param name="index">
    /// The index. 
    /// </param>
    /// <param name="oldIndex">
    /// The old index. 
    /// </param>
    private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
    {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
    }

    /// <summary>
    ///   The on collection reset.
    /// </summary>
    private void OnCollectionReset()
    {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    /// The on property changed.
    /// </summary>
    /// <param name="propertyName">
    /// The property name. 
    /// </param>
    private void OnPropertyChanged(string propertyName)
    {
      this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    #endregion
  }
}