namespace FrontEndToolFS.HexPlane

open Godot

type ICell =
    abstract member Index: int
    abstract member TerrainTypeIndex: int
    abstract member IsVisible: bool

type HexCellShaderData() =
    let mutable cellTexture: Image = null
    let mutable cellTextureData: Color array = null
    let mutable hexCellData: ImageTexture = null

    member this.Initialize x z =
        if cellTexture <> null then
            cellTexture.Resize(x, z)
        else
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

    member this.RefreshTerrain(cell: ICell) =
        cellTextureData[cell.Index].A8 <- cell.TerrainTypeIndex
        this.EnableUpdate cell

    member this.RefreshVisibility(cell: ICell) =
        cellTextureData[cell.Index].R8 <- if cell.IsVisible then 255 else 0
        this.EnableUpdate cell

    member private this.EnableUpdate(cell: ICell) =
        cellTexture.SetPixel(
            cell.Index % cellTexture.GetWidth(),
            cell.Index / cellTexture.GetWidth(),
            cellTextureData[cell.Index]
        )
        // 更新 Shader global uniform 变量（hex_cell_data）
        // 不能用这里的方法：RenderingServer.global_shader_parameter_get() 就没法在游戏循环里用
        // RenderingServer
        //     .GlobalShaderParameterGet("hex_cell_data")
        //     .As<ImageTexture>()
        //     .Update(cellTexture)
        hexCellData.Update(cellTexture)
