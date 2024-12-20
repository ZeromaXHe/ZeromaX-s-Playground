namespace FrontEndToolFS.SebastianPlanet

open System

type MinMax() =
    let mutable min = Single.MaxValue
    let mutable max = Single.MinValue
    member this.Min = min
    member this.Max = max
    
    member this.AddValue v =
        if v > max then
            max <- v
        if v < min then
            min <- v
