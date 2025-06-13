namespace TO.FSharp.Services.Types.HexSphereServiceT

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-30 22:39:30
type InitHexSphere = unit -> unit
type ClearOldData = unit -> unit
type HexSphereServiceDep = {
    InitHexSphere: InitHexSphere
    ClearOldData: ClearOldData
}
