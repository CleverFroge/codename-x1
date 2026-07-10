namespace CodenameX1.World;

/// <summary>PassEditor / 世界生成使用的后端逻辑。</summary>
public enum WorldGenBackend
{
	/// <summary>项目原生生成（Pass 管线尚未接入）。</summary>
	Native,
	/// <summary>泰拉瑞亚原版 WorldGen 参考。</summary>
	Terraria,
}

/// <summary>跨场景传递当前选择的生成后端。</summary>
public static class WorldGenSession
{
	public static WorldGenBackend Backend { get; set; } = WorldGenBackend.Terraria;
}
