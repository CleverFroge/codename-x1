using Microsoft.Xna.Framework;

namespace Terraria.GameContent;

public class TeleportHelpers
{
	public static bool FindClosestTeleportSpotNoSpace(Player player, out Vector2 resultPosition)
	{
		bool result = false;
		resultPosition = player.position;
		player.velocity = Vector2.Zero;
		Vector2 vector = new Vector2((float)player.width * 0.5f, player.height);
		Vector2 bottom = player.Bottom;
		Point point = bottom.ToTileCoordinates();
		int value = point.X - 25;
		int value2 = point.X + 25;
		int value3 = point.Y - 25;
		int value4 = point.Y + 25;
		value = Utils.Clamp(value, 40, Main.maxTilesX - 40);
		value2 = Utils.Clamp(value2, 40, Main.maxTilesX - 40);
		value3 = Utils.Clamp(value3, 40, Main.maxTilesY - 40);
		value4 = Utils.Clamp(value4, 40, Main.maxTilesY - 40);
		float num = float.MaxValue;
		for (int i = value; i < value2; i++)
		{
			for (int j = value3; j < value4; j++)
			{
				Vector2 vector2 = new Vector2(i * 16 + 8, j * 16 + 15) - vector;
				Tile tile = Main.tile[i, j];
				Tile tile2 = Main.tile[i, j + 1];
				bool flag = WorldGen.SolidOrSlopedTile(tile) || tile.liquid > 0;
				bool flag2 = WorldGen.SolidOrSlopedTile(tile2) && tile2.liquid == 0;
				if (!TileIsDangerous(i, j) && !flag && flag2 && !Collision.LavaCollision(vector2, player.width, player.height) && !Collision.AnyHurtingTiles(vector2, player.width, player.height) && !Collision.SolidCollision(vector2, player.width, player.height))
				{
					float num2 = (vector2 - bottom).Length();
					if (num2 < num)
					{
						resultPosition = vector2;
						num = num2;
						result = true;
					}
				}
			}
		}
		return result;
	}

	public static bool RequestMagicConchTeleportPosition(Player player, int crawlOffsetX, bool rightOcean, out Point landingPoint)
	{
		landingPoint = default(Point);
		int num = 50;
		int num2 = 50;
		int num3 = 0;
		if (WorldGen.Skyblock.lowTiles)
		{
			num2 = 100;
			num3 = 50;
		}
		int x = (rightOcean ? (Main.maxTilesX - num) : num);
		int y = num2;
		int num4 = (int)Main.worldSurface - num3;
		Point point = new Point(x, y);
		int num5 = 1;
		int num6 = -1;
		int num7 = 1;
		int num8 = 0;
		int num9 = 5000;
		Vector2 vector = new Vector2((float)player.width * 0.5f, player.height);
		int num10 = 40;
		bool flag = WorldGen.SolidOrSlopedTile(Main.tile[point.X, point.Y]);
		int num11 = 0;
		int num12 = 400;
		if (WorldGen.Skyblock.lowTiles)
		{
			num9 = num12 * ((int)Main.worldSurface - 10);
		}
		while (num8 < num9 && num11 < num12)
		{
			num8++;
			Tile tile = Main.tile[point.X, point.Y];
			Tile tile2 = Main.tile[point.X, point.Y + num7];
			bool flag2 = WorldGen.SolidOrSlopedTile(tile) || tile.liquid > 0;
			bool flag3 = WorldGen.SolidOrSlopedTile(tile2) || tile2.liquid > 0;
			if (IsInSolidTilesExtended(new Vector2(point.X * 16 + 8, point.Y * 16 + 15) - vector, player.velocity, player.width, player.height, (int)player.gravDir))
			{
				if (flag)
				{
					point.Y += num5;
				}
				else
				{
					point.Y += num6;
				}
				continue;
			}
			if (flag2)
			{
				if (flag)
				{
					point.Y += num5;
				}
				else
				{
					point.Y += num6;
				}
				continue;
			}
			flag = false;
			if (!IsInSolidTilesExtended(new Vector2(point.X * 16 + 8, point.Y * 16 + 15 + 16) - vector, player.velocity, player.width, player.height, (int)player.gravDir) && !flag3 && (double)point.Y < Main.worldSurface)
			{
				point.Y += num5;
				if (WorldGen.Skyblock.lowTiles && point.Y >= num4)
				{
					point.Y = y;
					point.X += crawlOffsetX;
					num11++;
				}
			}
			else if (tile2.liquid > 0)
			{
				point.X += crawlOffsetX;
				num11++;
			}
			else if (TileIsDangerous(point.X, point.Y))
			{
				point.X += crawlOffsetX;
				num11++;
			}
			else if (TileIsDangerous(point.X, point.Y + num7))
			{
				point.X += crawlOffsetX;
				num11++;
			}
			else
			{
				if (point.Y >= num10)
				{
					break;
				}
				point.Y += num5;
			}
		}
		if (num8 == num9 || num11 >= num12)
		{
			return false;
		}
		if (!WorldGen.InWorld(point.X, point.Y, 40))
		{
			return false;
		}
		bool flag4 = false;
		for (int i = 0; i < 10; i++)
		{
			int num13 = point.Y + i;
			Tile tile3 = Main.tile[point.X, num13];
			if (WorldGen.SolidOrSlopedTile(tile3) || tile3.liquid > 0)
			{
				flag4 = true;
				break;
			}
		}
		if (WorldGen.Skyblock.lowTiles)
		{
			if (!flag4)
			{
				for (int j = 0; j < 10; j++)
				{
					int num14 = point.Y + j;
					Tile tile4 = Main.tile[point.X - 1, num14];
					if (WorldGen.SolidOrSlopedTile(tile4) || tile4.liquid > 0)
					{
						point.X--;
						flag4 = true;
						break;
					}
				}
			}
			if (!flag4)
			{
				for (int k = 0; k < 10; k++)
				{
					int num15 = point.Y + k;
					Tile tile5 = Main.tile[point.X + 1, num15];
					if (WorldGen.SolidOrSlopedTile(tile5) || tile5.liquid > 0)
					{
						point.X++;
						flag4 = true;
						break;
					}
				}
			}
		}
		if (!flag4)
		{
			return false;
		}
		landingPoint = point;
		return true;
	}

	private static bool TileIsDangerous(int x, int y)
	{
		Tile tile = Main.tile[x, y];
		if (tile.liquid > 0 && tile.lava())
		{
			return true;
		}
		if (tile.wall == 87 && (double)y > Main.worldSurface && !NPC.downedPlantBoss)
		{
			return true;
		}
		if (Main.wallDungeon[tile.wall] && (double)y > Main.worldSurface && !NPC.downedBoss3)
		{
			return true;
		}
		return false;
	}

	private static bool IsInSolidTilesExtended(Vector2 testPosition, Vector2 playerVelocity, int width, int height, int gravDir)
	{
		if (Collision.LavaCollision(testPosition, width, height))
		{
			return true;
		}
		if (Collision.AnyHurtingTiles(testPosition, width, height))
		{
			return true;
		}
		if (Collision.SolidCollision(testPosition, width, height))
		{
			return true;
		}
		Vector2 vector = Vector2.UnitX * 16f;
		if (Collision.TileCollision(testPosition - vector, vector, width, height, fallThrough: true, fall2: true, gravDir) != vector)
		{
			return true;
		}
		vector = -Vector2.UnitX * 16f;
		if (Collision.TileCollision(testPosition - vector, vector, width, height, fallThrough: true, fall2: true, gravDir) != vector)
		{
			return true;
		}
		vector = Vector2.UnitY * 16f;
		if (Collision.TileCollision(testPosition - vector, vector, width, height, fallThrough: true, fall2: true, gravDir) != vector)
		{
			return true;
		}
		vector = -Vector2.UnitY * 16f;
		if (Collision.TileCollision(testPosition - vector, vector, width, height, fallThrough: true, fall2: true, gravDir) != vector)
		{
			return true;
		}
		return false;
	}
}
