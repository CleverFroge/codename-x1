namespace Terraria;

public struct ShoppingSettings
{
	public float PriceAdjustment;

	public string HappinessReport;

	public static ShoppingSettings NotInShop
	{
		get
		{
			ShoppingSettings result = default(ShoppingSettings);
			result.PriceAdjustment = 1f;
			result.HappinessReport = "";
			return result;
		}
	}
}
