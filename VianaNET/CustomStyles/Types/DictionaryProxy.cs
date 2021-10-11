// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DictionaryProxy.cs" company="Freie Universität Berlin">
//   ************************************************************************
//   Viana.NET - video analysis for physics education
//   Copyright (C) 2014 Dr. Adrian Voßkühler  
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
// --------------------------------------------------------------------------------------------------------------------

namespace VianaNET.CustomStyles.Types
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using System.Xml.Serialization;

  /// <summary>
  /// Proxy class to permit XML Serialization of generic dictionaries
  /// </summary>
  /// <typeparam name="TK">
  /// The type of the key
  /// </typeparam>
  /// <typeparam name="TV">
  /// The type of the value
  /// </typeparam>
  public class DictionaryProxy<TK, TV>
  {


    /// <summary>
    /// The list.
    /// </summary>
    private Collection<KeyAndValue> list;





    /// <summary>
    /// Initializes a new instance of the <see cref="DictionaryProxy{K,V}"/> class.
    /// </summary>
    /// <param name="original">
    /// The original.
    /// </param>
    public DictionaryProxy(IDictionary<TK, TV> original)
    {
      this.Original = original;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DictionaryProxy{K,V}"/> class. 
    ///   Default constructor so deserialization works
    /// </summary>
    public DictionaryProxy()
    {
    }





    /// <summary>
    /// Gets the keys and values.
    /// </summary>
    /// <remarks>
    /// XmlElementAttribute is used to prevent extra nesting level. It's
    ///   not necessary.
    /// </remarks>
    [XmlElement]
    public Collection<KeyAndValue> KeysAndValues
    {
      get
      {
        if (this.list == null)
        {
          this.list = new Collection<KeyAndValue>();
        }

        // On deserialization, Original will be null, just return what we have
        if (this.Original == null)
        {
          return this.list;
        }

        // If Original was present, add each of its elements to the list
        this.list.Clear();
        foreach (KeyValuePair<TK, TV> pair in this.Original)
        {
          this.list.Add(new KeyAndValue { Key = pair.Key, Value = pair.Value });
        }

        return this.list;
      }
    }

    /// <summary>
    ///   Gets or sets the dictionary.
    ///  Use to set the dictionary if necessary, but don't serialize
    /// </summary>
    [XmlIgnore]
    public IDictionary<TK, TV> Original { get; set; }





    /// <summary>
    /// Convenience method to return a dictionary from this proxy instance
    /// </summary>
    /// <returns>
    /// The <see cref="Dictionary"/>.
    /// </returns>
    public Dictionary<TK, TV> ToDictionary()
    {
      return this.KeysAndValues.ToDictionary(key => key.Key, value => value.Value);
    }



    /// <summary>
    ///   Holds the keys and values
    /// </summary>
    public class KeyAndValue
    {
  

      /// <summary>
      /// Gets or sets the key.
      /// </summary>
      public TK Key { get; set; }

      /// <summary>
      /// Gets or sets the value.
      /// </summary>
      public TV Value { get; set; }

  
    }
  }
}