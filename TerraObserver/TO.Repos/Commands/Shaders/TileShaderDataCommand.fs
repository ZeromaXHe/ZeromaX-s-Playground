namespace TO.Repos.Commands.Shaders

open TO.Repos.Data.Shaders

type InitShaderData = int -> unit

[<Interface>]
type ITileShaderDataCommand =
    abstract InitShaderData: InitShaderData

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-25 09:15:25
module TileShaderDataCommand =
    let initShaderData (tileShaderData: TileShaderData) : InitShaderData = tileShaderData.Initialize
