using RS.MemoryPinned.Model;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace RS.MemoryPinned.Extension
{
    /// <summary>
    /// Extension methods for array operations.
    /// 数组操作的扩展方法。
    /// </summary>
    public static unsafe class ArrayExtension
    {
        /// <summary>
        /// Allocates unmanaged memory and copies the array elements to it.
        /// 分配非托管内存并将数组元素复制到其中。
        /// </summary>
        /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
        /// <param name="array">The array to copy. 要复制的数组。</param>
        /// <returns>An IntPtr pointing to the allocated memory. 指向已分配内存的IntPtr。</returns>
        public static IntPtr AllocateCopyArray<TElement>(this Array array) where TElement : struct
        {
            int num = SizeHelper.SizeOfElement<TElement>();
            IntPtr intPtr = Marshal.AllocHGlobal(num * array.GetLength(0));
            for (int i = 0; i < array.Length; i++)
            {
                if (sizeof(IntPtr) == 8)
                {
                    Marshal.StructureToPtr(array.GetValue(i), (IntPtr)((long)intPtr + i * num), false);
                }
                else
                {
                    Marshal.StructureToPtr(array.GetValue(i), (IntPtr)((int)intPtr + i * num), false);
                }
            }
            return intPtr;
        }

        /// <summary>
        /// Allocates unmanaged memory and copies the array elements to it.
        /// 分配非托管内存并将数组元素复制到其中。
        /// </summary>
        /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
        /// <param name="array">The array to copy. 要复制的数组。</param>
        /// <returns>An IntPtr pointing to the allocated memory. 指向已分配内存的IntPtr。</returns>
        public static IntPtr AllocateCopyArray<TElement>(this TElement[] array) where TElement : struct
        {
            return ((Array)array).AllocateCopyArray<TElement>();
        }

        /// <summary>
        /// Copies elements from unmanaged memory to an array.
        /// 从非托管内存复制元素到数组。
        /// </summary>
        /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
        /// <param name="intPtr">The pointer to unmanaged memory. 非托管内存的指针。</param>
        /// <param name="array">The destination array. 目标数组。</param>
        public static void GetArray<TElement>(this IntPtr intPtr, TElement[] array) where TElement : struct
        {
            int num = SizeHelper.SizeOfElement<TElement>();
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = intPtr.ToObject<TElement>(i * num);
            }
        }

        /// <summary>
        /// Gets an array of IntPtrs from unmanaged memory.
        /// 从非托管内存获取IntPtr数组。
        /// </summary>
        /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
        /// <param name="intPtr">The pointer to unmanaged memory. 非托管内存的指针。</param>
        /// <param name="length">The number of elements. 元素数量。</param>
        /// <returns>An array of IntPtrs. IntPtr数组。</returns>
        public static IntPtr[] GetIntPtrs<TElement>(this IntPtr intPtr, int length) where TElement : struct
        {
            IntPtr[] array = new IntPtr[length];
            int num = SizeHelper.SizeOfElement<TElement>();
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = intPtr.ToIntPtr<TElement>(i * num);
            }
            return array;
        }

        /// <summary>
        /// Frees unmanaged memory allocated with AllocHGlobal.
        /// 释放使用AllocHGlobal分配的非托管内存。
        /// </summary>
        /// <param name="intPtr">The pointer to free. 要释放的指针。</param>
        /// <returns>IntPtr.Zero after freeing. 释放后返回IntPtr.Zero。</returns>
        public static IntPtr FreeGlobal(this IntPtr intPtr)
        {
            if (intPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(intPtr);
                return IntPtr.Zero;
            }
            return intPtr;
        }
    }

    /// <summary>
    /// Extension methods for IntPtr operations.
    /// IntPtr操作的扩展方法。
    /// </summary>
    public static unsafe class IntPtrExtension
    {
        /// <summary>
        /// Converts an IntPtr to an object of the specified type.
        /// 将IntPtr转换为指定类型的对象。
        /// </summary>
        /// <typeparam name="TElement">The type to convert to. 要转换到的类型。</typeparam>
        /// <param name="intPtr">The IntPtr to convert. 要转换的IntPtr。</param>
        /// <param name="offset">The byte offset. 字节偏移量。</param>
        /// <returns>The converted object. 转换后的对象。</returns>
        public static TElement ToObject<TElement>(this IntPtr intPtr, int offset = 0) where TElement : struct
        {
#if NETSTANDARD2_1
            return Marshal.PtrToStructure<TElement>(intPtr.ToIntPtr<TElement>(offset));
#else
            return (TElement)Marshal.PtrToStructure(intPtr.ToIntPtr<TElement>(offset), typeof(TElement));
#endif
        }

        /// <summary>
        /// Adds an offset to an IntPtr.
        /// 向IntPtr添加偏移量。
        /// </summary>
        /// <typeparam name="TElement">The element type for size calculation. 用于大小计算的元素类型。</typeparam>
        /// <param name="intPtr">The base IntPtr. 基础IntPtr。</param>
        /// <param name="offset">The byte offset. 字节偏移量。</param>
        /// <returns>The new IntPtr with offset applied. 应用偏移后的新IntPtr。</returns>
        public static IntPtr ToIntPtr<TElement>(this IntPtr intPtr, int offset = 0) where TElement : struct
        {
            if (sizeof(IntPtr) == 8)
            {
                return (IntPtr)((long)intPtr + offset);
            }
            return (IntPtr)((int)intPtr + offset);
        }
    }

    /// <summary>
    /// Extension methods for pinning objects.
    /// 固定对象的扩展方法。
    /// </summary>
    public static class PinExtension
    {
        /// <summary>
        /// Pins an array in memory.
        /// 将数组固定在内存中。
        /// </summary>
        /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
        /// <param name="variable">The array to pin. 要固定的数组。</param>
        /// <returns>A pinned array object. 固定数组对象。</returns>
        public static PinnedArray<TElement> Pin<TElement>(this TElement[] variable) where TElement : struct
        {
            return PinHandler.Pin(ref variable);
        }

        /// <summary>
        /// Pins a single element as an array.
        /// 将单个元素作为数组固定。
        /// </summary>
        /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
        /// <param name="variable">The element to pin. 要固定的元素。</param>
        /// <returns>A pinned array containing the element. 包含该元素的固定数组。</returns>
        public static PinnedArray<TElement> PinAsArray<TElement>(this TElement variable) where TElement : struct
        {
            return PinHandler.PinAsArray(ref variable);
        }

        /// <summary>
        /// Pins a single element as a structure.
        /// 将单个元素作为结构固定。
        /// </summary>
        /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
        /// <param name="variable">The element to pin. 要固定的元素。</param>
        /// <param name="checkIndexes">Whether to check indexes. 是否检查索引。</param>
        /// <returns>A pinned structure object. 固定结构对象。</returns>
        public static PinnedStructure<TElement> PinAsStructure<TElement>(this TElement variable, bool checkIndexes = true) where TElement : struct
        {
            return PinHandler.PinAsStructure(ref variable, checkIndexes);
        }

        /// <summary>
        /// Pins a string in memory.
        /// 将字符串固定在内存中。
        /// </summary>
        /// <param name="variable">The string to pin. 要固定的字符串。</param>
        /// <returns>A pinned string object. 固定字符串对象。</returns>
        public static PinnedString Pin(this string variable)
        {
            return PinHandler.Pin(ref variable);
        }

        /// <summary>
        /// Pins an object in memory with the specified element type.
        /// 使用指定的元素类型将对象固定在内存中。
        /// </summary>
        /// <typeparam name="T">The type of the object. 对象的类型。</typeparam>
        /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
        /// <param name="variable">The object to pin. 要固定的对象。</param>
        /// <returns>A pinned object. 固定对象。</returns>
        public static Pinned<T, TElement> Pin<T, TElement>(this T variable) where TElement : struct
        {
            return PinHandler.Pin<T, TElement>(ref variable);
        }

        /// <summary>
        /// Pins a pointer in memory.
        /// 将指针固定在内存中。
        /// </summary>
        /// <param name="variable">The pointer to pin. 要固定的指针。</param>
        /// <returns>A pinned pointer object. 固定指针对象。</returns>
        public unsafe static PinnedPointer Pin(void* variable)
        {
            return PinHandler.Pin(variable);
        }
    }

    /// <summary>
    /// Extension methods for pinned objects.
    /// 固定对象的扩展方法。
    /// </summary>
    public static class PinnedExtension
    {
        /// <summary>
        /// Gets the IntPtr at the specified index from a pinned object.
        /// 从固定对象获取指定索引处的IntPtr。
        /// </summary>
        /// <typeparam name="T">The type of the pinned object. 固定对象的类型。</typeparam>
        /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
        /// <param name="pointer">The pinned object. 固定对象。</param>
        /// <param name="index">The index. 索引。</param>
        /// <returns>The IntPtr at the specified index. 指定索引处的IntPtr。</returns>
        public static IntPtr GetIntPtr<T, TElement>(this PinnedBase<T, TElement> pointer, int index = 0) where TElement : struct
        {
            return SizeHelper.GetIntPtr<TElement>(pointer.IntPtr, index);
        }

        /// <summary>
        /// Gets the IntPtr at the specified index from a pinned structure.
        /// 从固定结构获取指定索引处的IntPtr。
        /// </summary>
        /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
        /// <param name="pointer">The pinned structure. 固定结构。</param>
        /// <param name="index">The index. 索引。</param>
        /// <returns>The IntPtr at the specified index. 指定索引处的IntPtr。</returns>
        public static IntPtr GetIntPtr<TElement>(this PinnedStructure<TElement> pointer, int index = 0) where TElement : struct
        {
            return SizeHelper.GetIntPtr<TElement>(pointer.IntPtr, index);
        }
    }
}
