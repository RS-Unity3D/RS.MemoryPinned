using System;
using RS.MemoryPinned;
using Xunit;

namespace RS.MemoryPinned.Tests
{
    public class UnsafeHelperTests
    {
        [Fact]
        public void Read_ShouldReadValueFromPointer()
        {
            int value = 12345;
            unsafe
            {
                int* ptr = &value;
                int result = UnsafeHelper.Read<int>(ptr);
                Assert.Equal(12345, result);
            }
        }

        [Fact]
        public void Write_ShouldWriteValueToPointer()
        {
            int value = 0;
            unsafe
            {
                int* ptr = &value;
                UnsafeHelper.Write(ptr, 99999);
                Assert.Equal(99999, value);
            }
        }

        [Fact]
        public void SizeOf_ShouldReturnCorrectSize()
        {
            Assert.Equal(4, UnsafeHelper.SizeOf<int>());
            Assert.Equal(8, UnsafeHelper.SizeOf<long>());
            Assert.Equal(2, UnsafeHelper.SizeOf<short>());
            Assert.Equal(1, UnsafeHelper.SizeOf<byte>());
            Assert.Equal(4, UnsafeHelper.SizeOf<float>());
            Assert.Equal(8, UnsafeHelper.SizeOf<double>());
        }

        [Fact]
        public void CopyBlock_ShouldCopyMemory()
        {
            int[] source = { 1, 2, 3, 4, 5 };
            int[] destination = new int[5];

            unsafe
            {
                fixed (int* srcPtr = source, dstPtr = destination)
                {
                    UnsafeHelper.CopyBlock(dstPtr, srcPtr, (uint)(5 * sizeof(int)));
                }
            }

            Assert.Equal(source, destination);
        }

        [Fact]
        public void InitBlock_ShouldInitializeMemory()
        {
            byte[] buffer = new byte[100];

            unsafe
            {
                fixed (byte* ptr = buffer)
                {
                   UnsafeHelper.InitBlock(ptr, 0xAB, 100);
                }
            }

            foreach (byte b in buffer)
            {
                Assert.Equal(0xAB, b);
            }
        }

        [Fact]
        public void Read_Write_ShouldWorkWithStruct()
        {
            TestStruct value = new TestStruct { X = 100, Y = 200 };
            TestStruct result;

            unsafe
            {
                TestStruct* ptr = &value;
                result = UnsafeHelper.Read<TestStruct>(ptr);
            }

            Assert.Equal(100, result.X);
            Assert.Equal(200, result.Y);
        }

        struct TestStruct
        {
            public int X;
            public int Y;
        }
    }
}
