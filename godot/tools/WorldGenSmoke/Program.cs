using CodenameX1.World;

const int w = 4200;
const int h = 1200;
const int seed = 42;

Console.WriteLine($"Terraria vanilla WorldGen {w}x{h} seed={seed} ...");
var sw = System.Diagnostics.Stopwatch.StartNew();
Action<string, float> progress = (msg, r) => Console.Write($"\r{msg,-40} {r * 100,5:F0}%");

WorldState world;
try
{
	world = TerrariaWorldExporter.Generate(w, h, seed, progress);
}
catch (Exception ex)
{
	Console.WriteLine();
	Console.WriteLine("FAILED: " + ex);
	return 1;
}

sw.Stop();
Console.WriteLine();
int solid = 0, liquid = 0;
for (int x = 0; x < world.MaxTilesX; x++)
for (int y = 0; y < world.MaxTilesY; y++)
{
	if (world.Tile(x, y).Active) solid++;
	if (world.Tile(x, y).Liquid > 0) liquid++;
}
Console.WriteLine($"OK in {sw.ElapsedMilliseconds}ms surface={world.WorldSurface} rock={world.RockLayer} solid={solid} liquid={liquid}");
return 0;
