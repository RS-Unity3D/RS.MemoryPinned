using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
namespace System.Runtime.CompilerServices
{
    /*
     ​RefSafetyRulesAttribute 是 C# 11 引入的一个由编译器自动生成的底层特性，它的核心作用是：为 ref struct（引用结构）和 ref 字段提供严格的“内存生命周期安全（逃逸分析）”规则校验。
     .net4.5默认支持
     */
#if NET2_0 || NET3_5 || NET4_0
    [CompilerGenerated]
    [Embedded]
    [AttributeUsage(AttributeTargets.Module,AllowMultiple = false,Inherited = false)]
    internal sealed class RefSafetyRulesAttribute : Attribute
    {
        public readonly int Version;

        public RefSafetyRulesAttribute(int P_0)
        {
            Version = P_0;
        }
    }
#endif
}
