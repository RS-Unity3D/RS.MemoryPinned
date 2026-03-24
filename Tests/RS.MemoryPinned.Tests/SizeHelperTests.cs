using System;
using Xunit;

namespace RS.MemoryPinned.Tests
{
    public class SizeHelperTests
    {
        [Fact]
        public void SizeOfElement_ShouldReturnCorrectSizeForInt()
        {
            Assert.Equal(4, SizeHelper.SizeOfElement<int>());
        }

        [Fact]
        public void SizeOfElement_ShouldReturnCorrectSizeForLong()
        {
            Assert.Equal(8, SizeHelper.SizeOfElement<long>());
        }

        [Fact]
        public void SizeOfElement_ShouldReturnCorrectSizeForShort()
        {
            Assert.Equal(2, SizeHelper.SizeOfElement<short>());
        }

        [Fact]
        public void SizeOfElement_ShouldReturnCorrectSizeForByte()
        {
            Assert.Equal(1, SizeHelper.SizeOfElement<byte>());
        }

        [Fact]
        public void SizeOfElement_ShouldReturnCorrectSizeForChar()
        {
            Assert.Equal(2, SizeHelper.SizeOfElement<char>());
        }

        [Fact]
        public void SizeOfElement_ShouldReturnCorrectSizeForFloat()
        {
            Assert.Equal(4, SizeHelper.SizeOfElement<float>());
        }

        [Fact]
        public void SizeOfElement_ShouldReturnCorrectSizeForDouble()
        {
            Assert.Equal(8, SizeHelper.SizeOfElement<double>());
        }

        [Fact]
        public void GetSize_ShouldReturnCorrectSizeForArray()
        {
            int[] array = new int[100];
            int size = SizeHelper.GetSize<int>(array);
            Assert.Equal(400, size);
        }

        [Fact]
        public void GetSize_ShouldReturnCorrectSizeForString()
        {
            string text = "Hello";
            int size = SizeHelper.GetSize<char>(text);
            Assert.Equal(10, size);
        }

        [Fact]
        public void GetSize_ShouldReturnZeroForNull()
        {
            int size = SizeHelper.GetSize<int>(null);
            Assert.Equal(0, size);
        }

        [Fact]
        public void GetSize_ShouldReturnCorrectSizeForStruct()
        {
            TestStruct value = new TestStruct();
            int size = SizeHelper.GetSize<TestStruct>(value);
            Assert.Equal(8, size);
        }

        [Fact]
        public void GetIntPtr_ShouldReturnCorrectOffset()
        {
            IntPtr basePtr = new IntPtr(1000);
            IntPtr result = SizeHelper.GetIntPtr<int>(basePtr, 5);
            Assert.Equal(new IntPtr(1020), result);
        }

        [Fact]
        public void GetIntPtr_WithSize_ShouldReturnCorrectOffset()
        {
            IntPtr basePtr = new IntPtr(1000);
            IntPtr result = SizeHelper.GetIntPtr(basePtr, 4, 10);
            Assert.Equal(new IntPtr(1040), result);
        }

        struct TestStruct
        {
            public int X;
            public int Y;
        }
    }
}
