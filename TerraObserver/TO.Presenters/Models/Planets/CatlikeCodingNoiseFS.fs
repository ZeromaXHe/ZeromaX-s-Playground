namespace TO.Presenters.Models.Planets

open Godot
open TO.Domains.Structs.Tiles
open TO.Domains.Utils.Commons
open TO.Domains.Utils.HexSpheres

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-21 20:23:21
[<AbstractClass>]
type CatlikeCodingNoiseFS() =
    inherit Resource()
    let hashGridSize = 256
    let hashGrid = Array.zeroCreate<HexHash> <| hashGridSize * hashGridSize
    let rng = new RandomNumberGenerator()
    // Export
    abstract NoiseSource: Texture2D with get, set
    abstract Seed: uint64 with get, set
    // 外部属性
    member val NoiseSourceImage: Image = null with get, set

    member this.InitializeHashGrid(seed: uint64) =
        let initState = rng.State
        rng.Seed <- seed

        for i in 0 .. hashGrid.Length - 1 do
            hashGrid[i] <- HexHash.Create()

        rng.State <- initState

    member this.SampleHashGrid(position: Vector3) =
        let position = Math3dUtil.ProjectToSphere(position, HexMetrics.StandardRadius)

        let mutable x =
            int
            <| Mathf.PosMod(position.X - position.Y * 0.5f - position.Z * 0.5f, float32 hashGridSize)

        if x = hashGridSize then // 前面噪声扰动那里说过 PosMod 文档返回 [0, b), 结果取到了 b，所以怕了…… 加个防御性处理
            x <- 0

        let mutable z =
            int
            <| Mathf.PosMod((position.Y - position.Z) * HexMetrics.OuterToInner, float32 hashGridSize)

        if z = hashGridSize then
            z <- 0

        hashGrid[x + z * hashGridSize]
