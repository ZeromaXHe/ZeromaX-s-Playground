namespace FrontEndToolFS.HexPlane

open System.Collections.Generic
open Godot

type ICell =
    abstract member Index: int
    abstract member TerrainTypeIndex: int
    abstract member IsVisible: bool
    abstract member IsExplored: bool
    abstract member IsUnderWater: bool
    abstract member WaterSurfaceY: float32

type IGridForShader =
    abstract member ResetVisibility: unit -> unit
    abstract member GetCell: int -> ICell

type HexCellShaderData() as this =
    let mutable cellTexture: Image = null
    let mutable cellTextureData: Color array = null
    let mutable hexCellData: ImageTexture = null
    let mutable enabled = false
    let transitioningCellIndices = List<int>()
    let transitionSpeed = 255.0
    let mutable needsVisibilityReset = false
    let mutable visibilityTransitions: bool array = null

    let changeCellPixel (cell: ICell) data =
        cellTexture.SetPixel(cell.Index % cellTexture.GetWidth(), cell.Index / cellTexture.GetWidth(), data)

    let updateCellData index delta =
        let cell = this.Grid.GetCell index
        let mutable data = cellTextureData[index]
        let mutable stillUpdating = false

        if cell.IsExplored && data.G8 < 255 then
            stillUpdating <- true
            let t = data.G8 + delta
            data.G8 <- if t >= 255 then 255 else t

        if cell.IsVisible then
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
        changeCellPixel cell data
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

    member this.RefreshTerrain(cell: ICell) =
        let mutable data = cellTextureData[cell.Index]

        data.B8 <-
            if cell.IsUnderWater then
                int <| cell.WaterSurfaceY * (255f / 30f)
            else
                0

        data.A8 <- int cell.TerrainTypeIndex
        cellTextureData[cell.Index] <- data
        changeCellPixel cell data
        enabled <- true

    member this.RefreshVisibility(cell: ICell) =
        let index = cell.Index

        if this.ImmediateMode then
            cellTextureData[index].R8 <- if cell.IsVisible then 255 else 0
            cellTextureData[index].G8 <- if cell.IsExplored then 255 else 0
            changeCellPixel cell cellTextureData[index]
        elif not visibilityTransitions[index] then
            visibilityTransitions[index] <- true
            transitioningCellIndices.Add cell.Index

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

    member this.ViewElevationChanged(cell: ICell) =
        cellTextureData[cell.Index].B8 <-
            if cell.IsUnderWater then
                int <| cell.WaterSurfaceY * (255f / 30f)
            else
                0

        changeCellPixel cell cellTextureData[cell.Index]
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
