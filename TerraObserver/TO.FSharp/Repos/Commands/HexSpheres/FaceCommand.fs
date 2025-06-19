namespace TO.FSharp.Repos.Commands.HexSpheres

open Godot
open Friflo.Engine.ECS
open TO.Domains.Components.HexSpheres.Faces
open TO.Domains.Functions.HexSpheres
open TO.FSharp.Repos.Data.Commons

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 16:21:19
module FaceCommand =
    let add (env: #IEntityStore) =
        fun (chunky: bool) (vertex1: Vector3) (vertex2: Vector3) (vertex3: Vector3) ->
            let center = (vertex1 + vertex2 + vertex3) / 3f
            let tag = ChunkFunction.chunkyTag chunky

            let face =
                env.EntityStore.CreateEntity(FaceComponent(center, vertex1, vertex2, vertex3), &tag)

            face.Id
