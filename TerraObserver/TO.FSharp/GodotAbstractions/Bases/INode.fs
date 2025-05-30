namespace TO.FSharp.GodotAbstractions.Bases

open System.Runtime.InteropServices
open Godot
open Microsoft.FSharp.Core.LanguagePrimitives

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-28 23:00:28
[<Interface>]
type INode =
    inherit IGodotObject
    abstract Name: StringName with get, set
    abstract GetPath: unit -> NodePath
    abstract SetProcess: bool -> unit
    abstract IsNodeReady: unit -> bool
    // F# 中声明可选参数、默认值给 C# 实现的话，需要使用互操作注解；以及 long 枚举的转换需要按此处处理，而不能只用 enum<'T>
    abstract AddChild:
        node: Node *
        [<Optional; DefaultParameterValue(false)>] forceReadableName: bool *
        [<Optional; DefaultParameterValue(EnumOfValue<int64, Node.InternalMode> 0L)>] ``internal``: Node.InternalMode ->
            unit

    abstract GetChild: idx: int * [<Optional; DefaultParameterValue(false)>] includeInternal: bool -> Node

    abstract GetChildren:
        [<Optional; DefaultParameterValue(false)>] includeInternal: bool -> Node Godot.Collections.Array

    abstract GetViewport: unit -> Viewport
    abstract QueueFree: unit -> unit
