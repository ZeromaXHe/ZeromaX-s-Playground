namespace FrontEndToolFS.SebastianPlanet

open System
open Godot
open Godot.Collections

// 参考 Godot 官方文档 _get_property_list() 示例：
// https://docs.godotengine.org/zh-cn/4.x/classes/class_object.html#class-object-private-method-get-property-list
// 实际貌似没有可读性和可用性。
// 可读性：貌似这种实现方式没法分组，可读性差
// 可用性：虽然解决了重载已保存场景时无法读取保存的自定义属性的问题，但编译后好像还是会读不出来（不确定 C# 版代码行不行，反正 F# 不行）
// （可用性这一点存疑，现在发现其实不管是 Export 还是自定义，都是编译后得重新加载已保存场景才能读出来）
type PropertyListToolTestFS() =
    inherit Node3D()

    member val _numberCount = 0 with get, set
    member val _numbers = Array<int>()

    override this._GetPropertyList() =
        let properties = Array<Dictionary>()

        for i in 0 .. this._numberCount - 1 do
            let items = new Dictionary()
            items["name"] <- $"numbers_{i}"
            items["type"] <- int Variant.Type.Int
            items["hint"] <- int PropertyHint.Enum
            items["hint_string"] <- "Zero,One,Two,Three,Four,Five"
            properties.Add items

        properties

    override this._Get property =
        let propertyName = property.ToString()

        if propertyName.StartsWith("numbers_") then
            let index = Int32.Parse << propertyName.Substring <| "numbers_".Length
            Variant.CreateFrom this._numbers[index]
        else
            new Variant()

    override this._Set(property, value) =
        let propertyName = property.ToString()

        if propertyName.StartsWith("numbers_") then
            let index = Int32.Parse << propertyName.Substring <| "numbers_".Length
            // 不加这里的逻辑会 index 超过范围，无法加载保存的记录，官方文档中没有
            if index >= this._numbers.Count then
                this._numbers.Resize(index + 1) |> ignore

            this._numbers[index] <- value.As<int>()
            true
        else
            false
