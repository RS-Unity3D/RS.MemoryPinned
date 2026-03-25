using RS.MemoryPinned.Extension;
using RS.MemoryPinned.Interfaces;
using RS.MemoryPinned.Internals.Get;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace RS.MemoryPinned.Model
{
    /// <summary>
    /// Base class for pinned memory objects.
    /// 固定内存对象的基类。
    /// </summary>
    /// <typeparam name="T">The type of the pinned object. 固定对象的类型。</typeparam>
    /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
    public class PinnedBase<T, TElement> : IPinnedBase<TElement> where TElement : struct
    {
        private Bytes<TElement> _bytes;

        private Chars<TElement> _chars;

        private Indexer<TElement> _indexer;

        private Integers<TElement> _integers;

        private Longs<TElement> _longs;

        private Shorts<TElement> _shorts;

        protected T BasePinnedObject;

        protected IntPtr Global;

        protected GCHandle Handle;

        /// <summary>
        /// Gets the IntPtr pointing to the pinned memory.
        /// 获取指向固定内存的IntPtr。
        /// </summary>
        public IntPtr IntPtr { get; protected set; }

        /// <summary>
        /// Gets the raw pointer to the pinned memory.
        /// 获取固定内存的原始指针。
        /// </summary>
        public unsafe void* Pointer => IntPtr.ToPointer();

        /// <summary>
        /// Gets or sets the pinned object.
        /// 获取或设置固定对象。
        /// </summary>
        public virtual T PinnedObject
        {
            get
            {
                return BasePinnedObject;
            }
            set
            {
                BasePinnedObject = value;
            }
        }

        /// <summary>
        /// Gets or sets the start index for slicing.
        /// 获取或设置切片的起始索引。
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// 获取或设置指定索引处的元素。
        /// </summary>
        /// <param name="index">The index. 索引。</param>
        /// <returns>The element at the specified index. 指定索引处的元素。</returns>
        public TElement this[int index]
        {
            get
            {
                return Indexer[index];
            }
            set
            {
                Indexer[index] = value;
            }
        }

        /// <summary>
        /// Gets the structure accessor for char elements.
        /// 获取char元素的结构访问器。
        /// </summary>
        public IStructure<char> Chars
        {
            get
            {
                if (_chars == null)
                {
                    _chars = new Chars<TElement>(this, StartIndex, IntPtr);
                }
                return _chars;
            }
        }

        /// <summary>
        /// Gets the structure accessor for the element type.
        /// 获取元素类型的结构访问器。
        /// </summary>
        public IStructure<TElement> Indexer
        {
            get
            {
                if (_indexer == null)
                {
                    _indexer = new Indexer<TElement>(this, StartIndex, IntPtr);
                }
                return _indexer;
            }
        }

        /// <summary>
        /// Gets the size of the pinned memory in elements.
        /// 获取固定内存的元素大小。
        /// </summary>
        public int Size { get; internal set; }

        /// <summary>
        /// Gets the structure accessor for byte elements.
        /// 获取byte元素的结构访问器。
        /// </summary>
        public IStructure<byte> Bytes
        {
            get
            {
                if (_bytes == null)
                {
                    _bytes = new Bytes<TElement>(this, StartIndex, IntPtr);
                }
                return _bytes;
            }
        }

        /// <summary>
        /// Gets the structure accessor for short elements.
        /// 获取short元素的结构访问器。
        /// </summary>
        public IStructure<short> Shorts
        {
            get
            {
                if (_shorts == null)
                {
                    _shorts = new Shorts<TElement>(this, StartIndex, IntPtr);
                }
                return _shorts;
            }
        }

        /// <summary>
        /// Gets the structure accessor for int elements.
        /// 获取int元素的结构访问器。
        /// </summary>
        public IStructure<int> Integers
        {
            get
            {
                if (_integers == null)
                {
                    _integers = new Integers<TElement>(this, StartIndex, IntPtr);
                }
                return _integers;
            }
        }

        /// <summary>
        /// Gets the structure accessor for long elements.
        /// 获取long元素的结构访问器。
        /// </summary>
        public IStructure<long> Longs
        {
            get
            {
                if (_longs == null)
                {
                    _longs = new Longs<TElement>(this, StartIndex, IntPtr);
                }
                return _longs;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to check indexes.
        /// 获取或设置一个值，指示是否检查索引。
        /// </summary>
        public bool CheckIndexes { get; set; }

        protected PinnedBase(ref T variable)
        {
            BasePinnedObject = variable;
            AllocateHandle(variable);
            CheckIndexes = true;
        }

        /// <summary>
        /// Allocates a GCHandle for the variable.
        /// 为变量分配GCHandle。
        /// </summary>
        /// <param name="variable">The variable to pin. 要固定的变量。</param>
        protected void AllocateHandle(T variable)
        {
            Type type = variable.GetType();
            if (type.HasElementType)
            {
                Type elementType = type.GetElementType();
                if (elementType != null && !elementType.IsPrimitive)
                {
                    Global = ((Array)(object)variable).AllocateCopyArray<TElement>();
                    IntPtr = Global;
                }
            }
            if (IntPtr == IntPtr.Zero)
            {
                Handle = GCHandle.Alloc(variable, GCHandleType.Pinned);
                IntPtr = Handle.AddrOfPinnedObject();
            }
        }

        /// <summary>
        /// Converts the GCHandle to IntPtr.
        /// 将GCHandle转换为IntPtr。
        /// </summary>
        /// <returns>The IntPtr representation. IntPtr表示。</returns>
        public IntPtr ToIntPtr()
        {
            return GCHandle.ToIntPtr(Handle);
        }

        protected PinnedBase(T variable, GCHandle handle)
        {
            BasePinnedObject = variable;
            Handle = handle;
            IntPtr = Handle.AddrOfPinnedObject();
            CheckIndexes = true;
        }

        ~PinnedBase()
        {
            Global = Global.FreeGlobal();
        }
    }

    /// <summary>
    /// Represents a pinned memory object with disposable pattern.
    /// 表示具有可释放模式的固定内存对象。
    /// </summary>
    /// <typeparam name="T">The type of the pinned object. 固定对象的类型。</typeparam>
    /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
    public class Pinned<T, TElement> : PinnedBase<T, TElement>, IPinned<TElement>, IPinnedBase<TElement>, IDisposable where TElement : struct
    {
        private readonly bool _hasToDispose;

        /// <summary>
        /// Gets the IntPtr indexer for accessing memory by index.
        /// 获取用于按索引访问内存的IntPtr索引器。
        /// </summary>
        public IIntPtrIndexer IntPtrs { get; }

        /// <summary>
        /// Gets the pointer indexer for accessing memory by index.
        /// 获取用于按索引访问内存的指针索引器。
        /// </summary>
        public IPointerIndexer<TElement> Pointers { get; }

        /// <summary>
        /// Gets or sets the pinned object.
        /// 获取或设置固定对象。
        /// </summary>
        public override T PinnedObject
        {
            get
            {
                if (Global == IntPtr.Zero)
                {
                    return base.PinnedObject;
                }
                Global.GetArray((TElement[])(object)base.PinnedObject);
                return base.PinnedObject;
            }
            set
            {
                if (!value.Equals(BasePinnedObject))
                {
                    ReleaseHandle(false);
                    base.PinnedObject = value;
                    AllocateHandle(base.PinnedObject);
                }
            }
        }

        /// <summary>
        /// Gets the parent pinned object for sliced views.
        /// 获取切片视图的父固定对象。
        /// </summary>
        public Pinned<T, TElement> Parent { get; private set; }

        /// <summary>
        /// Gets the length in chars.
        /// 获取字符长度。
        /// </summary>
        public int LengthAsChars => base.Size * SizeHelper.SizeOfElement<TElement>() / SizeHelper.SizeOfElement<char>();

        /// <summary>
        /// Gets the length in shorts.
        /// 获取short长度。
        /// </summary>
        public int LengthAsShorts => base.Size * SizeHelper.SizeOfElement<TElement>() / SizeHelper.SizeOfElement<short>();

        /// <summary>
        /// Gets the length in integers.
        /// 获取int长度。
        /// </summary>
        public int LengthAsIntegers => base.Size * SizeHelper.SizeOfElement<TElement>() / SizeHelper.SizeOfElement<int>();

        /// <summary>
        /// Gets the length in longs.
        /// 获取long长度。
        /// </summary>
        public int LengthAsLongs => base.Size * SizeHelper.SizeOfElement<TElement>() / SizeHelper.SizeOfElement<long>();

        /// <summary>
        /// Gets a value indicating whether the object is disposed.
        /// 获取一个值，指示对象是否已被释放。
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                if (!(base.IntPtr == IntPtr.Zero))
                {
                    return Parent?.IsDisposed ?? false;
                }
                return true;
            }
        }

        /// <summary>
        /// Gets the length in bytes.
        /// 获取字节长度。
        /// </summary>
        public int LengthAsBytes => base.Size * SizeHelper.SizeOfElement<TElement>() / SizeHelper.SizeOfElement<byte>();

        public Pinned(ref T variable)
            : this(ref variable, true, SizeHelper.SizeOfElement<TElement>())
        {
        }

        private Pinned(ref T variable, bool hasToDispose, int sizeForLength)
            : base(ref variable)
        {
            _hasToDispose = hasToDispose;
            base.Size = SizeHelper.GetSize<TElement>(BasePinnedObject) / sizeForLength;
            IntPtrs = new IntPtrIndexer<TElement>(base.IntPtr);
            Pointers = new PointerIndexer<TElement>(base.IntPtr);
        }

        private Pinned(T variable, GCHandle handle)
            : base(variable, handle)
        {
            _hasToDispose = false;
            IntPtrs = new IntPtrIndexer<TElement>(base.IntPtr);
            Pointers = new PointerIndexer<TElement>(base.IntPtr);
        }

        /// <summary>
        /// Writes a value of the specified type at the given index.
        /// 在指定索引处写入指定类型的值。
        /// </summary>
        /// <typeparam name="TElementToWrite">The type of the value to write. 要写入的值的类型。</typeparam>
        /// <param name="index">The index. 索引。</param>
        /// <param name="value">The value to write. 要写入的值。</param>
        public unsafe void WriteElementAtIndex<TElementToWrite>(int index, TElementToWrite value) where TElementToWrite : struct
        {
            EndureIndex<TElement>(index);
            TElementToWrite* ptr = (TElementToWrite*)new PointerIndexer<TElementToWrite>(base.IntPtr)[index];
            UnsafeHelper.Write(ptr, value);
        }

        /// <summary>
        /// Reads a value of the specified type from the given index.
        /// 从指定索引读取指定类型的值。
        /// </summary>
        /// <typeparam name="TElementToWrite">The type of the value to read. 要读取的值的类型。</typeparam>
        /// <param name="index">The index. 索引。</param>
        /// <returns>The value read from the index. 从索引读取的值。</returns>
        public TElementToWrite ReadElementAtIndex<TElementToWrite>(int index) where TElementToWrite : struct
        {
            EndureIndex<TElement>(index);
#if NETSTANDARD2_1
            return Marshal.PtrToStructure<TElementToWrite>(IntPtrs[index]);
#else
            return (TElementToWrite)Marshal.PtrToStructure(IntPtrs[index], typeof(TElementToWrite));
#endif
        }

        internal void EndureIndex<TElement>(int index) where TElement : struct
        {
            int size = SizeHelper.GetSize<TElement>();
            if (base.CheckIndexes && LengthAsBytes <= index * size)
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Releases the pinned memory.
        /// 释放固定内存。
        /// </summary>
        public void Dispose()
        {
            ReleaseHandle(true);
        }

        /// <summary>
        /// Explicitly converts a pinned object to string.
        /// 显式将固定对象转换为字符串。
        /// </summary>
        /// <param name="pinDisposable">The pinned object. 固定对象。</param>
        /// <returns>The string representation. 字符串表示。</returns>
        public static explicit operator string(Pinned<T, TElement> pinDisposable)
        {
            string text = pinDisposable.PinnedObject as string;
            if (text != null)
            {
                return text;
            }
            byte[] array = pinDisposable.PinnedObject as byte[];
            if (array != null)
            {
                return Encoding.ASCII.GetString(array);
            }
            char[] array2 = pinDisposable.PinnedObject as char[];
            if (array2 != null)
            {
                return new string(array2);
            }
            return pinDisposable.PinnedObject.ToString();
        }

        /// <summary>
        /// Implicitly converts a pinned object to its underlying type.
        /// 隐式将固定对象转换为其基础类型。
        /// </summary>
        /// <param name="pinDisposable">The pinned object. 固定对象。</param>
        /// <returns>The underlying object. 基础对象。</returns>
        public static implicit operator T(Pinned<T, TElement> pinDisposable)
        {
            return pinDisposable.PinnedObject;
        }

        /// <summary>
        /// Creates a sliced view of the pinned memory.
        /// 创建固定内存的切片视图。
        /// </summary>
        /// <param name="startIndex">The start index. 起始索引。</param>
        /// <param name="length">The length of the slice. 切片长度。</param>
        /// <param name="sizeOfElement">The size of each element. 每个元素的大小。</param>
        /// <returns>A sliced pinned object. 切片固定对象。</returns>
        public Pinned<T, TElement> Slice(int startIndex, int? length = null, int? sizeOfElement = null)
        {
            if (length.HasValue && length.Value > base.Size - startIndex * sizeOfElement.GetValueOrDefault(1))
            {
                throw new ArgumentException("length");
            }
            return new Pinned<T, TElement>(PinnedObject, Handle)
            {
                Size = (length ?? (base.Size - startIndex * sizeOfElement.GetValueOrDefault(1))),
                StartIndex = startIndex,
                Parent = this
            };
        }

        private void ReleaseHandle(bool disposing)
        {
            if (!_hasToDispose)
            {
                return;
            }
            if (base.IntPtr != IntPtr.Zero)
            {
                Pinned<T, TElement> parent = Parent;
                if (parent == null || !parent.IsDisposed)
                {
                    if (Handle.IsAllocated)
                    {
                        Handle.Free();
                    }
                    Global = Global.FreeGlobal();
                    base.IntPtr = IntPtr.Zero;
                }
            }
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        ~Pinned()
        {
            Dispose();
        }
    }

    /// <summary>
    /// Represents a pinned array.
    /// 表示固定数组。
    /// </summary>
    /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
    public class PinnedArray<TElement> : Pinned<TElement[], TElement> where TElement : struct
    {
        public PinnedArray(ref TElement[] variable)
            : base(ref variable)
        {
        }

        /// <summary>
        /// Implicitly converts a pinned array to its underlying array.
        /// 隐式将固定数组转换为其基础数组。
        /// </summary>
        /// <param name="pinDisposable">The pinned array. 固定数组。</param>
        /// <returns>The underlying array. 基础数组。</returns>
        public static implicit operator TElement[](PinnedArray<TElement> pinDisposable)
        {
            return pinDisposable.PinnedObject;
        }

        /// <summary>
        /// Explicitly converts an array to a pinned array.
        /// 显式将数组转换为固定数组。
        /// </summary>
        /// <param name="value">The array to pin. 要固定的数组。</param>
        /// <returns>A pinned array. 固定数组。</returns>
        public static explicit operator PinnedArray<TElement>(TElement[] value)
        {
            return new PinnedArray<TElement>(ref value);
        }
    }

    /// <summary>
    /// Represents a pinned pointer.
    /// 表示固定指针。
    /// </summary>
    public class PinnedPointer
    {
        protected unsafe void* BasePinnedObject;

        protected IntPtr Global;

        protected GCHandle Handle;

        /// <summary>
        /// Gets the pointer indexer for accessing memory by index.
        /// 获取用于按索引访问内存的指针索引器。
        /// </summary>
        public IPointerIndexer<byte> Pointers { get; }

        /// <summary>
        /// Gets the IntPtr indexer for accessing memory by index.
        /// 获取用于按索引访问内存的IntPtr索引器。
        /// </summary>
        public IIntPtrIndexer IntPtrs { get; }

        /// <summary>
        /// Gets the IntPtr pointing to the pinned memory.
        /// 获取指向固定内存的IntPtr。
        /// </summary>
        public IntPtr IntPtr { get; protected set; }

        /// <summary>
        /// Gets the raw pointer to the pinned memory.
        /// 获取固定内存的原始指针。
        /// </summary>
        public unsafe void* Pointer => IntPtr.ToPointer();

        /// <summary>
        /// Gets or sets the pinned object.
        /// 获取或设置固定对象。
        /// </summary>
        public unsafe virtual void* PinnedObject
        {
            get
            {
                return BasePinnedObject;
            }
            set
            {
                BasePinnedObject = value;
            }
        }

        /// <summary>
        /// Gets or sets the start index for slicing.
        /// 获取或设置切片的起始索引。
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// Gets or sets the byte at the specified index.
        /// 获取或设置指定索引处的字节。
        /// </summary>
        /// <param name="index">The index. 索引。</param>
        /// <returns>The byte at the specified index. 指定索引处的字节。</returns>
        public byte this[int index]
        {
            get
            {
                return ReadElementAtIndex<byte>(index);
            }
            set
            {
                WriteElementAtIndex(index, value);
            }
        }

        public unsafe PinnedPointer(void* variable)
        {
            BasePinnedObject = variable;
            AllocateHandle(variable);
            IntPtrs = new IntPtrIndexer<byte>(IntPtr);
            Pointers = new PointerIndexer<byte>(IntPtr);
        }

        protected unsafe void AllocateHandle(void* variable)
        {
            if (IntPtr == IntPtr.Zero)
            {
                Handle = GCHandle.FromIntPtr((IntPtr)variable);
                IntPtr = (IntPtr)variable;
            }
        }

        /// <summary>
        /// Converts the GCHandle to IntPtr.
        /// 将GCHandle转换为IntPtr。
        /// </summary>
        /// <returns>The IntPtr representation. IntPtr表示。</returns>
        public IntPtr ToIntPtr()
        {
            return GCHandle.ToIntPtr(Handle);
        }

        /// <summary>
        /// Writes a value of the specified type at the given index.
        /// 在指定索引处写入指定类型的值。
        /// </summary>
        /// <typeparam name="TElementToWrite">The type of the value to write. 要写入的值的类型。</typeparam>
        /// <param name="index">The index. 索引。</param>
        /// <param name="value">The value to write. 要写入的值。</param>
        public unsafe void WriteElementAtIndex<TElementToWrite>(int index, TElementToWrite value) where TElementToWrite : struct
        {
            TElementToWrite* ptr = (TElementToWrite*)new PointerIndexer<TElementToWrite>(IntPtr)[index];
            UnsafeHelper.Write(ptr, value);
        }

        /// <summary>
        /// Reads a value of the specified type from the given index.
        /// 从指定索引读取指定类型的值。
        /// </summary>
        /// <typeparam name="TElementToWrite">The type of the value to read. 要读取的值的类型。</typeparam>
        /// <param name="index">The index. 索引。</param>
        /// <returns>The value read from the index. 从索引读取的值。</returns>
        public TElementToWrite ReadElementAtIndex<TElementToWrite>(int index) where TElementToWrite : struct
        {
#if NETSTANDARD2_1
            return Marshal.PtrToStructure<TElementToWrite>(GetPointerAtIndex<TElementToWrite>(index));
#else
            return (TElementToWrite)Marshal.PtrToStructure(GetPointerAtIndex<TElementToWrite>(index), typeof(TElementToWrite));
#endif
        }

        /// <summary>
        /// Gets the pointer at the specified index.
        /// 获取指定索引处的指针。
        /// </summary>
        /// <typeparam name="TElementToWrite">The element type. 元素类型。</typeparam>
        /// <param name="index">The index. 索引。</param>
        /// <returns>The IntPtr at the specified index. 指定索引处的IntPtr。</returns>
        public IntPtr GetPointerAtIndex<TElementToWrite>(int index) where TElementToWrite : struct
        {
            return IntPtrs[index];
        }

        ~PinnedPointer()
        {
            Global = Global.FreeGlobal();
        }
    }

    /// <summary>
    /// Represents a pinned string.
    /// 表示固定字符串。
    /// </summary>
    public class PinnedString : Pinned<string, char>
    {
        public PinnedString(ref string variable)
            : base(ref variable)
        {
        }

        /// <summary>
        /// Implicitly converts a pinned string to a char array.
        /// 隐式将固定字符串转换为字符数组。
        /// </summary>
        /// <param name="pinDisposable">The pinned string. 固定字符串。</param>
        /// <returns>The char array. 字符数组。</returns>
        public static implicit operator char[](PinnedString pinDisposable)
        {
            return pinDisposable.PinnedObject.ToCharArray();
        }

        /// <summary>
        /// Implicitly converts a char array to a pinned string.
        /// 隐式将字符数组转换为固定字符串。
        /// </summary>
        /// <param name="value">The char array. 字符数组。</param>
        /// <returns>A pinned string. 固定字符串。</returns>
        public static implicit operator PinnedString(char[] value)
        {
            string variable = new string(value);
            return new PinnedString(ref variable);
        }
    }

    /// <summary>
    /// Represents a pinned structure.
    /// 表示固定结构。
    /// </summary>
    /// <typeparam name="TElement">The element type. 元素类型。</typeparam>
    public class PinnedStructure<TElement> : IPinned<TElement>, IPinnedBase<TElement>, IDisposable where TElement : struct
    {
        private readonly Pinned<TElement[], TElement> _pinned;

        /// <summary>
        /// Gets the IntPtr indexer for accessing memory by index.
        /// 获取用于按索引访问内存的IntPtr索引器。
        /// </summary>
        public IIntPtrIndexer IntPtrs => _pinned.IntPtrs;

        /// <summary>
        /// Gets the pointer indexer for accessing memory by index.
        /// 获取用于按索引访问内存的指针索引器。
        /// </summary>
        public IPointerIndexer<TElement> Pointers => _pinned.Pointers;

        /// <summary>
        /// Gets or sets the size of the pinned memory.
        /// 获取或设置固定内存的大小。
        /// </summary>
        public int Size
        {
            get
            {
                return _pinned.Size;
            }
            set
            {
                _pinned.Size = value;
            }
        }

        /// <summary>
        /// Gets the pinned structure value.
        /// 获取固定结构的值。
        /// </summary>
        public TElement PinnedObject => _pinned.PinnedObject[0];

        /// <summary>
        /// Gets the IntPtr pointing to the pinned memory.
        /// 获取指向固定内存的IntPtr。
        /// </summary>
        public IntPtr IntPtr => _pinned.IntPtr;

        /// <summary>
        /// Gets the raw pointer to the pinned memory.
        /// 获取固定内存的原始指针。
        /// </summary>
        public unsafe void* Pointer => IntPtr.ToPointer();

        /// <summary>
        /// Gets the structure accessor for byte elements.
        /// 获取byte元素的结构访问器。
        /// </summary>
        public IStructure<byte> Bytes => _pinned.Bytes;

        /// <summary>
        /// Gets the structure accessor for short elements.
        /// 获取short元素的结构访问器。
        /// </summary>
        public IStructure<short> Shorts => _pinned.Shorts;

        /// <summary>
        /// Gets the structure accessor for int elements.
        /// 获取int元素的结构访问器。
        /// </summary>
        public IStructure<int> Integers => _pinned.Integers;

        /// <summary>
        /// Gets the structure accessor for the element type.
        /// 获取元素类型的结构访问器。
        /// </summary>
        public IStructure<TElement> Indexer => _pinned.Indexer;

        /// <summary>
        /// Gets the structure accessor for long elements.
        /// 获取long元素的结构访问器。
        /// </summary>
        public IStructure<long> Longs => _pinned.Longs;

        /// <summary>
        /// Gets or sets a value indicating whether to check indexes.
        /// 获取或设置一个值，指示是否检查索引。
        /// </summary>
        public bool CheckIndexes
        {
            get
            {
                return _pinned.CheckIndexes;
            }
            set
            {
                _pinned.CheckIndexes = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the object is disposed.
        /// 获取一个值，指示对象是否已被释放。
        /// </summary>
        public bool IsDisposed => _pinned.IsDisposed;

        /// <summary>
        /// Gets the structure accessor for char elements.
        /// 获取char元素的结构访问器。
        /// </summary>
        public IStructure<char> Chars => _pinned.Chars;

        /// <summary>
        /// Gets the length in bytes.
        /// 获取字节长度。
        /// </summary>
        public int LengthAsBytes => _pinned.LengthAsBytes;

        /// <summary>
        /// Gets the length in chars.
        /// 获取字符长度。
        /// </summary>
        public int LengthAsChars => _pinned.LengthAsChars;

        /// <summary>
        /// Gets the length in shorts.
        /// 获取short长度。
        /// </summary>
        public int LengthAsShorts => _pinned.LengthAsShorts;

        /// <summary>
        /// Gets the length in integers.
        /// 获取int长度。
        /// </summary>
        public int LengthAsIntegers => _pinned.LengthAsIntegers;

        /// <summary>
        /// Gets the length in longs.
        /// 获取long长度。
        /// </summary>
        public int LengthAsLongs => _pinned.LengthAsLongs;

        public PinnedStructure(ref TElement variable, bool checkIndexes)
        {
            TElement[] variable2 = new TElement[1] { variable };
            _pinned = new Pinned<TElement[], TElement>(ref variable2);
            _pinned.CheckIndexes = checkIndexes;
        }

        public unsafe PinnedStructure(void* variable, bool checkIndexes)
        {
            TElement[] variable2 = new TElement[1] { UnsafeHelper.Read<TElement>(variable) };
            _pinned = new Pinned<TElement[], TElement>(ref variable2);
            _pinned.CheckIndexes = checkIndexes;
        }

        /// <summary>
        /// Writes a value of the specified type at the given index.
        /// 在指定索引处写入指定类型的值。
        /// </summary>
        /// <typeparam name="TElementToWrite">The type of the value to write. 要写入的值的类型。</typeparam>
        /// <param name="index">The index. 索引。</param>
        /// <param name="value">The value to write. 要写入的值。</param>
        public unsafe void WriteElementAtIndex<TElementToWrite>(int index, TElementToWrite value) where TElementToWrite : struct
        {
            _pinned.EndureIndex<TElement>(index);
            TElementToWrite* ptr = (TElementToWrite*)new PointerIndexer<TElementToWrite>(IntPtr)[index];
            UnsafeHelper.Write(ptr, value);
        }

        /// <summary>
        /// Reads a value of the specified type from the given index.
        /// 从指定索引读取指定类型的值。
        /// </summary>
        /// <typeparam name="TElementToWrite">The type of the value to read. 要读取的值的类型。</typeparam>
        /// <param name="index">The index. 索引。</param>
        /// <returns>The value read from the index. 从索引读取的值。</returns>
        public TElementToWrite ReadElementAtIndex<TElementToWrite>(int index) where TElementToWrite : struct
        {
            _pinned.EndureIndex<TElement>(index);
#if NETSTANDARD2_1
            return Marshal.PtrToStructure<TElementToWrite>(IntPtrs[index]);
#else
            return (TElementToWrite)Marshal.PtrToStructure(IntPtrs[index], typeof(TElementToWrite));
#endif
        }

        /// <summary>
        /// Releases the pinned memory.
        /// 释放固定内存。
        /// </summary>
        public void Dispose()
        {
            _pinned.Dispose();
        }

        /// <summary>
        /// Explicitly converts a value to a pinned structure.
        /// 显式将值转换为固定结构。
        /// </summary>
        /// <param name="value">The value to pin. 要固定的值。</param>
        /// <returns>A pinned structure. 固定结构。</returns>
        public static explicit operator PinnedStructure<TElement>(TElement value)
        {
            return new PinnedStructure<TElement>(ref value, true);
        }

        /// <summary>
        /// Implicitly converts a pinned structure to its underlying value.
        /// 隐式将固定结构转换为其基础值。
        /// </summary>
        /// <param name="pinDisposable">The pinned structure. 固定结构。</param>
        /// <returns>The underlying value. 基础值。</returns>
        public static implicit operator TElement(PinnedStructure<TElement> pinDisposable)
        {
            return pinDisposable._pinned.PinnedObject[0];
        }
    }
}
