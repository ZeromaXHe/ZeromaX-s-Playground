namespace FrontEndToolFS.HexPlane

open System.Collections.Generic
open Godot

type ICell =
    abstract member Index: int
    abstract member TerrainTypeIndex: int
    abstract member IsVisible: bool
    abstract member IsExplored: bool

type IGridVis =
    abstract member ResetVisibility: unit -> unit

type HexCellShaderData() =
    let mutable cellTexture: Image = null
    let mutable cellTextureData: Color array = null
    let mutable hexCellData: ImageTexture = null
    let mutable enabled = false
    let transitioningCells = List<ICell>()
    let transitionSpeed = 255.0
    let mutable needsVisibilityReset = false

    let changeCellPixel (cell: ICell) data =
        cellTexture.SetPixel(cell.Index % cellTexture.GetWidth(), cell.Index / cellTexture.GetWidth(), data)

    let updateCellData (cell: ICell) delta =
        let index = cell.Index
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
            data.B8 <- 0

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
        else
            cellTextureData
            |> Array.iteri (fun i _ -> cellTextureData[i] <- Color(0.0f, 0.0f, 0.0f, 0.0f))

        transitioningCells.Clear()
        enabled <- true

    member this.RefreshTerrain(cell: ICell) =
        cellTextureData[cell.Index].A8 <- cell.TerrainTypeIndex
        changeCellPixel cell cellTextureData[cell.Index]
        enabled <- true

    member this.RefreshVisibility(cell: ICell) =
        let index = cell.Index

        if this.ImmediateMode then
            cellTextureData[index].R8 <- if cell.IsVisible then 255 else 0
            cellTextureData[index].G8 <- if cell.IsExplored then 255 else 0
            changeCellPixel cell cellTextureData[index]
        elif cellTextureData[index].B8 <> 255 then
            // 使用一个尚未使用的数据通道来存储一个单元格是否处于转换状态，如 B 值
            cellTextureData[index].B8 <- 255
            transitioningCells.Add cell

        enabled <- true

    member this.UpdateData(delta: float) =
        if enabled then
            if needsVisibilityReset then
                needsVisibilityReset <- false
                this.Grid.ResetVisibility()

            let delta = int <| delta * transitionSpeed
            let delta = if delta = 0 then 1 else delta
            let mutable i = 0

            while i < transitioningCells.Count do
                if not <| updateCellData transitioningCells[i] delta then
                    transitioningCells[i] <- transitioningCells[transitioningCells.Count - 1]
                    transitioningCells.RemoveAt <| transitioningCells.Count - 1
                else
                    i <- i + 1
            // 更新 Shader global uniform 变量（hex_cell_data）
            // 不能用这里的方法：RenderingServer.global_shader_parameter_get() 就没法在游戏循环里用
            // RenderingServer
            //     .GlobalShaderParameterGet("hex_cell_data")
            //     .As<ImageTexture>()
            //     .Update(cellTexture)
            hexCellData.Update(cellTexture)
            enabled <- transitioningCells.Count > 0

    member val ImmediateMode = false with get, set

    member this.ViewElevationChanged() =
        needsVisibilityReset <- true
        enabled <- true

    [<DefaultValue>]
    val mutable Grid: IGridVis

    member this.SetMapData (cell: ICell) data =
        cellTextureData[cell.Index].B8 <-
            if data < 0f then 0
            elif data < 1f then int <| data * 254f
            else 254

        changeCellPixel cell cellTextureData[cell.Index]
        enabled <- true
