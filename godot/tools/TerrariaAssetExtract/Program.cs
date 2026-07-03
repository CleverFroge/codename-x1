using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content.Readers;

namespace CodenameX1.Tools;

/// <summary>
/// Extract Terraria Content/*.xnb to PNG/WAV.
/// Uses ReLogic XnbReader + MonoGame (handles LZX compression internally).
/// </summary>
internal sealed class Program : Game
{
	private static string _contentDir = "";
	private static string _outputDir = "";
	private static bool _verbose;
	private static int _limit;
	private static int _png, _wav, _skip, _err;

	private readonly GraphicsDeviceManager _graphics;
	private XnbReader _reader = null!;

	static int Main(string[] args)
	{
		ParseArgs(args);
		if (string.IsNullOrEmpty(_contentDir) || !Directory.Exists(_contentDir))
		{
			Console.Error.WriteLine("Terraria Content/ not found. Use: --input <path>");
			return 1;
		}

		Directory.CreateDirectory(_outputDir);
		Console.WriteLine("Content: " + _contentDir);
		Console.WriteLine("Output:  " + Path.GetFullPath(_outputDir));
		if (_limit > 0) Console.WriteLine("Limit:   " + _limit);
		Console.WriteLine();

		using var game = new Program();
		game.Run();
		return _err == 0 ? 0 : 1;
	}

	private Program()
	{
		_graphics = new GraphicsDeviceManager(this);
		_graphics.PreferredBackBufferWidth = 8;
		_graphics.PreferredBackBufferHeight = 8;
		IsMouseVisible = false;
	}

	protected override void LoadContent()
	{
		_reader = new XnbReader(Services);

		var xnbFiles = Directory.EnumerateFiles(_contentDir, "*.xnb", SearchOption.AllDirectories).ToList();
		if (_limit > 0) xnbFiles = xnbFiles.Take(_limit).ToList();

		foreach (var xnbPath in xnbFiles)
		{
			var rel = Path.GetRelativePath(_contentDir, xnbPath);
			var relStem = Path.ChangeExtension(rel, null)!;
			if (_verbose) Console.Write("  " + rel + " ... ");

			try
			{
				var raw = File.ReadAllBytes(xnbPath);

				if (TryExportTexture(raw, relStem))
				{
					_png++;
					if (_verbose) Console.WriteLine("PNG");
					continue;
				}

				if (TryExportSound(raw, relStem))
				{
					_wav++;
					if (_verbose) Console.WriteLine("WAV");
					continue;
				}

				_skip++;
				if (_verbose) Console.WriteLine("skip");
			}
			catch (Exception ex)
			{
				_err++;
				if (_verbose) Console.WriteLine("ERR: " + ex.Message);
			}
		}

		Console.WriteLine();
		Console.WriteLine("Done: {0} PNG, {1} WAV, {2} skipped, {3} errors", _png, _wav, _skip, _err);
		Console.WriteLine("Saved to: " + Path.GetFullPath(_outputDir));
		Exit();
	}

	private bool TryExportTexture(byte[] raw, string relStem)
	{
		using var stream = new MemoryStream(raw);
		Texture2D tex;
		try { tex = _reader.FromStream<Texture2D>(stream); }
		catch { return false; }

		var outPath = Path.Combine(_outputDir, relStem) + ".png";
		Directory.CreateDirectory(Path.GetDirectoryName(outPath)!);
		using (var fs = File.Create(outPath))
			tex.SaveAsPng(fs, tex.Width, tex.Height);
		tex.Dispose();
		return true;
	}

	private static bool TryExportSound(byte[] raw, string relStem)
	{
		var wav = XnbSoundExporter.TryExtractWav(raw);
		if (wav == null) return false;

		var outPath = Path.Combine(_outputDir, relStem) + ".wav";
		Directory.CreateDirectory(Path.GetDirectoryName(outPath)!);
		File.WriteAllBytes(outPath, wav);
		return true;
	}

	private static void ParseArgs(string[] args)
	{
		_outputDir = Path.Combine(Environment.CurrentDirectory, "terraria_assets");
		for (int i = 0; i < args.Length; i++)
		{
			switch (args[i])
			{
				case "-i":
				case "--input":
					_contentDir = args[++i];
					break;
				case "-o":
				case "--output":
					_outputDir = args[++i];
					break;
				case "-v":
				case "--verbose":
					_verbose = true;
					break;
				case "--limit":
					_limit = int.Parse(args[++i]);
					break;
			}
		}
		if (string.IsNullOrEmpty(_contentDir))
			_contentDir = FindContentDir() ?? "";
	}

	private static string? FindContentDir()
	{
		foreach (var c in new[]
		{
			@"D:\Program Files\Steam\steamapps\common\Terraria\Content",
			@"D:\Program Files (x86)\Steam\steamapps\common\Terraria\Content",
			@"C:\Program Files\Steam\steamapps\common\Terraria\Content",
			@"C:\Program Files (x86)\Steam\steamapps\common\Terraria\Content",
		})
			if (Directory.Exists(c)) return c;
		return null;
	}
}

static class XnbSoundExporter
{
	private static readonly Type? LzxStreamType =
		typeof(Microsoft.Xna.Framework.Content.ContentManager).Assembly
			.GetType("Microsoft.Xna.Framework.Content.LzxDecoderStream")
		?? typeof(Microsoft.Xna.Framework.Content.ContentManager).Assembly
			.GetType("MonoGame.Framework.Utilities.LzxDecoderStream");

	public static byte[]? TryExtractWav(byte[] data)
	{
		if (data.Length < 10 || data[0] != 'X') return null;

		byte flags = data[5];
		bool compressed = (flags & 0x80) != 0;
		uint xnbLength = BitConverter.ToUInt32(data, 6);

		byte[] body;
		if (compressed)
		{
			if (LzxStreamType == null) return null;
			uint decompSize = BitConverter.ToUInt32(data, 10);
			int compSize = (int)xnbLength - 14;
			using var input = new MemoryStream(data, 14, compSize, writable: false);
			var decoder = (Stream)Activator.CreateInstance(LzxStreamType, input, (int)decompSize, compSize)!;
			using var ms = new MemoryStream((int)decompSize);
			decoder.CopyTo(ms);
			body = ms.ToArray();
		}
		else
		{
			body = new byte[data.Length - 10];
			Array.Copy(data, 10, body, 0, body.Length);
		}

		return ParseSoundEffectBody(body);
	}

	private static byte[]? ParseSoundEffectBody(byte[] payload)
	{
		int pos = 0;
		int numReaders = Read7Bit(payload, ref pos);
		var readers = new List<string>();
		for (int i = 0; i < numReaders; i++)
		{
			int nameLen = Read7Bit(payload, ref pos);
			readers.Add(Encoding.UTF8.GetString(payload, pos, nameLen));
			pos += nameLen + 4;
		}

		int readerIdx = Read7Bit(payload, ref pos);
		if (readerIdx < 0 || readerIdx >= readers.Count) return null;
		if (!readers[readerIdx].Contains("SoundEffect", StringComparison.OrdinalIgnoreCase))
			return null;

		pos += 4; // num_bytes
		uint sampleRate = BitConverter.ToUInt32(payload, pos); pos += 4;
		pos += 4; // avg bps
		pos += 4; // block align
		ushort bits = BitConverter.ToUInt16(payload, pos); pos += 2;
		ushort channels = BitConverter.ToUInt16(payload, pos); pos += 2;
		uint audioSize = BitConverter.ToUInt32(payload, pos); pos += 4;
		var pcm = new byte[audioSize];
		Array.Copy(payload, pos, pcm, 0, (int)audioSize);
		return WrapWav(pcm, sampleRate, channels, bits);
	}

	private static byte[] WrapWav(byte[] pcm, uint sampleRate, ushort channels, ushort bits)
	{
		uint byteRate = sampleRate * channels * (uint)(bits / 8);
		ushort blockAlign = (ushort)(channels * (bits / 8));
		using var ms = new MemoryStream();
		var bw = new BinaryWriter(ms);
		bw.Write(Encoding.ASCII.GetBytes("RIFF"));
		bw.Write(36 + pcm.Length);
		bw.Write(Encoding.ASCII.GetBytes("WAVE"));
		bw.Write(Encoding.ASCII.GetBytes("fmt "));
		bw.Write(16);
		bw.Write((short)1);
		bw.Write((short)channels);
		bw.Write((int)sampleRate);
		bw.Write((int)byteRate);
		bw.Write(blockAlign);
		bw.Write(bits);
		bw.Write(Encoding.ASCII.GetBytes("data"));
		bw.Write(pcm.Length);
		bw.Write(pcm);
		return ms.ToArray();
	}

	private static int Read7Bit(byte[] buf, ref int pos)
	{
		int val = 0, shift = 0;
		while (true)
		{
			byte b = buf[pos++];
			val |= (b & 0x7F) << shift;
			shift += 7;
			if ((b & 0x80) == 0) break;
		}
		return val;
	}
}
