namespace TO.Repos.Commands.HexSpheres

open Godot
open Friflo.Engine.ECS
open TO.Domains.Alias.HexSpheres.Chunks
open TO.Domains.Alias.HexSpheres.Faces
open TO.Domains.Components.HexSpheres.Faces
open TO.Domains.Functions.HexSpheres

type AddFace = Chunky -> Vector3 -> Vector3 -> Vector3 -> FaceId

[<Interface>]
type IFaceCommand =
    abstract Add: AddFace

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 16:21:19
module FaceCommand =
    let add (store: EntityStore) : AddFace =
        fun (chunky: Chunky) (vertex1: Vector3) (vertex2: Vector3) (vertex3: Vector3) ->
            let center = (vertex1 + vertex2 + vertex3) / 3f
            let tag = ChunkFunction.chunkyTag chunky

            let face =
                store.CreateEntity(FaceComponent(center, vertex1, vertex2, vertex3), &tag)

            face.Id
