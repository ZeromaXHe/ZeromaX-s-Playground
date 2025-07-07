namespace TO.Domains.Functions.Chunks

open System.Collections.Generic
open Godot
open TO.Domains.Functions.HexSpheres
open TO.Domains.Types.Chunks
open TO.Domains.Types.Configs
open TO.Domains.Types.Friflos
open TO.Domains.Types.HexMeshes
open TO.Domains.Types.HexSpheres
open TO.Domains.Types.HexSpheres.Components
open TO.Domains.Types.HexSpheres.Components.Chunks
open TO.Domains.Types.HexSpheres.Components.Tiles

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-24 09:58:24
module ChunkLoaderQuery =
    let tryGetUsingChunk (env: #IChunkLoaderQuery) : TryGetUsingChunk =
        fun chunkId -> env.ChunkLoader.UsingChunks.TryGetValue(chunkId)

    let getAllUsingChunks (env: #IChunkLoaderQuery) : GetAllUsingChunks =
        fun () -> seq env.ChunkLoader.UsingChunks.Values

    let getViewportCamera (env: #IChunkLoaderQuery) : GetViewportCamera =
        fun () -> env.ChunkLoader.GetViewport().GetCamera3D()

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-24 10:27:24
module ChunkLoaderCommand =
    let resetInsightSetIdx (env: #IChunkLoaderQuery) : ResetInsightSetIdx =
        fun () -> env.ChunkLoader.InsightSetIdx <- 0

    let updateInsightSetNextIdx (env: #IChunkLoaderQuery) : UpdateInsightSetNextIdx =
        fun () -> env.ChunkLoader.InsightSetIdx <- env.ChunkLoader.InsightSetIdx ^^^ 1

    let clearOldData (env: 'E when 'E :> IChunkLoaderQuery and 'E :> IChunkLoaderCommand) : ClearChunkLoaderOldData =
        fun () ->
            let chunkLoader = env.ChunkLoader
            // 清空分块
            for child in chunkLoader.GetChildren() do
                child.QueueFree()

            chunkLoader.UsingChunks.Clear()
            chunkLoader.UnusedChunks.Clear()
            // 清空动态加载分块相关数据结构
            chunkLoader.ChunkQueryQueue.Clear()
            chunkLoader.VisitedChunkIds.Clear()
            chunkLoader.RimChunkIds.Clear()
            chunkLoader.InsightChunkIdsNow.Clear()
            env.ResetInsightSetIdx()

    let addUsingChunk (env: #IChunkLoaderQuery) : AddUsingChunk =
        fun chunkId chunk -> env.ChunkLoader.UsingChunks.Add(chunkId, chunk)

    let private updateLod
        (env: 'E when 'E :> IChunkQuery and 'E :> ILodMeshCacheQuery)
        (chunkId: ChunkId)
        (idChanged: bool)
        (hexGridChunk: IHexGridChunk)
        =
        let lod = env.GetChunkLod chunkId

        if lod = hexGridChunk.Lod && not idChanged then
            ()
        else
            hexGridChunk.Lod <- lod

            if env.IsHandlingLodGaps lod chunkId then
                // 对于需要处理接缝的情况，不使用缓存
                hexGridChunk.SetProcess true
            else
                let meshes = env.GetLodMeshes lod chunkId
                // 如果之前生成过 Lod 网格，直接应用；否则重新生成
                match meshes with
                | Some meshes -> hexGridChunk |> HexGridChunkCommand.showMesh meshes
                | None -> hexGridChunk.SetProcess true

    let private usedBy env (chunkId: ChunkId) (hexGridChunk: IHexGridChunk) =
        hexGridChunk.Id <- chunkId
        // 默认不生成网格，而是先查缓存
        hexGridChunk.SetProcess false
        hexGridChunk.Show()
        updateLod env chunkId true hexGridChunk

    let private showChunk (env: 'E when 'E :> IChunkLoaderQuery and 'E :> IChunkLoaderCommand) =
        fun chunkId ->
            match env.TryGetUsingChunk chunkId with
            | true, usingChunk -> updateLod env chunkId false usingChunk
            | false, _ ->
                // 没有空闲分块的话，初始化新的
                let hexGridChunk = env.ChunkLoader.GetUnusedChunk()
                usedBy env chunkId hexGridChunk
                env.AddUsingChunk chunkId hexGridChunk

    let private hideChunk (env: #IChunkLoaderQuery) =
        fun chunkId ->
            match env.TryGetUsingChunk chunkId with
            | true, chunk ->
                chunk.Hide()
                let chunkLoader = env.ChunkLoader
                chunkLoader.UsingChunks.Remove chunkId |> ignore
                chunkLoader.UnusedChunks.Enqueue chunk
            | false, _ -> ()

    let private searchNeighborChunk
        (env: 'E when 'E :> IChunkLoaderQuery and 'E :> IChunkLoaderCommand and 'E :> IPointQuery)
        =
        fun chunkId (filter: int HashSet) ->
            let chunkLoader = env.ChunkLoader

            for neighborId in env.GetNeighborIdsById chunkId do
                if filter = null || not <| filter.Contains neighborId then
                    if chunkLoader.VisitedChunkIds.Add neighborId then
                        chunkLoader.ChunkQueryQueue.Enqueue neighborId

    let private initOutRimChunks (env: 'E when 'E :> IChunkLoaderQuery and 'E :> IPointQuery and 'E :> IChunkCommand) =
        fun camPos ->
            let chunkLoader = env.ChunkLoader
            let rimChunkIds = chunkLoader.RimChunkIds
            let unloadSet = chunkLoader.UnloadSet
            let refreshSet = chunkLoader.RefreshSet
            let loadSet = chunkLoader.LoadSet
            let insightChunkIdsNow = chunkLoader.InsightChunkIdsNow

            insightChunkIdsNow
            |> Seq.collect env.GetNeighborIdsById
            |> Seq.filter (fun neighborId -> not <| insightChunkIdsNow.Contains neighborId)
            |> Seq.iter (fun rimId ->
                if rimChunkIds.Add rimId then
                    env.UpdateChunkInsightAndLod camPos true None rimId

                    if unloadSet.Contains rimId then
                        unloadSet.Remove rimId |> ignore
                        refreshSet.Add rimId |> ignore
                    else
                        loadSet.Add rimId |> ignore)

    let onChunkLoaderProcessed
        (env: 'E when 'E :> IChunkLoaderQuery and 'E :> IChunkLoaderCommand)
        : OnChunkLoaderProcessed =
        fun () ->
            let chunkLoader = env.ChunkLoader
            let stopwatch = chunkLoader.Stopwatch
            stopwatch.Restart()
            let loadSet = chunkLoader.LoadSet
            let mutable allClear = true
            let mutable limitCount = Mathf.Min(20, loadSet.Count)
#if MY_DEBUG
            let mutable loadCount = 0
#endif
            // 限制加载耗时（但加载优先级最高）
            while limitCount > 0 && stopwatch.ElapsedMilliseconds <= 14 do
                let chunkId = loadSet |> Seq.head
                loadSet.Remove chunkId |> ignore
                showChunk env chunkId
                limitCount <- limitCount - 1
#if MY_DEBUG
                loadCount <- loadCount + 1
#endif
            if loadSet.Count > 0 then
                allClear <- false

            let loadTime = stopwatch.ElapsedMilliseconds
            let mutable totalTime = loadTime
            stopwatch.Restart()
            let refreshSet = chunkLoader.RefreshSet
            limitCount <- Mathf.Min(20, refreshSet.Count)
#if MY_DEBUG
            let mutable refreshCount = 0
#endif
            // 限制刷新耗时（刷新优先级其次）
            while limitCount > 0 && totalTime + stopwatch.ElapsedMilliseconds <= 14 do
                let chunkId = refreshSet |> Seq.head
                refreshSet.Remove chunkId |> ignore
                showChunk env chunkId
                limitCount <- limitCount - 1
#if MY_DEBUG
                refreshCount <- refreshCount + 1
#endif
            if refreshSet.Count > 0 then
                allClear <- false

            let refreshTime = stopwatch.ElapsedMilliseconds
            totalTime <- totalTime + refreshTime
            stopwatch.Restart()
            let unloadSet = chunkLoader.UnloadSet
            limitCount <- Mathf.Min(100, unloadSet.Count)
#if MY_DEBUG
            let mutable unloadCount = 0
#endif
            // 限制卸载耗时（卸载优先级最低）
            while limitCount > 0 && totalTime + stopwatch.ElapsedMilliseconds <= 14 do
                let chunkId = unloadSet |> Seq.head
                unloadSet.Remove chunkId |> ignore
                hideChunk env chunkId
                limitCount <- limitCount - 1
#if MY_DEBUG
                unloadCount <- unloadCount + 1
#endif
            if unloadSet.Count > 0 then
                allClear <- false
#if MY_DEBUG // 好像默认 define 了 DEBUG，所以这里写 MY_DEBUG。
            let unloadTime = stopwatch.ElapsedMilliseconds
            totalTime <- totalTime + unloadTime

            let log =
                $"ChunkLoader _Process {totalTime} ms | load {loadCount}: {loadTime} ms, unload {unloadCount}: {unloadTime} ms, refresh {refreshCount}: {refreshTime} ms"

            if totalTime <= 16 then GD.Print log else GD.PrintErr log
#endif
            stopwatch.Stop()

            if allClear then
                chunkLoader.SetProcess false

    let initChunkNodes
        (env:
            'E
                when 'E :> IPlanetConfigQuery
                and 'E :> IChunkCommand
                and 'E :> IChunkLoaderQuery
                and 'E :> IEntityStoreQuery
                and 'E :> IEntityStoreCommand)
        : InitChunkNodes =
        fun () ->
            let time = Time.GetTicksMsec()
            let camera = env.GetViewportCamera()
            let chunkLoader = env.ChunkLoader
            let planetConfig = env.PlanetConfig

            env.ExecuteInCommandBuffer(fun cb ->
                let loadSet = chunkLoader.LoadSet
                let insightChunkIdsNow = chunkLoader.InsightChunkIdsNow
                let radius = planetConfig.Radius

                env
                    .Query<ChunkPos>()
                    .ForEachEntity(fun chunkPos chunkEntity ->
                        let id = chunkEntity.Id
                        // 此时拿不到真正 focusBase 的位置，暂且用相机自己的代替
                        if ChunkLodUtil.isChunkInsight chunkPos.Pos camera radius then
                            loadSet.Add id |> ignore
                            env.UpdateChunkInsightAndLod camera.GlobalPosition true <| Some cb <| id
                            insightChunkIdsNow.Add id |> ignore))

            initOutRimChunks env camera.GlobalPosition
            chunkLoader.SetProcess true
            GD.Print $"InitChunkNodes cost: {Time.GetTicksMsec() - time} ms"

    let onHexGridChunkProcessed
        (env:
            'E
                when 'E :> IEntityStoreQuery
                and 'E :> IChunkQuery
                and 'E :> ILodMeshCacheCommand
                and 'E :> IChunkTriangulationCommand)
        : OnHexGridChunkProcessed =
        fun (instance: IHexGridChunk) ->
            if instance.Id > 0 then
                // let time = Time.GetTicksMsec()
                instance |> HexGridChunkCommand.clearOldData

                env
                    .Query<TileChunkId>()
                    .HasValue<TileChunkId, ChunkId>(instance.Id)
                    .ForEachEntity(fun tileChunkId tileEntity -> env.Triangulate instance tileEntity)

                instance |> HexGridChunkCommand.applyNewData

                if not <| env.IsHandlingLodGaps instance.Lod instance.Id then
                    env.AddLodMeshes instance.Lod instance.Id
                    <| HexGridChunkCommand.getMeshes instance

            instance.SetProcess false

    // 后续优化可以考虑：
    // 全过程异步化，即：下次新任务来时停止上次任务，并保证一次任务能分成多帧执行。
    // 按照优先级，先加载附近的高模，再往外扩散。限制每帧的加载数量，保证不影响帧率。
    // 0. 预先计算好后续队列（加载队列、卸载队列、渲染队列、预加载队列）？
    // 1. 从相机当前位置开始，先保证视野范围内以最低 LOD 初始化
    //  1.1 先 BFS 最近的高精度 LOD 分块，按最低 LOD 初始化
    //  1.2 从正前方向视野两侧进行分块加载，同样按最低 LOD 初始化
    // 2. 提高视野范围内的 LOD 精度，同时对视野范围外的内容预加载（包括往外一圈的分块，和在玩家视角背后的）
    //
    // 动态卸载：
    // 建立一个清理队列，每次终止上次任务时，把所有分块加入到这个清理队列中。
    // 每帧从清理队列中出队一部分，校验它们当前的 LOD 状态，如果 LOD 状态不对，则卸载。
    let updateInsightChunks
        (env:
            'E
                when 'E :> IPlanetConfigQuery
                and 'E :> IChunkCommand
                and 'E :> IChunkLoaderQuery
                and 'E :> IChunkLoaderCommand
                and 'E :> IEntityStoreQuery)
        : UpdateInsightChunks =
        fun () ->
            // var time = Time.GetTicksMsec();
            let chunkLoader = env.ChunkLoader
            // 未能卸载的分块，说明本轮依然是在显示的分块
            let insightChunkIdsNow = chunkLoader.InsightChunkIdsNow
            let insightChunkIdsNext = chunkLoader.InsightChunkIdsNext
            let chunkQueryQueue = chunkLoader.ChunkQueryQueue
            let visitedChunkIds = chunkLoader.VisitedChunkIds
            let unloadSet = chunkLoader.UnloadSet
            let refreshSet = chunkLoader.RefreshSet
            let loadSet = chunkLoader.LoadSet
            let usingChunks = chunkLoader.UsingChunks
            let radius = env.PlanetConfig.Radius

            for unloadId in unloadSet |> Seq.filter usingChunks.ContainsKey do
                insightChunkIdsNow.Add unloadId |> ignore

            unloadSet.Clear()
            refreshSet.Clear() // 刷新分块一定在 _rimChunkIds 或 InsightChunkIdsNow 中，直接丢弃
            loadSet.Clear() // 未加载分块直接丢弃
            let camera = env.GetViewportCamera()
            let rimChunkIds = chunkLoader.RimChunkIds
            // 隐层边缘分块
            for chunkId in rimChunkIds |> Seq.filter usingChunks.ContainsKey do
                unloadSet.Add chunkId |> ignore
                env.UpdateChunkInsightAndLod camera.GlobalPosition false None chunkId

            rimChunkIds.Clear()

            for preInsightChunkId in insightChunkIdsNow do
                let preInsightChunkPos =
                    env.GetEntityById(preInsightChunkId).GetComponent<ChunkPos>().Pos

                visitedChunkIds.Add preInsightChunkId |> ignore

                if not <| ChunkLodUtil.isChunkInsight preInsightChunkPos camera radius then
                    // 分块不在视野范围内，隐藏它
                    unloadSet.Add preInsightChunkId |> ignore
                    env.UpdateChunkInsightAndLod camera.GlobalPosition false None preInsightChunkId
                else
                    insightChunkIdsNext.Add preInsightChunkId |> ignore
                    env.UpdateChunkInsightAndLod camera.GlobalPosition true None preInsightChunkId
                    // 刷新 Lod
                    if usingChunks.ContainsKey preInsightChunkId then
                        refreshSet.Add preInsightChunkId |> ignore
                    else
                        loadSet.Add preInsightChunkId |> ignore
                    // 分块在视野内，他的邻居才比较可能是在视野内
                    // 将之前不在但现在可能在视野范围内的 id 加入带查询队列
                    searchNeighborChunk env preInsightChunkId insightChunkIdsNow
            // 有种极端情况，就是新的视野范围内一个旧视野范围分块都没有！
            // 这时放开限制进行 BFS，直到找到第一个可见的分块
            // （因为我们认为新位置还是会具有空间上的相近性，BFS 应该会比随便找可见分块更好）
            if insightChunkIdsNext.Count = 0 then
                for chunkId in insightChunkIdsNow do
                    searchNeighborChunk env chunkId visitedChunkIds // 搜索所有外缘邻居

                let mutable breakNow = false

                while chunkQueryQueue.Count > 0 && not breakNow do
                    let chunkId = chunkQueryQueue.Dequeue()
                    let chunkPos = env.GetEntityById(chunkId).GetComponent<ChunkPos>().Pos

                    if ChunkLodUtil.isChunkInsight chunkPos camera radius then
                        // 找到第一个可见分块，重新入队，后面进行真正的处理
                        chunkQueryQueue.Enqueue chunkId
                        breakNow <- true
                    else
                        searchNeighborChunk env chunkId null
            // BFS 查询那些原来不在视野范围内的分块
            while chunkQueryQueue.Count > 0 do
                let chunkId = chunkQueryQueue.Dequeue()
                let chunkPos = env.GetEntityById(chunkId).GetComponent<ChunkPos>().Pos

                if ChunkLodUtil.isChunkInsight chunkPos camera radius then
                    if insightChunkIdsNext.Add chunkId then
                        loadSet.Add chunkId |> ignore
                        env.UpdateChunkInsightAndLod camera.GlobalPosition true None chunkId
                        searchNeighborChunk env chunkId null
            // 清理好各个数据结构，等下一次调用直接使用
            chunkQueryQueue.Clear()
            visitedChunkIds.Clear()
            insightChunkIdsNow.Clear()
            env.UpdateInsightSetNextIdx()
            // 显示外缘分块
            initOutRimChunks env camera.GlobalPosition
            chunkLoader.SetProcess true
    // GD.Print($"ChunkLoader UpdateInsightChunks cost {Time.GetTicksMsec() - time} ms");

    let refreshChunk (env: 'E when 'E :> IChunkLoaderQuery and 'E :> ILodMeshCacheCommand): RefreshChunk =
        fun (chunkId: ChunkId) ->
            match env.TryGetUsingChunk chunkId with
            | true, chunk ->
                // 让所有旧的网格缓存过期
                env.RemoveLodMeshes chunkId
                chunk.SetProcess true
            | false, _ -> ()
