namespace TO.Infras.Abstractions.Planets.Queries

open TO.Domains.Models.Planets

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-05-17 17:14:17
[<Interface>]
type IPointQuery =
    abstract GetAllByChunky: chunky: bool -> Point seq