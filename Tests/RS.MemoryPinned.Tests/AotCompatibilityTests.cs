using System;
using System.Runtime.InteropServices;
using RS.MemoryPinned;
using RS.MemoryPinned.Extension;
using RS.MemoryPinned.Model;
using Xunit;

namespace RS.MemoryPinned.Tests
{
    public class AotCompatibilityTests
    {
        [Fact]
        public void UnsafeHelper_Read_ShouldWorkWithVariousTypes()
        {
            TestPrimitiveRead(123);
            TestPrimitiveRead(123L);
            TestPrimitiveRead((short)123);
            TestPrimitiveRead((byte)123);
            TestPrimitiveRead(123.45f);
            TestPrimitiveRead(123.45);
            TestPrimitiveRead('A');
        }

        private unsafe void TestPrimitiveRead<T>(T value) where T : struct
        {
            T result = UnsafeHelper.Read<T>(&value);
            Assert.Equal(value, result);
        }

        [Fact]
        public void UnsafeHelper_Write_ShouldWorkWithVariousTypes()
        {
            TestPrimitiveWrite(123);
            TestPrimitiveWrite(123L);
            TestPrimitiveWrite((short)123);
            TestPrimitiveWrite((byte)123);
            TestPrimitiveWrite(123.45f);
            TestPrimitiveWrite(123.45);
        }

        private unsafe void TestPrimitiveWrite<T>(T value) where T : struct
        {
            T target = default;
            UnsafeHelper.Write(&target, value);
            Assert.Equal(value, target);
        }

        [Fact]
        public void UnsafeHelper_SizeOf_ShouldReturnCorrectSizes()
        {
            Assert.Equal(4, UnsafeHelper.SizeOf<int>());
            Assert.Equal(8, UnsafeHelper.SizeOf<long>());
            Assert.Equal(2, UnsafeHelper.SizeOf<short>());
            Assert.Equal(1, UnsafeHelper.SizeOf<byte>());
            Assert.Equal(4, UnsafeHelper.SizeOf<float>());
            Assert.Equal(8, UnsafeHelper.SizeOf<double>());
            Assert.True(UnsafeHelper.SizeOf<Guid>() > 0);
        }

        [Fact]
        public void UnsafeHelper_CopyBlock_ShouldCopyLargeBlocks()
        {
            int size = 10000;
            byte[] source = new byte[size];
            byte[] destination = new byte[size];

            Random rand = new Random(42);
            rand.NextBytes(source);

            unsafe
            {
                fixed (byte* srcPtr = source, dstPtr = destination)
                {
                    UnsafeHelper.CopyBlock(dstPtr, srcPtr, (uint)size);
                }
            }

            Assert.Equal(source, destination);
        }

        [Fact]
        public unsafe void UnsafeHelper_CopyBlock_ShouldCopyStructArrays()
        {
            ComplexStruct[] source = new ComplexStruct[100];
            ComplexStruct[] destination = new ComplexStruct[100];

            for (int i = 0; i < 100; i++)
            {
                source[i] = new ComplexStruct
                {
                    A = i,
                    B = i * 2.0,
                    C = (byte)(i % 256),
                    D = i * 3L
                };
            }

            fixed (ComplexStruct* srcPtr = source, dstPtr = destination)
            {
                UnsafeHelper.CopyBlock(dstPtr, srcPtr, (uint)(100 * sizeof(ComplexStruct)));
            }

            for (int i = 0; i < 100; i++)
            {
                Assert.Equal(source[i].A, destination[i].A);
                Assert.Equal(source[i].B, destination[i].B);
                Assert.Equal(source[i].C, destination[i].C);
                Assert.Equal(source[i].D, destination[i].D);
            }
        }

        [Fact]
        public unsafe void UnsafeHelper_CopyBlockUnaligned_ShouldCopyCorrectly()
        {
            byte[] source = new byte[100];
            byte[] destination = new byte[100];

            for (int i = 0; i < 100; i++)
            {
                source[i] = (byte)i;
            }

            fixed (byte* srcPtr = source, dstPtr = destination)
            {
                UnsafeHelper.CopyBlockUnaligned(dstPtr, srcPtr, 100);
            }

            Assert.Equal(source, destination);
        }

        [Fact]
        public unsafe void UnsafeHelper_InitBlock_ShouldInitializeToValue()
        {
            byte[] buffer = new byte[1000];
            byte fillValue = 0x7F;

            fixed (byte* ptr = buffer)
            {
                UnsafeHelper.InitBlock(ptr, fillValue, 1000);
            }

            foreach (byte b in buffer)
            {
                Assert.Equal(fillValue, b);
            }
        }

        [Fact]
        public void SizeHelper_SizeOfElement_ShouldWorkWithComplexTypes()
        {
            Assert.True(SizeHelper.SizeOfElement<ComplexStruct>() > 0);
            Assert.True(SizeHelper.SizeOfElement<Guid>() > 0);
        }

        [Fact]
        public void IntPtrExtension_ToObject_ShouldReadStruct()
        {
            ComplexStruct value = new ComplexStruct
            {
                A = 12345,
                B = 67.89,
                C = 255,
                D = 9876543210L
            };

            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ComplexStruct)));
            try
            {
                Marshal.StructureToPtr(value, ptr, false);
                ComplexStruct result = ptr.ToObject<ComplexStruct>();
                Assert.Equal(value.A, result.A);
                Assert.Equal(value.B, result.B);
                Assert.Equal(value.C, result.C);
                Assert.Equal(value.D, result.D);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        [Fact]
        public void PinnedArray_ReadElementAtIndex_ShouldWorkWithDifferentTypes()
        {
            int[] array = new int[10];
            for (int i = 0; i < 10; i++)
            {
                array[i] = i * 1000;
            }

            using (var pinned = array.Pin())
            {
                for (int i = 0; i < 10; i++)
                {
                    int value = pinned.ReadElementAtIndex<int>(i);
                    Assert.Equal(i * 1000, value);
                }
            }
        }

        [Fact]
        public void PinnedArray_WriteElementAtIndex_ShouldWorkWithDifferentTypes()
        {
            int[] array = new int[10];

            using (var pinned = array.Pin())
            {
                for (int i = 0; i < 10; i++)
                {
                    pinned.WriteElementAtIndex(i, i * 500);
                }
            }

            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(i * 500, array[i]);
            }
        }

        [Fact]
        public void PinnedStructure_ReadWrite_ShouldWorkCorrectly()
        {
            ComplexStruct value = new ComplexStruct
            {
                A = 111,
                B = 222.333,
                C = 100,
                D = 4444444444L
            };

            using (var pinned = value.PinAsStructure())
            {
                ComplexStruct readValue = pinned.ReadElementAtIndex<ComplexStruct>(0);
                Assert.Equal(value.A, readValue.A);
                Assert.Equal(value.B, readValue.B);
                Assert.Equal(value.C, readValue.C);
                Assert.Equal(value.D, readValue.D);
            }
        }

        [Fact]
        public void PinnedArray_MixedTypeAccess_ShouldWorkCorrectly()
        {
            long[] array = new long[4];
            array[0] = 0x0102030405060708;

            using (var pinned = array.Pin())
            {
                Assert.Equal(0x0102030405060708L, pinned.ReadElementAtIndex<long>(0));

                long writeValue = unchecked((long)0xABCDEF1234567890UL);
                pinned.WriteElementAtIndex(0, writeValue);
                Assert.Equal(writeValue, array[0]);
            }
        }

        [Fact]
        public unsafe void PinnedArray_PointerAccess_ShouldWorkCorrectly()
        {
            int[] array = new int[10];
            using (var pinned = array.Pin())
            {
                for (int i = 0; i < 10; i++)
                {
                    int* ptr = (int*)pinned.Pointers[i];
                    *ptr = i * 111;
                }
            }

            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(i * 111, array[i]);
            }
        }

        [Fact]
        public void PinnedArray_MultipleDispose_ShouldNotThrow()
        {
            int[] array = new int[10];
            var pinned = array.Pin();
            pinned.Dispose();
            pinned.Dispose();
        }

        [Fact]
        public void PinnedArray_SliceOperations_ShouldBeIndependent()
        {
            int[] array = new int[100];
            for (int i = 0; i < 100; i++)
            {
                array[i] = i;
            }

            using (var pinned = array.Pin())
            {
                pinned.CheckIndexes = false;
                var slice1 = pinned.Slice(0, 10, sizeof(int));
                var slice2 = pinned.Slice(10, 10, sizeof(int));

                slice1.CheckIndexes = false;
                slice2.CheckIndexes = false;

                slice1[0] = 999;
                slice2[0] = 888;
            }

            Assert.Equal(999, array[0]);
            Assert.Equal(888, array[10]);
        }

        [Fact]
        public void PinnedString_ShouldWorkCorrectly()
        {
            string text = "Hello, World!";
            using (var pinned = text.Pin())
            {
                Assert.Equal(text.Length, pinned.Size);
                Assert.NotEqual(IntPtr.Zero, pinned.IntPtr);
            }
        }

        [Fact]
        public void ArrayExtension_AllocateCopyArray_ShouldCopyCorrectly()
        {
            int[] array = { 1, 2, 3, 4, 5 };
            IntPtr ptr = array.AllocateCopyArray<int>();

            try
            {
                int[] result = new int[5];
                ptr.GetArray(result);
                Assert.Equal(array, result);
            }
            finally
            {
                ptr.FreeGlobal();
            }
        }

        [Fact]
        public void ArrayExtension_GetIntPtrs_ShouldReturnCorrectPointers()
        {
            int[] array = new int[5];
            IntPtr basePtr = Marshal.AllocHGlobal(5 * sizeof(int));

            try
            {
                IntPtr[] ptrs = basePtr.GetIntPtrs<int>(5);
                Assert.Equal(5, ptrs.Length);

                for (int i = 0; i < 5; i++)
                {
                    Assert.Equal(basePtr + i * sizeof(int), ptrs[i]);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(basePtr);
            }
        }

        [Fact]
        public unsafe void CrossFramework_MarshalOperations_ShouldBeConsistent()
        {
            ComplexStruct original = new ComplexStruct
            {
                A = int.MaxValue,
                B = double.MaxValue,
                C = byte.MaxValue,
                D = long.MaxValue
            };

            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ComplexStruct)));
            try
            {
                Marshal.StructureToPtr(original, ptr, false);

                ComplexStruct viaUnsafeHelper = UnsafeHelper.Read<ComplexStruct>(ptr.ToPointer());
                ComplexStruct viaExtension = ptr.ToObject<ComplexStruct>();

                Assert.Equal(original.A, viaUnsafeHelper.A);
                Assert.Equal(original.B, viaUnsafeHelper.B);
                Assert.Equal(original.C, viaUnsafeHelper.C);
                Assert.Equal(original.D, viaUnsafeHelper.D);

                Assert.Equal(original.A, viaExtension.A);
                Assert.Equal(original.B, viaExtension.B);
                Assert.Equal(original.C, viaExtension.C);
                Assert.Equal(original.D, viaExtension.D);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        struct ComplexStruct
        {
            public int A;
            public double B;
            public byte C;
            public long D;
        }
    }
}
