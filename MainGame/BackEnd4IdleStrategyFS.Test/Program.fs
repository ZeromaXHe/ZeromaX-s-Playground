open System
open System.Reactive.Subjects
open System.Reactive.Linq

module Program =
    let randomTest () =
        let random = Random(1662646297)

        for i in 1..3 do
            printfn $"random{i}: {random.Next()}"
        // random1: 1764945231
        // random2: 1770165129
        // random3: 1681579317
        0

    let gameProcessToTickTest () =

        // 创建 gameProcess Subject
        let gameProcess = new Subject<float>()

        // 创建 gameTicked Subject
        let gameTicked = new Subject<int64>()

        // 订阅 gameProcess，累计时间并处理
        let subscription =
            gameProcess
            |> Observable.scan
                (fun (acc, lastTick) delta ->
                    let newAcc = acc + delta

                    if newAcc >= 1.0 then
                        (newAcc - 1.0, true)
                    else
                        (newAcc, false))
                (0.0, false)
            |> Observable.filter snd
            |> Observable.subscribe (fun (acc, _) ->
                let currentTime = DateTime.UtcNow.Ticks
                currentTime |> gameTicked.OnNext
                printfn $"Game ticked at %f{1.0 - acc} seconds, current timestamp: %d{currentTime}")

        // 模拟游戏主循环，每 16 毫秒发出一个 delta 时间
        let gameLoop () =
            while true do
                let delta = 0.016 // 假设每帧大约 16 毫秒
                delta |> gameProcess.OnNext
                TimeSpan.FromMilliseconds 16.0 |> System.Threading.Thread.Sleep

        // 启动游戏主循环
        let gameThread = new System.Threading.Thread(gameLoop)
        gameThread.Start()

        // 订阅 gameTicked 以处理每秒的时间戳事件
        let gameTickedSubscription =
            gameTicked.Subscribe(fun timestamp -> printfn $"Received timestamp: %d{timestamp}")

        // 防止程序立即退出
        let str = Console.ReadLine()
        printfn $"{str}"
        0

    [<EntryPoint>]
    let main _ = gameProcessToTickTest ()
