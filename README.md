# RS.MemoryPinned

[中文](#中文文档) | [English](#english-documentation)

---

## 中文文档

### 概述

RS.MemoryPinned 是一个干净、无第三方依赖、强大易用的高性能跨平台非托管内存操作库。

### 特性

- **跨平台支持**: 支持 Unity 5+、.NET Framework 2.0+、.NET Standard 2.0/2.1、.NET Core 2.0+
- **AOT编译支持**: 完全支持 AOT 编译环境
- **零依赖**: 不依赖任何第三方库
- **高性能**: 直接操作非托管内存，避免 GC 压力
- **类型安全**: 提供泛型接口，确保类型安全

### 安装

#### Unity
将 `RS.MemoryPinned` 文件夹复制到 Unity 项目的 `Packages` 目录下。

#### .NET 项目
通过 NuGet 或直接引用 `RS.MemoryPinned.dll`。

### 快速开始

#### 1. 固定数组

```csharp
using RS.MemoryPinned;
using RS.MemoryPinned.Extension;

// 方式一：使用扩展方法
int[] array = new int[100];
using (var pinned = array.Pin())
{
    // 获取内存地址
    IntPtr ptr = pinned.IntPtr;
    
    // 按索引访问元素
    pinned[0] = 123;
    int value = pinned[1];
    
    // 按字节访问
    pinned.Bytes[0] = 0xFF;
    
    // 按其他类型访问
    pinned.Shorts[0] = 100;
    pinned.Integers[0] = 1000;
    pinned.Longs[0] = 10000L;
}

// 方式二：使用 PinHandler
int[] array2 = new int[50];
var pinnedArray = PinHandler.Pin(ref array2);
// 使用完毕后释放
pinnedArray.Dispose();
```

#### 2. 固定字符串

```csharp
using RS.MemoryPinned;
using RS.MemoryPinned.Extension;

string text = "Hello World";
using (var pinned = text.Pin())
{
    // 访问字符
    char c = pinned.Chars[0];
    
    // 修改字符（注意：字符串是不可变的，这里直接修改内存）
    pinned.Chars[0] = 'h';
}
```

#### 3. 固定结构体

```csharp
using RS.MemoryPinned;
using RS.MemoryPinned.Extension;

struct MyStruct
{
    public int X;
    public int Y;
}

MyStruct myStruct = new MyStruct { X = 10, Y = 20 };
using (var pinned = myStruct.PinAsStructure())
{
    // 访问结构体数据
    int x = pinned.PinnedObject.X;
    
    // 通过字节访问
    byte firstByte = pinned.Bytes[0];
}
```

#### 4. 内存切片

```csharp
using RS.MemoryPinned;
using RS.MemoryPinned.Extension;

int[] array = new int[100];
using (var pinned = array.Pin())
{
    // 创建切片视图
    var slice = pinned.Slice(10, 20); // 从索引10开始，长度20
    
    // 切片共享同一块内存
    slice[0] = 999; // 这会修改 array[10]
}
```

#### 5. 读写不同类型的数据

```csharp
using RS.MemoryPinned;
using RS.MemoryPinned.Extension;

byte[] buffer = new byte[1024];
using (var pinned = buffer.Pin())
{
    // 在字节缓冲区中写入 int
    pinned.WriteElementAtIndex<int>(0, 123456);
    
    // 读取 int
    int value = pinned.ReadElementAtIndex<int>(0);
    
    // 写入 long
    pinned.WriteElementAtIndex<long>(8, 9876543210L);
    
    // 读取 long
    long longValue = pinned.ReadElementAtIndex<long>(8);
}
```

### API 参考

#### 主要类

| 类名 | 说明 |
|------|------|
| `PinnedArray<T>` | 固定数组 |
| `PinnedString` | 固定字符串 |
| `PinnedStructure<T>` | 固定结构体 |
| `PinnedPointer` | 固定指针 |
| `PinHandler` | 创建固定对象的静态处理类 |

#### 扩展方法

| 方法 | 说明 |
|------|------|
| `Pin<T>()` | 固定数组 |
| `Pin()` | 固定字符串 |
| `PinAsArray<T>()` | 将单个元素作为数组固定 |
| `PinAsStructure<T>()` | 将单个元素作为结构体固定 |

#### 属性

| 属性 | 说明 |
|------|------|
| `IntPtr` | 获取固定内存的地址 |
| `Pointer` | 获取固定内存的原始指针 |
| `Size` | 获取元素数量 |
| `LengthAsBytes` | 获取字节长度 |
| `LengthAsChars` | 获取字符长度 |
| `Bytes` | 按字节访问 |
| `Chars` | 按字符访问 |
| `Shorts` | 按 short 访问 |
| `Integers` | 按 int 访问 |
| `Longs` | 按 long 访问 |

### 注意事项

1. **内存安全**: 使用 `using` 语句确保正确释放固定内存
2. **索引检查**: 默认启用索引检查，可通过 `CheckIndexes = false` 禁用以提高性能
3. **字符串修改**: 直接修改固定字符串的内存可能导致不可预期的行为

---

## English Documentation

### Overview

RS.MemoryPinned is a clean, dependency-free, powerful and easy-to-use high-performance cross-platform unmanaged memory manipulation library.

### Features

- **Cross-platform Support**: Supports Unity 5+, .NET Framework 2.0+, .NET Standard 2.0/2.1, .NET Core 2.0+
- **AOT Compilation Support**: Fully supports AOT compilation environments
- **Zero Dependencies**: No third-party library dependencies
- **High Performance**: Direct unmanaged memory operations, avoiding GC pressure
- **Type Safe**: Generic interfaces ensure type safety

### Installation

#### Unity
Copy the `RS.MemoryPinned` folder to your Unity project's `Packages` directory.

#### .NET Project
Reference via NuGet or directly reference `RS.MemoryPinned.dll`.

### Quick Start

#### 1. Pinning Arrays

```csharp
using RS.MemoryPinned;
using RS.MemoryPinned.Extension;

// Method 1: Using extension methods
int[] array = new int[100];
using (var pinned = array.Pin())
{
    // Get memory address
    IntPtr ptr = pinned.IntPtr;
    
    // Access elements by index
    pinned[0] = 123;
    int value = pinned[1];
    
    // Access as bytes
    pinned.Bytes[0] = 0xFF;
    
    // Access as other types
    pinned.Shorts[0] = 100;
    pinned.Integers[0] = 1000;
    pinned.Longs[0] = 10000L;
}

// Method 2: Using PinHandler
int[] array2 = new int[50];
var pinnedArray = PinHandler.Pin(ref array2);
// Dispose when done
pinnedArray.Dispose();
```

#### 2. Pinning Strings

```csharp
using RS.MemoryPinned;
using RS.MemoryPinned.Extension;

string text = "Hello World";
using (var pinned = text.Pin())
{
    // Access characters
    char c = pinned.Chars[0];
    
    // Modify characters (Note: strings are immutable, this directly modifies memory)
    pinned.Chars[0] = 'h';
}
```

#### 3. Pinning Structures

```csharp
using RS.MemoryPinned;
using RS.MemoryPinned.Extension;

struct MyStruct
{
    public int X;
    public int Y;
}

MyStruct myStruct = new MyStruct { X = 10, Y = 20 };
using (var pinned = myStruct.PinAsStructure())
{
    // Access struct data
    int x = pinned.PinnedObject.X;
    
    // Access via bytes
    byte firstByte = pinned.Bytes[0];
}
```

#### 4. Memory Slicing

```csharp
using RS.MemoryPinned;
using RS.MemoryPinned.Extension;

int[] array = new int[100];
using (var pinned = array.Pin())
{
    // Create a slice view
    var slice = pinned.Slice(10, 20); // Start at index 10, length 20
    
    // Slice shares the same memory
    slice[0] = 999; // This modifies array[10]
}
```

#### 5. Reading/Writing Different Types

```csharp
using RS.MemoryPinned;
using RS.MemoryPinned.Extension;

byte[] buffer = new byte[1024];
using (var pinned = buffer.Pin())
{
    // Write int to byte buffer
    pinned.WriteElementAtIndex<int>(0, 123456);
    
    // Read int
    int value = pinned.ReadElementAtIndex<int>(0);
    
    // Write long
    pinned.WriteElementAtIndex<long>(8, 9876543210L);
    
    // Read long
    long longValue = pinned.ReadElementAtIndex<long>(8);
}
```

### API Reference

#### Main Classes

| Class | Description |
|-------|-------------|
| `PinnedArray<T>` | Pinned array |
| `PinnedString` | Pinned string |
| `PinnedStructure<T>` | Pinned structure |
| `PinnedPointer` | Pinned pointer |
| `PinHandler` | Static handler for creating pinned objects |

#### Extension Methods

| Method | Description |
|--------|-------------|
| `Pin<T>()` | Pin an array |
| `Pin()` | Pin a string |
| `PinAsArray<T>()` | Pin a single element as an array |
| `PinAsStructure<T>()` | Pin a single element as a structure |

#### Properties

| Property | Description |
|----------|-------------|
| `IntPtr` | Get the address of pinned memory |
| `Pointer` | Get the raw pointer of pinned memory |
| `Size` | Get the element count |
| `LengthAsBytes` | Get the byte length |
| `LengthAsChars` | Get the char length |
| `Bytes` | Access as bytes |
| `Chars` | Access as chars |
| `Shorts` | Access as shorts |
| `Integers` | Access as integers |
| `Longs` | Access as longs |

### Notes

1. **Memory Safety**: Use `using` statements to ensure proper release of pinned memory
2. **Index Checking**: Index checking is enabled by default, disable with `CheckIndexes = false` for better performance
3. **String Modification**: Directly modifying pinned string memory may cause unexpected behavior

### License

AGPL 3.0
