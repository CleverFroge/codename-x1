using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Terraria.Port;

/// <summary>
/// Godot dynamic loading uses a custom ALC; Newtonsoft.Json cannot resolve System.Security.Permissions on its own.
/// </summary>
public static class RuntimeDependencyLoader
{
	private static bool _registered;
	private static bool _resolving;

	public static void EnsureRegistered()
	{
		if (_registered)
		{
			return;
		}

		_registered = true;
		AssemblyLoadContext.Default.Resolving += (_, name) => TryLoad(name.Name);
		TryLoad("System.Security.Permissions");
	}

	private static Assembly? TryLoad(string? simpleName)
	{
		if (string.IsNullOrEmpty(simpleName) || _resolving)
		{
			return null;
		}

		Assembly? existing = AppDomain.CurrentDomain.GetAssemblies()
			.FirstOrDefault(a => string.Equals(a.GetName().Name, simpleName, StringComparison.OrdinalIgnoreCase));
		if (existing != null)
		{
			return existing;
		}

		_resolving = true;
		try
		{
			string fileName = simpleName + ".dll";
			foreach (string dir in GetProbeDirectories())
			{
				string path = Path.Combine(dir, fileName);
				if (!File.Exists(path))
				{
					continue;
				}

				Assembly? loaded = LoadFromPath(path);
				if (loaded != null)
				{
					return loaded;
				}
			}

			return LoadEmbedded(simpleName);
		}
		finally
		{
			_resolving = false;
		}
	}

	private static Assembly? LoadFromPath(string path)
	{
		try
		{
			return AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
		}
		catch
		{
			try
			{
				return Assembly.LoadFrom(path);
			}
			catch
			{
				return null;
			}
		}
	}

	private static Assembly? LoadEmbedded(string simpleName)
	{
		if (simpleName != "System.Security.Permissions")
		{
			return null;
		}

		using Stream? stream = typeof(RuntimeDependencyLoader).Assembly
			.GetManifestResourceStream("Terraria.Port.Deps.System.Security.Permissions.dll");
		if (stream == null)
		{
			return null;
		}

		using MemoryStream buffer = new MemoryStream();
		stream.CopyTo(buffer);
		return Assembly.Load(buffer.ToArray());
	}

	private static IEnumerable<string> GetProbeDirectories()
	{
		var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		var dirs = new List<string>();

		void Add(string? dir)
		{
			if (string.IsNullOrWhiteSpace(dir))
			{
				return;
			}

			dir = Path.GetFullPath(dir);
			if (seen.Add(dir))
			{
				dirs.Add(dir);
			}
		}

		Add(AppContext.BaseDirectory);
		Add(Environment.CurrentDirectory);

		string cwd = Environment.CurrentDirectory;
		Add(Path.Combine(cwd, ".godot", "mono", "temp", "bin", "Debug"));
		Add(Path.Combine(cwd, ".godot", "mono", "temp", "bin", "Release"));

		foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
		{
			if (!string.IsNullOrEmpty(asm.Location))
			{
				Add(Path.GetDirectoryName(asm.Location));
			}
		}

		return dirs;
	}
}
