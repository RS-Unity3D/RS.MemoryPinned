using System;
using System.Collections.Generic;
using System.Text;

namespace RS.MemoryPinned.Interfaces
{
    /// <summary>
    /// Interface for IntPtr indexer access.
    /// IntPtr索引器访问接口。
    /// </summary>
    public interface IIntPtrIndexer
    {
        /// <summary>
        /// Gets the IntPtr at the specified index.
        /// 获取指定索引处的IntPtr。
        /// </summary>
        /// <param name="index">The index. 索引。</param>
        /// <returns>The IntPtr at the specified index. 指定索引处的IntPtr。</returns>
        IntPtr this[int index] { get; }
    }

    /// <summary>
    /// Interface for pinned memory objects with element type.
    /// 具有元素类型的固定内存对象接口。
    /// </summary>
    /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
    public interface IPinned<TElement> : IPinnedBase<TElement>, IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether the object is disposed.
        /// 获取一个值，指示对象是否已被释放。
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Gets the structure accessor for char elements.
        /// 获取char元素的结构访问器。
        /// </summary>
        IStructure<char> Chars { get; }

        /// <summary>
        /// Gets the length in bytes.
        /// 获取字节长度。
        /// </summary>
        int LengthAsBytes { get; }

        /// <summary>
        /// Gets the length in chars.
        /// 获取字符长度。
        /// </summary>
        int LengthAsChars { get; }

        /// <summary>
        /// Gets the length in shorts.
        /// 获取short长度。
        /// </summary>
        int LengthAsShorts { get; }

        /// <summary>
        /// Gets the length in integers.
        /// 获取int长度。
        /// </summary>
        int LengthAsIntegers { get; }

        /// <summary>
        /// Gets the length in longs.
        /// 获取long长度。
        /// </summary>
        int LengthAsLongs { get; }
    }

    /// <summary>
    /// Base interface for pinned memory objects.
    /// 固定内存对象的基础接口。
    /// </summary>
    /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
    public interface IPinnedBase<TElement>
    {
        /// <summary>
        /// Gets the size of the pinned memory.
        /// 获取固定内存的大小。
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Gets the structure accessor for byte elements.
        /// 获取byte元素的结构访问器。
        /// </summary>
        IStructure<byte> Bytes { get; }

        /// <summary>
        /// Gets the structure accessor for short elements.
        /// 获取short元素的结构访问器。
        /// </summary>
        IStructure<short> Shorts { get; }

        /// <summary>
        /// Gets the structure accessor for int elements.
        /// 获取int元素的结构访问器。
        /// </summary>
        IStructure<int> Integers { get; }

        /// <summary>
        /// Gets the structure accessor for the element type.
        /// 获取元素类型的结构访问器。
        /// </summary>
        IStructure<TElement> Indexer { get; }

        /// <summary>
        /// Gets the structure accessor for long elements.
        /// 获取long元素的结构访问器。
        /// </summary>
        IStructure<long> Longs { get; }

        /// <summary>
        /// Gets or sets a value indicating whether to check indexes.
        /// 获取或设置一个值，指示是否检查索引。
        /// </summary>
        bool CheckIndexes { get; set; }
    }

    /// <summary>
    /// Interface for pointer indexer access.
    /// 指针索引器访问接口。
    /// </summary>
    /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
    public interface IPointerIndexer<TElement> where TElement : struct
    {
        /// <summary>
        /// Gets the pointer at the specified index.
        /// 获取指定索引处的指针。
        /// </summary>
        /// <param name="index">The index. 索引。</param>
        /// <returns>The pointer at the specified index. 指定索引处的指针。</returns>
        unsafe void* this[int index] { get; }
    }

    /// <summary>
    /// Interface for structure accessors.
    /// 结构访问器接口。
    /// </summary>
    /// <typeparam name="T">The element type. 元素类型。</typeparam>
    public interface IStructure<T>
    {
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// 获取或设置指定索引处的元素。
        /// </summary>
        /// <param name="index">The index. 索引。</param>
        /// <returns>The element at the specified index. 指定索引处的元素。</returns>
        T this[int index] { get; set; }

        /// <summary>
        /// Gets the length of the structure.
        /// 获取结构的长度。
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Gets the elements as an array.
        /// 获取元素数组。
        /// </summary>
        T[] Array { get; }

        /// <summary>
        /// Gets the value at the specified index.
        /// 获取指定索引处的值。
        /// </summary>
        /// <typeparam name="TType">The type of the value. 值的类型。</typeparam>
        /// <param name="index">The index. 索引。</param>
        /// <returns>A reference to the value at the specified index. 指定索引处值的引用。</returns>
        ref TType GetValue<TType>(int index) where TType : struct;

        /// <summary>
        /// Gets the pointer at the specified index.
        /// 获取指定索引处的指针。
        /// </summary>
        /// <typeparam name="TType">The type of the element. 元素的类型。</typeparam>
        /// <param name="index">The index. 索引。</param>
        /// <returns>A pointer to the element at the specified index. 指定索引处元素的指针。</returns>
        unsafe void* GetPointer<TType>(int index) where TType : struct;
    }
}
