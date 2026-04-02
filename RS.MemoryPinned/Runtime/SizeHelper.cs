#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace RS.MemoryPinned
{
    /// <summary>
    /// Helper class for size calculations and pointer operations.
    /// 大小计算和指针操作的帮助类。
    /// </summary>
    public static unsafe class SizeHelper
    {
        /// <summary>
        /// Gets the size of the element type in bytes.
        /// 获取元素类型的字节大小。
        /// Uses sizeof for unmanaged types, Marshal.SizeOf for marshalable types.
        /// 对于非托管类型使用sizeof，对于可封送类型使用Marshal.SizeOf。
        /// </summary>
        /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
        /// <returns>The size of the element type in bytes. 元素类型的字节大小。</returns>
        /// <exception cref="NotSupportedException">
        /// Thrown when the type is not an unmanaged or marshalable type.
        /// 当类型不是非托管类型或可封送类型时抛出。
        /// </exception>
        public static int SizeOfElement<TElement>()
        {
            Type type = typeof(TElement);

            if (type == typeof(char))
            {
                return 2;
            }
            if (type == typeof(bool))
            {
                return 1;
            }
            if (type == typeof(DateTime))
            {
                return sizeof(DateTime);
            }
            if (type == typeof(DateTimeOffset))
            {
                return sizeof(DateTimeOffset);
            }
            if (type == typeof(TimeSpan))
            {
                return sizeof(TimeSpan);
            }
            if (type == typeof(Guid))
            {
                return sizeof(Guid);
            }
            if (type == typeof(decimal))
            {
                return sizeof(decimal);
            }
            if (type == typeof(IntPtr))
            {
                return sizeof(IntPtr);
            }
            if (type == typeof(UIntPtr))
            {
                return sizeof(UIntPtr);
            }

            Exception innerException = null;
            try
            {
                return sizeof(TElement);
            }
            catch (Exception ex)
            {
                innerException = ex;
            }

            try
            {
                return Marshal.SizeOf(type);
            }
            catch (Exception ex)
            {
                throw new NotSupportedException(
                    string.Format("Type '{0}' is not supported for size calculation. Only unmanaged types and marshalable types are supported. sizeof error: {1}; Marshal.SizeOf error: {2}",
                        type.FullName,
                        innerException?.Message ?? "N/A",
                        ex.Message),
                    ex);
            }
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
        /// <returns>The size in bytes. 字节大小。</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when variable is null and null is not expected.
        /// 当 variable 为 null 且不允许 null 时抛出。
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown when the type is not supported for size calculation.
        /// 当类型不支持大小计算时抛出。
        /// </exception>
        public static int GetSize<TElement>(object variable) where TElement : struct
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
                int elementSize = SizeOfElement<TElement>();
                return array.GetLength(0) * elementSize;
            }

            try
            {
#if NETSTANDARD2_1
                return Marshal.SizeOf<TElement>();
#else
                return Marshal.SizeOf(variable.GetType());
#endif
            }
            catch (Exception ex)
            {
                throw new NotSupportedException(
                    string.Format("Cannot determine size of variable of type '{0}'. Original error: {1}",
                        variable.GetType().FullName,
                        ex.Message),
                    ex);
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
