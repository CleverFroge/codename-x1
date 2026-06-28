using System.Threading.Tasks;
using CodenameX1.World;

try {
    WorldState? w = null;
    Task.Run(() => {
        var gen = new WorldGenerator();
        w = gen.Generate(WorldSize.HalfSmall, 42);
    }).Wait();
    Console.WriteLine($"OK {w!.MaxTilesX}x{w.MaxTilesY}");
} catch (Exception ex) {
    Console.WriteLine("FAILED: " + ex);
    for (var inner = ex.InnerException; inner != null; inner = inner.InnerException)
        Console.WriteLine("  INNER: " + inner);
    return 1;
}
return 0;
