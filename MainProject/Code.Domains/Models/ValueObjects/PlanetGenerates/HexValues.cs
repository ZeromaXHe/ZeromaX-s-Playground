using Godot;

namespace Domains.Models.ValueObjects.PlanetGenerates;

/// <summary>
/// Copyright 2022 Jasper Flick
/// 来源：Catlike Coding - Unity - Hex Map 教程
/// 源码地址：https://bitbucket.org/catlikecoding-projects/hex-map-project/src/2399393cdf64ad7d83eaff456f1207aa214356e2/Assets/Scripts/HexValues.cs?at=release%2F3.4.0
/// 由 ZeromaXHe 进行针对 Godot 球面六边形地图的改造
/// </summary>
public struct HexValues
{
	/// <summary>
	/// Seven values stored in 32 bits.
	/// TTTTTTTT SSSSSSSS PPFFUUWW WWWEEEEE.
	/// </summary>
	/// <remarks>Not readonly to support hot reloading in Unity.</remarks>
// #pragma warning disable IDE0044 // Add readonly modifier
    private int _values;
// #pragma warning restore IDE0044 // Add readonly modifier

    private readonly int Get(int mask, int shift) =>
		_values >>> shift & mask;

    private readonly HexValues With(int value, int mask, int shift) => new()
	{
		_values = (_values & ~(mask << shift)) | ((value & mask) << shift)
	};

	public readonly int Elevation => Get(31, 0);
	public readonly HexValues WithElevation(int value) => With(value, 31, 0);
	public readonly int WaterLevel => Get(31, 5);
	public readonly int ViewElevation => Mathf.Max(Elevation, WaterLevel);
	public readonly bool IsUnderwater => WaterLevel > Elevation;
	public readonly HexValues WithWaterLevel(int value) => With(value, 31, 5);
	public readonly int UrbanLevel => Get(3, 10);
	public readonly HexValues WithUrbanLevel(int value) => With(value, 3, 10);
	public readonly int FarmLevel => Get(3, 12);
	public readonly HexValues WithFarmLevel(int value) => With(value, 3, 12);
	public readonly int PlantLevel => Get(3, 14);
	public readonly HexValues WithPlantLevel(int value) => With(value, 3, 14);
	public readonly int SpecialIndex => Get(255, 16);
	public readonly HexValues WithSpecialIndex(int index) => With(index, 255, 16);
	public readonly int TerrainTypeIndex => Get(255, 24);
	public readonly HexValues WithTerrainTypeIndex(int index) => With(index, 255, 24);
}