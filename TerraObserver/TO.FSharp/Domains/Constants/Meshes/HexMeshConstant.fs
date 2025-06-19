namespace TO.FSharp.Domains.Constants.Meshes

open Godot

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-10 22:12:10
module HexMeshConstant =
    let weights1 = Colors.Red
    let weights2 = Colors.Green
    let weights3 = Colors.Blue
    let triArr<'T> (c: 'T) = [| c; c; c |]
    let quadArr<'T> (c: 'T) = [| c; c; c; c |]
    let quad2Arr<'T> (c1: 'T) (c2: 'T) = [| c1; c1; c2; c2 |]
