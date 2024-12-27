namespace FrontEndToolFS.SebastianPlanet

open System
open Godot
open Godot.Collections

// 参考 Godot 官方文档 _get_property_list() 示例：
// https://docs.godotengine.org/zh-cn/4.x/classes/class_object.html#class-object-private-method-get-property-list
// 可读性：能够使用 “/” 分割来实现分组
// 可用性：不管是 Export 还是自定义，都是编译后得重新加载已保存场景才能读出来
type PropertyListToolTestFS() =
    inherit Node3D()

    member val _numberCount = 0 with get, set
    member val _numbers = Array<int>()

    override this._GetPropertyList() =
        let properties = Array<Dictionary>()

        for i in 0 .. this._numberCount - 1 do
            let items = new Dictionary()
            items["name"] <- $"numbers/num{i}"
            items["type"] <- int Variant.Type.Int
            items["hint"] <- int PropertyHint.Enum
            items["hint_string"] <- "Zero,One,Two,Three,Four,Five"
            let usage =
                match i with
                | 0 -> int PropertyUsageFlags.ReadOnly + int PropertyUsageFlags.Editor // 需要相加（||| 也行）
                | 1 -> int PropertyUsageFlags.Editor
                | 2 -> int PropertyUsageFlags.NoEditor // 不显示
                | _ -> int PropertyUsageFlags.Default
            items["usage"] <- usage
            properties.Add items

        properties

    override this._Get property =
        let propertyName = property.ToString()

        if propertyName.StartsWith("numbers/num") then
            let index = Int32.Parse << propertyName.Substring <| "numbers/num".Length
            Variant.CreateFrom this._numbers[index]
        else
            new Variant()

    override this._Set(property, value) =
        let propertyName = property.ToString()

        if propertyName.StartsWith("numbers/num") then
            let index = Int32.Parse << propertyName.Substring <| "numbers/num".Length
            // 不加这里的逻辑会 index 超过范围，无法加载保存的记录，官方文档中没有
            if index >= this._numbers.Count then
                this._numbers.Resize(index + 1) |> ignore

            this._numbers[index] <- value.As<int>()
            true
        else
            false
