using System;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Graphics;

public static class GraphicsDeviceXnaCompat
{
	public static VertexBufferBinding[] GetVertexBuffers(this GraphicsDevice device)
	{
		return Array.Empty<VertexBufferBinding>();
	}
}
