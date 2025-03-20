using System.Collections.Generic;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.Framework.Base;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Utils.HexSphereGrid;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Entities;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-02-14 11:50
public class Point(bool chunky, Vector3 position, SphereAxial coords, int id = -1) : AEntity(id)
{
    // 是，属于分块；否，属于地块
    public bool Chunky { get; } = chunky;
    public Vector3 Position { get; } = position;
    public SphereAxial Coords { get; } = coords;
    public List<int> FaceIds;
}