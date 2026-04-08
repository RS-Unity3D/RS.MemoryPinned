using RS.MemoryPinned.Extension;
using RS.MemoryPinned.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace RS.MemoryPinned.Internals.Get
{
    /// <summary>
    /// Base class for structure accessors that provide typed access to pinned memory.
    /// 结构访问器的基类，提供对固定内存的类型化访问。
    /// </summary>
    /// <typeparam name="TElement">The element type of the pinned memory. 固定内存的元素类型。</typeparam>
    /// <typeparam name="TTargetType">The target type for access. 访问的目标类型。</typeparam>
    internal class StructBase<TElement, TTargetType> where TElement : struct where TTargetType : struct
    {
        private readonly IPinnedBase<TElement> _pinDisposable;

        public readonly IntPtr Pointer;

        protected readonly int StartIndex;

        public virtual int Length => GetLength<TElement>();

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// 获取或设置指定索引处的元素。
        /// </summary>
        /// <param name="index">The index. 索引。</param>
        /// <returns>The element at the specified index. 指定索引处的元素。</returns>
        public TTargetType this[int index]
        {
            get
            {
                return GetValue<TTargetType>(index);
            }
            set
            {
                SetValue(index, value);
            }
        }

        /// <summary>
        /// Gets all elements as an array.
        /// 获取所有元素作为数组。
        /// </summary>
        public TTargetType[] Array
        {
            get
            {
                TTargetType[] array = new TTargetType[Length];
                Pointer.GetArray(array);
                return array;
            }
        }

        protected StructBase(IPinnedBase<TElement> pinDisposable, IntPtr pointer, int startIndex)
        {
            _pinDisposable = pinDisposable;
            Pointer = pointer;
            StartIndex = startIndex;
        }

        /// <summary>
        /// Gets a pointer to the element at the specified index.
        /// 获取指定索引处元素的指针。
        /// </summary>
        /// <typeparam name="TargetType">The target type. 目标类型。</typeparam>
        /// <param name="index">The index. 索引。</param>
        /// <returns>A pointer to the element. 元素的指针。</returns>
        public unsafe void* GetPointer<TargetType>(int index) where TargetType : struct
        {
            EnsureIndex<TElement, TargetType>(index + StartIndex);
            return GetIntPtr(index + StartIndex, SizeHelper.SizeOfElement<TargetType>()).ToPointer();
        }

        /// <summary>
        /// Gets the value at the specified index.
        /// 获取指定索引处的值。
        /// </summary>
        /// <typeparam name="TargetType">The type of the value. 值的类型。</typeparam>
        /// <param name="index">The index. 索引。</param>
        /// <returns>A reference to the value at the specified index. 指定索引处值的引用。</returns>
        public unsafe ref TargetType GetValue<TargetType>(int index) where TargetType : struct
        {
            EnsureIndex<TElement, TargetType>(index + StartIndex);
            void* ptr = GetIntPtr(index + StartIndex, SizeHelper.SizeOfElement<TargetType>()).ToPointer();
            return ref *(TargetType*)ptr;
        }

        /// <summary>
        /// Ensures the index is within valid bounds.
        /// 确保索引在有效范围内。
        /// </summary>
        /// <typeparam name="TElementLocal">The element type. 元素类型。</typeparam>
        /// <typeparam name="T">The target type. 目标类型。</typeparam>
        /// <param name="index">The index to check. 要检查的索引。</param>
        protected void EnsureIndex<TElementLocal, T>(int index) where TElementLocal : struct where T : struct
        {
            if (_pinDisposable.CheckIndexes && (index < 0 || index >= GetLength<T>()))
            {
                throw new IndexOutOfRangeException("index");
            }
        }

        protected IntPtr GetIntPtr(int index, int size)
        {
            return SizeHelper.GetIntPtr(Pointer, size, index);
        }

        protected int GetLength<T>() where T : struct
        {
            return _pinDisposable.Size * SizeHelper.SizeOfElement<TElement>() / SizeHelper.SizeOfElement<T>();
        }

        /// <summary>
        /// Sets the value at the specified index.
        /// 设置指定索引处的值。
        /// </summary>
        /// <typeparam name="TType">The type of the value. 值的类型。</typeparam>
        /// <param name="index">The index. 索引。</param>
        /// <param name="value">The value to set. 要设置的值。</param>
        protected unsafe void SetValue<TType>(int index, TType value) where TType : struct
        {
            EnsureIndex<TElement, TType>(index + StartIndex);
            UnsafeHelper.Write(GetIntPtr(index + StartIndex, SizeHelper.SizeOfElement<TType>()).ToPointer(), value);
        }
    }

    /// <summary>
    /// Provides byte-level access to pinned memory.
    /// 提供对固定内存的字节级访问。
    /// </summary>
    /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
    internal class Bytes<TElement> : StructBase<TElement, byte>, IStructure<byte> where TElement : struct
    {
        public override int Length => GetLength<byte>();

        public Bytes(IPinnedBase<TElement> pinDisposable, int startIndex, IntPtr pointer)
            : base(pinDisposable, pointer, startIndex)
        {
        }
    }

    /// <summary>
    /// Provides char-level access to pinned memory.
    /// 提供对固定内存的字符级访问。
    /// </summary>
    /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
    internal class Chars<TElement> : StructBase<TElement, char>, IStructure<char> where TElement : struct
    {
        private readonly IPinned<TElement> _pinDisposable;

        public override int Length => GetLength<char>();

        public Chars(IPinnedBase<TElement> pinDisposable, int startIndex, IntPtr pointer)
            : base(pinDisposable, pointer, startIndex)
        {
        }
    }

    /// <summary>
    /// Provides element-level access to pinned memory.
    /// 提供对固定内存的元素级访问。
    /// </summary>
    /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
    internal class Indexer<TElement> : StructBase<TElement, TElement>, IStructure<TElement> where TElement : struct
    {
        public override int Length => GetLength<TElement>();

        public Indexer(IPinnedBase<TElement> pinDisposable, int startIndex, IntPtr pointer)
            : base(pinDisposable, pointer, startIndex)
        {
        }
    }

    /// <summary>
    /// Provides int-level access to pinned memory.
    /// 提供对固定内存的int级访问。
    /// </summary>
    /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
    internal class Integers<TElement> : StructBase<TElement, int>, IStructure<int> where TElement : struct
    {
        public override int Length => GetLength<int>();

        public Integers(IPinnedBase<TElement> pinDisposable, int startIndex, IntPtr pointer)
            : base(pinDisposable, pointer, startIndex)
        {
        }
    }

    /// <summary>
    /// Provides IntPtr access to pinned memory by index.
    /// 提供按索引访问固定内存的IntPtr访问器。
    /// </summary>
    /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
    internal class IntPtrIndexer<TElement> : IIntPtrIndexer where TElement : struct
    {
        private IntPtr _intPtr;

        public IntPtr this[int index] => GetIntPtr(_intPtr, index);

        public IntPtrIndexer(IntPtr intPtr)
        {
            _intPtr = intPtr;
        }

        public IntPtr GetIntPtr(IntPtr intPtr, int i)
        {
            int num = SizeHelper.SizeOfElement<TElement>();
            return intPtr.ToIntPtr<TElement>(i * num);
        }
    }

    /// <summary>
    /// Helper class for IntPtr references.
    /// IntPtr引用的帮助类。
    /// </summary>
    /// <typeparam name="T">The struct type. 结构类型。</typeparam>
    internal class IntPtrRef<T> where T : struct
    {
        public IntPtr IntPtr { get; set; }

        public IntPtrRef(ref GCHandle value)
        {
            IntPtr = value.AddrOfPinnedObject();
        }
    }

    /// <summary>
    /// Provides long-level access to pinned memory.
    /// 提供对固定内存的long级访问。
    /// </summary>
    /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
    internal class Longs<TElement> : StructBase<TElement, long>, IStructure<long> where TElement : struct
    {
        public override int Length => GetLength<long>();

        public Longs(IPinnedBase<TElement> pinDisposable, int startIndex, IntPtr pointer)
            : base(pinDisposable, pointer, startIndex)
        {
        }
    }

    /// <summary>
    /// Provides pointer access to pinned memory by index.
    /// 提供按索引访问固定内存的指针访问器。
    /// </summary>
    /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
    internal class PointerIndexer<TElement> : IPointerIndexer<TElement> where TElement : struct
    {
        private IntPtr _intPtr;

        public unsafe void* this[int index] => GetIntPtr(_intPtr, index).ToPointer();

        public PointerIndexer(IntPtr intPtr)
        {
            _intPtr = intPtr;
        }

        public IntPtr GetIntPtr(IntPtr intPtr, int i)
        {
            int num = SizeHelper.SizeOfElement<TElement>();
            return _intPtr.ToIntPtr<TElement>(i * num);
        }
    }

    /// <summary>
    /// Provides short-level access to pinned memory.
    /// 提供对固定内存的short级访问。
    /// </summary>
    /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
    internal class Shorts<TElement> : StructBase<TElement, short>, IStructure<short> where TElement : struct
    {
        public override int Length => GetLength<short>();

        public Shorts(IPinnedBase<TElement> pinDisposable, int startIndex, IntPtr pointer)
            : base(pinDisposable, pointer, startIndex)
        {
        }
    }
}
