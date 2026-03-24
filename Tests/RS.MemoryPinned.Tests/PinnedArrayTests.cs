using System;
using Xunit;
using RS.MemoryPinned.Extension;
using RS.MemoryPinned.Model;

namespace RS.MemoryPinned.Tests
{
    public class PinnedArrayTests
    {
        [Fact]
        public void Pin_ShouldCreatePinnedArray()
        {
            int[] array = new int[100];
            using (var pinned = array.Pin())
            {
                Assert.NotNull(pinned);
                Assert.Equal(100, pinned.Size);
                Assert.NotEqual(IntPtr.Zero, pinned.IntPtr);
            }
        }

        [Fact]
        public void Indexer_ShouldAccessElements()
        {
            int[] array = new int[10];
            for (int i = 0; i < 10; i++)
            {
                array[i] = i * 10;
            }

            using (var pinned = array.Pin())
            {
                for (int i = 0; i < 10; i++)
                {
                    Assert.Equal(i * 10, pinned[i]);
                }
            }
        }

        [Fact]
        public void Indexer_ShouldModifyElements()
        {
            int[] array = new int[10];
            using (var pinned = array.Pin())
            {
                for (int i = 0; i < 10; i++)
                {
                    pinned[i] = i * 100;
                }
            }

            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(i * 100, array[i]);
            }
        }

        [Fact]
        public void Bytes_ShouldAccessAsBytes()
        {
            int[] array = new int[1] { 0x12345678 };
            using (var pinned = array.Pin())
            {
                Assert.Equal(4, pinned.LengthAsBytes);
                Assert.NotNull(pinned.Bytes);
            }
        }

        [Fact]
        public void Shorts_ShouldAccessAsShorts()
        {
            int[] array = new int[2] { 0x00010002, 0x00030004 };
            using (var pinned = array.Pin())
            {
                Assert.Equal(4, pinned.LengthAsShorts);
                Assert.NotNull(pinned.Shorts);
            }
        }

        [Fact]
        public void Integers_ShouldAccessAsIntegers()
        {
            int[] array = new int[5];
            using (var pinned = array.Pin())
            {
                Assert.Equal(5, pinned.LengthAsIntegers);
                Assert.NotNull(pinned.Integers);
            }
        }

        [Fact]
        public void Longs_ShouldAccessAsLongs()
        {
            int[] array = new int[4];
            using (var pinned = array.Pin())
            {
                Assert.Equal(2, pinned.LengthAsLongs);
                Assert.NotNull(pinned.Longs);
            }
        }

        [Fact]
        public void Slice_ShouldCreateSlice()
        {
            int[] array = new int[100];
            for (int i = 0; i < 100; i++)
            {
                array[i] = i;
            }

            using (var pinned = array.Pin())
            {
                var slice = pinned.Slice(10, 20);
                Assert.Equal(20, slice.Size);
                Assert.Equal(10, slice.StartIndex);
            }
        }

        [Fact]
        public void Slice_ShouldShareMemory()
        {
            int[] array = new int[100];
            using (var pinned = array.Pin())
            {
                pinned.CheckIndexes = false;
                var slice = pinned.Slice(10, 10, sizeof(int));
                slice.CheckIndexes = false;
                slice[0] = 999;
            }

            Assert.Equal(999, array[10]);
        }

        [Fact]
        public void WriteElementAtIndex_ShouldWriteCorrectType()
        {
            int[] buffer = new int[4];
            using (var pinned = buffer.Pin())
            {
                pinned.WriteElementAtIndex(0, 0x12345678);
                pinned.WriteElementAtIndex(1, 0xABCDEF00);
            }

            Assert.Equal(0x12345678, buffer[0]);
            Assert.Equal(unchecked((int)0xABCDEF00), buffer[1]);
        }

        [Fact]
        public void ReadElementAtIndex_ShouldReadCorrectType()
        {
            int[] buffer = new int[4];
            buffer[0] = 0x12345678;
            buffer[1] = unchecked((int)0xABCDEF00);

            using (var pinned = buffer.Pin())
            {
                Assert.Equal(0x12345678, pinned.ReadElementAtIndex<int>(0));
                Assert.Equal(unchecked((int)0xABCDEF00), pinned.ReadElementAtIndex<int>(1));
            }
        }

        [Fact]
        public void Dispose_ShouldReleaseMemory()
        {
            int[] array = new int[10];
            var pinned = array.Pin();
            pinned.Dispose();

            Assert.True(pinned.IsDisposed);
        }

        [Fact]
        public void ImplicitOperator_ShouldConvertToArray()
        {
            int[] array = new int[10];
            using (var pinned = array.Pin())
            {
                int[] result = pinned;
                Assert.Same(array, result);
            }
        }

        [Fact]
        public void ExplicitOperator_ShouldConvertFromArray()
        {
            int[] array = new int[10];
            using (var pinned = (PinnedArray<int>)array)
            {
                Assert.NotNull(pinned);
            }
        }

        [Fact]
        public void CheckIndexes_ShouldThrowWhenOutOfRange()
        {
            int[] array = new int[10];
            using (var pinned = array.Pin())
            {
                pinned.CheckIndexes = true;
                Assert.Throws<IndexOutOfRangeException>(() => pinned[100] = 0);
            }
        }

        [Fact]
        public void CheckIndexes_WhenDisabled_ShouldNotThrow()
        {
            int[] array = new int[10];
            using (var pinned = array.Pin())
            {
                pinned.CheckIndexes = false;
                pinned[100] = 0;
            }
        }

        [Fact]
        public void IntPtrs_ShouldProvideIntPtrAccess()
        {
            int[] array = new int[10];
            using (var pinned = array.Pin())
            {
                Assert.NotNull(pinned.IntPtrs);
                for (int i = 0; i < 10; i++)
                {
                    Assert.NotEqual(IntPtr.Zero, pinned.IntPtrs[i]);
                }
            }
        }

        [Fact]
        public unsafe void Pointers_ShouldProvidePointerAccess()
        {
            int[] array = new int[10];
            using (var pinned = array.Pin())
            {
                Assert.NotNull(pinned.Pointers);
                for (int i = 0; i < 10; i++)
                {
                    Assert.True(pinned.Pointers[i] != null);
                }
            }
        }
    }
}
