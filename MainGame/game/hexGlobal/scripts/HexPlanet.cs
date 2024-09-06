using Godot;
using System;
using System.Collections.Generic;
using HexGlobalGame.game.hexGlobal.scripts.core;

[Tool]
public partial class HexPlanet : Node3D
{
    [Export(PropertyHint.Range, "1,10000")]
    public float Radius = 100;

    [Export(PropertyHint.Range, "0,7")] public int Subdivisions;
    [Export(PropertyHint.Range, "0,6")] public int ChunkSubdivisions;
    [Export] private bool _debugPointLine = false;

    public Material ChunkMaterial;

    public PerlinTerrainGenerator TerrainGenerator;
    private Node3D test;

    public List<HexTile> Tiles;
    public List<HexChunk> Chunks;

    public override void _Ready()
    {
        test = GetNode<Node3D>("Test");
        TerrainGenerator = GetNode<PerlinTerrainGenerator>("PerlinTerrainGenerator");
    }

    public HexTile GetTile(int id)
    {
        return Tiles[id];
    }

    public HexChunk GetChunk(int id)
    {
        return Chunks[id];
    }

    public void ClearSpheresAndLines()
    {
        foreach (var child in test.GetChildren())
        {
            child.QueueFree();
        }
    }

    public void DrawSpheres(List<Vector3> tileCenters, List<Vector3> chunkOrigins)
    {
        if (!_debugPointLine) return;

        foreach (var tileCenter in tileCenters)
        {
            DrawSphereOnPos(tileCenter, Colors.Green);
        }

        foreach (var chunkOrigin in chunkOrigins)
        {
            DrawSphereOnPos(chunkOrigin, Colors.Red);
        }
    }

    private void DrawSphereOnPos(Vector3 pos, Color color)
    {
        var ins = new MeshInstance3D();
        test.AddChild(ins);
        ins.Position = pos;
        var sphere = new SphereMesh();
        sphere.Radius = Radius / 40;
        sphere.Height = Radius / 20;
        ins.Mesh = sphere;
        var material = new StandardMaterial3D();
        material.AlbedoColor = color;
        ins.SetSurfaceOverrideMaterial(0, material);
    }

    public void DrawLine(Vector3 from, Vector3 to)
    {
        if (!_debugPointLine) return;

        float cylinderRadius = Radius / 60;
        var ins = new MeshInstance3D();
        test.AddChild(ins);
        ins.Position = (from + to) / 2;
        var diff = to - from;
        if (diff.X != 0)
        {
            // bug：现在看的方向好像部分会报错？
            ins.LookAt(new Vector3(-diff.Y, diff.X, 0), diff);
        }
        else
        {
            ins.LookAt(new Vector3(0, diff.Z, -diff.Y), diff);
        }

        var cylinder = new CylinderMesh();
        cylinder.TopRadius = cylinderRadius;
        cylinder.BottomRadius = cylinderRadius;
        cylinder.Height = from.DistanceTo(to);
        cylinder.RadialSegments = 6;

        var material = new StandardMaterial3D();
        material.AlbedoColor = Colors.White;
        ins.SetSurfaceOverrideMaterial(0, material);
        ins.Mesh = cylinder;
    }

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        foreach (HexTile tile in Tiles)
        {
            tile.Planet = this;
        }

        foreach (HexChunk chunk in Chunks)
        {
            chunk.Planet = this;
            chunk.SetupEvents();
        }
    }
}