using System;
using System.Runtime.CompilerServices;

namespace Microsoft.CodeAnalysis
{
#if NET2_0 || NET3_5
/*
在 C# 中，开发者通常不需要手动编写或调用 EmbeddedAttribute。它是 C# 编译器（Roslyn）在后台自动生成并使用的一个内部特性（通常位于 Microsoft.CodeAnalysis 命名空间下）。
它的唯一作用是：告诉编译器和运行环境，当前这个类型（通常是其他特性类）不是来自 .NET 框架的官方类库（BCL），而是编译器自己悄悄“塞（Embed）”进当前程序集里的。​
C# 8.0 的可空引用类型（Nullable Reference Types）
C# 9.0 的 init 属性访问器和 record 类型
C# 7.2 的 ref struct 和 in 参数
*/
    [CompilerGenerated, Embedded]
    internal sealed class EmbeddedAttribute : Attribute
    {
        // Methods
        public EmbeddedAttribute() { }
    }
#endif
}


