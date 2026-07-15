namespace CodenameX1.World;

public readonly record struct ChunkCoord(int X, int Y);

public static class WorldCoordinates
{
	public static ChunkCoord TileToChunk(int tileX, int tileY) =>
		new(FloorDivide(tileX, WorldConfig.ChunkTileSize), FloorDivide(tileY, WorldConfig.ChunkTileSize));

	private static int FloorDivide(int value, int divisor) =>
		value >= 0 ? value / divisor : (value - divisor + 1) / divisor;
}
