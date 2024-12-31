namespace FrontEndCommonFS.Util

open System
open System.Threading.Tasks
open Godot

module AwaitUtil =
    /// 将 C# 可以直接 await 的 Godot SignalAwaiter 转换为 F# 可以异步的 Task
    /// <param name="awaiter">来源一般是 GodotObject.ToSignal() </param>
    let awaiterToTask (awaiter: SignalAwaiter) =
        let tcs = TaskCompletionSource<Variant[]>()
        let awaiter = awaiter.GetAwaiter()

        awaiter.OnCompleted(fun () ->
            try
                if awaiter.IsCompleted then
                    tcs.SetResult(awaiter.GetResult())
                else
                    tcs.SetException(InvalidOperationException("Awaiter is not completed"))
            with ex ->
                tcs.SetException(ex))

        tcs.Task
