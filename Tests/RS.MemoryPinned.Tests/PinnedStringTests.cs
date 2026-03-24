using System;
using Xunit;
using RS.MemoryPinned.Extension;

namespace RS.MemoryPinned.Tests
{
    public class PinnedStringTests
    {
        [Fact]
        public void Pin_ShouldCreatePinnedString()
        {
            string text = "Hello World";
            using (var pinned = text.Pin())
            {
                Assert.NotNull(pinned);
                Assert.Equal(11, pinned.Size);
                Assert.NotEqual(IntPtr.Zero, pinned.IntPtr);
            }
        }

        [Fact]
        public void Chars_ShouldAccessCharacters()
        {
            string text = "Hello World";
            using (var pinned = text.Pin())
            {
                Assert.Equal('H', pinned.Chars[0]);
                Assert.Equal('e', pinned.Chars[1]);
                Assert.Equal('l', pinned.Chars[2]);
                Assert.Equal('l', pinned.Chars[3]);
                Assert.Equal('o', pinned.Chars[4]);
                Assert.Equal(' ', pinned.Chars[5]);
                Assert.Equal('W', pinned.Chars[6]);
            }
        }

        [Fact]
        public void LengthAsChars_ShouldReturnCorrectLength()
        {
            string text = "Hello World";
            using (var pinned = text.Pin())
            {
                Assert.Equal(11, pinned.LengthAsChars);
            }
        }

        [Fact]
        public void LengthAsBytes_ShouldReturnCorrectLength()
        {
            string text = "Hello World";
            using (var pinned = text.Pin())
            {
                Assert.Equal(22, pinned.LengthAsBytes);
            }
        }

        [Fact]
        public void ImplicitOperator_ShouldConvertToCharArray()
        {
            string text = "Hello";
            using (var pinned = text.Pin())
            {
                char[] chars = pinned;
                Assert.Equal(5, chars.Length);
                Assert.Equal('H', chars[0]);
                Assert.Equal('e', chars[1]);
            }
        }

        [Fact]
        public void ImplicitOperator_ShouldConvertFromCharArray()
        {
            char[] chars = new char[] { 'H', 'e', 'l', 'l', 'o' };
            using (var pinned = (RS.MemoryPinned.Model.PinnedString)chars)
            {
                Assert.NotNull(pinned);
                Assert.Equal(5, pinned.Size);
            }
        }

        [Fact]
        public void Indexer_ShouldAccessCharacters()
        {
            string text = "Hello";
            using (var pinned = text.Pin())
            {
                Assert.Equal('H', pinned[0]);
                Assert.Equal('e', pinned[1]);
                Assert.Equal('l', pinned[2]);
            }
        }

        [Fact]
        public void EmptyString_ShouldWork()
        {
            string text = "";
            using (var pinned = text.Pin())
            {
                Assert.NotNull(pinned);
                Assert.Equal(0, pinned.Size);
            }
        }

        [Fact]
        public void UnicodeString_ShouldWork()
        {
            string text = "你好世界";
            using (var pinned = text.Pin())
            {
                Assert.NotNull(pinned);
                Assert.Equal(4, pinned.Size);
            }
        }
    }
}
