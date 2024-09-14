namespace ZeromaXPlayground.game.inGame.map.scripts.utils.mySimSql;

public class IdGenerator
{
    private int _id = 0;

    public int Next()
    {
        return ++_id;
    }

    public void Reset()
    {
        _id = 0;
    }
}