namespace Test.FSharp.Grammars

open Xunit

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-27 18:16:27
module FlexibleTypeTests =
    [<Interface>]
    type ITest1 =
        abstract Get: unit -> int

    [<Interface>]
    type ITest2 =
        abstract Get: unit -> string

    // 测试发现，同名的接口方法，会优先使用泛型约束中靠前的
    module Test =
        let test1 (env: 'E when 'E :> ITest1 and 'E :> ITest2) =
            let i = env.Get()
            i

        let test2 (env: 'E when 'E :> ITest2 and 'E :> ITest1) =
            let i = env.Get()
            i

    type Env() =
        interface ITest1 with
            member this.Get() = 1

        interface ITest2 with
            member this.Get() = "2"

    [<Fact>]
    let ``可变类型测试`` () =
        // Arrange
        let i1 =
            { new ITest1 with
                member this.Get() = 1 } // 使用对象表达式实现接口

        let i2 =
            { new ITest2 with
                member this.Get() = "2" } // 只是需要一个支持接口方法的对象

        let i3 = Env()
        // Act
        let r1 = Test.test1 i3
        let r2 = Test.test2 i3
        // Assert
        Assert.Equal(1, r1)
        Assert.Equal("2", r2)
