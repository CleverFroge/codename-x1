using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content.Readers;

namespace Terraria.Testing;

public sealed class FxReader : IAssetReader
{
	public FxReader(GraphicsDevice graphicsDevice)
	{
	}

	public T FromStream<T>(Stream stream) where T : class
	{
		throw new NotSupportedException("FxReader is disabled in CODENAME_X1_PORT.");
	}
}
