using System;
using System.Runtime.InteropServices;

namespace RS.MemoryPinned
{
    public static unsafe class UnsafeHelper
    {
        /// <summary>
        /// Reads a value of type T from the given pointer.
        /// 从给定指针读取类型为T的值。
        /// </summary>
        /// <typeparam name="T">The type of the value to read. 要读取的值的类型。</typeparam>
        /// <param name="source">The pointer to read from. 要读取的指针。</param>
        /// <returns>The value read from the pointer. 从指针读取的值。</returns>
        public static T Read<T>(void* source) where T : struct
        {
            return (T)Marshal.PtrToStructure((IntPtr)source, typeof(T));
        }

        /// <summary>
        /// Writes a value of type T to the given pointer.
        /// 将类型为T的值写入给定指针。
        /// </summary>
        /// <typeparam name="T">The type of the value to write. 要写入的值的类型。</typeparam>
        /// <param name="destination">The pointer to write to. 要写入的指针。</param>
        /// <param name="value">The value to write. 要写入的值。</param>
        public static void Write<T>(void* destination, T value) where T : struct
        {
            Marshal.StructureToPtr(value, (IntPtr)destination, false);
        }

        /// <summary>
        /// Returns the size of type T in bytes.
        /// 返回类型T的字节大小。
        /// </summary>
        /// <typeparam name="T">The type to get the size of. 要获取大小的类型。</typeparam>
        /// <returns>The size of type T in bytes. 类型T的字节大小。</returns>
        public static int SizeOf<T>() where T : struct
        {
            return Marshal.SizeOf(typeof(T));
        }

        /// <summary>
        /// Copies a block of memory from source to destination.
        /// 将内存块从源复制到目标。
        /// </summary>
        /// <param name="destination">The destination pointer. 目标指针。</param>
        /// <param name="source">The source pointer. 源指针。</param>
        /// <param name="byteCount">The number of bytes to copy. 要复制的字节数。</param>
        public static void CopyBlock(void* destination, void* source, uint byteCount)
        {
            if (byteCount > 0)
            {
                Buffer.MemoryCopy(source, destination, byteCount, byteCount);
            }
        }

        /// <summary>
        /// Copies a block of memory from source to destination without alignment.
        /// 将内存块从源复制到目标（不要求对齐）。
        /// </summary>
        /// <param name="destination">The destination pointer. 目标指针。</param>
        /// <param name="source">The source pointer. 源指针。</param>
        /// <param name="byteCount">The number of bytes to copy. 要复制的字节数。</param>
        public static void CopyBlockUnaligned(void* destination, void* source, uint byteCount)
        {
            if (byteCount > 0)
            {
                Buffer.MemoryCopy(source, destination, byteCount, byteCount);
            }
        }

        /// <summary>
        /// Initializes a block of memory with the given value.
        /// 使用给定值初始化内存块。
        /// </summary>
        /// <param name="startAddress">The starting address. 起始地址。</param>
        /// <param name="value">The value to initialize with. 用于初始化的值。</param>
        /// <param name="byteCount">The number of bytes to initialize. 要初始化的字节数。</param>
        public static void InitBlock(void* startAddress, byte value, uint byteCount)
        {
            if (byteCount > 0)
            {
                byte* ptr = (byte*)startAddress;
                for (uint i = 0; i < byteCount; i++)
                {
                    ptr[i] = value;
                }
            }
        }
    }
}
