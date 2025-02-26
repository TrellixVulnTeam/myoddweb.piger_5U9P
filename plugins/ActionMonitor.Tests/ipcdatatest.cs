﻿using System;
using ActionMonitor.Myodd;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ActionMonitor.Tests
{
  internal enum ExpectedIpcDataType : byte
  {
    None = 0,
    Guid = 1,
    Int32 = 2,
    Int64 = 3,
    String = 4,
    StringAscii = 5,
  };

  [TestClass]
  public class IpcDataTest
  {
    [TestMethod]
    public void TestHasGuidByDefault()
    {
      var emptyIpcData = new IpcData();

      // we have a guid by default.
      Assert.IsTrue(emptyIpcData.HasGuid());

      //  check that the string is not empty
      Assert.IsFalse( string.IsNullOrEmpty(emptyIpcData.Guid) );
    }

    [TestMethod]
    public void TestGetSingleTypes()
    {
      var emptyIpcData = new IpcData();
      emptyIpcData.Add( "Hello" );
      emptyIpcData.Add( "World" );

      // check the values.
      Assert.AreEqual( "Hello", emptyIpcData.Get<string>(0));
      Assert.AreEqual( "World", emptyIpcData.Get<string>(1));
    }

    [TestMethod]
    public void TestGetDifferentTypes()
    {
      var emptyIpcData = new IpcData();
      emptyIpcData.Add("Hello");
      emptyIpcData.Add("World");
      emptyIpcData.Add( 42 );
      emptyIpcData.Add( "Something", false );

      // check the values.
      Assert.AreEqual("Hello", emptyIpcData.Get<string>(0));
      Assert.AreEqual("World", emptyIpcData.Get<string>(1));
      Assert.AreEqual(42, emptyIpcData.Get<int>(2));
      Assert.AreEqual("Something", emptyIpcData.Get<string>(3));
    }

    [TestMethod]
    public void TestTheArgumentCountIsValid()
    {
      var emptyIpcData = new IpcData();

      Assert.AreEqual( (uint)0, emptyIpcData.ArgumentsCount);
      emptyIpcData.Add("Hello");
      emptyIpcData.Add("World");
      Assert.AreEqual( (uint)2 , emptyIpcData.ArgumentsCount );
      emptyIpcData.Add(42);
      Assert.AreEqual( (uint)3, emptyIpcData.ArgumentsCount);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException), "There are no items in the list to get.")]
    public void TestTryingToGetAnEmptyItemOutOfRange()
    {
      var emptyIpcData = new IpcData();
      emptyIpcData.Get<object>(0);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException), "There are no items in the list to get.")]
    public void TestTryingToGetANonEmptyItemOutOfRange()
    {
      var emptyIpcData = new IpcData();
      emptyIpcData.Add(42);

      Assert.AreEqual(42, emptyIpcData.Get<int>(0));

      //  out of range
      emptyIpcData.Get<object>(1);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "The argument must have at least a version number.")]
    public void TestTryingToPassEmptyByteArray()
    {
      new IpcData( new byte[] {} );
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "The argument must have at least a version number.")]
    public void TestTryingToPassByteArrayTooSmall()
    {
      //  the size should be 4
      new IpcData(new byte[] { 0,0,0 });
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "The version number is too big for our current number.")]
    public void TestVersionNumberIsPastOurCurrentNumber()
    {
      //  the current version number is 100
      new IpcData(new byte[] { 200, 0, 0, 0 });
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Unknown argument type, I am unable to read the size/data for it.")]
    public void TestDataTypeIsUnknown()
    {
      //  current version number is 100

      new IpcData(new byte[] { 100, 0, 0, 0, //  version
                                12, 0         //  unknown type
                              });
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException), "Unknown argument type, I am unable to read the size/data for it.")]
    public void TestNotEnoughDataForDataType()
    {
      //  current version number is 100

      new IpcData(new byte[] { 100, 0, 0, 0, //  version
                                2             //  too small.
                              });
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException), "There is no data to read the Guid.")]
    public void TestNoDataToReadGuid()
    {
      //  give a data type of int32, but no data to read
      new IpcData(new byte[] { 100, 0, 0, 0, //  version
                                (byte) ExpectedIpcDataType.Guid, 0          //  data type guid
                              });
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException), "There is no data to read the int32.")]
    public void TestNoDataToReadInteger()
    {
      //  give a data type of int32, but no data to read
      new IpcData(new byte[] { 100, 0, 0, 0, //  version
                                (byte) ExpectedIpcDataType.Int32, 0          //  data type int32
                              });
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException), "There is no data to read the int64.")]
    public void TestNoDataToReadLong()
    {
      //  give a data type of int32, but no data to read
      new IpcData(new byte[] { 100, 0, 0, 0, //  version
                                (byte) ExpectedIpcDataType.Int64, 0          //  data type int64
                              });
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException), "There is no data to read the string.")]
    public void TestNoDataToReadString()
    {
      //  give a data type of int32, but no data to read
      new IpcData(new byte[] { 100, 0, 0, 0, //  version
                                (byte) ExpectedIpcDataType.String, 0          //  data type string
                              });
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException), "There is no data to read the ascii string.")]
    public void TestNoDataToReadStringAscii()
    {
      //  give a data type of int32, but no data to read
      new IpcData(new byte[] { 100, 0, 0, 0, //  version
                                (byte) ExpectedIpcDataType.StringAscii, 0          //  data type string ascii
                              });
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException), "There is no data to read the guid.")]
    public void TestNotEnoughDataToReadAGuid()
    {
      //  give a data type of int32, but no data to read
      new IpcData(new byte[] { 100, 0, 0, 0, //  version
                                (byte) ExpectedIpcDataType.Guid, 0,         //  data type guid
                                0, 0, 0       //  not enough data to make an int32
                              });
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException), "There is no data to read the guid.")]
    public void TestCanReadGuidSizeButNoData()
    {
      //  give a data type of int32, but no data to read
      new IpcData(new byte[] { 100, 0, 0, 0, //  version
                                (byte) ExpectedIpcDataType.Guid, 0,        //  data type guid
                                10, 0, 0,0   //  size of 10
                                             //  but no more data
                              });
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException), "There is no data to read the guid.")]
    public void TestCanReadStringSizeButNoData()
    {
      //  give a data type of int32, but no data to read
      new IpcData(new byte[] { 100, 0, 0, 0, //  version
                                (byte) ExpectedIpcDataType.String, 0,        //  data type string
                                10, 0, 0,0   //  size of 10
                                             //  but no more data
                              });
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException), "There is no data to read the guid.")]
    public void TestCanReadAsciiStringSizeButNoData()
    {
      //  give a data type of int32, but no data to read
      new IpcData(new byte[] { 100, 0, 0, 0, //  version
                                (byte) ExpectedIpcDataType.StringAscii, 0,        //  data type ascii string
                                10, 0, 0,0   //  size of 10
                                             //  but no more data
                              });
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException), "There is no data to read the int32.")]
    public void TestNotEnoughDataToReadAnInteger()
    {
      //  give a data type of int32, but no data to read
      new IpcData(new byte[] { 100, 0, 0, 0, //  version
                                (byte) ExpectedIpcDataType.Int32, 0,         //  data type int32
                                0, 0, 0       //  not enough data to make an int32
                              });
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException), "There is no data to read the unicode string.")]
    public void TestNotEnoughDataToReadAString()
    {
      //  give a data type of int32, but no data to read
      new IpcData(new byte[] { 100, 0, 0, 0, //  version
                                (byte) ExpectedIpcDataType.String, 0,         //  data type unicode string
                                0, 0, 0       //  not enough data to make an int32
                              });
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException), "There is no data to read the unicode string.")]
    public void TestNotEnoughDataToReadAnAsciiString()
    {
      //  give a data type of int32, but no data to read
      new IpcData(new byte[] { 100, 0, 0, 0, //  version
                                (byte) ExpectedIpcDataType.StringAscii, 0,         //  data type ascii string
                                0, 0, 0       //  not enough data to make an int32
                              });
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException), "There is no data to read the int32.")]
    public void TestCanReadOneInt32ButNotEnoughDataToReadAnotherInteger()
    {
      //  give a data type of int32, but no data to read
      new IpcData(new byte[] { 100, 0, 0, 0, //  version
                                 (byte) ExpectedIpcDataType.Int32, 0,         //  data type int32
                                 20, 0, 0, 0,  //  first interger is valid
                                 (byte) ExpectedIpcDataType.Int32, 0,         //  data type int32
                                 0, 0, 0       //  not enough data to make an int32
                               });
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException), "There is no data to read the int32.")]
    public void TestCanReadOneInt64ButNotEnoughDataToReadAnotherInteger()
    {
      //  give a data type of int32, but no data to read
      new IpcData(new byte[] { 100, 0, 0, 0,           //  version
                                 (byte) ExpectedIpcDataType.Int64, 0,                 //  data type int64
                                 20, 0, 0, 0,0,0,0,0,  //  first long is valid
                                 (byte) ExpectedIpcDataType.Int64, 0,                 //  data type int64
                                 20, 0, 0, 0,0,0       //  not enough data to make an int64
                               });
    }

    [TestMethod]
    public void TestStringCanBeSizeZero()
    {
      //  give a data type of int32, but no data to read
      new IpcData(new byte[] { 100, 0, 0, 0, //  version
                                (byte) ExpectedIpcDataType.String, 0,        //  data type string
                                0, 0, 0,0    //  size of 0
                                             //  no more data needed.
                              });
    }

    [TestMethod]
    public void TestCastingToDifferentNumbers()
    {
      var emptyIpcData = new IpcData();
      emptyIpcData.Add(42);
      emptyIpcData.Add(52);
      emptyIpcData.Add(67);

      // check the values.
      Assert.AreEqual((long)42, emptyIpcData.Get<long>(0));
      Assert.AreEqual((uint)52, emptyIpcData.Get<uint>(1));
      Assert.AreEqual((int)67, emptyIpcData.Get<int>(2));
      Assert.AreEqual((short)67, emptyIpcData.Get<short>(2));

      Assert.AreEqual((double)67, emptyIpcData.Get<double>(2));
      Assert.AreEqual((float)67, emptyIpcData.Get<float>(2));
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidCastException), "Tring to convert a string to a number.")]
    public void TestTryingtogetANumberWhenTheArgumentIsAString()
    {
      var emptyIpcData = new IpcData();
      emptyIpcData.Add("Hello");

      // this is not a number.
      emptyIpcData.Get<long>(0);
    }

    [TestMethod]
    public void TestANumberCanBeCastToAString()
    {
      var emptyIpcData = new IpcData();
      emptyIpcData.Add( 12 );

      // we can cast the number to a string.
      Assert.AreEqual( "12", emptyIpcData.Get<string>(0) );
    }

    [TestMethod]
    public void TestByteArraySetsIntOneByte()
    {
      var random = new Random();
      var r1 = (byte)random.Next(0, 255);

      //  give a data type of int32, but no data to read
      var emptyIpcData = new IpcData(new byte[] { 100, 0, 0, 0, //  version
                                (byte) ExpectedIpcDataType.Int32, 0,        //  data type int
                                r1, 0, 0,0   //  int value
                              });

      Assert.AreEqual(r1 , emptyIpcData.Get<int>(0));
    }

    [TestMethod]
    public void TestByteArraySetsIntTwoBytes()
    {
      var random = new Random();
      var r1 = (byte)random.Next(0, 255);
      var r2 = (byte)random.Next(0, 255);

      //  give a data type of int32, but no data to read
      var emptyIpcData = new IpcData(new byte[] { 100, 0, 0, 0, //  version
                                (byte) ExpectedIpcDataType.Int32, 0,        //  data type int
                                r1, r2, 0,0   //  int value
                              });

      Assert.AreEqual((r1+r2*256), emptyIpcData.Get<int>(0));
    }

    [TestMethod]
    public void TestByteArraySetsIntThreeBytes()
    {
      var random = new Random();
      var r1 = (byte)random.Next(0, 255);
      var r2 = (byte)random.Next(0, 255);
      var r3 = (byte)random.Next(0, 255);

      //  give a data type of int32, but no data to read
      var emptyIpcData = new IpcData(new byte[] { 100, 0, 0, 0, //  version
                                (byte) ExpectedIpcDataType.Int32, 0,        //  data type int
                                r1, r2, r3,0   //  int value
                              });

      Assert.AreEqual((r1 + r2 * 256 + r3*256*256), emptyIpcData.Get<int>(0));
    }

    [TestMethod]
    public void TestByteArraySetsIntFourBytes()
    {
      var random = new Random();
      var r1 = (byte)random.Next(0, 255);
      var r2 = (byte)random.Next(0, 255);
      var r3 = (byte)random.Next(0, 255);
      var r4 = (byte)random.Next(0, 255);

      //  give a data type of int32, but no data to read
      var emptyIpcData = new IpcData(new byte[] { 100, 0, 0, 0, //  version
                                (byte) ExpectedIpcDataType.Int32, 0,        //  data type int
                                r1, r2, r3,r4   //  int value
                              });

      Assert.AreEqual((r1 + r2 * 256 + r3 * 256 * 256 + r4 * 256 * 256*256), emptyIpcData.Get<int>(0));
    }

    [TestMethod]
    public void TestIsNumeric()
    {
      //  give a data type of int32, but no data to read
      var emptyIpcData = new IpcData(new byte[] { 100, 0, 0, 0, //  version
                                (byte) ExpectedIpcDataType.Int32, 0,        //  data type int
                                32, 0, 0,0   //  int value
                              });

      Assert.IsTrue(emptyIpcData.IsInt(0));
    }

    [TestMethod]
    public void TestWideStringIsString()
    {
      //  give a data type of int32, but no data to read
      var emptyIpcData = new IpcData(new byte[] { 100, 0, 0, 0, //  version
                                (byte) ExpectedIpcDataType.String, 0,        //  data type string
                                5, 0, 0,0,   //  the len
                                (byte)'h', 0,
                                (byte)'e', 0,
                                (byte)'l', 0,
                                (byte)'l', 0,
                                (byte)'o', 0,
                              });

      Assert.IsTrue(emptyIpcData.IsString(0));
      Assert.AreEqual("hello", emptyIpcData.Get<string>(0));
    }

    [TestMethod]
    public void TestAsciiStringIsString()
    {
      //  give a data type of int32, but no data to read
      var emptyIpcData = new IpcData(new byte[] { 100, 0, 0, 0, //  version
                                (byte) ExpectedIpcDataType.StringAscii, 0,        //  data type string
                                5, 0, 0,0,   //  the len
                                (byte)'h',
                                (byte)'e',
                                (byte)'l',
                                (byte)'l',
                                (byte)'o',
                              });

      Assert.IsTrue(emptyIpcData.IsString(0));
      Assert.AreEqual("hello", emptyIpcData.Get<string>(0));
    }

    [TestMethod]
    public void TestIsNumericPastGuid()
    {
      var emptyIpcData = new IpcData();
      emptyIpcData.Add(12);

      Assert.IsTrue(emptyIpcData.IsInt(0));
      Assert.IsFalse(emptyIpcData.IsString(0));
    }

    [TestMethod]
    public void TestIsStringPastGuid()
    {
      var emptyIpcData = new IpcData();
      emptyIpcData.Add("Hello");

      Assert.IsFalse(emptyIpcData.IsInt(0));
      Assert.IsTrue(emptyIpcData.IsString(0));
    }

    [TestMethod]
    public void TestIsNumericPastGuidAndStrings()
    {
      var random = new Random();
      var r1 = random.Next(0, int.MaxValue);
      var r2 = random.Next(0, int.MaxValue);

      var emptyIpcData = new IpcData();
      emptyIpcData.Add(r1);
      emptyIpcData.Add("Hello");
      emptyIpcData.Add(r2);

      Assert.IsTrue(emptyIpcData.IsInt(0));
      Assert.IsFalse(emptyIpcData.IsInt(1));
      Assert.IsTrue(emptyIpcData.IsInt(2));

      Assert.AreEqual(r1, emptyIpcData.Get<int>(0));
      Assert.AreEqual(r2, emptyIpcData.Get<int>(2));
    }

    [TestMethod]
    public void TestIsNumericPastGuidAndInteger()
    {
      var random = new Random();
      var r1 = random.Next(0, Int32.MaxValue);

      var emptyIpcData = new IpcData();
      emptyIpcData.Add("Hello1");
      emptyIpcData.Add(r1);
      emptyIpcData.Add("Hello2");

      Assert.IsTrue(emptyIpcData.IsString(0));
      Assert.IsFalse(emptyIpcData.IsString(1));
      Assert.IsTrue(emptyIpcData.IsString(2));

      Assert.AreEqual("Hello1", emptyIpcData.Get<string>(0));
      Assert.AreEqual("Hello2", emptyIpcData.Get<string>(2));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException), "Trying to check if an index out of range is an integer.")]
    public void TestIsNumericInvalidIndex()
    {
      var emptyIpcData = new IpcData();
      emptyIpcData.Add(12);
      emptyIpcData.Add("Hello");

      emptyIpcData.IsInt(2);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException), "Trying to check if an index out of range is a string.")]
    public void TestIsStringInvalidIndex()
    {
      var emptyIpcData = new IpcData();
      emptyIpcData.Add(12);
      emptyIpcData.Add("Hello");

      emptyIpcData.IsString(2);
    }

    [TestMethod]
    public void TestGetBooleanFromIntegerEqualOne()
    {
      var emptyIpcData = new IpcData();
      emptyIpcData.Add(1);
      Assert.IsTrue(emptyIpcData.Get<bool>(0)); 
    }

    [TestMethod]
    public void TestGetBooleanFromIntegerNotEqualOne()
    {
      var random = new Random();
      var r = random.Next(2, Int32.MaxValue);
      if(random.Next(0, 1) == 1)
      {
        r *= -1;
      }

      var emptyIpcData = new IpcData();
      emptyIpcData.Add(1);
      Assert.IsTrue(emptyIpcData.Get<bool>(0));
    }

    [TestMethod]
    public void TestGetBooleanTrueString()
    {
      var emptyIpcData = new IpcData();
      emptyIpcData.Add("True");
      Assert.IsTrue(emptyIpcData.Get<bool>(0));
    }

    [TestMethod]
    public void TestGetBooleanFalseString()
    {
      var emptyIpcData = new IpcData();
      emptyIpcData.Add("False");
      Assert.IsFalse(emptyIpcData.Get<bool>(0));
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidCastException), "The given string is not a valid boolean.")]
    public void TestGetBooleanNeitherTrueNorFalseString()
    {
      var emptyIpcData = new IpcData();
      emptyIpcData.Add("NotAValidString");
      emptyIpcData.Get<bool>(0);
    }

    [TestMethod]
    public void TestFromInt64ToPtrAndBack()
    {
      var ipc = new IpcData();

      var random = new Random();
      var n = (long)(random.NextDouble() * long.MaxValue);
      var uuid = ipc.Guid;
      ipc.Add( long.MaxValue );
      ipc.Add(n);

      var payload = ipc.GetPtr();
      var dataBlockSize = ipc.GetSize();
      var byteArray = new byte[dataBlockSize];

      System.Runtime.InteropServices.Marshal.Copy(payload, byteArray, 0, dataBlockSize);
      var ipc2 = new IpcData(byteArray);

      Assert.AreEqual(uuid, ipc2.Guid);
      Assert.AreEqual(Int64.MaxValue, ipc2.Get<long>(0));
      Assert.AreEqual(n, ipc2.Get<long>(1));
    }

    [TestMethod]
    public void TestFromInt32ToPtrAndBack()
    {
      var ipc = new IpcData();

      var uuid = ipc.Guid;
      var random = new Random();
      var n = (Int32)random.Next(0, Int32.MaxValue);

      ipc.Add(Int32.MaxValue);
      ipc.Add(n);

      var payload = ipc.GetPtr();
      var dataBlockSize = ipc.GetSize();
      var byteArray = new byte[dataBlockSize];

      System.Runtime.InteropServices.Marshal.Copy(payload, byteArray, 0, dataBlockSize);
      var ipc2 = new IpcData(byteArray);

      Assert.AreEqual(uuid, ipc2.Guid);
      Assert.AreEqual(Int32.MaxValue, ipc2.Get<Int32>(0));
      Assert.AreEqual(n, ipc2.Get<Int32>(1));
    }

    [TestMethod]
    public void TestFromInt64And32ToPtrAndBack()
    {
      var ipc = new IpcData();

      var random = new Random();
      var m = (int)(random.NextDouble() * int.MaxValue);
      var n = (long)(random.NextDouble() * long.MaxValue);

      var uuid = ipc.Guid;
      ipc.Add(m);
      ipc.Add(n);

      var payload = ipc.GetPtr();
      var dataBlockSize = ipc.GetSize();
      var byteArray = new byte[dataBlockSize];

      System.Runtime.InteropServices.Marshal.Copy(payload, byteArray, 0, dataBlockSize);
      var ipc2 = new IpcData(byteArray);

      Assert.AreEqual(uuid, ipc2.Guid);
      Assert.AreEqual(m, ipc2.Get<int>(0));
      Assert.AreEqual(n, ipc2.Get<long>(1));
    }

    [TestMethod]
    public void TestFromInt32CanBeConvertedToInt64()
    {
      var ipc = new IpcData();

      var random = new Random();
      var m = (int)(random.NextDouble() * int.MaxValue);
      var n = (long)(random.NextDouble() * long.MaxValue);

      ipc.Add(m);
      ipc.Add(n);

      Assert.AreEqual(m, ipc.Get<int>(0));
      Assert.AreEqual(m, ipc.Get<long>(0)); // as a long
      Assert.AreEqual(n, ipc.Get<long>(1));
    }
  }
}
