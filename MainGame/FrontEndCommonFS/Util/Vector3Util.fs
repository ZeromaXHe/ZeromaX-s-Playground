namespace FrontEndCommonFS.Util

open Godot

module Vector3Util =
    /// 置换 Vector3 的 Y
    let changeY (v: Vector3) y = Vector3(v.X, y, v.Z)
