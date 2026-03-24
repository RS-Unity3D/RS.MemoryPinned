using System;
using Xunit;
using RS.MemoryPinned.Extension;
using RS.MemoryPinned.Model;

namespace RS.MemoryPinned.Tests
{
    public class PinHandlerTests
    {
        [Fact]
        public void Pin_Array_ShouldCreatePinnedArray()
        {
            int[] array = new int[10];
            var pinned = PinHandler.Pin(ref array);
            Assert.NotNull(pinned);
            Assert.IsType<PinnedArray<int>>(pinned);
            pinned.Dispose();
        }

        [Fact]
        public void PinAsArray_ShouldCreatePinnedArrayWithSingleElement()
        {
            int value = 123;
            var pinned = PinHandler.PinAsArray(ref value);
            Assert.NotNull(pinned);
            Assert.Equal(1, pinned.Size);
            pinned.Dispose();
        }

        [Fact]
        public void PinAsStructure_ShouldCreatePinnedStructure()
        {
            TestStruct value = new TestStruct { X = 100, Y = 200 };
            var pinned = PinHandler.PinAsStructure(ref value);
            Assert.NotNull(pinned);
            Assert.IsType<PinnedStructure<TestStruct>>(pinned);
            pinned.Dispose();
        }

        [Fact]
        public void Pin_String_ShouldCreatePinnedString()
        {
            string text = "Hello";
            var pinned = PinHandler.Pin(ref text);
            Assert.NotNull(pinned);
            Assert.IsType<PinnedString>(pinned);
            pinned.Dispose();
        }

        [Fact]
        public void Pin_Generic_ShouldCreatePinnedObject()
        {
            int[] array = new int[10];
            var pinned = PinHandler.Pin<int[], int>(ref array);
            Assert.NotNull(pinned);
            pinned.Dispose();
        }

        [Fact]
        public unsafe void Pin_Pointer_ShouldCreatePinnedPointer()
        {
            int value = 123;
            int* ptr = &value;
            var pinned = PinHandler.Pin(ptr);
            Assert.NotNull(pinned);
            Assert.IsType<PinnedPointer>(pinned);
        }

        struct TestStruct
        {
            public int X;
            public int Y;
        }
    }
}
