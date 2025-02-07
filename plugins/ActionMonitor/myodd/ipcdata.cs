﻿//This file is part of Myoddweb.Piger.
//
//    Myoddweb.Piger is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    Myoddweb.Piger is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with Myoddweb.Piger.  If not, see<https://www.gnu.org/licenses/gpl-3.0.en.html>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ActionMonitor.Myodd
{
  internal class IpcData
  {
    private enum DataType : short
    {
      Guid = 1,
      Int32 = 2,
      Int64 = 3,
      String = 4,
      StringAscii = 5
    };

    private List<dynamic> IpcArguments { get; set; }

    /// <summary>
    /// Check if we have a valid guid or not.
    /// </summary>
    /// <returns></returns>
    public bool HasGuid()
    {
      return !string.IsNullOrEmpty(Guid);
    }

    /// <summary>
    /// Get the number of arguments we have in the IpcData
    /// </summary>
    public uint ArgumentsCount { get; private set; }

    /// <summary>
    /// Get the current Guid for this IpcData
    /// </summary>
    public string Guid { get; }

    // The current version number
    // this will help us to read the data.
    private const int VersionNumber = 100;

    private byte[] Bytes { get; set; }

    private IntPtr? HGlobal { get; set; }

    public IpcData()
    {
      //  the list of dynamics
      IpcArguments = new List<dynamic>();

      // make sure it is null by default.
      HGlobal = null;

      // we have no arguments.
      ArgumentsCount = 0;

      // create a brand new byte and just add the version number.
      Bytes = BitConverter.GetBytes(VersionNumber);

      // set the guid
      Guid = System.Guid.NewGuid().ToString();

      // add this guid.
      AddGuid();
    }

    /// <summary>
    /// Constructor with given bytes.
    /// </summary>
    /// <param name="bytes">The bytes data</param>
    public IpcData(byte[] bytes)
    {
      //  the list of dynamics
      IpcArguments = new List<dynamic>();

      // make sure it is null by default.
      HGlobal = null;

      // we have no arguments.
      ArgumentsCount = 0;

      //  we have no guid.
      Guid = "";

      // create a brand new byte and just add the version number.
      Bytes = BitConverter.GetBytes(VersionNumber);

      // now read it.
      var dataSize = bytes.Length;
      if (dataSize < sizeof(int))
      {
        throw new ArgumentException("Invalid data size, we need at least the version number.");
      }

      // the pointer to keep track of where we are.
      var pointer = 0;

      //  get the version number
      var versionNumber = ReadVersionNumber(bytes, ref pointer);
      if (versionNumber > VersionNumber)
      {
        throw new ArgumentException("The given version number is greater than the supported version number.");
      }

      //  now read all the data
      for (; pointer < dataSize;)
      {
        // get the next item type.
        var dataType = ReadDataType(bytes, ref pointer);
        switch (dataType)
        {
          case DataType.Guid:
            {
              // set the guid.
              Guid = ReadGuid(bytes, ref pointer);
              AddGuid();
            }
            break;

          case DataType.Int32:
            {
              Add(ReadInt32(bytes, ref pointer));
            }
            break;

          case DataType.Int64:
            {
              Add(ReadInt64(bytes, ref pointer));
            }
            break;

          case DataType.String: //string unicode
            {
              Add(ReadString(bytes, ref pointer));
            }
            break;

          case DataType.StringAscii:
            {
              Add(ReadAsciiString(bytes, ref pointer), false);
            }
            break;

          default:
            // no point goint further as we do no know the size of that argument.
            // even if we can move the pointer forward, it would be more luck than anything.
            throw new ArgumentException("Unknown argument type, I am unable to read the size/data for it.");
        }
      }
    }

    ~IpcData()
    {
      // do we need to free some memory?
      ResetPtr();
    }

    /// <summary>
    /// Get current data type.
    /// </summary>
    /// <param name="bytes">the bytes we are reading from</param>
    /// <param name="pointer">the current pointer location</param>
    /// <returns></returns>
    private static DataType ReadDataType(byte[] bytes, ref int pointer)
    {
      //  check that we have enough data.
      if (pointer + sizeof(short) > bytes.Length)
      {
        throw new ArgumentOutOfRangeException(nameof(bytes), "We there is not enough data to read the data type.");
      }

      // create the destination item
      var dst = new byte[sizeof(short)];

      // copy from where we are to where we are going.
      Array.Copy(bytes, pointer, dst, 0, sizeof(short));

      // update the pointer.
      pointer += sizeof(short);

      // return the converted value.
      return (DataType)BitConverter.ToInt16(dst, 0);
    }

    /// <summary>
    /// Get the string an integer.
    /// </summary>
    /// <param name="bytes">the bytes we are reading from</param>
    /// <param name="pointer">the current pointer location</param>
    /// <returns></returns>
    private static int ReadInt32(byte[] bytes, ref int pointer)
    {
      //  check that we have enough data.
      if (pointer + sizeof(int) > bytes.Length)
      {
        throw new ArgumentOutOfRangeException(nameof(bytes), "We there is not enough data to read.");
      }

      // create the destination item
      var dst = new byte[sizeof(int)];

      // copy from where we are to where we are going.
      Array.Copy(bytes, pointer, dst, 0, sizeof(int));

      // update the pointer.
      pointer += sizeof(int);

      // return the converted value.
      return BitConverter.ToInt32(dst, 0);
    }

    /// <summary>
    /// Get the string an integer.
    /// </summary>
    /// <param name="bytes">the bytes we are reading from</param>
    /// <param name="pointer">the current pointer location</param>
    /// <returns></returns>
    private static long ReadInt64(byte[] bytes, ref int pointer)
    {
      //  check that we have enough data.
      if (pointer + sizeof(long) > bytes.Length)
      {
        throw new ArgumentOutOfRangeException(nameof(bytes), "We there is not enough data to read.");
      }

      // create the destination item
      var dst = new byte[sizeof(long)];

      // copy from where we are to where we are going.
      Array.Copy(bytes, pointer, dst, 0, sizeof(long));

      // update the pointer.
      pointer += sizeof(long);

      // return the converted value.
      return BitConverter.ToInt64(dst, 0);
    }

    /// <summary>
    /// Get the string as unicode
    /// </summary>
    /// <param name="bytes">the bytes we are reading from</param>
    /// <param name="pointer">the current pointer location</param>
    /// <returns></returns>
    private static string ReadString(byte[] bytes, ref int pointer)
    {
      //  check that we have enough data for the length.
      if (pointer + sizeof(int) > bytes.Length)
      {
        throw new ArgumentOutOfRangeException(nameof(bytes), "We there is not enough data to read.");
      }

      // create the destination item
      var dstDataSize = new byte[sizeof(int)];

      // copy from where we are to where we are going.
      Array.Copy(bytes, pointer, dstDataSize, 0, sizeof(int));
      var dataSize = BitConverter.ToInt32(dstDataSize, 0);

      // update the pointer.
      pointer += sizeof(int);

      // do we have anything to read?
      if (dataSize == 0)
      {
        return "";
      }

      //  check that we have enough data for the size.
      if (pointer + (dataSize * sizeof(char)) > bytes.Length)
      {
        throw new ArgumentOutOfRangeException(nameof(bytes), "We there is not enough data to read.");
      }

      //  now get the data itself
      // create the destination item
      var dstData = new byte[dataSize * sizeof(char)];

      // copy from where we are to where we are going.
      Array.Copy(bytes, pointer, dstData, 0, dataSize * sizeof(char));

      // update the pointer.
      pointer += dataSize * sizeof(char);

      // return the converted value.
      return Encoding.Unicode.GetString(dstData);
    }

    /// <summary>
    /// Get the string as ascii
    /// </summary>
    /// <param name="bytes">the bytes we are reading from</param>
    /// <param name="pointer">the current pointer location</param>
    /// <returns></returns>
    private static string ReadAsciiString(byte[] bytes, ref int pointer)
    {
      //  check that we have enough data for the length.
      if (pointer + sizeof(int) > bytes.Length)
      {
        throw new ArgumentOutOfRangeException(nameof(bytes), "We there is not enough data to read.");
      }

      // create the destination item
      var dstDataSize = new byte[sizeof(int)];

      // copy from where we are to where we are going.
      Array.Copy(bytes, pointer, dstDataSize, 0, sizeof(int));
      var dataSize = BitConverter.ToInt32(dstDataSize, 0);

      // update the pointer.
      pointer += sizeof(int);

      // do we have anything to read?
      if (dataSize == 0)
      {
        return "";
      }

      //  check that we have enough data for the size.
      if (pointer + dataSize > bytes.Length)
      {
        throw new ArgumentOutOfRangeException(nameof(bytes), "We there is not enough data to read.");
      }

      //  now get the data itself
      // create the destination item
      var dstData = new byte[dataSize];

      // copy from where we are to where we are going.
      Array.Copy(bytes, pointer, dstData, 0, dataSize);

      // update the pointer.
      pointer += dataSize;

      // return the converted value.
      return Encoding.ASCII.GetString(dstData);
    }

    /// <summary>
    /// Get the Guid, (a string as unicode)
    /// </summary>
    /// <param name="bytes">the bytes we are reading from</param>
    /// <param name="pointer">the current pointer location</param>
    /// <returns></returns>
    private static string ReadGuid(byte[] bytes, ref int pointer)
    {
      // this is a wide string
      return ReadString(bytes, ref pointer);
    }

    /// <summary>
    /// Get the current version number.
    /// </summary>
    /// <param name="bytes">the bytes we are reading from</param>
    /// <param name="pointer">the current pointer location</param>
    /// <returns></returns>
    private static int ReadVersionNumber(byte[] bytes, ref int pointer)
    {
      // create the destination item
      var dst = new byte[sizeof(int)];

      // copy from where we are to where we are going.
      Array.Copy(bytes, pointer, dst, 0, sizeof(int));

      // update the pointer.
      pointer += sizeof(int);

      // return the converted value.
      return BitConverter.ToInt32(dst, 0);
    }

    /// <summary>
    /// Add the GUID as a string.
    /// </summary>
    /// <returns></returns>
    private void AddGuid()
    {
      Add(Combine(new []
      {
        BitConverter.GetBytes((short) DataType.Guid), //  short
        BitConverter.GetBytes(Guid.Length), //
        Encoding.Unicode.GetBytes(Guid)
      }
      ));

      //  add to the arguments list.
      IpcArguments.Add(new
      {
        data = Guid,
        type = DataType.Guid
      });
    }

    /// <summary>
    /// Add a string to our list.
    /// </summary>
    /// <param name="stringToAdd">the string to add</param>
    /// <param name="asUnicode">If we want to send it as unicode or not.</param>
    public void Add(string stringToAdd, bool asUnicode = true)
    {
      if (asUnicode)
      {
        var stringInBytes = Encoding.Unicode.GetBytes(stringToAdd);
        Add(Combine(new[]
          {
          BitConverter.GetBytes((short) DataType.String), //  short
          BitConverter.GetBytes(stringToAdd.Length), //  Int32, size is guaranteed.
          stringInBytes
        }
          ));
      }
      else
      {
        Add(Combine(BitConverter.GetBytes((short) DataType.StringAscii), BitConverter.GetBytes(stringToAdd.Length), Encoding.ASCII.GetBytes(stringToAdd)));
      }

      //  add to the arguments list.
      IpcArguments.Add(new
      {
        data = stringToAdd,
        type = (asUnicode ? DataType.String : DataType.StringAscii)
      });

      // update the argument count.
      ArgumentsCount++;
    }

    /// <summary>
    /// Add a number to the list
    /// </summary>
    /// <param name="numberToAdd">The number to add.</param>
    public void Add(int numberToAdd)
    {
      Add(Combine(new[]
      {
        BitConverter.GetBytes((short) DataType.Int32), //  short
        BitConverter.GetBytes(numberToAdd) //  int32
      }
        ));

      //  add to the arguments list.
      IpcArguments.Add(new
      {
        data = numberToAdd,
        type = DataType.Int32
      });

      // update the argument count.
      ArgumentsCount++;
    }

    /// <summary>
    /// Add a number to the list
    /// </summary>
    /// <param name="numberToAdd">The number to add.</param>
    public void Add(long numberToAdd)
    {
      Add(Combine(new[]
        {
          BitConverter.GetBytes((short) DataType.Int64), // short
          BitConverter.GetBytes(numberToAdd) //  int64
        }
        ));

      //  add to the arguments list.
      IpcArguments.Add(new
      {
        data = numberToAdd,
        type = DataType.Int64
      });

      // update the argument count.
      ArgumentsCount++;
    }

    /// <summary>
    /// Add a number to the list
    /// </summary>
    /// <param name="numberToAdd">The number to add.</param>
    public void Add(uint numberToAdd)
    {
      Add((int)numberToAdd);
    }

    /// <summary>
    /// Add a true/false flag
    /// </summary>
    /// <param name="flag">True/False.</param>
    public void Add(bool flag )
    {
      Add( flag ? 1 : 0 );
    }

    /// <summary>
    /// Add a created byte to our current byte.
    /// </summary>
    /// <param name="newBytes"></param>
    private void Add(byte[] newBytes)
    {
      //  join the old and the new together.
      Bytes = Combine(Bytes, newBytes);
    }

    /// <summary>
    /// Join one or more pointer together.
    /// </summary>
    /// <param name="arrays"></param>
    /// <returns></returns>
    private static byte[] Combine(params byte[][] arrays)
    {
      var rv = new byte[arrays.Sum(a => a.Length)];
      var offset = 0;
      foreach (var array in arrays)
      {
        Buffer.BlockCopy(array, 0, rv, offset, array.Length);
        offset += array.Length;
      }
      return rv;
    }

    /// <summary>
    /// Clear the pointer and free memeory if need be.
    /// </summary>
    private void ResetPtr()
    {
      if (HGlobal != null)
      {
        Marshal.FreeHGlobal((IntPtr)HGlobal);
        HGlobal = null;
      }
    }

    /// <summary>
    /// Get the data size fully created.
    /// </summary>
    /// <returns>int the size of the item</returns>
    public int GetSize()
    {
      // we know that the size is, at the very least, the version number.
      // if it is null, then we have no data
      // but we have the version number.
      return Bytes?.Length ?? sizeof(int);
    }

    /// <summary>
    /// Get the pointer for transmission.
    /// </summary>
    /// <returns></returns>
    public IntPtr GetPtr()
    {
      if (null != HGlobal)
      {
        return (IntPtr)HGlobal;
      }

      // make sure we clean what needs to be
      ResetPtr();

      // build the pointer.
      // var msgBytes = System.Text.Encoding.Default.GetBytes(msg);
      var bytesSize = Bytes.Length;
      HGlobal = Marshal.AllocHGlobal(bytesSize);
      Marshal.Copy(Bytes, 0, (IntPtr)HGlobal, bytesSize);

      return (IntPtr)HGlobal;
    }

    private static bool IsNumericType(Type t)
    {
      switch (Type.GetTypeCode(t))
      {
        case TypeCode.Byte:
        case TypeCode.SByte:
        case TypeCode.UInt16:
        case TypeCode.UInt32:
        case TypeCode.UInt64:
        case TypeCode.Int16:
        case TypeCode.Int32:
        case TypeCode.Int64:
        case TypeCode.Decimal:
        case TypeCode.Double:
        case TypeCode.Single:
          return true;

        default:
          return false;
      }
    }

    private static bool IsStringType(Type t)
    {
      switch (Type.GetTypeCode(t))
      {
        case TypeCode.String:
        case TypeCode.Char:
          return true;

        default:
          return false;
      }
    }

    public T Get<T>(uint index)
    {
      if (index >= ArgumentsCount)
      {
        throw new ArgumentOutOfRangeException(nameof(index));
      }

      // rebuild the arguments without Guid.
      var arguments = IpcArguments.Where(t => t.type != DataType.Guid).ToList();
      
      // then look for the argument we are after.
      var objectData = arguments[(int)index].data;
      var type = typeof(T);
      if (Type.GetTypeCode(type) == TypeCode.Object)
      {
        return objectData;
      }

      // if it is a boolean...
      var objectType = (DataType)arguments[(int)index].type;

      if (Type.GetTypeCode(type) == TypeCode.Boolean)
      {
        switch (objectType)
        {
          case DataType.Int32:
            return (T)((object)((int)objectData == 1));

          case DataType.String:
          case DataType.StringAscii:
            var s = ((string)objectData).ToLower().Trim();
            if( s == "true" || s == "1")
            {
              return (T)((object)true);
            }
            if (s == "false" || s == "0")
            {
              return (T)((object)false);
            }
            // if we are here, we do not know what it is.
            throw new InvalidCastException();

          default:
            throw new InvalidCastException();
        }
      }

      if (IsNumericType(type))
      {
        switch (objectType)
        {
          case DataType.Int32:
          case DataType.Int64:
            return (T)objectData;

          default:
            throw new InvalidCastException();
        }
      }

      if (IsStringType(type))
      {
        switch (objectType)
        {
          case DataType.Int32:
            {
              var i = (int)objectData;
              var s = i.ToString();
              var o = (object)s;
              return (T)o;
            }

          case DataType.Int64:
            {
              var i = (long)objectData;
              var s = i.ToString();
              var o = (object)s;
              return (T)o;
            }

          case DataType.Guid:
          case DataType.String:
          case DataType.StringAscii:
            return (T)objectData;

          default:
            throw new InvalidCastException();
        }
      }

      //  we cannot convert that type.
      throw new InvalidCastException();
    }

    /// <summary>
    /// Check if a certain argument is a string or not.
    /// </summary>
    /// <param name="index">The index we are checking.</param>
    /// <returns></returns>
    public bool IsString( uint index )
    {
      if (index >= ArgumentsCount)
      {
        throw new ArgumentOutOfRangeException(nameof(index));
      }

      // rebuild the arguments without Guid.
      var arguments = IpcArguments.Where(t => t.type != DataType.Guid).ToList();

      // then look for the argument we are after.
      var objectType = (DataType)arguments[(int)index].type;
      switch (objectType)
      {
        case DataType.Int32:
          return false;

        case DataType.String:
        case DataType.StringAscii:
          return true;

        default:
          // new type?
          throw new InvalidCastException();
      }
    }

    /// <summary>
    /// Check if an argument is a number or not.
    /// </summary>
    /// <param name="index">The index we are checking.</param>
    /// <returns></returns>
    public bool IsInt(uint index)
    {
      if (index >= ArgumentsCount)
      {
        throw new ArgumentOutOfRangeException(nameof(index));
      }

      // rebuild the arguments without Guid.
      var arguments = IpcArguments.Where(t => t.type != DataType.Guid).ToList();

      // then look for the argument we are after.
      var objectType = (DataType)arguments[(int)index].type;
      switch (objectType)
      {
        case DataType.Int32:
          return true;

        case DataType.String:
        case DataType.StringAscii:
          return false;

        default:
          // new type?
          throw new InvalidCastException();
      }
    }
  }
}
