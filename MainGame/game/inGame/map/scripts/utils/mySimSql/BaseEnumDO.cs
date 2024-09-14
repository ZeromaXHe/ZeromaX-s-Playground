namespace ZeromaXPlayground.game.inGame.map.scripts.utils.mySimSql;

public class BaseEnumDO : BaseDataObj
{
    /**
     * 类型枚举键名
     */
    public string EnumName { get; set; }
    /**
     * 类型枚举值
     */
    public int EnumVal { get; set; }

    /**
     * 视图层名字
     */
    public string ViewName { get; set; }
}