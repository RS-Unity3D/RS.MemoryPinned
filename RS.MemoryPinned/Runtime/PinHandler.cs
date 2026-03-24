using RS.MemoryPinned.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace RS.MemoryPinned
{
    /// <summary>
    /// Handler class for creating pinned memory objects.
    /// 用于创建固定内存对象的处理类。
    /// </summary>
    public static class PinHandler
    {
        /// <summary>
        /// Pins an array in memory.
        /// 将数组固定在内存中。
        /// </summary>
        /// <typeparam name="TElement">The element type of the array. 数组的元素类型。</typeparam>
        /// <param name="variable">The array to pin. 要固定的数组。</param>
        /// <returns>A pinned array object. 固定数组对象。</returns>
        public static PinnedArray<TElement> Pin<TElement>(ref TElement[] variable) where TElement : struct
        {
            return new PinnedArray<TElement>(ref variable);
        }

        /// <summary>
        /// Pins a single element as an array.
        /// 将单个元素作为数组固定。
        /// </summary>
        /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
        /// <param name="variable">The element to pin. 要固定的元素。</param>
        /// <returns>A pinned array containing the element. 包含该元素的固定数组。</returns>
        public static PinnedArray<TElement> PinAsArray<TElement>(ref TElement variable) where TElement : struct
        {
            TElement[] variable2 = new TElement[1] { variable };
            return new PinnedArray<TElement>(ref variable2);
        }

        /// <summary>
        /// Pins a single element as a structure.
        /// 将单个元素作为结构固定。
        /// </summary>
        /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
        /// <param name="variable">The element to pin. 要固定的元素。</param>
        /// <param name="checkIndexes">Whether to check indexes. 是否检查索引。</param>
        /// <returns>A pinned structure object. 固定结构对象。</returns>
        public static PinnedStructure<TElement> PinAsStructure<TElement>(ref TElement variable, bool checkIndexes = true) where TElement : struct
        {
            return new PinnedStructure<TElement>(ref variable, checkIndexes);
        }

        /// <summary>
        /// Pins a pointer in memory.
        /// 将指针固定在内存中。
        /// </summary>
        /// <param name="variable">The pointer to pin. 要固定的指针。</param>
        /// <param name="checkIndexes">Whether to check indexes. 是否检查索引。</param>
        /// <returns>A pinned pointer object. 固定指针对象。</returns>
        public unsafe static PinnedPointer Pin(void* variable, bool checkIndexes = true)
        {
            return new PinnedPointer(variable);
        }

        /// <summary>
        /// Pins a string in memory.
        /// 将字符串固定在内存中。
        /// </summary>
        /// <param name="variable">The string to pin. 要固定的字符串。</param>
        /// <returns>A pinned string object. 固定字符串对象。</returns>
        public static PinnedString Pin(ref string variable)
        {
            return new PinnedString(ref variable);
        }

        /// <summary>
        /// Pins an object in memory with the specified element type.
        /// 使用指定的元素类型将对象固定在内存中。
        /// </summary>
        /// <typeparam name="T">The type of the object. 对象的类型。</typeparam>
        /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
        /// <param name="variable">The object to pin. 要固定的对象。</param>
        /// <returns>A pinned object. 固定对象。</returns>
        public static Pinned<T, TElement> Pin<T, TElement>(ref T variable) where TElement : struct
        {
            return new Pinned<T, TElement>(ref variable);
        }
    }
}
