using Godot;
using System;
using System.Collections.Generic;
using HexGlobalGame.game.hexGlobal.scripts.core;

[Tool]
public partial class PerlinTerrainGenerator : BaseTerrainGenerator
{
    public class ColorHeight
    {
        public Color Color;
        public float MaxHeight;

        public ColorHeight(Color color, float maxHeight)
        {
            Color = color;
            MaxHeight = maxHeight;
        }
    }

    [Export(PropertyHint.Range, "1, 8")] public int Octaves = 1;
    [Export(PropertyHint.Range, "0, 1")] public float Persistence = 0.5f;
    [Export(PropertyHint.Range, "1, 10")] public float Lacunarity = 2;
    [Export] public float MinHeight = 0.0f;
    [Export] public float MaxHeight = 10.0f;
    [Export] public float NoiseScaling = 100.0f;

    public List<ColorHeight> ColorHeights = new()
    {
        new(Colors.Blue, 0.0f),
        new(Colors.DarkGreen, 2.0f),
        new(Colors.DarkOliveGreen, 4.0f),
        new(Colors.GreenYellow, 6.0f),
        new(Colors.YellowGreen, 8.0f),
        new(Colors.White, 10.0f),
    };

    private static FastNoiseLite _perlinNoise;

    static PerlinTerrainGenerator()
    {
        _perlinNoise = new FastNoiseLite();
        _perlinNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
    }

    public override void AfterTileCreation(HexTile newTile)
    {
        float height = Mathf.Floor(3 * (((MaxHeight - MinHeight) * GetNoise(newTile.Center.Normalized().X,
            newTile.Center.Normalized().Y, newTile.Center.Normalized().Z)) + MinHeight)) / 3.0f;
        newTile.Height = height;

        for (int i = ColorHeights.Count - 1; i >= 0; i--)
        {
            if (height < ColorHeights[i].MaxHeight)
            {
                newTile.Color = ColorHeights[i].Color;
            }
        }

        // GD.Print($"newTile.id: {newTile.Id}, height: {newTile.Height}, color: {newTile.Color}");
    }

    private float GetNoise(float x, float y, float z)
    {
        float value = 0.0f;
        float scale = NoiseScaling;
        float effect = 1.0f;
        for (int i = 0; i < Octaves; i++)
        {
            value += effect * PerlinNoise3D(scale * x, scale * y, scale * z);
            scale *= Lacunarity;
            effect *= (1 - Persistence);
        }

        return value;
    }

    private static float PerlinNoise3D(float x, float y, float z)
    {
        x += 15;
        y += 25;
        z += 35;
        float xy = _perlinNoise.GetNoise2D(x, y);
        float xz = _perlinNoise.GetNoise2D(x, z);
        float yz = _perlinNoise.GetNoise2D(y, z);
        float yx = _perlinNoise.GetNoise2D(y, x);
        float zx = _perlinNoise.GetNoise2D(z, x);
        float zy = _perlinNoise.GetNoise2D(z, y);
        return (xy + xz + yz + yx + zx + zy) / 6;
    }
}