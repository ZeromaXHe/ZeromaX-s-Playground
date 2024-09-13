using Godot;
using Microsoft.EntityFrameworkCore;

namespace ZeromaXPlayground.game.inGame.map.scripts.dao;

public class DatabaseContext: DbContext
{
    public string DbFilePath { get; }
    
    public DatabaseContext()
    {
        // user:// 默认路径 Windows：%APPDATA%\Godot\app_userdata\[项目名称]
        var dbDirPath = ProjectSettings.GlobalizePath("user://database/");
        GD.Print($"DbDirPath: {dbDirPath}");
        DbFilePath = System.IO.Path.Join(dbDirPath, "map.db");
        GD.Print($"DBPath: {DbFilePath}");
    }

    // 下面配置 EF 来创建一个 SQLite 数据库文件
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        GD.Print($"DatabaseContext OnConfiguring");
        optionsBuilder.UseSqlite($"Data Source={DbFilePath}");
    }
}