namespace CodenameX1.World;

/// <summary>PassEditor / 世界生成后端：管线为项目自研；Pass 来源分原生与泰拉参考。</summary>
public enum WorldGenBackend
{
	/// <summary>项目原生 Pass（管线已接入，Pass 列表待实现）。</summary>
	Native,
	/// <summary>泰拉瑞亚 GenPass 实现（经 TerrariaPassCatalog 注册）。</summary>
	Terraria,
}

/// <summary>跨场景传递当前选择的生成后端。</summary>
public static class WorldGenSession
{
	public static WorldGenBackend Backend { get; set; } = WorldGenBackend.Terraria;
}
