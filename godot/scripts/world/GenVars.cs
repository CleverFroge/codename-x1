namespace CodenameX1.World;

public sealed class GenVars
{
	public int LeftBeachEnd;
	public int RightBeachStart;

	public double WorldSurface;
	public double WorldSurfaceHigh;
	public double WorldSurfaceLow;

	public double RockLayer;
	public double RockLayerHigh;
	public double RockLayerLow;

	public int WaterLine;
	public int LavaLine;

	public int BeachBordersWidth = 275;
	public int BeachSandRandomCenter;
	public int BeachSandRandomWidthRange = 20;
	public int BeachSandDungeonExtraWidth = 40;
	public int BeachSandJungleExtraWidth = 20;
	public int EvilBiomeBeachAvoidance;
	public int SmallHolesBeachAvoidance;
	public int SkyLakes = 1;

	public bool Crimson;
	public bool CrimsonLeft;
	public int DungeonSide; // 0 left, 1 right
	public int JungleOriginX;

	public int Copper = TileIds.Copper;
	public int Iron = TileIds.Iron;
	public int Silver = TileIds.Silver;
	public int Gold = TileIds.Gold;
	public int EvilStone = TileIds.Ebonstone;

	public int MaxTunnels = 30;
	public int NumTunnels;
	public int[] TunnelX = new int[30];

	public int MaxMCaves = 30;
	public int NumMCaves;
	public int[] MCaveX = new int[30];
	public int[] MCaveY = new int[30];

	public int NumIslandHouses;
	public int[] FloatingIslandHouseX = new int[20];
	public int[] FloatingIslandHouseY = new int[20];

	public void ScaleForWorld(int maxTilesX)
	{
		double scale = maxTilesX / 4200.0;
		BeachBordersWidth = Math.Max(40, (int)(275 * scale));
		BeachSandRandomCenter = BeachBordersWidth + 45;
		EvilBiomeBeachAvoidance = BeachSandRandomCenter + 60;
		SmallHolesBeachAvoidance = BeachSandRandomCenter + 20;
		if (maxTilesX > 6000) SkyLakes++;
		if (maxTilesX > 8000) SkyLakes++;
	}
}
