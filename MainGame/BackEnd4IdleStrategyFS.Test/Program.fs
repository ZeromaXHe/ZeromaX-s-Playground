open System

module Program =
    [<EntryPoint>]
    let main _ =
        let random = Random(1662646297)

        for i in 1..3 do
            printfn $"random{i}: {random.Next()}"
        // random1: 1764945231
        // random2: 1770165129
        // random3: 1681579317
        0
