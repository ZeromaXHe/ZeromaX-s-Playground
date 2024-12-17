namespace FrontEndToolFS.HexPlane

open System.Collections.Generic
open Godot

type ICell =
    abstract member Index: int
    abstract member TerrainTypeIndex: int
    abstract member IsExplored: bool
    abstract member IsUnderWater: bool

type IGridForShader =
    abstract member ResetVisibility: unit -> unit
    abstract member IsCellVisible: int -> bool
    abstract member CellData: HexCellData array

type HexCellShaderData() as this =
    let mutable cellTexture: Image = null
    let mutable cellTextureData: Color array = null
    let mutable hexCellData: ImageTexture = null
    let mutable enabled = false
    let transitioningCellIndices = List<int>()
    let transitionSpeed = 255.0
    let mutable needsVisibilityReset = false
    let mutable visibilityTransitions: bool array = null

    let changeCellPixel cellIndex data =
        cellTexture.SetPixel(cellIndex % cellTexture.GetWidth(), cellIndex / cellTexture.GetWidth(), data)

    let updateCellData index delta =
        let mutable data = cellTextureData[index]
        let mutable stillUpdating = false

        if this.Grid.CellData[index].IsExplored && data.G8 < 255 then
            stillUpdating <- true
            let t = data.G8 + delta
            data.G8 <- if t >= 255 then 255 else t

        if this.Grid.IsCellVisible index then
            if data.R8 < 255 then
                stillUpdating <- true
                let t = data.R8 + delta
                data.R8 <- if t >= 255 then 255 else t
        elif data.R8 > 0 then
            stillUpdating <- true
            let t = data.R8 - delta
            data.R8 <- if t <= 0 then 0 else t

        if not stillUpdating then
            visibilityTransitions[index] <- false

        cellTextureData[index] <- data
        changeCellPixel index data
        stillUpdating

    member this.Initialize x z =
        // if cellTexture <> null then
        //     cellTexture.Resize(x, z)
        // else
        cellTexture <- Image.CreateEmpty(x, z, false, Image.Format.Rgba8)
        // hexCellData.SetSizeOverride <| cellTexture.GetSize() 这个好像也是没卵用，只能重新赋值
        hexCellData <- ImageTexture.CreateFromImage cellTexture
        // 对应：Shader.SetGlobalTexture("_HexCellData", cellTexture);
        RenderingServer.GlobalShaderParameterSet("hex_cell_data", Variant.CreateFrom(hexCellData))
        // 对应：Shader.SetGlobalVector("_HexCellData_TexelSize", new Vector4(1f / x, 1f / z, x, z));
        RenderingServer.GlobalShaderParameterSet(
            "hex_cell_data_texel_size",
            Vector4(1f / float32 x, 1f / float32 z, float32 x, float32 z)
        )

        if cellTextureData = null || cellTextureData.Length <> x * z then
            cellTextureData <- Array.zeroCreate <| x * z
            visibilityTransitions <- Array.zeroCreate <| x * z
        else
            cellTextureData
            |> Array.iteri (fun i _ ->
                cellTextureData[i] <- Color(0.0f, 0.0f, 0.0f, 0.0f)
                visibilityTransitions[i] <- false)

        transitioningCellIndices.Clear()
        enabled <- true

    member this.RefreshTerrain cellIndex =
        let cell = this.Grid.CellData[cellIndex]
        let mutable data = cellTextureData[cellIndex]

        data.B8 <-
            if cell.IsUnderWater then
                int <| cell.WaterSurfaceY * (255f / 30f)
            else
                0

        data.A8 <- int cell.TerrainTypeIndex
        cellTextureData[cellIndex] <- data
        changeCellPixel cellIndex data
        enabled <- true

    member this.RefreshVisibility cellIndex =
        if this.ImmediateMode then
            cellTextureData[cellIndex].R8 <- if this.Grid.IsCellVisible cellIndex then 255 else 0
            cellTextureData[cellIndex].G8 <- if this.Grid.CellData[cellIndex].IsExplored then 255 else 0
            changeCellPixel cellIndex cellTextureData[cellIndex]
        elif not visibilityTransitions[cellIndex] then
            visibilityTransitions[cellIndex] <- true
            transitioningCellIndices.Add cellIndex

        enabled <- true

    member this.UpdateData(delta: float) =
        if enabled then
            if needsVisibilityReset then
                needsVisibilityReset <- false
                this.Grid.ResetVisibility()

            let delta = int <| delta * transitionSpeed
            let delta = if delta = 0 then 1 else delta
            let mutable i = 0

            while i < transitioningCellIndices.Count do
                if not <| updateCellData transitioningCellIndices[i] delta then
                    transitioningCellIndices[i] <- transitioningCellIndices[transitioningCellIndices.Count - 1]
                    transitioningCellIndices.RemoveAt <| transitioningCellIndices.Count - 1
                else
                    i <- i + 1
            // 更新 Shader global uniform 变量（hex_cell_data）
            // 不能用这里的方法：RenderingServer.global_shader_parameter_get() 就没法在游戏循环里用
            // RenderingServer
            //     .GlobalShaderParameterGet("hex_cell_data")
            //     .As<ImageTexture>()
            //     .Update(cellTexture)
            hexCellData.Update(cellTexture)
            enabled <- transitioningCellIndices.Count > 0

    member val ImmediateMode = false with get, set

    member this.ViewElevationChanged cellIndex =
        let cell = this.Grid.CellData[cellIndex]

        cellTextureData[cellIndex].B8 <-
            if cell.IsUnderWater then
                int <| cell.WaterSurfaceY * (255f / 30f)
            else
                0

        changeCellPixel cellIndex cellTextureData[cellIndex]
        needsVisibilityReset <- true
        enabled <- true

    [<DefaultValue>]
    val mutable Grid: IGridForShader

// member this.SetMapData (cell: ICell) data =
//     cellTextureData[cell.Index].B8 <-
//         if data < 0f then 0
//         elif data < 1f then int <| data * 255f
//         else 255
//
//     changeCellPixel cell cellTextureData[cell.Index]
//     enabled <- true
