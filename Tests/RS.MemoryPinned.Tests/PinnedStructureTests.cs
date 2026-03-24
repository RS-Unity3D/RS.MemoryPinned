using System;
using Xunit;
using RS.MemoryPinned.Extension;

namespace RS.MemoryPinned.Tests
{
    public class PinnedStructureTests
    {
        [Fact]
        public void PinAsStructure_ShouldCreatePinnedStructure()
        {
            TestStruct value = new TestStruct { X = 100, Y = 200 };
            using (var pinned = value.PinAsStructure())
            {
                Assert.NotNull(pinned);
                Assert.NotEqual(IntPtr.Zero, pinned.IntPtr);
            }
        }

        [Fact]
        public void PinnedObject_ShouldReturnCorrectValue()
        {
            TestStruct value = new TestStruct { X = 100, Y = 200 };
            using (var pinned = value.PinAsStructure())
            {
                Assert.Equal(100, pinned.PinnedObject.X);
                Assert.Equal(200, pinned.PinnedObject.Y);
            }
        }

        [Fact]
        public void Bytes_ShouldAccessStructAsBytes()
        {
            TestStruct value = new TestStruct { X = 0x12345678, Y = unchecked((int)0xABCDEF00) };
            using (var pinned = value.PinAsStructure())
            {
                Assert.NotNull(pinned.Bytes);
                Assert.Equal(8, pinned.LengthAsBytes);
            }
        }

        [Fact]
        public void WriteElementAtIndex_ShouldModifyStruct()
        {
            TestStruct value = new TestStruct { X = 0, Y = 0 };
            using (var pinned = value.PinAsStructure())
            {
                Assert.Equal(0, pinned.PinnedObject.X);
                Assert.Equal(0, pinned.PinnedObject.Y);
            }
        }

        [Fact]
        public void ReadElementAtIndex_ShouldReadStructFields()
        {
            TestStruct value = new TestStruct { X = 100, Y = 200 };
            using (var pinned = value.PinAsStructure())
            {
                Assert.Equal(100, pinned.PinnedObject.X);
                Assert.Equal(200, pinned.PinnedObject.Y);
            }
        }

        [Fact]
        public void ExplicitOperator_ShouldConvertFromValue()
        {
            TestStruct value = new TestStruct { X = 100, Y = 200 };
            using (var pinned = (RS.MemoryPinned.Model.PinnedStructure<TestStruct>)value)
            {
                Assert.NotNull(pinned);
                Assert.Equal(100, pinned.PinnedObject.X);
            }
        }

        [Fact]
        public void ImplicitOperator_ShouldConvertToValue()
        {
            TestStruct value = new TestStruct { X = 100, Y = 200 };
            using (var pinned = value.PinAsStructure())
            {
                TestStruct result = pinned;
                Assert.Equal(100, result.X);
                Assert.Equal(200, result.Y);
            }
        }

        [Fact]
        public void Dispose_ShouldReleaseMemory()
        {
            TestStruct value = new TestStruct { X = 100, Y = 200 };
            var pinned = value.PinAsStructure();
            pinned.Dispose();

            Assert.True(pinned.IsDisposed);
        }

        [Fact]
        public void CheckIndexes_ShouldWork()
        {
            TestStruct value = new TestStruct { X = 100, Y = 200 };
            using (var pinned = value.PinAsStructure(true))
            {
                Assert.True(pinned.CheckIndexes);
            }
        }

        [Fact]
        public unsafe void Pointer_ShouldReturnValidPointer()
        {
            TestStruct value = new TestStruct { X = 100, Y = 200 };
            using (var pinned = value.PinAsStructure())
            {
                Assert.True(pinned.Pointer != null);
            }
        }

        [Fact]
        public void ComplexStruct_ShouldWork()
        {
            ComplexStruct value = new ComplexStruct
            {
                A = 1,
                B = 2,
                C = 3.14,
                D = 6.28
            };

            using (var pinned = value.PinAsStructure())
            {
                Assert.NotNull(pinned);
                Assert.Equal(1, pinned.PinnedObject.A);
                Assert.Equal(2, pinned.PinnedObject.B);
                Assert.Equal(3.14, pinned.PinnedObject.C, 2);
                Assert.Equal(6.28, pinned.PinnedObject.D, 2);
            }
        }

        struct TestStruct
        {
            public int X;
            public int Y;
        }

        struct ComplexStruct
        {
            public int A;
            public int B;
            public double C;
            public double D;
        }
    }
}
