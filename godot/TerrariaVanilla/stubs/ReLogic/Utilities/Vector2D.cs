using System;

namespace ReLogic.Utilities;

/// <summary>ReLogic stub for Terraria world-gen port (Vector2D API used by WorldGen.TileRunner).</summary>
public struct Vector2D
{
	public double X;
	public double Y;

	public static Vector2D Zero => default;

	public Vector2D(double x, double y)
	{
		X = x;
		Y = y;
	}

	public readonly double Length() => Math.Sqrt(X * X + Y * Y);

	public static double Distance(Vector2D a, Vector2D b)
	{
		double dx = a.X - b.X;
		double dy = a.Y - b.Y;
		return Math.Sqrt(dx * dx + dy * dy);
	}

	public static Vector2D operator +(Vector2D a, Vector2D b) =>
		new(a.X + b.X, a.Y + b.Y);

	public static Vector2D operator -(Vector2D a, Vector2D b) =>
		new(a.X - b.X, a.Y - b.Y);

	public static Vector2D operator *(Vector2D v, double s) =>
		new(v.X * s, v.Y * s);
}
