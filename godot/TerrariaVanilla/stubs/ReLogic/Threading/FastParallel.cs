namespace ReLogic.Threading;

// Stub: world gen does not require async threading from ReLogic.
public static class FastParallel
{
	public static void For(int fromInclusive, int toExclusive, Action<int> body)
	{
		for (int i = fromInclusive; i < toExclusive; i++)
			body(i);
	}
}
