#define TRACE
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RS.MemoryPinned
{
    /// <summary>
    /// Helper class for size calculations and pointer operations.
    /// 大小计算和指针操作的帮助类。
    /// </summary>
    public static class SizeHelper
    {
        /// <summary>
        /// Gets the size of the element type in bytes.
        /// 获取元素类型的字节大小。
        /// </summary>
        /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
        /// <returns>The size of the element type in bytes. 元素类型的字节大小。</returns>
        public static int SizeOfElement<TElement>()
        {
            if (typeof(TElement) != typeof(char))
            {
                return Marshal.SizeOf(typeof(TElement));
            }
            return 2;
        }

        /// <summary>
        /// Gets the size of the default value of type TElement.
        /// 获取TElement类型默认值的大小。
        /// </summary>
        /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
        /// <returns>The size in bytes. 字节大小。</returns>
        public static int GetSize<TElement>() where TElement : struct
        {
            return GetSize<TElement>(default(TElement));
        }

        /// <summary>
        /// Gets the size of the variable in bytes.
        /// 获取变量的字节大小。
        /// </summary>
        /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
        /// <param name="variable">The variable to measure. 要测量的变量。</param>
        /// <returns>The size in bytes, or -1 on error. 字节大小，错误时返回-1。</returns>
        public static int GetSize<TElement>(object variable) where TElement : struct
        {
            try
            {
                if (variable == null)
                {
                    return 0;
                }
                string text = variable as string;
                if (text != null)
                {
                    return text.Length * 2;
                }
                Array array = variable as Array;
                if (array != null)
                {
                    array.GetType().GetElementType();
                    int num = SizeOfElement<TElement>();
                    return array.GetLength(0) * num;
                }
                return Marshal.SizeOf(variable.GetType());
            }
            catch (Exception value)
            {
                Trace.WriteLine(value);
                return -1;
            }
        }

        /// <summary>
        /// Gets the IntPtr at the specified index.
        /// 获取指定索引处的IntPtr。
        /// </summary>
        /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
        /// <param name="pointer">The base pointer. 基础指针。</param>
        /// <param name="index">The index. 索引。</param>
        /// <returns>The IntPtr at the specified index. 指定索引处的IntPtr。</returns>
        public static IntPtr GetIntPtr<TElement>(IntPtr pointer, int index = 0)
        {
            return GetIntPtr(pointer, SizeOfElement<TElement>(), index);
        }

        /// <summary>
        /// Gets the IntPtr at the specified index with the given element size.
        /// 使用给定的元素大小获取指定索引处的IntPtr。
        /// </summary>
        /// <param name="pointer">The base pointer. 基础指针。</param>
        /// <param name="size">The size of each element. 每个元素的大小。</param>
        /// <param name="index">The index. 索引。</param>
        /// <returns>The IntPtr at the specified index. 指定索引处的IntPtr。</returns>
        public static unsafe IntPtr GetIntPtr(IntPtr pointer, int size, int index = 0)
        {
            if (sizeof(IntPtr) == 8)
            {
                return (IntPtr)((long)pointer + index * size);
            }
            return (IntPtr)(uint)((int)pointer + index * size);
        }
    }
}
