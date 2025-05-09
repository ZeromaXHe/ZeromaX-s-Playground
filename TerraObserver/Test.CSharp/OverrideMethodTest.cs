namespace Test.CSharp;

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// 测试 override new 在父子类型方法中的表现
/// Author: Zhu XH
/// Date: 2025-05-09 16:28:14
public class OverrideMethodTest
{
    private class Base
    {
        public string Method() => "Base Method";
        public string MethodForNew() => "Base MethodForNew";
        public virtual string VirtualMethod() => "Base VirtualMethod";
        public virtual string VirtualMethodForNew() => "Base VirtualMethodForNew";
        public virtual string VirtualMethodForOverride() => "Base VirtualMethodForOverride";
    }

    private class Derived : Base
    {
        // 普通方法没法 override，只能 new
        public string Method() => "Derived Method"; // 相当于 new
        public new string MethodForNew() => "Derived MethodForNew";
        public string VirtualMethod() => "Derived VirtualMethod"; // 相当于 new
        public new string VirtualMethodForNew() => "Derived VirtualMethodForNew";
        public override string VirtualMethodForOverride() => "Derived VirtualMethodForOverride";
    }

    private class Child : Derived
    {
        // 普通方法没法 override，只能 new
        public string Method() => base.Method(); // 相当于 new
        public new string MethodForNew() => base.MethodForNew();
        public string VirtualMethod() => base.VirtualMethod(); // 相当于 new
        public new string VirtualMethodForNew() => base.VirtualMethodForNew();
        public override string VirtualMethodForOverride() => base.VirtualMethodForOverride();
    }

    [Fact]
    public void Test()
    {
        Base baseDerived = new Derived();
        Assert.Equal("Base Method", baseDerived.Method());
        Assert.Equal("Base MethodForNew", baseDerived.MethodForNew());
        Assert.Equal("Base VirtualMethod", baseDerived.VirtualMethod());
        Assert.Equal("Base VirtualMethodForNew", baseDerived.VirtualMethodForNew());
        // 只有 override 才能让子类方法覆盖父类方法（即使使用父类类型调用，依然是执行子类方法）
        Assert.Equal("Derived VirtualMethodForOverride", baseDerived.VirtualMethodForOverride());

        // base 调用取决于实际类型
        Base baseChild = new Child();
        Assert.Equal("Base Method", baseChild.Method());
        Assert.Equal("Base MethodForNew", baseChild.MethodForNew());
        Assert.Equal("Base VirtualMethod", baseChild.VirtualMethod());
        Assert.Equal("Base VirtualMethodForNew", baseChild.VirtualMethodForNew());
        Assert.Equal("Derived VirtualMethodForOverride", baseChild.VirtualMethodForOverride());
        Derived derivedChild = new Child();
        Assert.Equal("Derived Method", derivedChild.Method());
        Assert.Equal("Derived MethodForNew", derivedChild.MethodForNew());
        Assert.Equal("Derived VirtualMethod", derivedChild.VirtualMethod());
        Assert.Equal("Derived VirtualMethodForNew", derivedChild.VirtualMethodForNew());
        Assert.Equal("Derived VirtualMethodForOverride", derivedChild.VirtualMethodForOverride());
        // 类型是 Child 时，调用 Child 直接继承的类型（Derived）的方法
        var child = new Child();
        Assert.Equal("Derived Method", child.Method());
        Assert.Equal("Derived MethodForNew", child.MethodForNew());
        Assert.Equal("Derived VirtualMethod", child.VirtualMethod());
        Assert.Equal("Derived VirtualMethodForNew", child.VirtualMethodForNew());
        Assert.Equal("Derived VirtualMethodForOverride", child.VirtualMethodForOverride());
    }
}