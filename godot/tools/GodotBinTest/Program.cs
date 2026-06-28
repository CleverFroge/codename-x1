using System.Reflection;
using System.Runtime.Loader;

var dir = @"C:\Users\15778\Projects\codename-x1\godot\.godot\mono\temp\bin\Debug";
AssemblyLoadContext.Default.Resolving += (_, name) =>
{
	var path = Path.Combine(dir, name.Name + ".dll");
	return File.Exists(path) ? AssemblyLoadContext.Default.LoadFromAssemblyPath(path) : null;
};

try
{
	var asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(dir, "CodenameX1.dll"));
	var wgType = asm.GetType("CodenameX1.World.WorldGenerator")!;
	var wg = Activator.CreateInstance(wgType)!;
	var gen = wgType.GetMethod("Generate")!;
	var worldSize = asm.GetType("CodenameX1.World.WorldSize")!;
	var world = gen.Invoke(wg, new object[] { Enum.ToObject(worldSize, 1), 42 })!;
	Console.WriteLine("OK " + world.GetType().GetProperty("MaxTilesX")!.GetValue(world));
}
catch (Exception ex)
{
	Console.WriteLine("FAILED: " + ex);
	if (ex.InnerException != null) Console.WriteLine("INNER: " + ex.InnerException);
	Environment.Exit(1);
}
