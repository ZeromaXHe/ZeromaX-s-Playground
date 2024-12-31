namespace FrontEnd4IdleStrategyFS.Global

open Godot

type IDebug =
    abstract member AddProperty<'a>: title: string -> value: 'a -> order: int -> unit

type FpsGlobalNodeFS() =
    inherit Node()
    member val debug = Unchecked.defaultof<IDebug> with get, set
