namespace CodenameX1.World;

public struct WorldTile
{
	public bool Active;
	public int Type;
	public byte Liquid;
	public bool Lava;
	public bool Honey;

	public void Clear()
	{
		Active = false;
		Type = TileIds.Dirt;
		Liquid = 0;
		Lava = false;
		Honey = false;
	}
}
