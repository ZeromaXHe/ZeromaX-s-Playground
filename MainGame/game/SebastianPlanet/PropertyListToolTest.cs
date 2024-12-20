using FrontEndToolFS.SebastianPlanet;
using Godot;
using Godot.Collections;

namespace ZeromaXPlayground.game.SebastianPlanet;

[Tool]
public partial class PropertyListToolTest : PropertyListToolTestFS
{
    // private int _numberCount;

    [Export]
    public int NumberCount
    {
        get => _numberCount;
        set
        {
            _numberCount = value;
            _numbers.Resize(_numberCount);
            NotifyPropertyListChanged();
        }
    }

    // private Array<int> _numbers = [];
    //
    // public override Array<Dictionary> _GetPropertyList()
    // {
    //     var properties = new Array<Dictionary>();
    //
    //     for (var i = 0; i < _numberCount; i++)
    //     {
    //         properties.Add(new Dictionary
    //         {
    //             { "name", $"number_{i}" },
    //             { "type", (int)Variant.Type.Int },
    //             { "hint", (int)PropertyHint.Enum },
    //             { "hint_string", "Zero,One,Two,Three,Four,Five" },
    //         });
    //     }
    //
    //     return properties;
    // }
    //
    // public override Variant _Get(StringName property)
    // {
    //     var propertyName = property.ToString();
    //     if (!propertyName.StartsWith("number_")) return default;
    //     var index = int.Parse(propertyName["number_".Length..]);
    //     return _numbers[index];
    // }
    //
    // public override bool _Set(StringName property, Variant value)
    // {
    //     var propertyName = property.ToString();
    //     if (!propertyName.StartsWith("number_")) return false;
    //     var index = int.Parse(propertyName["number_".Length..]);
    //     if (index >= _numbers.Count) _numbers.Resize(index + 1); // 不加这一行会 index 超过范围，无法加载保存的记录
    //     _numbers[index] = value.As<int>();
    //     return true;
    // }

    // 请忽略 IDE 冗余提示，需要保留此处和 partial
    public override Array<Dictionary> _GetPropertyList() => base._GetPropertyList();
    public override Variant _Get(StringName property) => base._Get(property);
    public override bool _Set(StringName property, Variant value) => base._Set(property, value);
}