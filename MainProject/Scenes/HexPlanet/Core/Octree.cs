using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ZeromaXsPlaygroundProject.Scenes.HexPlanet.Core;

public class Octree<K>
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
    private Octree<K> _bblOctree = null;
    private Octree<K> _fblOctree = null;
    private Octree<K> _btlOctree = null;
    private Octree<K> _ftlOctree = null;
    private Octree<K> _bbrOctree = null;
    private Octree<K> _fbrOctree = null;
    private Octree<K> _btrOctree = null;
    private Octree<K> _ftrOctree = null;

    public Octree(Vector3 backBottomLeft, Vector3 frontUpperRight, int depth) : this(backBottomLeft, frontUpperRight)
    {
        _depth = depth;
    }

    public Octree(Vector3 backBottomLeft, Vector3 frontUpperRight)
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
        _bblOctree = new Octree<K>(_backBottomLeft, _center, _depth + 1);
        _fblOctree = new Octree<K>(_backBottomLeft + (Vector3.Back * (_size.Z / 2)),
            _center + (Vector3.Back * (_size.Z / 2)), _depth + 1);
        _btlOctree = new Octree<K>(_backBottomLeft + (Vector3.Up * (_size.Y / 2)),
            _center + (Vector3.Up * (_size.Y / 2)), _depth + 1);
        _ftlOctree = new Octree<K>(_backBottomLeft + (Vector3.Up * (_size.Y / 2)) + (Vector3.Back * (_size.Z / 2)),
            _center + (Vector3.Up * (_size.Y / 2)) + (Vector3.Back * (_size.Z / 2)), _depth + 1);
        _bbrOctree = new Octree<K>(_backBottomLeft + (Vector3.Right * (_size.X / 2)),
            _center + (Vector3.Right * (_size.X / 2)), _depth + 1);
        _fbrOctree =
            new Octree<K>(_backBottomLeft + (Vector3.Back * (_size.Z / 2)) + (Vector3.Right * (_size.X / 2)),
                _center + (Vector3.Back * (_size.Z / 2)) + (Vector3.Right * (_size.X / 2)), _depth + 1);
        _btrOctree = new Octree<K>(_backBottomLeft + (Vector3.Up * (_size.Y / 2)) + (Vector3.Right * (_size.X / 2)),
            _center + (Vector3.Up * (_size.Y / 2)) + (Vector3.Right * (_size.X / 2)), _depth + 1);
        _ftrOctree = new Octree<K>(
            _backBottomLeft + (Vector3.Up * (_size.Y / 2)) + (Vector3.Back * (_size.Z / 2)) +
            (Vector3.Right * (_size.X / 2)),
            _center + (Vector3.Up * (_size.Y / 2)) + (Vector3.Back * (_size.Z / 2)) +
            (Vector3.Right * (_size.X / 2)), _depth + 1);

        foreach (var kvp in _points)
            InsertPointInternally(kvp.Key, kvp.Value);

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
                    _ftrOctree.InsertPoint(key, pos);
                }
                else
                {
                    // Back
                    // 这里 Unity 和 Godot 是反的， Godot 的 Front，Unity 的 Back
                    _btrOctree.InsertPoint(key, pos);
                }
            }
            else
            {
                // Bottom
                if (pos.Z > _center.Z)
                {
                    // Front
                    // 这里 Unity 和 Godot 是反的， Godot 的 Back，Unity 的 Front
                    _fbrOctree.InsertPoint(key, pos);
                }
                else
                {
                    // Back
                    // 这里 Unity 和 Godot 是反的， Godot 的 Front，Unity 的 Back
                    _bbrOctree.InsertPoint(key, pos);
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
                    _ftlOctree.InsertPoint(key, pos);
                }
                else
                {
                    // Back
                    // 这里 Unity 和 Godot 是反的， Godot 的 Front，Unity 的 Back
                    _btlOctree.InsertPoint(key, pos);
                }
            }
            else
            {
                // Bottom
                if (pos.Z > _center.Z)
                {
                    // Front
                    // 这里 Unity 和 Godot 是反的， Godot 的 Back，Unity 的 Front
                    _fblOctree.InsertPoint(key, pos);
                }
                else
                {
                    // Back
                    // 这里 Unity 和 Godot 是反的， Godot 的 Front，Unity 的 Back
                    _bblOctree.InsertPoint(key, pos);
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
            Split();
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

        ret.AddRange(_bblOctree.GetPoints(center, size));
        ret.AddRange(_fblOctree.GetPoints(center, size));
        ret.AddRange(_btlOctree.GetPoints(center, size));
        ret.AddRange(_ftlOctree.GetPoints(center, size));
        ret.AddRange(_bbrOctree.GetPoints(center, size));
        ret.AddRange(_fbrOctree.GetPoints(center, size));
        ret.AddRange(_btrOctree.GetPoints(center, size));
        ret.AddRange(_ftrOctree.GetPoints(center, size));

        return ret;
    }

    private bool BoxIntersectsBox(Vector3 boxACenter, Vector3 boxASize, Vector3 boxBCenter, Vector3 boxBSize) =>
        ((boxACenter.X - boxASize.X <= boxBCenter.X + boxBSize.X) &&
         (boxACenter.X + boxASize.X >= boxBCenter.X - boxBSize.X)) &&
        ((boxACenter.Y - boxASize.Y <= boxBCenter.Y + boxBSize.Y) &&
         (boxACenter.Y + boxASize.Y >= boxBCenter.Y - boxBSize.Y)) &&
        ((boxACenter.Z - boxASize.Z <= boxBCenter.Z + boxBSize.Z) &&
         (boxACenter.Z + boxASize.Z >= boxBCenter.Z - boxBSize.Z));

    private bool PointWithinBox(Vector3 boxCenter, Vector3 boxSize, Vector3 point) =>
        (point.X <= boxCenter.X + boxSize.X) && (point.X >= boxCenter.X - boxSize.X) &&
        (point.Y <= boxCenter.Y + boxSize.Y) && (point.Y >= boxCenter.Y - boxSize.Y) &&
        (point.Z <= boxCenter.Z + boxSize.Z) && (point.Z >= boxCenter.Z - boxSize.Z);
}