// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SortedCollection.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2021 Dr. Adrian Voßkühler  
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
//   Collections that holds elements in the specified order. The complexity and efficiency
//   of the algorithm is comparable to the SortedList from .NET collections. In contrast
//   to the SortedList SortedCollection accepts redundant elements. If no comparer is
//   is specified the list will use the default comparer for given type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace VianaNET.CustomStyles.Types
{
  using System;
  using System.Collections;
  using System.Collections.Generic;

  /// <summary>
  /// Collections that holds elements in the specified order. The complexity and efficiency
  ///   of the algorithm is comparable to the SortedList from .NET collections. In contrast 
  ///   to the SortedList SortedCollection accepts redundant elements. If no comparer is 
  ///   is specified the list will use the default comparer for given type.
  /// </summary>
  /// <typeparam name="TValue">
  /// </typeparam>
  /// <author>consept</author>
  public class SortedCollection<TValue> : IList<TValue>
  {
    // Fields


    /// <summary>
    ///   The defaul t_ capacity.
    /// </summary>
    private const int DEFAULT_CAPACITY = 4;





    /// <summary>
    ///   The empty values.
    /// </summary>
    private static readonly TValue[] emptyValues;





    /// <summary>
    ///   The comparer.
    /// </summary>
    private readonly IComparer<TValue> comparer;

    /// <summary>
    ///   The size.
    /// </summary>
    private int size;

    /// <summary>
    ///   The values.
    /// </summary>
    private TValue[] values;

    // for enumeration
    /// <summary>
    ///   The version.
    /// </summary>
    private int version;





    /// <summary>
    ///   Initializes static members of the <see cref="SortedCollection" /> class.
    /// </summary>
    static SortedCollection()
    {
      emptyValues = new TValue[0];
    }

    // Constructors
    /// <summary>
    ///   Initializes a new instance of the <see cref="SortedCollection{TValue}" /> class.
    /// </summary>
    public SortedCollection()
    {
      this.values = emptyValues;
      this.comparer = Comparer<TValue>.Default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SortedCollection{TValue}"/> class.
    /// </summary>
    /// <param name="comparer">
    /// The comparer. 
    /// </param>
    public SortedCollection(IComparer<TValue> comparer)
    {
      this.values = emptyValues;
      this.comparer = comparer;
    }





    /// <summary>
    ///   Gets or sets the capacity.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public int Capacity
    {
      get => this.values.Length;

      set
      {
        if (this.values.Length != value)
        {
          if (value < this.size)
          {
            throw new ArgumentException("Too small capacity.");
          }

          if (value > 0)
          {
            TValue[] tempValues = new TValue[value];
            if (this.size > 0)
            {
              // copy only when size is greater than zero
              Array.Copy(this.values, 0, tempValues, 0, this.size);
            }

            this.values = tempValues;
          }
          else
          {
            this.values = emptyValues;
          }
        }
      }
    }

    /// <summary>
    ///   Gets the count.
    /// </summary>
    public int Count => this.size;

    /// <summary>
    ///   Gets a value indicating whether is read only.
    /// </summary>
    public bool IsReadOnly => false;





    /// <summary>
    /// The this.
    /// </summary>
    /// <param name="index">
    /// The index. 
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// </exception>
    /// <returns>
    /// The <see cref="TValue"/> . 
    /// </returns>
    public virtual TValue this[int index]
    {
      get
      {
        if (index < 0 || index >= this.size)
        {
          throw new ArgumentOutOfRangeException();
        }

        return this.values[index];
      }

      set
      {
        if (index < 0 || index >= this.size)
        {
          throw new ArgumentOutOfRangeException();
        }

        this.values[index] = value;
        this.version++;
      }
    }



    // Methods


    /// <summary>
    /// The add.
    /// </summary>
    /// <param name="value">
    /// The value. 
    /// </param>
    /// <exception cref="ArgumentException">
    /// </exception>
    public void Add(TValue value)
    {
      if (value == null)
      {
        throw new ArgumentException("Value can't be null");
      }

      // check where the element should be placed
      int index = Array.BinarySearch(this.values, 0, this.size, value, this.comparer);
      if (index < 0)
      {
        // xor
        index = ~index;
      }

      this.Insert(index, value);
    }

    /// <summary>
    ///   The clear.
    /// </summary>
    public virtual void Clear()
    {
      this.version++;
      Array.Clear(this.values, 0, this.size);
      this.size = 0;
    }

    /// <summary>
    /// The contains.
    /// </summary>
    /// <param name="value">
    /// The value. 
    /// </param>
    /// <returns>
    /// The <see cref="bool"/> . 
    /// </returns>
    public bool Contains(TValue value)
    {
      return this.IndexOf(value) >= 0;
    }

    /// <summary>
    /// The copy to.
    /// </summary>
    /// <param name="array">
    /// The array. 
    /// </param>
    /// <param name="arrayIndex">
    /// The array index. 
    /// </param>
    public void CopyTo(TValue[] array, int arrayIndex)
    {
      Array.Copy(this.values, 0, array, arrayIndex, this.size);
    }

    /// <summary>
    ///   The get enumerator.
    /// </summary>
    /// <returns> The <see cref="IEnumerator" /> . </returns>
    public IEnumerator<TValue> GetEnumerator()
    {
      return new SortedCollectionEnumerator(this);
    }

    /// <summary>
    /// The index of.
    /// </summary>
    /// <param name="value">
    /// The value. 
    /// </param>
    /// <returns>
    /// The <see cref="int"/> . 
    /// </returns>
    /// <exception cref="ArgumentException">
    /// </exception>
    public int IndexOf(TValue value)
    {
      if (value == null)
      {
        throw new ArgumentException("Value can't be null.");
      }

      int index = Array.BinarySearch(this.values, 0, this.size, value, this.comparer);
      if (index >= 0)
      {
        return index;
      }

      return -1;
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
    /// <exception cref="ArgumentException">
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// </exception>
    public virtual void Insert(int index, TValue value)
    {
      if (value == null)
      {
        throw new ArgumentException("Value can't be null.");
      }

      if (index < 0 || index > this.size)
      {
        throw new ArgumentOutOfRangeException();
      }

      if (this.size == this.values.Length)
      {
        this.CheckCapacity(this.size + 1);
      }

      if (index < this.size)
      {
        Array.Copy(this.values, index, this.values, index + 1, this.size - index);
      }

      this.values[index] = value;
      this.size++;
      this.version++;
    }

    /// <summary>
    /// The remove.
    /// </summary>
    /// <param name="value">
    /// The value. 
    /// </param>
    /// <returns>
    /// The <see cref="bool"/> . 
    /// </returns>
    public bool Remove(TValue value)
    {
      int index = this.IndexOf(value);
      if (index < 0)
      {
        return false;
      }

      this.RemoveAt(index);
      return true;
    }

    /// <summary>
    /// The remove at.
    /// </summary>
    /// <param name="index">
    /// The index. 
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// </exception>
    public virtual void RemoveAt(int index)
    {
      if (index < 0 || index >= this.size)
      {
        throw new ArgumentOutOfRangeException();
      }

      this.size--;
      this.version++;
      Array.Copy(this.values, index + 1, this.values, index, this.size - index);
      this.values[this.size] = default(TValue);
    }





    /// <summary>
    ///   The get enumerator.
    /// </summary>
    /// <returns> The <see cref="IEnumerator" /> . </returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return new SortedCollectionEnumerator(this);
    }





    /// <summary>
    /// The check capacity.
    /// </summary>
    /// <param name="min">
    /// The min. 
    /// </param>
    private void CheckCapacity(int min)
    {
      // double the capacity
      int num = this.values.Length == 0 ? DEFAULT_CAPACITY : this.values.Length * 2;
      if (min > num)
      {
        num = min;
      }

      this.Capacity = num;
    }



    /// <summary>
    ///   The sorted collection enumerator.
    /// </summary>
    [Serializable]
    private sealed class SortedCollectionEnumerator : IEnumerator<TValue>, IDisposable, IEnumerator
    {
      // Fields
  

      /// <summary>
      ///   The collection.
      /// </summary>
      private readonly SortedCollection<TValue> collection;

      /// <summary>
      ///   The version.
      /// </summary>
      private readonly int version;

      /// <summary>
      ///   The current value.
      /// </summary>
      private TValue currentValue;

      /// <summary>
      ///   The index.
      /// </summary>
      private int index;

  

      // Methods
  

      /// <summary>
      /// Initializes a new instance of the <see cref="SortedCollectionEnumerator"/> class.
      /// </summary>
      /// <param name="collection">
      /// The collection. 
      /// </param>
      internal SortedCollectionEnumerator(SortedCollection<TValue> collection)
      {
        this.collection = collection;
        this.version = collection.version;
      }

  

  

      /// <summary>
      ///   Gets the current.
      /// </summary>
      public TValue Current => this.currentValue;

  

  

      /// <summary>
      ///   Gets the current.
      /// </summary>
      /// <exception cref="ArgumentException"></exception>
      object IEnumerator.Current
      {
        get
        {
          if ((this.index == 0) || (this.index == this.collection.Count + 1))
          {
            throw new ArgumentException("Enumerator not initilized. Call MoveNext first.");
          }

          return this.currentValue;
        }
      }

  

  

      /// <summary>
      ///   The dispose.
      /// </summary>
      public void Dispose()
      {
        this.index = 0;
        this.currentValue = default(TValue);
      }

      /// <summary>
      ///   The move next.
      /// </summary>
      /// <returns> The <see cref="bool" /> . </returns>
      /// <exception cref="ArgumentException"></exception>
      public bool MoveNext()
      {
        if (this.version != this.collection.version)
        {
          throw new ArgumentException("Collection was changed while iterating!");
        }

        if (this.index < this.collection.Count)
        {
          this.currentValue = this.collection.values[this.index];
          this.index++;
          return true;
        }

        this.index = this.collection.Count + 1;
        this.currentValue = default(TValue);
        return false;
      }

  

  

      /// <summary>
      ///   The reset.
      /// </summary>
      /// <exception cref="ArgumentException"></exception>
      void IEnumerator.Reset()
      {
        if (this.version != this.collection.version)
        {
          throw new ArgumentException("Collection was changed while iterating!");
        }

        this.index = 0;
        this.currentValue = default(TValue);
      }

  

      // Properties
    }
  }
}