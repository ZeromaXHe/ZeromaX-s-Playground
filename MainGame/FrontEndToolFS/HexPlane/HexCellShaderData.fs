namespace FrontEndToolFS.HexPlane

open Godot

type ICell =
    abstract member Index: int
    abstract member TerrainTypeIndex: int
    abstract member IsVisible: bool

type HexCellShaderData() =
    let mutable cellTexture: Image = null
    let mutable cellTextureData: Color array = null

    member this.HexCellData = cellTexture

    [<DefaultValue>]
    val mutable HexCellDataTexelSize: Vector4

    [<DefaultValue>]
    val mutable HexCellDataUpdater: Image -> unit

    member this.Initialize x z =
        if cellTexture <> null then
            cellTexture.Resize(x, z)
        else
            // 没有 Unity 类似的 API：Shader.SetGlobalTexture("_HexCellData", cellTexture);
            cellTexture <- Image.CreateEmpty(x, z, false, Image.Format.Rgba8)

        // 没有 Unity 类似的 API：Shader.SetGlobalVector("_HexCellData_TexelSize", new Vector4(1f / x, 1f / z, x, z));
        // 需要在外面调用处（HexGrid）来修改 Shader uniform 变量（hex_cell_data 和 hex_cell_data_texel_size）
        this.HexCellDataTexelSize <- Vector4(1f / float32 x, 1f / float32 z, float32 x, float32 z)

        if cellTextureData = null || cellTextureData.Length <> x * z then
            cellTextureData <- Array.zeroCreate <| x * z
        else
            cellTextureData
            |> Array.iteri (fun i _ -> cellTextureData[i] <- Color(0.0f, 0.0f, 0.0f, 0.0f))

    member this.RefreshTerrain(cell: ICell) =
        cellTextureData[cell.Index].A8 <- cell.TerrainTypeIndex
        this.EnableUpdate cell

    member this.RefreshVisibility (cell: ICell) =
        cellTextureData[cell.Index].R8 <- if cell.IsVisible then 255 else 0
        this.EnableUpdate cell

    member private this.EnableUpdate(cell: ICell) =
        cellTexture.SetPixel(
            cell.Index % cellTexture.GetWidth(),
            cell.Index / cellTexture.GetWidth(),
            cellTextureData[cell.Index]
        )
        // 更新外界 Shader uniform 变量（hex_cell_data）
        this.HexCellDataUpdater cellTexture
