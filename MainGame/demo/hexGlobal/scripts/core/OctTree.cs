using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ZeromaXPlayground.demo.hexGlobal.scripts.core;

public class OctTree<K>
{
    public const int MaxDepth = 5;
    public const int MaxPointsPerLeaf = 6;

    private readonly Vector3 _backBottomLeft;
    private Vector3 _frontUpperRight;
    private readonly Vector3 _center;
    private readonly Vector3 _size;
    private readonly Dictionary<K, Vector3> _points;
    private readonly int _depth = 0;
    private bool _split = false;

    // 现在命名是按 Unity 版的叫法，实际 Godot 里面 front 和 back 和命名是反的
    private OctTree<K> _bblOctTree = null;
    private OctTree<K> _fblOctTree = null;
    private OctTree<K> _btlOctTree = null;
    private OctTree<K> _ftlOctTree = null;
    private OctTree<K> _bbrOctTree = null;
    private OctTree<K> _fbrOctTree = null;
    private OctTree<K> _btrOctTree = null;
    private OctTree<K> _ftrOctTree = null;

    /**
     * TODO：暂时还不能单元测试？后续研究一下 NuGet，得开始弄单元测试了
     */
    public static void Main(string[] args)
    {
        OctTree<Vector3> vertOctTree = new OctTree<Vector3>(Vector3.One * (100 * -1.1f),
            Vector3.One * (100 * 1.1f));
        List<Vector3> uniqueVerts = new List<Vector3>
        {
            new(52.57311f, 0, 85.06508f),
            new(0, 85.06508f, 52.57311f),
            new(-52.57311f, 0, 85.06508f),
            new(-85.06508f, 52.57311f, 0f),
            new(0, 85.06508f, -52.57311f),
            new(85.06508f, 52.57311f, 0f),
            new(85.06508f, -52.57311f, 0f),
            new(52.57311f, 0, -85.06508f),
            new(-52.57311f, 0, -85.06508f),
            new(0, -85.06508f, -52.57311f),
            new(0, -85.06508f, 52.57311f),
            new(-85.06508f, -52.57311f, 0f),
            new(30.901697f, 50, 80.9017f),
            new(-30.901697f, 50, 80.9017f),
            new(0, 0, 100f),
            new(-50, 80.9017f, 30.901697f),
            new(-80.9017f, 30.901697f, 50f),
            new(0, 100, 0f),
            new(-50, 80.9017f, -30.901697f),
            new(50, 80.9017f, -30.901697f),
            new(50, 80.9017f, 30.901697f),
            new(80.9017f, 30.901697f, 50f),
            new(80.9017f, -30.901697f, 50f),
            new(100, 0, 0f),
            new(80.9017f, -30.901697f, -50f),
            new(80.9017f, 30.901697f, -50f),
            new(30.901697f, 50, -80.9017f),
            new(0, 0, -100f),
            new(-30.901697f, 50, -80.9017f),
            new(30.901697f, -50, -80.9017f),
            new(-30.901697f, -50, -80.9017f),
            new(50, -80.9017f, -30.901697f),
            new(50, -80.9017f, 30.901697f),
            new(0, -100, 0f),
            new(-50, -80.9017f, 30.901697f),
            new(-50, -80.9017f, -30.901697f),
            new(-30.901697f, -50, 80.9017f),
            new(-80.9017f, -30.901697f, 50f),
            new(30.901697f, -50, 80.9017f),
            new(-100, 0, 0f),
            new(-80.9017f, -30.901697f, -50f),
            new(-80.9017f, 30.901697f, -50f),
        };
        foreach (Vector3 v in uniqueVerts)
        {
            vertOctTree.InsertPoint(v, v);
        }

        float maxDistBetweenNeighbots = Mathf.Sqrt((from vert in uniqueVerts
            orderby (vert - uniqueVerts[^1]).LengthSquared()
            select (vert - uniqueVerts[^1]).LengthSquared()).Take(7).ToList()[6]) * 1.2f;

        for (int i = 0; i < uniqueVerts.Count; i++)
        {
            Vector3 uniqueVert = uniqueVerts[i];
            List<Vector3> closestVerts = vertOctTree.GetPoints(uniqueVert, Vector3.One * maxDistBetweenNeighbots);
            Console.Out.WriteLine("closestVerts: " + closestVerts);
        }
    }

    public OctTree(Vector3 backBottomLeft, Vector3 frontUpperRight, int depth) : this(backBottomLeft, frontUpperRight)
    {
        this._depth = depth;
    }

    public OctTree(Vector3 backBottomLeft, Vector3 frontUpperRight)
    {
        _frontUpperRight = frontUpperRight;
        _backBottomLeft = backBottomLeft;
        _center = (frontUpperRight + backBottomLeft) / 2.0f;
        _size = frontUpperRight - backBottomLeft;

        _points = new Dictionary<K, Vector3>();
    }

    private void Split()
    {
        _split = true;

        // 现在命名是按 Unity 版的叫法，实际 Godot 里面 front 和 back 和命名是反的
        _bblOctTree = new OctTree<K>(_backBottomLeft, _center, _depth + 1);
        _fblOctTree = new OctTree<K>(_backBottomLeft + (Vector3.Back * (_size.Z / 2)),
            _center + (Vector3.Back * (_size.Z / 2)), _depth + 1);
        _btlOctTree = new OctTree<K>(_backBottomLeft + (Vector3.Up * (_size.Y / 2)),
            _center + (Vector3.Up * (_size.Y / 2)), _depth + 1);
        _ftlOctTree = new OctTree<K>(_backBottomLeft + (Vector3.Up * (_size.Y / 2)) + (Vector3.Back * (_size.Z / 2)),
            _center + (Vector3.Up * (_size.Y / 2)) + (Vector3.Back * (_size.Z / 2)), _depth + 1);
        _bbrOctTree = new OctTree<K>(_backBottomLeft + (Vector3.Right * (_size.X / 2)),
            _center + (Vector3.Right * (_size.X / 2)), _depth + 1);
        _fbrOctTree =
            new OctTree<K>(_backBottomLeft + (Vector3.Back * (_size.Z / 2)) + (Vector3.Right * (_size.X / 2)),
                _center + (Vector3.Back * (_size.Z / 2)) + (Vector3.Right * (_size.X / 2)), _depth + 1);
        _btrOctTree = new OctTree<K>(_backBottomLeft + (Vector3.Up * (_size.Y / 2)) + (Vector3.Right * (_size.X / 2)),
            _center + (Vector3.Up * (_size.Y / 2)) + (Vector3.Right * (_size.X / 2)), _depth + 1);
        _ftrOctTree = new OctTree<K>(
            _backBottomLeft + (Vector3.Up * (_size.Y / 2)) + (Vector3.Back * (_size.Z / 2)) +
            (Vector3.Right * (_size.X / 2)),
            _center + (Vector3.Up * (_size.Y / 2)) + (Vector3.Back * (_size.Z / 2)) +
            (Vector3.Right * (_size.X / 2)), _depth + 1);

        foreach (KeyValuePair<K, Vector3> kvp in _points)
        {
            InsertPointInternally(kvp.Key, kvp.Value);
        }

        _points.Clear();
    }

    private void InsertPointInternally(K key, Vector3 pos)
    {
        if (pos.X > _center.X)
        {
            // Right
            if (pos.Y > _center.Y)
            {
                // Top
                if (pos.Z > _center.Z)
                {
                    // Front
                    // 这里 Unity 和 Godot 是反的， Godot 的 Back，Unity 的 Front
                    _ftrOctTree.InsertPoint(key, pos);
                }
                else
                {
                    // Back
                    // 这里 Unity 和 Godot 是反的， Godot 的 Front，Unity 的 Back
                    _btrOctTree.InsertPoint(key, pos);
                }
            }
            else
            {
                // Bottom
                if (pos.Z > _center.Z)
                {
                    // Front
                    // 这里 Unity 和 Godot 是反的， Godot 的 Back，Unity 的 Front
                    _fbrOctTree.InsertPoint(key, pos);
                }
                else
                {
                    // Back
                    // 这里 Unity 和 Godot 是反的， Godot 的 Front，Unity 的 Back
                    _bbrOctTree.InsertPoint(key, pos);
                }
            }
        }
        else
        {
            // Left
            if (pos.Y > _center.Y)
            {
                // Top
                if (pos.Z > _center.Z)
                {
                    // Front
                    // 这里 Unity 和 Godot 是反的， Godot 的 Back，Unity 的 Front
                    _ftlOctTree.InsertPoint(key, pos);
                }
                else
                {
                    // Back
                    // 这里 Unity 和 Godot 是反的， Godot 的 Front，Unity 的 Back
                    _btlOctTree.InsertPoint(key, pos);
                }
            }
            else
            {
                // Bottom
                if (pos.Z > _center.Z)
                {
                    // Front
                    // 这里 Unity 和 Godot 是反的， Godot 的 Back，Unity 的 Front
                    _fblOctTree.InsertPoint(key, pos);
                }
                else
                {
                    // Back
                    // 这里 Unity 和 Godot 是反的， Godot 的 Front，Unity 的 Back
                    _bblOctTree.InsertPoint(key, pos);
                }
            }
        }
    }

    public void InsertPoint(K key, Vector3 pos)
    {
        if (!_split && _points.Count < MaxPointsPerLeaf)
        {
            _points.Add(key, pos);
            return;
        }

        if (!_split && _depth >= MaxDepth)
        {
            // Can't split anymore, adding to list
            _points.Add(key, pos);
            return;
        }

        // Split
        if (!_split)
        {
            Split();
        }

        InsertPointInternally(key, pos);
    }

    public List<K> GetPoints(Vector3 center, Vector3 size)
    {
        List<K> ret = new List<K>();

        if (!BoxIntersectsBox(center, size, _center, _size))
        {
            return ret;
        }

        if (!_split)
        {
            foreach (KeyValuePair<K, Vector3> kvp in _points)
            {
                if (PointWithinBox(center, size, kvp.Value))
                {
                    ret.Add(kvp.Key);
                }
            }

            return ret;
        }

        ret.AddRange(_bblOctTree.GetPoints(center, size));
        ret.AddRange(_fblOctTree.GetPoints(center, size));
        ret.AddRange(_btlOctTree.GetPoints(center, size));
        ret.AddRange(_ftlOctTree.GetPoints(center, size));
        ret.AddRange(_bbrOctTree.GetPoints(center, size));
        ret.AddRange(_fbrOctTree.GetPoints(center, size));
        ret.AddRange(_btrOctTree.GetPoints(center, size));
        ret.AddRange(_ftrOctTree.GetPoints(center, size));

        return ret;
    }

    private bool BoxIntersectsBox(Vector3 boxACenter, Vector3 boxASize, Vector3 boxBCenter, Vector3 boxBSize)
    {
        return ((boxACenter.X - boxASize.X <= boxBCenter.X + boxBSize.X) &&
                (boxACenter.X + boxASize.X >= boxBCenter.X - boxBSize.X)) &&
               ((boxACenter.Y - boxASize.Y <= boxBCenter.Y + boxBSize.Y) &&
                (boxACenter.Y + boxASize.Y >= boxBCenter.Y - boxBSize.Y)) &&
               ((boxACenter.Z - boxASize.Z <= boxBCenter.Z + boxBSize.Z) &&
                (boxACenter.Z + boxASize.Z >= boxBCenter.Z - boxBSize.Z));
    }

    private bool PointWithinBox(Vector3 boxCenter, Vector3 boxSize, Vector3 point)
    {
        return (point.X <= boxCenter.X + boxSize.X) && (point.X >= boxCenter.X - boxSize.X) &&
               (point.Y <= boxCenter.Y + boxSize.Y) && (point.Y >= boxCenter.Y - boxSize.Y) &&
               (point.Z <= boxCenter.Z + boxSize.Z) && (point.Z >= boxCenter.Z - boxSize.Z);
    }
}