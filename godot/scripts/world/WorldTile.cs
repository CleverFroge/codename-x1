namespace CodenameX1.World;

public sealed class WorldTile
{
	public bool Active;
	public int Type = TileIds.Dirt;
	public byte Liquid;
	public bool Lava;

	public void Clear()
	{
		Active = false;
		Type = TileIds.Dirt;
		Liquid = 0;
		Lava = false;
	}
}
