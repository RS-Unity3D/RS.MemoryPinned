using System;
using System.Runtime.InteropServices;
using RS.MemoryPinned;
using RS.MemoryPinned.Extension;
using RS.MemoryPinned.Model;
using Xunit;

namespace RS.MemoryPinned.Tests
{
    public class CommonDataTypesTests
    {
        [Fact]
        public void PinnedArray_Bool_ShouldWorkCorrectly()
        {
            bool[] array = new bool[5];
            array[0] = true;
            array[1] = false;

            using (var pinned = array.Pin())
            {
                Assert.True(pinned[0]);
                Assert.False(pinned[1]);

                pinned[2] = true;
                Assert.True(array[2]);

                pinned[3] = false;
                Assert.False(array[3]);
            }
        }

        [Fact]
        public void PinnedArray_Byte_ShouldWorkCorrectly()
        {
            byte[] array = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                array[i] = (byte)i;
            }

            using (var pinned = array.Pin())
            {
                for (int i = 0; i < 256; i++)
                {
                    Assert.Equal((byte)i, pinned[i]);
                }

                pinned[0] = 255;
                pinned[255] = 0;
                Assert.Equal(255, array[0]);
                Assert.Equal(0, array[255]);
            }
        }

        [Fact]
        public void PinnedArray_SByte_ShouldWorkCorrectly()
        {
            sbyte[] array = new sbyte[10];
            array[0] = -128;
            array[1] = 127;
            array[2] = 0;

            using (var pinned = array.Pin())
            {
                Assert.Equal(-128, pinned[0]);
                Assert.Equal(127, pinned[1]);
                Assert.Equal(0, pinned[2]);

                pinned[3] = -1;
                Assert.Equal(-1, array[3]);
            }
        }

        [Fact]
        public void PinnedArray_Short_ShouldWorkCorrectly()
        {
            short[] array = new short[10];
            array[0] = short.MinValue;
            array[1] = short.MaxValue;

            using (var pinned = array.Pin())
            {
                Assert.Equal(short.MinValue, pinned[0]);
                Assert.Equal(short.MaxValue, pinned[1]);

                pinned[2] = -12345;
                Assert.Equal(-12345, array[2]);
            }
        }

        [Fact]
        public void PinnedArray_UShort_ShouldWorkCorrectly()
        {
            ushort[] array = new ushort[10];
            array[0] = ushort.MinValue;
            array[1] = ushort.MaxValue;

            using (var pinned = array.Pin())
            {
                Assert.Equal(ushort.MinValue, pinned[0]);
                Assert.Equal(ushort.MaxValue, pinned[1]);

                pinned[2] = 12345;
                Assert.Equal(12345, array[2]);
            }
        }

        [Fact]
        public void PinnedArray_Int_ShouldWorkCorrectly()
        {
            int[] array = new int[10];
            array[0] = int.MinValue;
            array[1] = int.MaxValue;

            using (var pinned = array.Pin())
            {
                Assert.Equal(int.MinValue, pinned[0]);
                Assert.Equal(int.MaxValue, pinned[1]);

                pinned[2] = -123456;
                Assert.Equal(-123456, array[2]);
            }
        }

        [Fact]
        public void PinnedArray_UInt_ShouldWorkCorrectly()
        {
            uint[] array = new uint[10];
            array[0] = uint.MinValue;
            array[1] = uint.MaxValue;

            using (var pinned = array.Pin())
            {
                Assert.Equal(uint.MinValue, pinned[0]);
                Assert.Equal(uint.MaxValue, pinned[1]);

                pinned[2] = 123456789;
                Assert.Equal(123456789u, array[2]);
            }
        }

        [Fact]
        public void PinnedArray_Long_ShouldWorkCorrectly()
        {
            long[] array = new long[10];
            array[0] = long.MinValue;
            array[1] = long.MaxValue;

            using (var pinned = array.Pin())
            {
                Assert.Equal(long.MinValue, pinned[0]);
                Assert.Equal(long.MaxValue, pinned[1]);

                pinned[2] = -1234567890123L;
                Assert.Equal(-1234567890123L, array[2]);
            }
        }

        [Fact]
        public void PinnedArray_ULong_ShouldWorkCorrectly()
        {
            ulong[] array = new ulong[10];
            array[0] = ulong.MinValue;
            array[1] = ulong.MaxValue;

            using (var pinned = array.Pin())
            {
                Assert.Equal(ulong.MinValue, pinned[0]);
                Assert.Equal(ulong.MaxValue, pinned[1]);

                pinned[2] = 12345678901234567890;
                Assert.Equal(12345678901234567890ul, array[2]);
            }
        }

        [Fact]
        public void PinnedArray_Float_ShouldWorkCorrectly()
        {
            float[] array = new float[10];
            array[0] = float.MinValue;
            array[1] = float.MaxValue;
            array[2] = float.Epsilon;
            array[3] = float.NaN;
            array[4] = float.PositiveInfinity;
            array[5] = float.NegativeInfinity;

            using (var pinned = array.Pin())
            {
                Assert.Equal(float.MinValue, pinned[0]);
                Assert.Equal(float.MaxValue, pinned[1]);
                Assert.Equal(float.Epsilon, pinned[2]);
                Assert.True(float.IsNaN(pinned[3]));
                Assert.True(float.IsPositiveInfinity(pinned[4]));
                Assert.True(float.IsNegativeInfinity(pinned[5]));

                pinned[6] = 3.14159f;
                Assert.Equal(3.14159f, array[6]);
            }
        }

        [Fact]
        public void PinnedArray_Double_ShouldWorkCorrectly()
        {
            double[] array = new double[10];
            array[0] = double.MinValue;
            array[1] = double.MaxValue;
            array[2] = double.Epsilon;
            array[3] = double.NaN;
            array[4] = double.PositiveInfinity;
            array[5] = double.NegativeInfinity;

            using (var pinned = array.Pin())
            {
                Assert.Equal(double.MinValue, pinned[0]);
                Assert.Equal(double.MaxValue, pinned[1]);
                Assert.Equal(double.Epsilon, pinned[2]);
                Assert.True(double.IsNaN(pinned[3]));
                Assert.True(double.IsPositiveInfinity(pinned[4]));
                Assert.True(double.IsNegativeInfinity(pinned[5]));

                pinned[6] = 3.14159265358979;
                Assert.Equal(3.14159265358979, array[6]);
            }
        }

        [Fact]
        public void PinnedArray_Char_ShouldWorkCorrectly()
        {
            char[] array = new char[10];
            array[0] = 'A';
            array[1] = 'Z';
            array[2] = '0';

            using (var pinned = array.Pin())
            {
                Assert.Equal('A', pinned[0]);
                Assert.Equal('Z', pinned[1]);
                Assert.Equal('0', pinned[2]);

                pinned[3] = '\n';
                Assert.Equal('\n', array[3]);
            }
        }

        [Fact]
        public void PinnedArray_IntPtr_ShouldWorkCorrectly()
        {
            IntPtr[] array = new IntPtr[10];
            array[0] = IntPtr.Zero;
            array[1] = new IntPtr(12345);
            array[2] = new IntPtr(-67890);

            using (var pinned = array.Pin())
            {
                Assert.Equal(IntPtr.Zero, pinned[0]);
                Assert.Equal(new IntPtr(12345), pinned[1]);
                Assert.Equal(new IntPtr(-67890), pinned[2]);

                pinned[3] = new IntPtr(int.MaxValue);
                Assert.Equal(new IntPtr(int.MaxValue), array[3]);
            }
        }

        [Fact]
        public void PinnedArray_UIntPtr_ShouldWorkCorrectly()
        {
            UIntPtr[] array = new UIntPtr[10];
            array[0] = UIntPtr.Zero;
            array[1] = new UIntPtr(12345);
            array[2] = new UIntPtr(uint.MaxValue);

            using (var pinned = array.Pin())
            {
                Assert.Equal(UIntPtr.Zero, pinned[0]);
                Assert.Equal(new UIntPtr(12345), pinned[1]);
                Assert.Equal(new UIntPtr(uint.MaxValue), pinned[2]);

                pinned[3] = new UIntPtr(99999);
                Assert.Equal(new UIntPtr(99999), array[3]);
            }
        }

        [Fact]
        public unsafe void UnsafeHelper_CopyBlock_TimeSpanArray_ShouldCopyCorrectly()
        {
            TimeSpan[] source = new TimeSpan[10];
            TimeSpan[] destination = new TimeSpan[10];

            for (int i = 0; i < 10; i++)
            {
                source[i] = TimeSpan.FromMinutes(i * 10);
            }

            fixed (TimeSpan* srcPtr = source, dstPtr = destination)
            {
                UnsafeHelper.CopyBlock(dstPtr, srcPtr, (uint)(10 * sizeof(TimeSpan)));
            }

            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(source[i], destination[i]);
            }
        }

        [Fact]
        public unsafe void UnsafeHelper_CopyBlock_DecimalArray_ShouldCopyCorrectly()
        {
            decimal[] source = new decimal[10];
            decimal[] destination = new decimal[10];

            for (int i = 0; i < 10; i++)
            {
                source[i] = i * 1.5m;
            }

            fixed (decimal* srcPtr = source, dstPtr = destination)
            {
                UnsafeHelper.CopyBlock(dstPtr, srcPtr, (uint)(10 * sizeof(decimal)));
            }

            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(source[i], destination[i]);
            }
        }

        [Fact]
        public unsafe void UnsafeHelper_CopyBlock_GuidArray_ShouldCopyCorrectly()
        {
            Guid[] source = new Guid[10];
            Guid[] destination = new Guid[10];

            for (int i = 0; i < 10; i++)
            {
                source[i] = Guid.NewGuid();
            }

            fixed (Guid* srcPtr = source, dstPtr = destination)
            {
                UnsafeHelper.CopyBlock(dstPtr, srcPtr, (uint)(10 * sizeof(Guid)));
            }

            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(source[i], destination[i]);
            }
        }

        [Fact]
        public void PinnedArray_MixedNumericTypes_SliceAccess()
        {
            byte[] buffer = new byte[1024];

            using (var pinned = buffer.Pin())
            {
                for (int i = 0; i < 16; i++)
                {
                    pinned.Integers[i] = i * 100;
                }

                for (int i = 0; i < 16; i++)
                {
                    Assert.Equal(i * 100, pinned.Integers[i]);
                }

                for (int i = 0; i < 8; i++)
                {
                    pinned.Longs[i] = i * 1000L;
                }

                for (int i = 0; i < 8; i++)
                {
                    Assert.Equal(i * 1000L, pinned.Longs[i]);
                }
            }
        }

        [Fact]
        public void SizeHelper_SizeOfElement_PrimitiveTypes()
        {
            Assert.Equal(1, SizeHelper.SizeOfElement<byte>());
            Assert.Equal(1, SizeHelper.SizeOfElement<sbyte>());
            Assert.Equal(2, SizeHelper.SizeOfElement<short>());
            Assert.Equal(2, SizeHelper.SizeOfElement<ushort>());
            Assert.Equal(4, SizeHelper.SizeOfElement<int>());
            Assert.Equal(4, SizeHelper.SizeOfElement<uint>());
            Assert.Equal(8, SizeHelper.SizeOfElement<long>());
            Assert.Equal(8, SizeHelper.SizeOfElement<ulong>());
            Assert.Equal(4, SizeHelper.SizeOfElement<float>());
            Assert.Equal(8, SizeHelper.SizeOfElement<double>());
            Assert.Equal(2, SizeHelper.SizeOfElement<char>());
            Assert.Equal(1, SizeHelper.SizeOfElement<bool>());
        }

        [Fact]
        public unsafe void SizeHelper_SizeOfElement_ComplexTypes()
        {
            Assert.Equal(16, SizeHelper.SizeOfElement<Guid>());
            Assert.Equal(8, SizeHelper.SizeOfElement<TimeSpan>());
            Assert.Equal(16, SizeHelper.SizeOfElement<decimal>());
            Assert.Equal(sizeof(IntPtr), SizeHelper.SizeOfElement<IntPtr>());
            Assert.Equal(sizeof(UIntPtr), SizeHelper.SizeOfElement<UIntPtr>());
        }

        [Fact]
        public void UnsafeHelper_SizeOf_PrimitiveTypes()
        {
            Assert.Equal(1, UnsafeHelper.SizeOf<byte>());
            Assert.Equal(1, UnsafeHelper.SizeOf<sbyte>());
            Assert.Equal(2, UnsafeHelper.SizeOf<short>());
            Assert.Equal(2, UnsafeHelper.SizeOf<ushort>());
            Assert.Equal(4, UnsafeHelper.SizeOf<int>());
            Assert.Equal(4, UnsafeHelper.SizeOf<uint>());
            Assert.Equal(8, UnsafeHelper.SizeOf<long>());
            Assert.Equal(8, UnsafeHelper.SizeOf<ulong>());
            Assert.Equal(4, UnsafeHelper.SizeOf<float>());
            Assert.Equal(8, UnsafeHelper.SizeOf<double>());
            Assert.Equal(2, UnsafeHelper.SizeOf<char>());
            Assert.Equal(1, UnsafeHelper.SizeOf<bool>());
        }

        [Fact]
        public void UnsafeHelper_SizeOf_ComplexTypes()
        {
            Assert.Equal(16, UnsafeHelper.SizeOf<Guid>());
            Assert.Equal(8, UnsafeHelper.SizeOf<TimeSpan>());
            Assert.Equal(16, UnsafeHelper.SizeOf<decimal>());
        }

        [Fact]
        public unsafe void UnsafeHelper_ReadWrite_PrimitiveTypes()
        {
            int intValue = 12345;
            int intResult = UnsafeHelper.Read<int>(&intValue);
            Assert.Equal(12345, intResult);

            long longValue = 9876543210L;
            long longResult = UnsafeHelper.Read<long>(&longValue);
            Assert.Equal(9876543210L, longResult);

            double doubleValue = 3.14159265358979;
            double doubleResult = UnsafeHelper.Read<double>(&doubleValue);
            Assert.Equal(3.14159265358979, doubleResult);

            float floatValue = 2.71828f;
            float floatResult = UnsafeHelper.Read<float>(&floatValue);
            Assert.Equal(2.71828f, floatResult);
        }

        [Fact]
        public unsafe void UnsafeHelper_ReadWrite_ComplexTypes()
        {
            Guid guidValue = Guid.NewGuid();
            Guid guidResult = UnsafeHelper.Read<Guid>(&guidValue);
            Assert.Equal(guidValue, guidResult);

            TimeSpan timeSpanValue = TimeSpan.FromHours(2.5);
            TimeSpan timeSpanResult = UnsafeHelper.Read<TimeSpan>(&timeSpanValue);
            Assert.Equal(timeSpanValue, timeSpanResult);

            decimal decimalValue = 12345.6789m;
            decimal decimalResult = UnsafeHelper.Read<decimal>(&decimalValue);
            Assert.Equal(decimalValue, decimalResult);
        }
    }
}
