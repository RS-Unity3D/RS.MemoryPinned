using System;
using System.Runtime.InteropServices;
using RS.MemoryPinned;
using RS.MemoryPinned.Extension;
using Xunit;

namespace RS.MemoryPinned.Tests
{
    public class ComplexTypesTests
    {
        [Fact]
        public void PinnedArray_DateTime_ShouldWorkCorrectly()
        {
            DateTime[] array = new DateTime[5];
            array[0] = new DateTime(2024, 1, 1, 12, 0, 0);
            array[1] = new DateTime(2024, 6, 15, 8, 30, 45);
            array[2] = DateTime.UtcNow;

            using (var pinned = array.Pin())
            {
                Assert.Equal(new DateTime(2024, 1, 1, 12, 0, 0), pinned[0]);
                Assert.Equal(new DateTime(2024, 6, 15, 8, 30, 45), pinned[1]);
                Assert.Equal(array[2], pinned[2]);

                pinned[3] = new DateTime(2025, 12, 31, 23, 59, 59);
                Assert.Equal(new DateTime(2025, 12, 31, 23, 59, 59), array[3]);
            }
        }

        [Fact]
        public void PinnedArray_DateTimeOffset_ShouldWorkCorrectly()
        {
            DateTimeOffset[] array = new DateTimeOffset[5];
            array[0] = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.FromHours(8));
            array[1] = new DateTimeOffset(2024, 6, 15, 8, 30, 45, TimeSpan.FromHours(-5));

            using (var pinned = array.Pin())
            {
                Assert.Equal(new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.FromHours(8)), pinned[0]);
                Assert.Equal(new DateTimeOffset(2024, 6, 15, 8, 30, 45, TimeSpan.FromHours(-5)), pinned[1]);

                pinned[2] = DateTimeOffset.UtcNow;
                Assert.Equal(array[2], pinned[2]);
            }
        }

        [Fact]
        public void PinnedArray_TimeSpan_ShouldWorkCorrectly()
        {
            TimeSpan[] array = new TimeSpan[5];
            array[0] = TimeSpan.FromHours(1);
            array[1] = TimeSpan.FromMinutes(30);
            array[2] = TimeSpan.FromSeconds(45);

            using (var pinned = array.Pin())
            {
                Assert.Equal(TimeSpan.FromHours(1), pinned[0]);
                Assert.Equal(TimeSpan.FromMinutes(30), pinned[1]);
                Assert.Equal(TimeSpan.FromSeconds(45), pinned[2]);

                pinned[3] = TimeSpan.FromDays(1);
                Assert.Equal(TimeSpan.FromDays(1), array[3]);

                pinned[4] = TimeSpan.FromMilliseconds(123.456);
                Assert.Equal(TimeSpan.FromMilliseconds(123.456), array[4]);
            }
        }

        [Fact]
        public void PinnedArray_Decimal_ShouldWorkCorrectly()
        {
            decimal[] array = new decimal[5];
            array[0] = 123.456m;
            array[1] = -789.012m;
            array[2] = decimal.MaxValue;
            array[3] = decimal.MinValue;

            using (var pinned = array.Pin())
            {
                Assert.Equal(123.456m, pinned[0]);
                Assert.Equal(-789.012m, pinned[1]);
                Assert.Equal(decimal.MaxValue, pinned[2]);
                Assert.Equal(decimal.MinValue, pinned[3]);

                pinned[4] = 3.14159265358979323846m;
                Assert.Equal(3.14159265358979323846m, array[4]);
            }
        }

        [Fact]
        public void PinnedArray_Guid_ShouldWorkCorrectly()
        {
            Guid[] array = new Guid[5];
            array[0] = new Guid("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
            array[1] = Guid.NewGuid();
            array[2] = Guid.Empty;

            using (var pinned = array.Pin())
            {
                Assert.Equal(new Guid("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"), pinned[0]);
                Assert.Equal(array[1], pinned[1]);
                Assert.Equal(Guid.Empty, pinned[2]);

                pinned[3] = new Guid("12345678-1234-1234-1234-123456789abc");
                Assert.Equal(new Guid("12345678-1234-1234-1234-123456789abc"), array[3]);
            }
        }
    }

    public struct SimpleNestedStruct
    {
        public int X;
        public int Y;
        public double Value;
    }

    public struct ComplexNestedStruct
    {
        public DateTime Timestamp;
        public TimeSpan Duration;
        public Guid Id;
        public decimal Amount;
    }

    public struct DeepNestedStruct
    {
        public SimpleNestedStruct Inner;
        public int Count;
        public long Total;
    }

    public struct StructWithAutoProperty
    {
        public int Value { get; set; }
        public string Name { get; set; }
    }

    public class NestedStructTests
    {
        [Fact]
        public void PinnedArray_SimpleNestedStruct_ShouldWorkCorrectly()
        {
            SimpleNestedStruct[] array = new SimpleNestedStruct[5];
            array[0] = new SimpleNestedStruct { X = 10, Y = 20, Value = 3.14159 };
            array[1] = new SimpleNestedStruct { X = -5, Y = -10, Value = -2.71828 };

            using (var pinned = array.Pin())
            {
                Assert.Equal(10, pinned[0].X);
                Assert.Equal(20, pinned[0].Y);
                Assert.Equal(3.14159, pinned[0].Value);

                Assert.Equal(-5, pinned[1].X);
                Assert.Equal(-10, pinned[1].Y);
                Assert.Equal(-2.71828, pinned[1].Value);

                pinned[2] = new SimpleNestedStruct { X = 100, Y = 200, Value = 1.414 };
                Assert.Equal(100, array[2].X);
                Assert.Equal(200, array[2].Y);
                Assert.Equal(1.414, array[2].Value);
            }
        }

        [Fact]
        public void PinnedArray_ComplexNestedStruct_ShouldWorkCorrectly()
        {
            ComplexNestedStruct[] array = new ComplexNestedStruct[3];
            array[0] = new ComplexNestedStruct
            {
                Timestamp = new DateTime(2024, 1, 1, 12, 0, 0),
                Duration = TimeSpan.FromHours(2.5),
                Id = new Guid("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
                Amount = 1234.5678m
            };

            using (var pinned = array.Pin())
            {
                Assert.Equal(new DateTime(2024, 1, 1, 12, 0, 0), pinned[0].Timestamp);
                Assert.Equal(TimeSpan.FromHours(2.5), pinned[0].Duration);
                Assert.Equal(new Guid("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"), pinned[0].Id);
                Assert.Equal(1234.5678m, pinned[0].Amount);

                pinned[1] = new ComplexNestedStruct
                {
                    Timestamp = DateTime.UtcNow,
                    Duration = TimeSpan.FromMinutes(90),
                    Id = Guid.NewGuid(),
                    Amount = decimal.MaxValue
                };

                Assert.Equal(array[1].Timestamp, pinned[1].Timestamp);
                Assert.Equal(TimeSpan.FromMinutes(90), array[1].Duration);
                Assert.Equal(array[1].Id, pinned[1].Id);
                Assert.Equal(decimal.MaxValue, array[1].Amount);
            }
        }

        [Fact]
        public void PinnedArray_DeepNestedStruct_ShouldWorkCorrectly()
        {
            DeepNestedStruct[] array = new DeepNestedStruct[3];
            array[0] = new DeepNestedStruct
            {
                Inner = new SimpleNestedStruct { X = 1, Y = 2, Value = 3.0 },
                Count = 100,
                Total = 999999L
            };

            using (var pinned = array.Pin())
            {
                Assert.Equal(1, pinned[0].Inner.X);
                Assert.Equal(2, pinned[0].Inner.Y);
                Assert.Equal(3.0, pinned[0].Inner.Value);
                Assert.Equal(100, pinned[0].Count);
                Assert.Equal(999999L, pinned[0].Total);

                pinned[1] = new DeepNestedStruct
                {
                    Inner = new SimpleNestedStruct { X = 10, Y = 20, Value = 30.0 },
                    Count = 200,
                    Total = 888888L
                };

                Assert.Equal(10, array[1].Inner.X);
                Assert.Equal(20, array[1].Inner.Y);
                Assert.Equal(30.0, array[1].Inner.Value);
                Assert.Equal(200, array[1].Count);
                Assert.Equal(888888L, array[1].Total);
            }
        }

        [Fact]
        public unsafe void UnsafeHelper_ReadWrite_NestedStruct()
        {
            SimpleNestedStruct value = new SimpleNestedStruct { X = 100, Y = 200, Value = 3.14159 };
            SimpleNestedStruct result = UnsafeHelper.Read<SimpleNestedStruct>(&value);
            Assert.Equal(100, result.X);
            Assert.Equal(200, result.Y);
            Assert.Equal(3.14159, result.Value);
        }

        [Fact]
        public unsafe void UnsafeHelper_CopyBlock_NestedStructArray()
        {
            SimpleNestedStruct[] source = new SimpleNestedStruct[5];
            SimpleNestedStruct[] destination = new SimpleNestedStruct[5];

            for (int i = 0; i < 5; i++)
            {
                source[i] = new SimpleNestedStruct { X = i, Y = i * 10, Value = i * 1.5 };
            }

            fixed (SimpleNestedStruct* srcPtr = source, dstPtr = destination)
            {
                UnsafeHelper.CopyBlock(dstPtr, srcPtr, (uint)(5 * sizeof(SimpleNestedStruct)));
            }

            for (int i = 0; i < 5; i++)
            {
                Assert.Equal(source[i].X, destination[i].X);
                Assert.Equal(source[i].Y, destination[i].Y);
                Assert.Equal(source[i].Value, destination[i].Value);
            }
        }

        [Fact]
        public void SizeHelper_NestedStruct_Size()
        {
            Assert.Equal(16, SizeHelper.SizeOfElement<SimpleNestedStruct>());
            Assert.Equal(48, SizeHelper.SizeOfElement<ComplexNestedStruct>());
            Assert.Equal(32, SizeHelper.SizeOfElement<DeepNestedStruct>());
        }
    }
}
