using System;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Enum;

/// <summary>
/// Copyright 2022 Jasper Flick
/// 来源：Catlike Coding - Unity - Hex Map 教程
/// 源码地址：https://bitbucket.org/catlikecoding-projects/hex-map-project/src/2399393cdf64ad7d83eaff456f1207aa214356e2/Assets/Scripts/HexFlags.cs?at=release%2F3.4.0
/// 由 ZeromaXHe 进行针对 Godot 球面六边形地图的改造
/// </summary>
[Flags]
public enum HexFlags
{
    Empty = 0,

    Road0 = 0b000001,
    Road1 = 0b000010,
    Road2 = 0b000100,
    Road3 = 0b001000,
    Road4 = 0b010000,
    Road5 = 0b100000,

    Roads = 0b111111,
    RoadsPentagon = 0b011111,

    RiverIn0 = 0b000001_000000,
    RiverIn1 = 0b000010_000000,
    RiverIn2 = 0b000100_000000,
    RiverIn3 = 0b001000_000000,
    RiverIn4 = 0b010000_000000,
    RiverIn6 = 0b100000_000000,

    RiverIn = 0b111111_000000,
    RiverInPentagon = 0b011111_000000,

    RiverOut0 = 0b000001_000000_000000,
    RiverOut1 = 0b000010_000000_000000,
    RiverOut2 = 0b000100_000000_000000,
    RiverOut3 = 0b001000_000000_000000,
    RiverOut4 = 0b010000_000000_000000,
    RiverOut5 = 0b100000_000000_000000,

    RiverOut = 0b111111_000000_000000,
    RiverOutPentagon = 0b011111_000000_000000,

    River = 0b111111_111111_000000,
    RiverPentagon = 0b011111_011111_000000,

    Walled = 0b1_000000_000000_000000,

    Explored = 0b010_000000_000000_000000,
    Explorable = 0b100_000000_000000_000000,
}

public static class HexFlagsExtensions
{
    /// <summary>
    /// 是否有任何标志位被设置。
    /// Whether any flags of a mask are set.
    /// </summary>
    /// <param name="flags">标志位。Flags.</param>
    /// <param name="mask">掩码。Mask.</param>
    /// <returns>是否有任何标志位被设置。Whether any of the flags are set.</returns>
    public static bool HasAny(this HexFlags flags, HexFlags mask) =>
        (flags & mask) != 0;

    /// <summary>
    /// 是否掩码所有的标志位都被设置了。
    /// Whether all flags of a mask are set.
    /// </summary>
    /// <param name="flags">标志位。Flags.</param>
    /// <param name="mask">掩码。Mask.</param>
    /// <returns>是否所有标志位被设置。Whether all the flags are set.</returns>
    public static bool HasAll(this HexFlags flags, HexFlags mask) =>
        (flags & mask) == mask;

    /// <summary>
    /// 是否掩码没有一位被设置。
    /// Whether no flags of a mask are set.
    /// </summary>
    /// <param name="flags">标志位。Flags.</param>
    /// <param name="mask">掩码。Mask.</param>
    /// <returns>是否没有标志位被设置。Whether none of the flags are set.</returns>
    public static bool HasNone(this HexFlags flags, HexFlags mask) =>
        (flags & mask) == 0;

    /// <summary>
    /// 返回被设置了入参掩码的标志位。
    /// Returns flags with bits of the given mask set.
    /// </summary>
    /// <param name="flags">标志位。Flags.</param>
    /// <param name="mask">要设置的掩码。Mask to set.</param>
    /// <returns>设置好的标志位。The set flags.</returns>
    public static HexFlags With(this HexFlags flags, HexFlags mask) =>
        flags | mask;

    /// <summary>
    /// 返回被清理了入参掩码的标志位。
    /// Returns flags with bits of the given mask cleared.
    /// </summary>
    /// <param name="flags">标志位。Flags.</param>
    /// <param name="mask">要清理的掩码。Mask to clear.</param>
    /// <returns>清理过的掩码。The cleared flags.</returns>
    public static HexFlags Without(this HexFlags flags, HexFlags mask) =>
        flags & ~mask;

    /// <summary>
    /// 标志位是否设置了给定方向的道路。
    /// Whether the flag for a road in a given direction is set.
    /// </summary>
    /// <param name="flags">标志位。Flags.</param>
    /// <param name="direction">路的方向。Road direction.</param>
    /// <returns>那条路是否被设置了。Whether the road is set.</returns>
    public static bool HasRoad(this HexFlags flags, int direction) =>
        flags.Has(HexFlags.Road0, direction);

    /// <summary>
    /// 返回设置好了给定道路的标志位。
    /// Returns the flags with the bit for a given road set.
    /// </summary>
    /// <param name="flags">标志位。Flags.</param>
    /// <param name="direction">道路方向。Road direction.</param>
    /// <returns>设置了给定道路位的标志位。Flags with the road bit set.</returns>
    public static HexFlags WithRoad(this HexFlags flags, int direction) =>
        flags.With(HexFlags.Road0, direction);

    /// <summary>
    /// 返回取消给定道路位的标志位。
    /// Returns the flags without the bit for a given road set.
    /// </summary>
    /// <param name="flags">标志位。Flags.</param>
    /// <param name="direction">道路方向。Road direction.</param>
    /// <returns>没有给定道路位的标志位。Flags without the road bit set.</returns>
    public static HexFlags WithoutRoad(this HexFlags flags, int direction) =>
        flags.Without(HexFlags.Road0, direction);

    /// <summary>
    /// 标志位是否设置了给定方向的流入河流。
    /// Whether the flag for an incoming river in a given direction is set.
    /// </summary>
    /// <param name="flags">标志位。Flags.</param>
    /// <param name="direction">流入河流方向。Incoming river direction.</param>
    /// <returns>是否河流被设置。Whether the river is set.</returns>
    public static bool HasRiverIn(this HexFlags flags, int direction) =>
        flags.Has(HexFlags.RiverIn0, direction);

    /// <summary>
    /// 返回设置好了给定流入河流位的标志位。
    /// Returns the flags with the bit for a given incoming river set.
    /// </summary>
    /// <param name="flags">标志位。Flags.</param>
    /// <param name="direction">流入河流方向。Incoming river direction.</param>
    /// <returns>设置了河流位的标志位。Flags with the river bit set.</returns>
    public static HexFlags WithRiverIn(this HexFlags flags, int direction) =>
        flags.With(HexFlags.RiverIn0, direction);

    /// <summary>
    /// 返回取消给定流入河流位的标志位。
    /// Returns the flags without the bit for a given incoming river set.
    /// </summary>
    /// <param name="flags">标志位。Flags.</param>
    /// <param name="direction">流入河流方向。Incoming river direction.</param>
    /// <returns>没有设置河流位的标志位。Flags without the river bit set.</returns>
    public static HexFlags WithoutRiverIn(this HexFlags flags, int direction) =>
        flags.Without(HexFlags.RiverIn0, direction);

    /// <summary>
    /// 标志位是否设置了给定方向的流出河流。
    /// Whether the flag for an outgoing river in a given direction is set.
    /// </summary>
    /// <param name="flags">标志位。Flags.</param>
    /// <param name="direction">流出河流方向。Outgoing river direction.</param>
    /// <returns>是否河流被设置。Whether the river is set.</returns>
    public static bool HasRiverOut(this HexFlags flags, int direction) =>
        flags.Has(HexFlags.RiverOut0, direction);

    /// <summary>
    /// 标志位在给定方向上是否有设置流入或流出河流。
    /// Whether the flags of either an incoming or outgoing river in a given direction is set.
    /// </summary>
    /// <param name="flags">标志位。Flags.</param>
    /// <param name="direction">河流方向。River direction.</param>
    /// <returns>是否设置了河流。Whether the river is set.</returns>
    public static bool HasRiver(this HexFlags flags, int direction) =>
        flags.HasRiverIn(direction) || flags.HasRiverOut(direction);

    /// <summary>
    /// 返回设置好了给定流出河流位的标志位。
    /// Returns the flags with the bit for a given outgoing river set.
    /// </summary>
    /// <param name="flags">标志位。Flags.</param>
    /// <param name="direction">Outgoing river direction.</param>
    /// <returns>Flags with the river bit set.</returns>
    public static HexFlags WithRiverOut(this HexFlags flags, int direction) => flags.With(HexFlags.RiverOut0, direction);

    /// <summary>
    /// 返回取消给定流出河流位的标志位。
    /// Returns the flags without the bit for a given outgoing river set.
    /// </summary>
    /// <param name="flags">标志位。Flags.</param>
    /// <param name="direction">流出河流方向。Outgoing river direction.</param>
    /// <returns>没有设置河流位的标志位。Flags without the river bit set.</returns>
    public static HexFlags WithoutRiverOut(this HexFlags flags, int direction) => flags.Without(HexFlags.RiverOut0, direction);

    /// <summary>
    /// 返回流入河流方向。仅在河流存在时有效。
    /// Returns the incoming river direction. Only valid if the river exists.
    /// </summary>
    /// <param name="flags">标志位。Flags.</param>
    /// <returns>河流方向。River direction.</returns>
    public static int RiverInDirection(this HexFlags flags) => flags.ToDirection(6);

    /// <summary>
    /// 返回流出河流方向。仅在河流存在时有效。
    /// Returns the outgoing river direction. Only valid if the river exists.
    /// </summary>
    /// <param name="flags">标志位。Flags.</param>
    /// <returns>河流方向。River direction.</returns>
    public static int RiverOutDirection(this HexFlags flags) => flags.ToDirection(12);

    private static bool Has(this HexFlags flags, HexFlags start, int direction) => ((int)flags & ((int)start << direction)) != 0;

    private static HexFlags With(this HexFlags flags, HexFlags start, int direction) => flags | (HexFlags)((int)start << direction);

    private static HexFlags Without(this HexFlags flags, HexFlags start, int direction) => flags & ~(HexFlags)((int)start << direction);

    private static int ToDirection(this HexFlags flags, int shift) => 
        (((int)flags >> shift) & 0b111111) switch
        {
            0b000001 => 0,
            0b000010 => 1,
            0b000100 => 2,
            0b001000 => 3,
            0b010000 => 4,
            _ => 5
        };
}