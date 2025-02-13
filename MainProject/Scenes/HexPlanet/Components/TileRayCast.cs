using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using ZeromaXsPlaygroundProject.Scenes.HexPlanet.Core;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Components;

public partial class TileRayCast : Node3D
{
    private HashSet<HexTile> _selectedHexTiles;
    private List<CsgSphere3D> _cursors;

    private float _targetGUIHeight = 0.0f;

    // Start is called before the first frame update
    public override void _Ready()
    {
        _selectedHexTiles = new HashSet<HexTile>();
        _cursors = new List<CsgSphere3D>();
    }

    // Update is called once per frame
    public override void _PhysicsProcess(double delta)
    {
        var camera = GetViewport().GetCamera3D();
        var mousePos = GetViewport().GetMousePosition();
        var origin = camera.ProjectRayOrigin(mousePos);
        var end = origin + camera.ProjectRayNormal(mousePos) * 10000;
        var query = PhysicsRayQueryParameters3D.Create(origin, end);
        var result = GetWorld3D().DirectSpaceState.IntersectRay(query);
        if (result != null && result.Count > 0)
        {
            HexChunkRenderer hcr = result["collider"].As<StaticBody3D>().GetParent() as HexChunkRenderer;
            Vector3 planetPosition = hcr.Position;
            if (hcr != null)
            {
                HexChunk hc = hcr.GetHexChunk();
                if (hc != null)
                {
                    HexTile tile = hc.GetClosestTileAngle(result["position"].AsVector3() - planetPosition);

                    // LineRenderer lr = GetComponent<LineRenderer>();
                    List<Vector3> points = new List<Vector3>();
                    for (int i = 0; i < tile.Vertices.Count; i++)
                    {
                        points.Add(tile.Vertices[i] + (tile.Center.Normalized() * tile.Height) + planetPosition);
                    }

                    // lr.positionCount = points.Count;
                    // lr.SetPositions(points.ToArray());

                    if (Input.IsActionJustPressed("mouseLeft") && !Input.IsKeyPressed(Key.Shift))
                    {
                        _selectedHexTiles.Clear();
                        ResetCursors();
                    }

                    if (Input.IsMouseButtonPressed(MouseButton.Left))
                    {
                        if (Input.IsKeyPressed(Key.Ctrl))
                        {
                            _selectedHexTiles.Remove(tile);
                            ResetCursors();
                            return;
                        }

                        if (_selectedHexTiles.Add(tile))
                        {
                            // Add cursor
                            var newCursor = new CsgSphere3D();
                            AddChild(newCursor);
                            newCursor.Position =
                                tile.Center + (tile.Center.Normalized() * tile.Height) + planetPosition;
                            _cursors.Add(newCursor);
                        }

                        _targetGUIHeight = tile.Height;
                        GD.Print(_selectedHexTiles.Count);
                    }
                }
            }
        }

        // OnGui
        if (Input.IsActionJustPressed("ui_home"))
        {
            foreach (HexTile tile in _selectedHexTiles)
            {
                // GD.Print(tile.Height);
                tile.SetHeight(tile.Height + 1);
            }
        }

        if (Input.IsActionJustPressed("ui_end"))
        {
            foreach (HexTile tile in _selectedHexTiles)
                tile.SetHeight(tile.Height - 1);
        }
    }

    // void OnGUI()
    // {
    //     // Make a background box
    //
    //     // // Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
    //     // if(GUI.Button(new Rect(20,40,80,20), "Level 1")) {
    //     //     Application.LoadLevel(1);
    //     // }
    //
    //     // // Make the second button.
    //     // if(GUI.Button(new Rect(20,70,80,20), "Level 2")) {
    //     //     Application.LoadLevel(2);
    //     // }
    //
    //     GUI.Label(new Rect(25, 5, 200, 50),
    //         "Selected: " + _selectedHexTiles.Count + " tile" + (_selectedHexTiles.Count == 1 ? "" : "s"));
    //     float newTargetGUIHeight = GUI.HorizontalSlider(new Rect(25, 25, 100, 15), _targetGUIHeight, -2.0f, 15.0f);
    //     if (newTargetGUIHeight != _targetGUIHeight)
    //     {
    //         foreach (HexTile tile in _selectedHexTiles)
    //         {
    //             tile.SetHeight(newTargetGUIHeight);
    //         }
    //     }
    //
    //     _targetGUIHeight = newTargetGUIHeight;
    //
    //     if (GUI.Button(new Rect(25, 45, 30, 20), "+"))
    //     {
    //         foreach (HexTile tile in _selectedHexTiles)
    //         {
    //             tile.SetHeight(tile.Height + 1);
    //         }
    //     }
    //
    //     if (GUI.Button(new Rect(60, 45, 30, 20), "-"))
    //     {
    //         foreach (HexTile tile in _selectedHexTiles)
    //         {
    //             tile.SetHeight(tile.Height - 1);
    //         }
    //     }
    // }

    private void ResetCursors()
    {
        foreach (var cursor in _cursors)
        {
            cursor.QueueFree();
        }

        foreach (HexTile tile in _selectedHexTiles)
        {
            var newCursor = new CsgSphere3D();
            AddChild(newCursor);
            newCursor.Position = tile.Center + (tile.Center.Normalized() * tile.Height);
            _cursors.Add(newCursor);
        }
    }
}