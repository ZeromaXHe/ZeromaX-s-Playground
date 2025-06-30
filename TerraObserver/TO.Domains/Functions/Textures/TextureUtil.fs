namespace TO.Domains.Functions.Textures

open Godot

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-21 20:35:21
module TextureUtil =
    // 模拟 Unity Texture2D.GetPixelBilinear API
    // （入参是 0 ~ 1 的 float，超过范围则取小数部分，即“包裹模式进行重复”）
    // 参考：https://docs.unity3d.com/cn/2021.3/ScriptReference/Texture2D.GetPixelBilinear.html
    let GetPixelBilinear (img: Image) (u: float32) (v: float32) =
        let mutable x =
            int <| Mathf.PosMod(u * float32 (img.GetWidth()), float32 <| img.GetWidth())

        let mutable y =
            int <| Mathf.PosMod(v * float32 (img.GetHeight()), float32 <| img.GetHeight())
        // 这里现在 Godot（4.3）有 bug 啊，文档说 PosMod 返回 [0, b), 结果我居然取到了 b……
        if x = img.GetWidth() then
            // GD.PrintErr($"WTF! PosMod not working for ({u}, {v}) => ({img.GetWidth()}, {img.GetHeight()}) => ({x}, {y})");
            x <- 0

        if y = img.GetHeight() then
            // GD.PrintErr($"WTF! PosMod not working for ({u}, {v}) => ({img.GetWidth()}, {img.GetHeight()}) => ({x}, {y})");
            y <- 0

        let color = img.GetPixel(x, y)
        Vector4(color.R, color.G, color.B, color.A)
