using System.Numerics;

namespace BackEnd4IdleStrategy.Game.UserInterface.Adapter;

public interface INavigationService
{
    void AddPoint(int id, Vector2 vec);
    void ConnectPoints(int fromId, int toId);
    long[] GetPointConnections(int id);
}