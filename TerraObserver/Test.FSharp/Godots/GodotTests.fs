module GodotTests

open Godot
open Xunit

[<Fact>]
let ``Transform rotate test`` () =
    // Arrange
    let mutable transform = Transform3D.Identity
    // Act
    let newTransform = transform.Rotated(Vector3.Right, Mathf.Pi)
    // Assert
    Assert.Equal(Transform3D.Identity, transform)
    Assert.Equal(Transform3D.Identity.Rotated(Vector3.Right, Mathf.Pi), newTransform)
