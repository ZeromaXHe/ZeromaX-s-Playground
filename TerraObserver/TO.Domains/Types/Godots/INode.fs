namespace TO.Domains.Types.Godots

open System
open System.Runtime.InteropServices
open Godot

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-16 16:49:16
[<Interface>]
[<AllowNullLiteral>]
type INode =
    inherit IGodotObject
    abstract Name: StringName with get, set // 374

    abstract AddChild:
        node: Node *
        [<Optional; DefaultParameterValue(false)>] forceReadableName: bool *
        [<Optional; DefaultParameterValue( (*EnumOfValue<int64, Node.InternalMode> 0L*) Node.InternalMode.Disabled)>] ``internal``:
            Node.InternalMode ->
            unit // 791

    abstract GetChildCount: [<Optional; DefaultParameterValue(false)>] includeInternal: bool -> int // 827

    abstract GetChildren:
        [<Optional; DefaultParameterValue(false)>] includeInternal: bool -> Node Godot.Collections.Array // 839

    abstract GetChild: idx: int * [<Optional; DefaultParameterValue(false)>] includeInternal: bool -> Node // 861
    abstract GetPath: unit -> NodePath // 1066
    abstract SetProcess: bool -> unit // 1360
    abstract GetViewport: unit -> Viewport // 1873
    abstract QueueFree: unit -> unit // 1886
    abstract IsNodeReady: unit -> bool // 1910

    [<CLIEvent>]
    abstract Ready: Action IDelegateEvent // 2208

    [<CLIEvent>]
    abstract TreeExiting: Action IDelegateEvent // 2252
