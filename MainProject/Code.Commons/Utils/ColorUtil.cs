using Godot;

namespace Commons.Utils;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH
/// Date: 2025-04-08 17:01:00
public class ColorUtil
{
    private static readonly Dictionary<Color, string> ColorNames = new()
    {
        [Colors.AliceBlue] = "AliceBlue",
        [Colors.AntiqueWhite] = "AntiqueWhite",
        [Colors.Aqua] = "Aqua",
        [Colors.Aquamarine] = "Aquamarine",
        [Colors.Azure] = "Azure",
        [Colors.Beige] = "Beige",
        [Colors.Bisque] = "Bisque",
        [Colors.Black] = "Black",
        [Colors.BlanchedAlmond] = "BlanchedAlmond",
        [Colors.Blue] = "Blue",
        [Colors.BlueViolet] = "BlueViolet",
        [Colors.Brown] = "Brown",
        [Colors.Burlywood] = "Burlywood",
        [Colors.CadetBlue] = "CadetBlue",
        [Colors.Chartreuse] = "Chartreuse",
        [Colors.Chocolate] = "Chocolate",
        [Colors.Coral] = "Coral",
        [Colors.CornflowerBlue] = "CornflowerBlue",
        [Colors.Cornsilk] = "Cornsilk",
        [Colors.Crimson] = "Crimson",
        [Colors.Cyan] = "Cyan",
        [Colors.DarkBlue] = "DarkBlue",
        [Colors.DarkCyan] = "DarkCyan",
        [Colors.DarkGoldenrod] = "DarkGoldenrod",
        [Colors.DarkGray] = "DarkGray",
        [Colors.DarkGreen] = "DarkGreen",
        [Colors.DarkKhaki] = "DarkKhaki",
        [Colors.DarkMagenta] = "DarkMagenta",
        [Colors.DarkOliveGreen] = "DarkOliveGreen",
        [Colors.DarkOrange] = "DarkOrange",
        [Colors.DarkOrchid] = "DarkOrchid",
        [Colors.DarkRed] = "DarkRed",
        [Colors.DarkSalmon] = "DarkSalmon",
        [Colors.DarkSeaGreen] = "DarkSeaGreen",
        [Colors.DarkSlateBlue] = "DarkSlateBlue",
        [Colors.DarkSlateGray] = "DarkSlateGray",
        [Colors.DarkTurquoise] = "DarkTurquoise",
        [Colors.DarkViolet] = "DarkViolet",
        [Colors.DeepPink] = "DeepPink",
        [Colors.DeepSkyBlue] = "DeepSkyBlue",
        [Colors.DimGray] = "DimGray",
        [Colors.DodgerBlue] = "DodgerBlue",
        [Colors.Firebrick] = "Firebrick",
        [Colors.FloralWhite] = "FloralWhite",
        [Colors.ForestGreen] = "ForestGreen",
        [Colors.Fuchsia] = "Fuchsia",
        [Colors.Gainsboro] = "Gainsboro",
        [Colors.GhostWhite] = "GhostWhite",
        [Colors.Gold] = "Gold",
        [Colors.Goldenrod] = "Goldenrod",
        [Colors.Gray] = "Gray",
        [Colors.Green] = "Green",
        [Colors.GreenYellow] = "GreenYellow",
        [Colors.Honeydew] = "Honeydew",
        [Colors.HotPink] = "HotPink",
        [Colors.IndianRed] = "IndianRed",
        [Colors.Indigo] = "Indigo",
        [Colors.Ivory] = "Ivory",
        [Colors.Khaki] = "Khaki",
        [Colors.Lavender] = "Lavender",
        [Colors.LavenderBlush] = "LavenderBlush",
        [Colors.LawnGreen] = "LawnGreen",
        [Colors.LemonChiffon] = "LemonChiffon",
        [Colors.LightBlue] = "LightBlue",
        [Colors.LightCoral] = "LightCoral",
        [Colors.LightCyan] = "LightCyan",
        [Colors.LightGoldenrod] = "LightGoldenrod",
        [Colors.LightGray] = "LightGray",
        [Colors.LightGreen] = "LightGreen",
        [Colors.LightPink] = "LightPink",
        [Colors.LightSalmon] = "LightSalmon",
        [Colors.LightSeaGreen] = "LightSeaGreen",
        [Colors.LightSkyBlue] = "LightSkyBlue",
        [Colors.LightSlateGray] = "LightSlateGray",
        [Colors.LightSteelBlue] = "LightSteelBlue",
        [Colors.LightYellow] = "LightYellow",
        [Colors.Lime] = "Lime",
        [Colors.LimeGreen] = "LimeGreen",
        [Colors.Linen] = "Linen",
        [Colors.Magenta] = "Magenta",
        [Colors.Maroon] = "Maroon",
        [Colors.MediumAquamarine] = "MediumAquamarine",
        [Colors.MediumBlue] = "MediumBlue",
        [Colors.MediumOrchid] = "MediumOrchid",
        [Colors.MediumPurple] = "MediumPurple",
        [Colors.MediumSeaGreen] = "MediumSeaGreen",
        [Colors.MediumSlateBlue] = "MediumSlateBlue",
        [Colors.MediumSpringGreen] = "MediumSpringGreen",
        [Colors.MediumTurquoise] = "MediumTurquoise",
        [Colors.MediumVioletRed] = "MediumVioletRed",
        [Colors.MidnightBlue] = "MidnightBlue",
        [Colors.MintCream] = "MintCream",
        [Colors.MistyRose] = "MistyRose",
        [Colors.Moccasin] = "Moccasin",
        [Colors.NavajoWhite] = "NavajoWhite",
        [Colors.NavyBlue] = "NavyBlue",
        [Colors.OldLace] = "OldLace",
        [Colors.Olive] = "Olive",
        [Colors.OliveDrab] = "OliveDrab",
        [Colors.Orange] = "Orange",
        [Colors.OrangeRed] = "OrangeRed",
        [Colors.Orchid] = "Orchid",
        [Colors.PaleGoldenrod] = "PaleGoldenrod",
        [Colors.PaleGreen] = "PaleGreen",
        [Colors.PaleTurquoise] = "PaleTurquoise",
        [Colors.PaleVioletRed] = "PaleVioletRed",
        [Colors.PapayaWhip] = "PapayaWhip",
        [Colors.PeachPuff] = "PeachPuff",
        [Colors.Peru] = "Peru",
        [Colors.Pink] = "Pink",
        [Colors.Plum] = "Plum",
        [Colors.PowderBlue] = "PowderBlue",
        [Colors.Purple] = "Purple",
        [Colors.RebeccaPurple] = "RebeccaPurple",
        [Colors.Red] = "Red",
        [Colors.RosyBrown] = "RosyBrown",
        [Colors.RoyalBlue] = "RoyalBlue",
        [Colors.SaddleBrown] = "SaddleBrown",
        [Colors.Salmon] = "Salmon",
        [Colors.SandyBrown] = "SandyBrown",
        [Colors.SeaGreen] = "SeaGreen",
        [Colors.Seashell] = "Seashell",
        [Colors.Sienna] = "Sienna",
        [Colors.Silver] = "Silver",
        [Colors.SkyBlue] = "SkyBlue",
        [Colors.SlateBlue] = "SlateBlue",
        [Colors.SlateGray] = "SlateGray",
        [Colors.Snow] = "Snow",
        [Colors.SpringGreen] = "SpringGreen",
        [Colors.SteelBlue] = "SteelBlue",
        [Colors.Tan] = "Tan",
        [Colors.Teal] = "Teal",
        [Colors.Thistle] = "Thistle",
        [Colors.Tomato] = "Tomato",
        [Colors.Transparent] = "Transparent",
        [Colors.Turquoise] = "Turquoise",
        [Colors.Violet] = "Violet",
        [Colors.WebGray] = "WebGray",
        [Colors.WebGreen] = "WebGreen",
        [Colors.WebMaroon] = "WebMaroon",
        [Colors.WebPurple] = "WebPurple",
        [Colors.Wheat] = "Wheat",
        [Colors.White] = "White",
        [Colors.WhiteSmoke] = "WhiteSmoke",
        [Colors.Yellow] = "Yellow",
        [Colors.YellowGreen] = "YellowGreen",
    };

    private static readonly VpTree<Color> ColorTree = new();
    private static bool _colorTreeInitialized;

    public static string GetClosestName(Color color) => ColorNames.GetValueOrDefault(GetClosestColor(color), "Unknown");

    public static Color GetClosestColor(Color color)
    {
        if (!_colorTreeInitialized)
        {
            // 初始化颜色 VP 树
            ColorTree.Create(ColorNames.Keys.ToArray(), (c1, c2) =>
            {
                var rDiff = Mathf.Abs(c1.R - c2.R);
                var gDiff = Mathf.Abs(c1.G - c2.G);
                var bDiff = Mathf.Abs(c1.B - c2.B);
                return rDiff * rDiff + gDiff * gDiff + bDiff * bDiff; // 平方距离
            });
            _colorTreeInitialized = true;
        }

        ColorTree.Search(color, 1, out var results, out _);
        return results[0];
    }
}