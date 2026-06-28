namespace CodenameX1.World;

public sealed class GenRandom
{
	private readonly Random _rng;

	public GenRandom(int seed) => _rng = new Random(seed);

	public int Next(int maxExclusive) => _rng.Next(maxExclusive);

	public int Next(int minInclusive, int maxExclusive) => _rng.Next(minInclusive, maxExclusive);

	public double NextDouble() => _rng.NextDouble();
}
