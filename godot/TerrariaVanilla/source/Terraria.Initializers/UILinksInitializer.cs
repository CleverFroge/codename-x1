using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Tile_Entities;
using Terraria.GameContent.UI;
using Terraria.GameContent.UI.States;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.Social;
using Terraria.UI;
using Terraria.UI.Gamepad;

namespace Terraria.Initializers;

public class UILinksInitializer
{
	public class SomeVarsForUILinkers
	{
		public static Recipe SequencedCraftingCurrent;

		public static int HairMoveCD;
	}

	private static List<string> RightStickGlyphBinding = new List<string> { "RightStickAxis" };

	public static int MainfocusRecipe
	{
		get
		{
			return Main.focusRecipe;
		}
		set
		{
			Main.focusRecipe = value;
		}
	}

	public static int MainFocusBanner
	{
		get
		{
			return Main.focusRecipe;
		}
		set
		{
			Main.focusRecipe = value;
		}
	}

	public static int MainnumAvailableRecipes
	{
		get
		{
			return Main.numAvailableRecipes;
		}
		set
		{
			Main.numAvailableRecipes = value;
		}
	}

	public static int MainnumAvailableRecipes2
	{
		get
		{
			return Main.numAvailableRecipes;
		}
		set
		{
			Main.numAvailableRecipes = value;
		}
	}

	public static bool NothingMoreImportantThanNPCChat()
	{
		if (!Main.hairWindow && Main.npcShop == 0)
		{
			return Main.player[Main.myPlayer].chest == -1;
		}
		return false;
	}

	public static float HandleSliderHorizontalInput(float currentValue, float min, float max, float deadZone = 0.2f, float sensitivity = 0.5f)
	{
		float x = PlayerInput.GamepadThumbstickLeft.X;
		x = ((!(x < 0f - deadZone) && !(x > deadZone)) ? 0f : (MathHelper.Lerp(0f, sensitivity / 60f, (Math.Abs(x) - deadZone) / (1f - deadZone)) * (float)Math.Sign(x)));
		return MathHelper.Clamp((currentValue - min) / (max - min) + x, 0f, 1f) * (max - min) + min;
	}

	public static float HandleSliderVerticalInput(float currentValue, float min, float max, float deadZone = 0.2f, float sensitivity = 0.5f)
	{
		float num = 0f - PlayerInput.GamepadThumbstickLeft.Y;
		num = ((!(num < 0f - deadZone) && !(num > deadZone)) ? 0f : (MathHelper.Lerp(0f, sensitivity / 60f, (Math.Abs(num) - deadZone) / (1f - deadZone)) * (float)Math.Sign(num)));
		return MathHelper.Clamp((currentValue - min) / (max - min) + num, 0f, 1f) * (max - min) + min;
	}

	public static bool CanExecuteInputCommand()
	{
		return PlayerInput.AllowExecutionOfGamepadInstructions;
	}

	public static void Load()
	{
		Func<string> value = () => PlayerInput.BuildCommand(Lang.misc[53].Value, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
		UILinkPage uILinkPage = new UILinkPage();
		uILinkPage.UpdateEvent += delegate
		{
			PlayerInput.GamepadAllowScrolling = true;
		};
		for (int i = 0; i < 20; i++)
		{
			uILinkPage.LinkMap.Add(2000 + i, new UILinkPoint(2000 + i, enabled: true, -3, -4, -1, -2));
		}
		uILinkPage.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[53].Value, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]) + PlayerInput.BuildCommand(Lang.misc[82].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]);
		uILinkPage.UpdateEvent += delegate
		{
			bool flag11 = PlayerInput.Triggers.JustPressed.Inventory;
			if (Main.inputTextEscape)
			{
				Main.inputTextEscape = false;
				flag11 = true;
			}
			if (CanExecuteInputCommand() && flag11)
			{
				FancyExit();
			}
			UILinkPointNavigator.Shortcuts.BackButtonInUse = flag11;
			HandleOptionsSpecials();
		};
		uILinkPage.IsValidEvent += () => Main.gameMenu && !Main.MenuUI.IsVisible;
		uILinkPage.CanEnterEvent += () => Main.gameMenu && !Main.MenuUI.IsVisible;
		UILinkPointNavigator.RegisterPage(uILinkPage, 1000);
		UILinkPage cp22 = new UILinkPage();
		cp22.LinkMap.Add(2500, new UILinkPoint(2500, enabled: true, -3, -4, -1, -2));
		cp22.LinkMap.Add(2501, new UILinkPoint(2501, enabled: true, -3, -4, -1, -2));
		cp22.LinkMap.Add(2502, new UILinkPoint(2502, enabled: true, -3, -4, -1, -2));
		cp22.LinkMap.Add(2503, new UILinkPoint(2503, enabled: true, -3, -4, -1, -2));
		cp22.LinkMap.Add(2504, new UILinkPoint(2504, enabled: true, -3, -4, -1, -2));
		cp22.LinkMap.Add(2505, new UILinkPoint(2505, enabled: true, -3, -4, -1, -2));
		cp22.LinkMap.Add(2506, new UILinkPoint(2506, enabled: true, -3, -4, -1, -2));
		cp22.LinkMap.Add(2507, new UILinkPoint(2507, enabled: true, -3, -4, -1, -2));
		cp22.LinkMap.Add(2508, new UILinkPoint(2508, enabled: true, -3, -4, -1, -2));
		cp22.LinkMap.Add(2509, new UILinkPoint(2509, enabled: true, -3, -4, -1, -2));
		cp22.LinkMap.Add(2510, new UILinkPoint(2510, enabled: true, -3, -4, -1, -2));
		cp22.LinkMap.Add(2511, new UILinkPoint(2511, enabled: true, -3, -4, -1, -2));
		cp22.UpdateEvent += delegate
		{
			if (UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsNew)
			{
				for (int num69 = 0; num69 < UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsCount; num69++)
				{
					if (num69 - 4 >= 0)
					{
						cp22.LinkMap[2500 + num69].Up = 2500 + num69 - 4;
					}
					else
					{
						cp22.LinkMap[2500 + num69].Up = -1;
					}
					if (num69 + 4 < UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsCount)
					{
						cp22.LinkMap[2500 + num69].Down = 2500 + num69 + 4;
					}
					else
					{
						cp22.LinkMap[2500 + num69].Down = -2;
					}
					cp22.LinkMap[2500 + num69].Left = ((num69 > 0) ? (2500 + num69 - 1) : (-3));
					cp22.LinkMap[2500 + num69].Right = ((num69 < UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsCount - 1) ? (2500 + num69 + 1) : (-4));
				}
			}
			else
			{
				cp22.LinkMap[2501].Right = (UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight ? 2502 : (-4));
				if (cp22.LinkMap[2501].Right == -4 && UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight2)
				{
					cp22.LinkMap[2501].Right = 2503;
				}
				cp22.LinkMap[2502].Right = (UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight2 ? 2503 : (-4));
				cp22.LinkMap[2503].Left = (UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight ? 2502 : 2501);
			}
		};
		cp22.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[53].Value, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]) + PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]);
		cp22.IsValidEvent += () => (Main.player[Main.myPlayer].talkNPC != -1 || Main.player[Main.myPlayer].sign != -1) && NothingMoreImportantThanNPCChat();
		cp22.CanEnterEvent += () => (Main.player[Main.myPlayer].talkNPC != -1 || Main.player[Main.myPlayer].sign != -1) && NothingMoreImportantThanNPCChat();
		cp22.EnterEvent += delegate
		{
			Main.player[Main.myPlayer].releaseInventory = false;
		};
		cp22.LeaveEvent += delegate
		{
			Main.npcChatRelease = false;
			Main.player[Main.myPlayer].LockGamepadTileInteractions();
		};
		UILinkPointNavigator.RegisterPage(cp22, 1003);
		UILinkPage cp21 = new UILinkPage();
		cp21.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value2 = delegate
		{
			int currentPoint5 = UILinkPointNavigator.CurrentPoint;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].inventory, 0, currentPoint5);
		};
		Func<string> value3 = () => ItemSlot.GetGamepadInstructions(ref Main.player[Main.myPlayer].trashItem, 6);
		for (int j = 0; j <= 49; j++)
		{
			UILinkPoint uILinkPoint = new UILinkPoint(j, enabled: true, j - 1, j + 1, j - 10, j + 10);
			uILinkPoint.OnSpecialInteracts += value2;
			int num = j;
			if (num < 10)
			{
				uILinkPoint.Up = -1;
			}
			if (num >= 40)
			{
				uILinkPoint.Down = -2;
			}
			if (num % 10 == 9)
			{
				uILinkPoint.Right = -4;
			}
			if (num % 10 == 0)
			{
				uILinkPoint.Left = -3;
			}
			cp21.LinkMap.Add(j, uILinkPoint);
		}
		cp21.LinkMap[9].Right = 0;
		cp21.LinkMap[19].Right = 50;
		cp21.LinkMap[29].Right = 51;
		cp21.LinkMap[39].Right = 52;
		cp21.LinkMap[49].Right = 53;
		cp21.LinkMap[0].Left = 9;
		cp21.LinkMap[10].Left = 54;
		cp21.LinkMap[20].Left = 55;
		cp21.LinkMap[30].Left = 56;
		cp21.LinkMap[40].Left = 57;
		cp21.LinkMap.Add(300, new UILinkPoint(300, enabled: true, 309, 310, 49, -2));
		cp21.LinkMap.Add(309, new UILinkPoint(309, enabled: true, 310, 300, 302, 54));
		cp21.LinkMap.Add(310, new UILinkPoint(310, enabled: true, 300, 309, 301, 50));
		cp21.LinkMap.Add(301, new UILinkPoint(301, enabled: true, 300, 302, 53, 310));
		cp21.LinkMap.Add(302, new UILinkPoint(302, enabled: true, 301, 300, 57, 309));
		cp21.LinkMap.Add(311, new UILinkPoint(311, enabled: true, -3, -4, 40, -2));
		cp21.LinkMap[301].OnSpecialInteracts += value;
		cp21.LinkMap[302].OnSpecialInteracts += value;
		cp21.LinkMap[309].OnSpecialInteracts += value;
		cp21.LinkMap[310].OnSpecialInteracts += value;
		cp21.LinkMap[300].OnSpecialInteracts += value3;
		cp21.UpdateEvent += delegate
		{
			bool inReforgeMenu = Main.InReforgeMenu;
			bool flag7 = Main.LocalPlayer.chest != -1;
			bool flag8 = Main.npcShop != 0;
			TileEntity tileEntity = Main.LocalPlayer.tileEntityAnchor.GetTileEntity();
			bool flag9 = tileEntity is TEHatRack;
			bool flag10 = tileEntity is TEDisplayDoll;
			if (NewCraftingUI.Visible)
			{
				flag7 = false;
			}
			for (int num66 = 40; num66 <= 49; num66++)
			{
				if (inReforgeMenu)
				{
					cp21.LinkMap[num66].Down = ((num66 < 45) ? 303 : 304);
				}
				else if (flag7)
				{
					cp21.LinkMap[num66].Down = 400 + num66 - 40;
				}
				else if (flag8)
				{
					cp21.LinkMap[num66].Down = 2700 + num66 - 40;
				}
				else if (num66 == 40 && Main.IsJourneyMode && !Main.CreativeMenu.Blocked)
				{
					cp21.LinkMap[num66].Down = 311;
				}
				else if (!NewCraftingUI.Visible)
				{
					cp21.LinkMap[num66].Down = -2;
				}
			}
			if (flag10)
			{
				for (int num67 = 41; num67 <= 48; num67++)
				{
					cp21.LinkMap[num67].Down = 5100 + (int)Math.Round((double)((num67 - 40) * 10) / 9.0) - 1;
				}
				cp21.LinkMap[40].Down = 5118;
			}
			if (flag9)
			{
				for (int num68 = 44; num68 <= 45; num68++)
				{
					cp21.LinkMap[num68].Down = 5000 + num68 - 44;
				}
			}
			if (NewCraftingUI.Visible && Main.LocalPlayer.chest != -1)
			{
				cp21.LinkMap[49].Down = 300;
				cp21.LinkMap[300].Up = 49;
				cp21.LinkMap[300].Right = 310;
				cp21.LinkMap[310].Up = 53;
				cp21.LinkMap[309].Up = 57;
			}
			else if (flag7)
			{
				cp21.LinkMap[300].Up = 439;
				cp21.LinkMap[300].Right = 310;
				cp21.LinkMap[300].Left = 309;
				cp21.LinkMap[310].Up = ((Main.LocalPlayer.chest < -1) ? 505 : 504);
				cp21.LinkMap[309].Up = ((Main.LocalPlayer.chest < -1) ? 505 : 504);
			}
			else if (flag8)
			{
				cp21.LinkMap[300].Up = 2739;
				cp21.LinkMap[300].Right = 310;
				cp21.LinkMap[300].Left = 309;
				cp21.LinkMap[310].Up = 53;
				cp21.LinkMap[309].Up = 57;
			}
			else
			{
				cp21.LinkMap[49].Down = 300;
				cp21.LinkMap[300].Up = 49;
				cp21.LinkMap[300].Right = 301;
				if (!NewCraftingUI.Visible)
				{
					cp21.LinkMap[300].Left = 302;
				}
				cp21.LinkMap[309].Up = 302;
				cp21.LinkMap[310].Up = 301;
			}
			if (!NewCraftingUI.Visible)
			{
				cp21.LinkMap[311].Right = -1;
				cp21.LinkMap[311].Down = -1;
				cp21.LinkMap[300].Down = -1;
			}
			cp21.LinkMap[0].Left = 9;
			cp21.LinkMap[10].Left = 54;
			cp21.LinkMap[20].Left = 55;
			cp21.LinkMap[30].Left = 56;
			cp21.LinkMap[40].Left = 57;
			if (UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 0)
			{
				cp21.LinkMap[0].Left = 6000;
			}
			if (UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 2)
			{
				cp21.LinkMap[10].Left = 6002;
			}
			if (UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 4)
			{
				cp21.LinkMap[20].Left = 6004;
			}
			if (UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 6)
			{
				cp21.LinkMap[30].Left = 6006;
			}
			if (UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 8)
			{
				cp21.LinkMap[40].Left = 6008;
			}
			cp21.PageOnLeft = 9;
			if (Main.InPipBanner)
			{
				cp21.PageOnLeft = 22;
			}
			if (Main.CreativeMenu.Enabled)
			{
				cp21.PageOnLeft = 1005;
			}
			if (NewCraftingUI.Visible)
			{
				cp21.PageOnLeft = 24;
			}
			if (Main.InReforgeMenu)
			{
				cp21.PageOnLeft = 5;
			}
			if (flag10)
			{
				cp21.PageOnLeft = 20;
			}
			if (flag9)
			{
				cp21.PageOnLeft = 21;
			}
		};
		cp21.IsValidEvent += () => Main.playerInventory;
		cp21.PageOnLeft = 9;
		cp21.PageOnRight = 2;
		UILinkPointNavigator.RegisterPage(cp21, 0);
		UILinkPage cp20 = new UILinkPage();
		cp20.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value4 = delegate
		{
			int currentPoint4 = UILinkPointNavigator.CurrentPoint;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].inventory, 1, currentPoint4);
		};
		for (int k = 50; k <= 53; k++)
		{
			UILinkPoint uILinkPoint2 = new UILinkPoint(k, enabled: true, -3, -4, k - 1, k + 1);
			uILinkPoint2.OnSpecialInteracts += value4;
			cp20.LinkMap.Add(k, uILinkPoint2);
		}
		cp20.LinkMap[50].Left = 19;
		cp20.LinkMap[51].Left = 29;
		cp20.LinkMap[52].Left = 39;
		cp20.LinkMap[53].Left = 49;
		cp20.LinkMap[50].Right = 54;
		cp20.LinkMap[51].Right = 55;
		cp20.LinkMap[52].Right = 56;
		cp20.LinkMap[53].Right = 57;
		cp20.LinkMap[50].Up = 310;
		cp20.UpdateEvent += delegate
		{
			if (Main.npcShop != 0)
			{
				cp20.LinkMap[53].Down = 310;
			}
			else if (Main.player[Main.myPlayer].chest != -1)
			{
				cp20.LinkMap[53].Down = (NewCraftingUI.Visible ? 310 : 500);
			}
			else
			{
				cp20.LinkMap[53].Down = 301;
			}
		};
		cp20.IsValidEvent += () => Main.playerInventory;
		cp20.PageOnLeft = 0;
		cp20.PageOnRight = 2;
		UILinkPointNavigator.RegisterPage(cp20, 1);
		UILinkPage cp19 = new UILinkPage();
		cp19.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value5 = delegate
		{
			int currentPoint3 = UILinkPointNavigator.CurrentPoint;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].inventory, 2, currentPoint3);
		};
		for (int l = 54; l <= 57; l++)
		{
			UILinkPoint uILinkPoint3 = new UILinkPoint(l, enabled: true, -3, -4, l - 1, l + 1);
			uILinkPoint3.OnSpecialInteracts += value5;
			cp19.LinkMap.Add(l, uILinkPoint3);
		}
		cp19.LinkMap[54].Left = 50;
		cp19.LinkMap[55].Left = 51;
		cp19.LinkMap[56].Left = 52;
		cp19.LinkMap[57].Left = 53;
		cp19.LinkMap[54].Right = 10;
		cp19.LinkMap[55].Right = 20;
		cp19.LinkMap[56].Right = 30;
		cp19.LinkMap[57].Right = 40;
		cp19.LinkMap[54].Up = 309;
		cp19.UpdateEvent += delegate
		{
			if (Main.npcShop != 0)
			{
				cp19.LinkMap[57].Down = 309;
			}
			else if (Main.player[Main.myPlayer].chest != -1)
			{
				cp19.LinkMap[57].Down = (NewCraftingUI.Visible ? 310 : 500);
			}
			else
			{
				cp19.LinkMap[57].Down = 302;
			}
		};
		cp19.PageOnLeft = 0;
		cp19.PageOnRight = 8;
		UILinkPointNavigator.RegisterPage(cp19, 2);
		UILinkPage cp18 = new UILinkPage();
		cp18.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value6 = delegate
		{
			int num65 = UILinkPointNavigator.CurrentPoint - 100;
			if (num65 % 10 == 8 && !Main.LocalPlayer.CanDemonHeartAccessoryBeShown())
			{
				num65++;
			}
			bool flag6 = num65 >= 10;
			int context2 = ((num65 % 10 >= 3) ? (flag6 ? 11 : 10) : (flag6 ? 9 : 8));
			return ItemSlot.GetGamepadInstructions(Main.LocalPlayer.armor, context2, num65);
		};
		Func<string> value7 = delegate
		{
			int num64 = UILinkPointNavigator.CurrentPoint - 120;
			if (num64 % 10 == 8 && !Main.LocalPlayer.CanDemonHeartAccessoryBeShown())
			{
				num64++;
			}
			return ItemSlot.GetGamepadInstructions(Main.LocalPlayer.dye, 12, num64);
		};
		for (int m = 100; m <= 119; m++)
		{
			UILinkPoint uILinkPoint4 = new UILinkPoint(m, enabled: true, m + 10, m - 10, m - 1, m + 1);
			uILinkPoint4.OnSpecialInteracts += value6;
			int num2 = m - 100;
			if (num2 == 0)
			{
				uILinkPoint4.Up = 305;
			}
			if (num2 == 10)
			{
				uILinkPoint4.Up = 306;
			}
			if (num2 == 9 || num2 == 19)
			{
				uILinkPoint4.Down = -2;
			}
			if (num2 >= 10)
			{
				uILinkPoint4.Left = 120 + num2 % 10;
			}
			else if (num2 >= 3)
			{
				uILinkPoint4.Right = -4;
			}
			else
			{
				uILinkPoint4.Right = 312 + num2;
			}
			cp18.LinkMap.Add(m, uILinkPoint4);
		}
		for (int n = 120; n <= 129; n++)
		{
			UILinkPoint uILinkPoint4 = new UILinkPoint(n, enabled: true, -3, -10 + n, n - 1, n + 1);
			uILinkPoint4.OnSpecialInteracts += value7;
			int num3 = n - 120;
			if (num3 == 0)
			{
				uILinkPoint4.Up = 307;
			}
			if (num3 == 9)
			{
				uILinkPoint4.Down = 308;
				uILinkPoint4.Left = 1557;
			}
			if (num3 == 8)
			{
				uILinkPoint4.Left = 1570;
			}
			cp18.LinkMap.Add(n, uILinkPoint4);
		}
		for (int num4 = 312; num4 <= 314; num4++)
		{
			int num5 = num4 - 312;
			UILinkPoint uILinkPoint4 = new UILinkPoint(num4, enabled: true, 100 + num5, -4, num4 - 1, num4 + 1);
			if (num5 == 0)
			{
				uILinkPoint4.Up = -1;
			}
			if (num5 == 2)
			{
				uILinkPoint4.Down = -2;
			}
			uILinkPoint4.OnSpecialInteracts += value;
			cp18.LinkMap.Add(num4, uILinkPoint4);
		}
		cp18.IsValidEvent += () => Main.playerInventory && Main.EquipPage == 0;
		cp18.UpdateEvent += delegate
		{
			int num60 = 107;
			int amountOfExtraAccessorySlotsToShow = Main.player[Main.myPlayer].GetAmountOfExtraAccessorySlotsToShow();
			for (int num61 = 0; num61 < amountOfExtraAccessorySlotsToShow; num61++)
			{
				cp18.LinkMap[num60 + num61].Down = num60 + num61 + 1;
				cp18.LinkMap[num60 - 100 + 120 + num61].Down = num60 - 100 + 120 + num61 + 1;
				cp18.LinkMap[num60 + 10 + num61].Down = num60 + 10 + num61 + 1;
			}
			cp18.LinkMap[num60 + amountOfExtraAccessorySlotsToShow].Down = 308;
			cp18.LinkMap[num60 + 10 + amountOfExtraAccessorySlotsToShow].Down = 308;
			cp18.LinkMap[num60 - 100 + 120 + amountOfExtraAccessorySlotsToShow].Down = 308;
			for (int num62 = 120; num62 <= 129; num62++)
			{
				UILinkPoint uILinkPoint16 = cp18.LinkMap[num62];
				int num63 = num62 - 120;
				uILinkPoint16.Left = -3;
				if (num63 == 0)
				{
					uILinkPoint16.Left = (Main.ShouldPVPDraw ? 1550 : (-3));
				}
				if (num63 == 1)
				{
					uILinkPoint16.Left = (Main.ShouldTeamSelectDraw ? 1552 : (-3));
				}
				if (num63 == 2)
				{
					uILinkPoint16.Left = (Main.ShouldTeamSelectDraw ? 1556 : (-3));
				}
				if (num63 == 3)
				{
					uILinkPoint16.Left = ((UILinkPointNavigator.Shortcuts.INFOACCCOUNT >= 1) ? 1558 : (-3));
				}
				if (num63 == 4)
				{
					uILinkPoint16.Left = ((UILinkPointNavigator.Shortcuts.INFOACCCOUNT >= 5) ? 1562 : (-3));
				}
				if (num63 == 5)
				{
					uILinkPoint16.Left = ((UILinkPointNavigator.Shortcuts.INFOACCCOUNT >= 9) ? 1566 : (-3));
				}
			}
			cp18.LinkMap[num60 - 100 + 120 + amountOfExtraAccessorySlotsToShow].Left = 1557;
			cp18.LinkMap[num60 - 100 + 120 + amountOfExtraAccessorySlotsToShow - 1].Left = 1570;
		};
		cp18.PageOnLeft = 8;
		cp18.PageOnRight = 8;
		UILinkPointNavigator.RegisterPage(cp18, 3);
		UILinkPage cp17 = new UILinkPage();
		cp17.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value8 = delegate
		{
			int slot10 = UILinkPointNavigator.CurrentPoint - 400;
			int context = 4;
			Item[] item = Main.player[Main.myPlayer].bank.item;
			switch (Main.player[Main.myPlayer].chest)
			{
			case -1:
				return "";
			case -3:
				item = Main.player[Main.myPlayer].bank2.item;
				break;
			case -4:
				item = Main.player[Main.myPlayer].bank3.item;
				break;
			case -5:
				item = Main.player[Main.myPlayer].bank4.item;
				context = 32;
				break;
			default:
				item = Main.chest[Main.player[Main.myPlayer].chest].item;
				context = 3;
				break;
			case -2:
				break;
			}
			return ItemSlot.GetGamepadInstructions(item, context, slot10);
		};
		for (int num6 = 400; num6 <= 439; num6++)
		{
			UILinkPoint uILinkPoint5 = new UILinkPoint(num6, enabled: true, num6 - 1, num6 + 1, num6 - 10, num6 + 10);
			uILinkPoint5.OnSpecialInteracts += value8;
			int num7 = num6 - 400;
			if (num7 < 10)
			{
				uILinkPoint5.Up = 40 + num7;
			}
			if (num7 >= 30)
			{
				uILinkPoint5.Down = -2;
			}
			if (num7 % 10 == 9)
			{
				uILinkPoint5.Right = -4;
			}
			if (num7 % 10 == 0)
			{
				uILinkPoint5.Left = -3;
			}
			cp17.LinkMap.Add(num6, uILinkPoint5);
		}
		cp17.LinkMap.Add(500, new UILinkPoint(500, enabled: true, 409, -4, 53, 501));
		cp17.LinkMap.Add(501, new UILinkPoint(501, enabled: true, 419, -4, 500, 502));
		cp17.LinkMap.Add(502, new UILinkPoint(502, enabled: true, 429, -4, 501, 503));
		cp17.LinkMap.Add(503, new UILinkPoint(503, enabled: true, 439, -4, 502, 505));
		cp17.LinkMap.Add(505, new UILinkPoint(505, enabled: true, 439, -4, 503, 504));
		cp17.LinkMap.Add(504, new UILinkPoint(504, enabled: true, 439, -4, 505, 310));
		cp17.LinkMap[500].OnSpecialInteracts += value;
		cp17.LinkMap[501].OnSpecialInteracts += value;
		cp17.LinkMap[502].OnSpecialInteracts += value;
		cp17.LinkMap[503].OnSpecialInteracts += value;
		cp17.LinkMap[504].OnSpecialInteracts += value;
		cp17.LinkMap[505].OnSpecialInteracts += value;
		cp17.LinkMap[409].Right = 500;
		cp17.LinkMap[419].Right = 501;
		cp17.LinkMap[429].Right = 502;
		cp17.LinkMap[439].Right = 503;
		cp17.LinkMap[439].Down = 300;
		cp17.PageOnLeft = 0;
		cp17.PageOnRight = 0;
		cp17.DefaultPoint = 400;
		cp17.UpdateEvent += delegate
		{
			if (Main.LocalPlayer.chest < -1)
			{
				cp17.LinkMap[505].Down = 310;
			}
			else
			{
				cp17.LinkMap[505].Down = 504;
			}
		};
		UILinkPointNavigator.RegisterPage(cp17, 4, automatedDefault: false);
		cp17.IsValidEvent += () => Main.playerInventory && Main.player[Main.myPlayer].chest != -1 && !NewCraftingUI.Visible;
		UILinkPage uILinkPage2 = new UILinkPage();
		uILinkPage2.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value9 = delegate
		{
			int slot9 = UILinkPointNavigator.CurrentPoint - 5100;
			return (!(Main.LocalPlayer.tileEntityAnchor.GetTileEntity() is TEDisplayDoll tEDisplayDoll)) ? "" : tEDisplayDoll.GetItemGamepadInstructions(slot9);
		};
		int num8;
		UILinkPoint uILinkPoint6;
		for (num8 = 5100; num8 < 5118; num8++)
		{
			uILinkPoint6 = new UILinkPoint(num8, enabled: true, num8 - 1, num8 + 1, num8 - 9, num8 + 9);
			uILinkPoint6.OnSpecialInteracts += value9;
			int num9 = num8 - 5100;
			if (num9 < 9)
			{
				uILinkPoint6.Up = 40 + (int)Math.Round((double)(num9 + 1) * 0.9);
			}
			if (num9 >= 9)
			{
				uILinkPoint6.Down = -2;
			}
			if (num9 % 9 == 8)
			{
				uILinkPoint6.Right = -4;
			}
			if (num9 % 9 == 0)
			{
				uILinkPoint6.Left = -3;
			}
			uILinkPage2.LinkMap.Add(num8, uILinkPoint6);
		}
		uILinkPoint6 = new UILinkPoint(num8, enabled: true, -3, 5100, 40, -2);
		uILinkPoint6.OnSpecialInteracts += value9;
		uILinkPage2.LinkMap.Add(num8, uILinkPoint6);
		uILinkPage2.LinkMap[5100].Left = num8;
		uILinkPage2.PageOnLeft = 0;
		uILinkPage2.PageOnRight = 0;
		uILinkPage2.DefaultPoint = 5100;
		UILinkPointNavigator.RegisterPage(uILinkPage2, 20, automatedDefault: false);
		uILinkPage2.IsValidEvent += () => Main.playerInventory && Main.LocalPlayer.tileEntityAnchor.GetTileEntity() is TEDisplayDoll;
		UILinkPage uILinkPage3 = new UILinkPage();
		uILinkPage3.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value10 = delegate
		{
			int slot8 = UILinkPointNavigator.CurrentPoint - 5000;
			return (!(Main.LocalPlayer.tileEntityAnchor.GetTileEntity() is TEHatRack tEHatRack)) ? "" : tEHatRack.GetItemGamepadInstructions(slot8);
		};
		for (int num10 = 5000; num10 <= 5003; num10++)
		{
			UILinkPoint uILinkPoint7 = new UILinkPoint(num10, enabled: true, num10 - 1, num10 + 1, num10 - 2, num10 + 2);
			uILinkPoint7.OnSpecialInteracts += value10;
			int num11 = num10 - 5000;
			if (num11 < 2)
			{
				uILinkPoint7.Up = 44 + num11;
			}
			if (num11 >= 2)
			{
				uILinkPoint7.Down = -2;
			}
			if (num11 % 2 == 1)
			{
				uILinkPoint7.Right = -4;
			}
			if (num11 % 2 == 0)
			{
				uILinkPoint7.Left = -3;
			}
			uILinkPage3.LinkMap.Add(num10, uILinkPoint7);
		}
		uILinkPage3.PageOnLeft = 0;
		uILinkPage3.PageOnRight = 0;
		uILinkPage3.DefaultPoint = 5000;
		UILinkPointNavigator.RegisterPage(uILinkPage3, 21, automatedDefault: false);
		uILinkPage3.IsValidEvent += () => Main.playerInventory && Main.LocalPlayer.tileEntityAnchor.GetTileEntity() is TEHatRack;
		UILinkPage uILinkPage4 = new UILinkPage();
		uILinkPage4.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value11 = delegate
		{
			if (Main.npcShop == 0)
			{
				return "";
			}
			int slot7 = UILinkPointNavigator.CurrentPoint - 2700;
			return ItemSlot.GetGamepadInstructions(Main.instance.shop[Main.npcShop].item, 15, slot7);
		};
		for (int num12 = 2700; num12 <= 2739; num12++)
		{
			UILinkPoint uILinkPoint8 = new UILinkPoint(num12, enabled: true, num12 - 1, num12 + 1, num12 - 10, num12 + 10);
			uILinkPoint8.OnSpecialInteracts += value11;
			int num13 = num12 - 2700;
			if (num13 < 10)
			{
				uILinkPoint8.Up = 40 + num13;
			}
			if (num13 >= 30)
			{
				uILinkPoint8.Down = -2;
			}
			if (num13 % 10 == 9)
			{
				uILinkPoint8.Right = -4;
			}
			if (num13 % 10 == 0)
			{
				uILinkPoint8.Left = -3;
			}
			uILinkPage4.LinkMap.Add(num12, uILinkPoint8);
		}
		uILinkPage4.LinkMap[2739].Down = 300;
		uILinkPage4.PageOnLeft = 0;
		uILinkPage4.PageOnRight = 0;
		UILinkPointNavigator.RegisterPage(uILinkPage4, 13);
		uILinkPage4.IsValidEvent += () => Main.playerInventory && Main.npcShop != 0;
		UILinkPage cp16 = new UILinkPage();
		cp16.LinkMap.Add(303, new UILinkPoint(303, enabled: true, 304, 304, 40, -2));
		cp16.LinkMap.Add(304, new UILinkPoint(304, enabled: true, 303, 303, 40, -2));
		cp16.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value12 = () => ItemSlot.GetGamepadInstructions(ref Main.reforgeItem, 5);
		cp16.LinkMap[303].OnSpecialInteracts += value12;
		cp16.LinkMap[304].OnSpecialInteracts += () => Lang.misc[53].Value;
		cp16.UpdateEvent += delegate
		{
			if (Main.reforgeItem.type > 0)
			{
				cp16.LinkMap[303].Left = (cp16.LinkMap[303].Right = 304);
			}
			else
			{
				if (UILinkPointNavigator.OverridePoint == -1 && cp16.CurrentPoint == 304)
				{
					UILinkPointNavigator.ChangePoint(303);
				}
				cp16.LinkMap[303].Left = -3;
				cp16.LinkMap[303].Right = -4;
			}
		};
		cp16.IsValidEvent += () => Main.playerInventory && Main.InReforgeMenu;
		cp16.PageOnLeft = 0;
		cp16.PageOnRight = 0;
		cp16.EnterEvent += delegate
		{
			PlayerInput.LockGamepadButtons("MouseLeft");
		};
		UILinkPointNavigator.RegisterPage(cp16, 5);
		UILinkPage cp15 = new UILinkPage();
		cp15.OnSpecialInteracts += delegate
		{
			string text3 = PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
			if (PlayerInput.ControllerHousingCursorActive)
			{
				bool flag4 = UILinkPointNavigator.CurrentPoint == 600;
				bool flag5 = UILinkPointNavigator.Shortcuts.NPCS_HoveredBanner >= 0;
				if (flag5)
				{
					string fullName = Main.npc[UILinkPointNavigator.Shortcuts.NPCS_HoveredBanner].FullName;
					text3 += PlayerInput.BuildCommand(Language.GetTextValue("UI.HousingEvict", fullName), PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]);
				}
				else if (flag4)
				{
					text3 += PlayerInput.BuildCommand(Lang.misc[70].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]);
				}
				else if (UILinkPointNavigator.Shortcuts.NPCS_SelectedNPC >= 0)
				{
					string fullName2 = Main.npc[UILinkPointNavigator.Shortcuts.NPCS_SelectedNPC].FullName;
					text3 += PlayerInput.BuildCommand(Language.GetTextValue("UI.HousingAssign", fullName2), PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]);
				}
				if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.Grapple)
				{
					Point point = PlayerInput.HousingWorldPosition.ToTileCoordinates();
					if (flag5)
					{
						WorldGen.kickOut(UILinkPointNavigator.Shortcuts.NPCS_HoveredBanner);
						SoundEngine.PlaySound(12);
					}
					else if (flag4)
					{
						Main.instance.PerformHousingCheck(point.X, point.Y);
					}
					else if (UILinkPointNavigator.Shortcuts.NPCS_SelectedNPC >= 0)
					{
						Main.instance.TryMovingNPC(point.X, point.Y, UILinkPointNavigator.Shortcuts.NPCS_SelectedNPC);
					}
					PlayerInput.LockGamepadButtons("Grapple");
					PlayerInput.SettingsForUI.TryRevertingToMouseMode();
				}
				text3 += PlayerInput.BuildCommand(Language.GetTextValue("UI.HousingAim"), RightStickGlyphBinding);
			}
			return text3;
		};
		for (int num14 = 600; num14 <= 650; num14++)
		{
			UILinkPoint value13 = new UILinkPoint(num14, enabled: true, num14 + 10, num14 - 10, num14 - 1, num14 + 1);
			cp15.LinkMap.Add(num14, value13);
		}
		cp15.UpdateEvent += delegate
		{
			int num58 = UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn;
			if (num58 == 0)
			{
				num58 = 100;
			}
			for (int num59 = 0; num59 < 50; num59++)
			{
				cp15.LinkMap[600 + num59].Up = ((num59 % num58 == 0) ? (-1) : (600 + num59 - 1));
				if (cp15.LinkMap[600 + num59].Up == -1)
				{
					if (num59 >= num58 * 2)
					{
						cp15.LinkMap[600 + num59].Up = 307;
					}
					else if (num59 >= num58)
					{
						cp15.LinkMap[600 + num59].Up = 306;
					}
					else
					{
						cp15.LinkMap[600 + num59].Up = 305;
					}
				}
				cp15.LinkMap[600 + num59].Down = (((num59 + 1) % num58 == 0 || num59 == UILinkPointNavigator.Shortcuts.NPCS_IconsTotal - 1) ? 308 : (600 + num59 + 1));
				cp15.LinkMap[600 + num59].Left = ((num59 < UILinkPointNavigator.Shortcuts.NPCS_IconsTotal - num58) ? (600 + num59 + num58) : (-3));
				cp15.LinkMap[600 + num59].Right = ((num59 < num58) ? (-4) : (600 + num59 - num58));
			}
		};
		cp15.IsValidEvent += () => Main.playerInventory && Main.EquipPage == 1;
		cp15.PageOnLeft = 8;
		cp15.PageOnRight = 8;
		UILinkPointNavigator.RegisterPage(cp15, 6);
		UILinkPage cp14 = new UILinkPage();
		cp14.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value14 = delegate
		{
			int slot6 = UILinkPointNavigator.CurrentPoint - 180;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscEquips, 20, slot6);
		};
		Func<string> value15 = delegate
		{
			int slot5 = UILinkPointNavigator.CurrentPoint - 180;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscEquips, 19, slot5);
		};
		Func<string> value16 = delegate
		{
			int slot4 = UILinkPointNavigator.CurrentPoint - 180;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscEquips, 18, slot4);
		};
		Func<string> value17 = delegate
		{
			int slot3 = UILinkPointNavigator.CurrentPoint - 180;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscEquips, 17, slot3);
		};
		Func<string> value18 = delegate
		{
			int slot2 = UILinkPointNavigator.CurrentPoint - 180;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscEquips, 16, slot2);
		};
		Func<string> value19 = delegate
		{
			int slot = UILinkPointNavigator.CurrentPoint - 185;
			return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscDyes, 33, slot);
		};
		for (int num15 = 180; num15 <= 184; num15++)
		{
			UILinkPoint uILinkPoint9 = new UILinkPoint(num15, enabled: true, 185 + num15 - 180, -4, num15 - 1, num15 + 1);
			int num16 = num15 - 180;
			if (num16 == 0)
			{
				uILinkPoint9.Up = 305;
			}
			if (num16 == 4)
			{
				uILinkPoint9.Down = 308;
			}
			cp14.LinkMap.Add(num15, uILinkPoint9);
			switch (num15)
			{
			case 180:
				uILinkPoint9.OnSpecialInteracts += value15;
				break;
			case 181:
				uILinkPoint9.OnSpecialInteracts += value14;
				break;
			case 182:
				uILinkPoint9.OnSpecialInteracts += value16;
				break;
			case 183:
				uILinkPoint9.OnSpecialInteracts += value17;
				break;
			case 184:
				uILinkPoint9.OnSpecialInteracts += value18;
				break;
			}
		}
		for (int num17 = 185; num17 <= 189; num17++)
		{
			UILinkPoint uILinkPoint9 = new UILinkPoint(num17, enabled: true, -3, -5 + num17, num17 - 1, num17 + 1);
			uILinkPoint9.OnSpecialInteracts += value19;
			int num18 = num17 - 185;
			if (num18 == 0)
			{
				uILinkPoint9.Up = 306;
			}
			if (num18 == 4)
			{
				uILinkPoint9.Down = 308;
			}
			cp14.LinkMap.Add(num17, uILinkPoint9);
		}
		cp14.UpdateEvent += delegate
		{
			cp14.LinkMap[184].Down = ((UILinkPointNavigator.Shortcuts.BUFFS_DRAWN > 0) ? 9000 : 308);
			cp14.LinkMap[189].Down = ((UILinkPointNavigator.Shortcuts.BUFFS_DRAWN > 0) ? 9000 : 308);
		};
		cp14.IsValidEvent += () => Main.playerInventory && Main.EquipPage == 2;
		cp14.PageOnLeft = 8;
		cp14.PageOnRight = 8;
		UILinkPointNavigator.RegisterPage(cp14, 7);
		UILinkPage cp13 = new UILinkPage();
		cp13.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		cp13.LinkMap.Add(305, new UILinkPoint(305, enabled: true, 306, -4, 308, -2));
		cp13.LinkMap.Add(306, new UILinkPoint(306, enabled: true, 307, 305, 308, -2));
		cp13.LinkMap.Add(307, new UILinkPoint(307, enabled: true, -3, 306, 308, -2));
		cp13.LinkMap.Add(308, new UILinkPoint(308, enabled: true, -3, -4, -1, 305));
		cp13.LinkMap[305].OnSpecialInteracts += value;
		cp13.LinkMap[306].OnSpecialInteracts += value;
		cp13.LinkMap[307].OnSpecialInteracts += value;
		cp13.LinkMap[308].OnSpecialInteracts += value;
		cp13.UpdateEvent += delegate
		{
			switch (Main.EquipPage)
			{
			case 0:
				cp13.LinkMap[305].Down = 100;
				cp13.LinkMap[306].Down = 110;
				cp13.LinkMap[307].Down = 120;
				cp13.LinkMap[308].Up = 108 + Main.player[Main.myPlayer].GetAmountOfExtraAccessorySlotsToShow() - 1;
				break;
			case 1:
			{
				cp13.LinkMap[305].Down = 600;
				cp13.LinkMap[306].Down = ((UILinkPointNavigator.Shortcuts.NPCS_IconsTotal > UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn) ? (600 + UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn) : 600);
				cp13.LinkMap[307].Down = ((UILinkPointNavigator.Shortcuts.NPCS_IconsTotal > UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn * 2) ? (600 + UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn * 2) : cp13.LinkMap[306].Down);
				int num57 = UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn;
				if (num57 == 0)
				{
					num57 = 100;
				}
				if (num57 == 100)
				{
					num57 = UILinkPointNavigator.Shortcuts.NPCS_IconsTotal;
				}
				cp13.LinkMap[308].Up = 600 + num57 - 1;
				break;
			}
			case 2:
				cp13.LinkMap[305].Down = 180;
				cp13.LinkMap[306].Down = 185;
				cp13.LinkMap[307].Down = -2;
				cp13.LinkMap[308].Up = ((UILinkPointNavigator.Shortcuts.BUFFS_DRAWN > 0) ? 9000 : 184);
				break;
			}
			cp13.PageOnRight = GetCornerWrapPageIdFromRightToLeft();
		};
		cp13.IsValidEvent += () => Main.playerInventory;
		cp13.PageOnLeft = 0;
		cp13.PageOnRight = 0;
		UILinkPointNavigator.RegisterPage(cp13, 8);
		UILinkPage cp12 = new UILinkPage();
		cp12.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		cp12.OnSpecialInteractsLate += () => ItemSlot.GetGamepadInstructions(Main.InPipBanner ? 35 : 22);
		for (int num19 = 1500; num19 < 1550; num19++)
		{
			UILinkPoint value20 = new UILinkPoint(num19, enabled: true, num19, num19, -1, -2);
			cp12.LinkMap.Add(num19, value20);
		}
		cp12.LinkMap.Add(11001, new UILinkPoint(11001, enabled: true, 1501, 11002, -1, 11003));
		cp12.LinkMap.Add(11002, new UILinkPoint(11002, enabled: true, 11001, -4, -1, 11003));
		cp12.LinkMap.Add(11003, new UILinkPoint(11003, enabled: true, 1501, -4, 11001, 1502));
		cp12.LinkMap[1500].OnSpecialInteracts += () => ItemSlot.GetGamepadInstructions(ref Main.guideItem, 7);
		cp12.LinkMap[11001].OnSpecialInteracts += () => PlayerInput.BuildCommand(Language.GetTextValue("UI.ToggleClassicGrid"), PlayerInput.ProfileGamepadUI.KeyStatus["MouseRight"]);
		cp12.UpdateEvent += delegate
		{
			cp12.PageOnLeft = ((Player.Settings.CraftingGridControl == Player.Settings.CraftingGridMode.Classic) ? 10 : 8);
			int num54 = UILinkPointNavigator.Shortcuts.CRAFT_CurrentIngredientsCount;
			int num55 = num54;
			if (MainnumAvailableRecipes > 0)
			{
				num55 += 2;
			}
			if (num54 < num55)
			{
				num54 = num55;
			}
			if (UILinkPointNavigator.OverridePoint == -1)
			{
				if (cp12.CurrentPoint == 11003)
				{
					if (Main.InGuideCraftMenu)
					{
						UILinkPointNavigator.ChangePoint(1501);
					}
				}
				else if (cp12.CurrentPoint != 11001)
				{
					if (cp12.CurrentPoint == 11002)
					{
						if (!Main.bannerUI.AnyAvailableBanners || Main.InGuideCraftMenu)
						{
							UILinkPointNavigator.ChangePoint(11001);
						}
					}
					else if (cp12.CurrentPoint == 1500)
					{
						if (!Main.InGuideCraftMenu)
						{
							UILinkPointNavigator.ChangePoint(1501);
						}
					}
					else if (cp12.CurrentPoint > 1500 + num54)
					{
						UILinkPointNavigator.ChangePoint(1500);
					}
				}
			}
			bool flag3 = Main.LocalPlayer.chest != -1;
			for (int num56 = 1; num56 < num54; num56++)
			{
				cp12.LinkMap[1500 + num56].Left = 1500 + num56 - 1;
				cp12.LinkMap[1500 + num56].Right = ((num56 == num54 - 2) ? (-4) : (1500 + num56 + 1));
				if (num56 >= 2)
				{
					cp12.LinkMap[1500 + num56].Up = (Main.InGuideCraftMenu ? 1500 : (flag3 ? 11003 : (-1)));
					cp12.LinkMap[1500 + num56].Down = (flag3 ? (-1) : ((num56 >= 3 && Main.bannerUI.AnyAvailableBanners) ? 11002 : 11001));
				}
			}
			cp12.LinkMap[1501].Left = -3;
			if (num54 > 0)
			{
				cp12.LinkMap[1500 + num54 - 1].Right = -4;
			}
			cp12.LinkMap[1500].Down = ((num54 >= 2) ? 1502 : (-2));
			cp12.LinkMap[1500].Left = ((num54 >= 1) ? 1501 : (-3));
			cp12.LinkMap[1500].Up = 11001;
			cp12.LinkMap[11001].Left = (Main.InPipCrafting ? 1501 : 12000);
			cp12.LinkMap[11001].Down = ((!Main.InPipCrafting) ? (-1) : (Main.InGuideCraftMenu ? 1500 : 11003));
			cp12.LinkMap[11001].Right = ((!Main.bannerUI.AnyAvailableBanners || Main.InGuideCraftMenu) ? (-1) : 11002);
			cp12.LinkMap[11001].Up = (flag3 ? (-1) : 1502);
			cp12.LinkMap[11002].Down = ((!Main.InPipCrafting) ? (-1) : 11003);
			cp12.LinkMap[11002].Up = (flag3 ? (-1) : ((num54 >= 5) ? 1503 : 1502));
			cp12.LinkMap[11003].Down = (flag3 ? 1502 : (-1));
		};
		cp12.LinkMap[1501].OnSpecialInteracts += () => ItemSlot.GetCraftSlotGamepadInstructions();
		cp12.ReachEndEvent += delegate(int current, int next)
		{
			switch (current)
			{
			case 1501:
				switch (next)
				{
				case -1:
					if (MainfocusRecipe > 0)
					{
						MainfocusRecipe--;
					}
					break;
				case -2:
					if (MainfocusRecipe < MainnumAvailableRecipes - 1)
					{
						MainfocusRecipe++;
					}
					break;
				}
				break;
			default:
				switch (next)
				{
				case -1:
					if (MainfocusRecipe > 0)
					{
						UILinkPointNavigator.ChangePoint(1501);
						MainfocusRecipe--;
					}
					break;
				case -2:
					if (MainfocusRecipe < MainnumAvailableRecipes - 1)
					{
						UILinkPointNavigator.ChangePoint(1501);
						MainfocusRecipe++;
					}
					break;
				}
				break;
			case 1500:
				break;
			}
		};
		cp12.EnterEvent += delegate
		{
			Main.PipsUseGrid = false;
			PlayerInput.LockGamepadButtons("MouseLeft");
		};
		cp12.CanEnterEvent += () => Main.playerInventory && (MainnumAvailableRecipes > 0 || Main.InGuideCraftMenu);
		cp12.IsValidEvent += () => Main.playerInventory && (MainnumAvailableRecipes > 0 || Main.InGuideCraftMenu);
		cp12.PageOnLeft = 8;
		cp12.PageOnRight = 0;
		UILinkPointNavigator.RegisterPage(cp12, 9);
		UILinkPage cp11 = new UILinkPage();
		cp11.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		cp11.OnSpecialInteractsLate += () => ItemSlot.GetGamepadInstructions(Main.InPipBanner ? 35 : 22);
		for (int num20 = 22000; num20 < 30000; num20++)
		{
			UILinkPoint uILinkPoint10 = new UILinkPoint(num20, enabled: true, num20, num20, num20, num20);
			int IHateLambda = num20;
			uILinkPoint10.OnSpecialInteracts += delegate
			{
				string text2 = PlayerInput.BuildCommand(Lang.misc[73].Value, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
				if (TryQuickCrafting(22000, IHateLambda))
				{
					text2 += PlayerInput.BuildCommand(Lang.misc[71].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]);
				}
				return text2;
			};
			cp11.LinkMap.Add(num20, uILinkPoint10);
		}
		cp11.UpdateEvent += delegate
		{
			int num51 = UILinkPointNavigator.Shortcuts.CRAFT_IconsPerRow;
			int cRAFT_IconsPerColumn = UILinkPointNavigator.Shortcuts.CRAFT_IconsPerColumn;
			if (num51 == 0)
			{
				num51 = 100;
			}
			int num52 = num51 * cRAFT_IconsPerColumn;
			if (num52 > 8000)
			{
				num52 = 8000;
			}
			if (num52 > MainnumAvailableRecipes)
			{
				num52 = MainnumAvailableRecipes;
			}
			for (int num53 = 0; num53 < num52; num53++)
			{
				cp11.LinkMap[22000 + num53].Left = ((num53 % num51 == 0) ? (-3) : (22000 + num53 - 1));
				cp11.LinkMap[22000 + num53].Right = (((num53 + 1) % num51 == 0 || num53 == MainnumAvailableRecipes - 1) ? (-4) : (22000 + num53 + 1));
				cp11.LinkMap[22000 + num53].Down = ((num53 < num52 - num51) ? (22000 + num53 + num51) : (-2));
				cp11.LinkMap[22000 + num53].Up = ((num53 < num51) ? (-1) : (22000 + num53 - num51));
			}
			cp11.PageOnLeft = GetCornerWrapPageIdFromLeftToRight();
		};
		cp11.ReachEndEvent += delegate(int current, int next)
		{
			int cRAFT_IconsPerRow = UILinkPointNavigator.Shortcuts.CRAFT_IconsPerRow;
			switch (next)
			{
			case -1:
				Main.recStart -= cRAFT_IconsPerRow;
				if (Main.recStart < 0)
				{
					Main.recStart = 0;
				}
				break;
			case -2:
				Main.recStart += cRAFT_IconsPerRow;
				SoundEngine.PlaySound(12);
				if (Main.recStart > MainnumAvailableRecipes - cRAFT_IconsPerRow)
				{
					Main.recStart = MainnumAvailableRecipes - cRAFT_IconsPerRow;
				}
				break;
			}
		};
		cp11.EnterEvent += delegate
		{
			Main.PipsUseGrid = true;
		};
		cp11.LeaveEvent += delegate
		{
			Main.PipsUseGrid = false;
		};
		cp11.CanEnterEvent += () => Main.playerInventory && MainnumAvailableRecipes > 0;
		cp11.IsValidEvent += () => Main.playerInventory && Main.PipsUseGrid && MainnumAvailableRecipes > 0;
		cp11.PageOnLeft = 0;
		cp11.PageOnRight = 9;
		UILinkPointNavigator.RegisterPage(cp11, 10);
		UILinkPage cp10 = new UILinkPage();
		cp10.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		for (int num21 = 2605; num21 < 2620; num21++)
		{
			UILinkPoint uILinkPoint11 = new UILinkPoint(num21, enabled: true, num21, num21, num21, num21);
			uILinkPoint11.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[73].Value, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
			cp10.LinkMap.Add(num21, uILinkPoint11);
		}
		cp10.UpdateEvent += delegate
		{
			int num47 = 5;
			int num48 = 3;
			int num49 = num47 * num48;
			int count = Main.Hairstyles.AvailableHairstyles.Count;
			for (int num50 = 0; num50 < num49; num50++)
			{
				cp10.LinkMap[2605 + num50].Left = ((num50 % num47 == 0) ? (-3) : (2605 + num50 - 1));
				cp10.LinkMap[2605 + num50].Right = (((num50 + 1) % num47 == 0 || num50 == count - 1) ? (-4) : (2605 + num50 + 1));
				cp10.LinkMap[2605 + num50].Down = ((num50 < num49 - num47) ? (2605 + num50 + num47) : (-2));
				cp10.LinkMap[2605 + num50].Up = ((num50 < num47) ? (-1) : (2605 + num50 - num47));
			}
		};
		cp10.ReachEndEvent += delegate(int current, int next)
		{
			int num46 = 5;
			switch (next)
			{
			case -1:
				Main.hairStart -= num46;
				SoundEngine.PlaySound(12);
				break;
			case -2:
				Main.hairStart += num46;
				SoundEngine.PlaySound(12);
				break;
			}
		};
		cp10.CanEnterEvent += () => Main.hairWindow;
		cp10.IsValidEvent += () => Main.hairWindow;
		cp10.PageOnLeft = 12;
		cp10.PageOnRight = 12;
		UILinkPointNavigator.RegisterPage(cp10, 11);
		UILinkPage uILinkPage5 = new UILinkPage();
		uILinkPage5.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		uILinkPage5.LinkMap.Add(2600, new UILinkPoint(2600, enabled: true, -3, -4, -1, 2601));
		uILinkPage5.LinkMap.Add(2601, new UILinkPoint(2601, enabled: true, -3, -4, 2600, 2602));
		uILinkPage5.LinkMap.Add(2602, new UILinkPoint(2602, enabled: true, -3, -4, 2601, 2603));
		uILinkPage5.LinkMap.Add(2603, new UILinkPoint(2603, enabled: true, -3, 2604, 2602, -2));
		uILinkPage5.LinkMap.Add(2604, new UILinkPoint(2604, enabled: true, 2603, -4, 2602, -2));
		uILinkPage5.UpdateEvent += delegate
		{
			Vector3 value28 = Main.rgbToHsl(Main.selColor);
			float interfaceDeadzoneX2 = PlayerInput.CurrentProfile.InterfaceDeadzoneX;
			float x2 = PlayerInput.GamepadThumbstickLeft.X;
			x2 = ((!(x2 < 0f - interfaceDeadzoneX2) && !(x2 > interfaceDeadzoneX2)) ? 0f : (MathHelper.Lerp(0f, 1f / 120f, (Math.Abs(x2) - interfaceDeadzoneX2) / (1f - interfaceDeadzoneX2)) * (float)Math.Sign(x2)));
			int currentPoint2 = UILinkPointNavigator.CurrentPoint;
			if (currentPoint2 == 2600)
			{
				Main.hBar = MathHelper.Clamp(Main.hBar + x2, 0f, 1f);
			}
			if (currentPoint2 == 2601)
			{
				Main.sBar = MathHelper.Clamp(Main.sBar + x2, 0f, 1f);
			}
			if (currentPoint2 == 2602)
			{
				Main.lBar = MathHelper.Clamp(Main.lBar + x2, 0.15f, 1f);
			}
			Vector3.Clamp(value28, Vector3.Zero, Vector3.One);
			if (x2 != 0f)
			{
				if (Main.hairWindow)
				{
					Main.player[Main.myPlayer].hairColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
				}
				SoundEngine.PlaySound(12);
			}
		};
		uILinkPage5.CanEnterEvent += () => Main.hairWindow;
		uILinkPage5.IsValidEvent += () => Main.hairWindow;
		uILinkPage5.PageOnLeft = 11;
		uILinkPage5.PageOnRight = 11;
		UILinkPointNavigator.RegisterPage(uILinkPage5, 12);
		UILinkPage cp9 = new UILinkPage();
		for (int num22 = 0; num22 < 30; num22++)
		{
			cp9.LinkMap.Add(2900 + num22, new UILinkPoint(2900 + num22, enabled: true, -3, -4, -1, -2));
			cp9.LinkMap[2900 + num22].OnSpecialInteracts += value;
		}
		cp9.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		cp9.TravelEvent += delegate
		{
			if (UILinkPointNavigator.CurrentPage == cp9.ID)
			{
				int num45 = cp9.CurrentPoint - 2900;
				if (num45 < 5)
				{
					IngameOptions.category = num45;
				}
			}
		};
		cp9.UpdateEvent += delegate
		{
			int num41 = UILinkPointNavigator.Shortcuts.INGAMEOPTIONS_BUTTONS_LEFT;
			if (num41 == 0)
			{
				num41 = 5;
			}
			if (UILinkPointNavigator.OverridePoint == -1 && cp9.CurrentPoint < 2930 && cp9.CurrentPoint > 2900 + num41 - 1)
			{
				UILinkPointNavigator.ChangePoint(2900);
			}
			for (int num42 = 2900; num42 < 2900 + num41; num42++)
			{
				cp9.LinkMap[num42].Up = num42 - 1;
				cp9.LinkMap[num42].Down = num42 + 1;
			}
			cp9.LinkMap[2900].Up = 2900 + num41 - 1;
			cp9.LinkMap[2900 + num41 - 1].Down = 2900;
			int num43 = cp9.CurrentPoint - 2900;
			if (num43 < 4 && CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.MouseLeft)
			{
				IngameOptions.category = num43;
				UILinkPointNavigator.ChangePage(1002);
			}
			int num44 = ((SocialAPI.Network != null && SocialAPI.Network.CanInvite()) ? 1 : 0);
			if (num43 == 4 + num44 && CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.MouseLeft)
			{
				UILinkPointNavigator.ChangePage(1004);
			}
		};
		cp9.EnterEvent += delegate
		{
			cp9.CurrentPoint = 2900 + IngameOptions.category;
		};
		cp9.PageOnLeft = (cp9.PageOnRight = 1002);
		cp9.IsValidEvent += () => Main.ingameOptionsWindow && !Main.InGameUI.IsVisible;
		cp9.CanEnterEvent += () => Main.ingameOptionsWindow && !Main.InGameUI.IsVisible;
		UILinkPointNavigator.RegisterPage(cp9, 1001);
		UILinkPage cp8 = new UILinkPage();
		for (int num23 = 0; num23 < 30; num23++)
		{
			cp8.LinkMap.Add(2930 + num23, new UILinkPoint(2930 + num23, enabled: true, -3, -4, -1, -2));
			cp8.LinkMap[2930 + num23].OnSpecialInteracts += value;
		}
		cp8.EnterEvent += delegate
		{
			Main.mouseLeftRelease = false;
		};
		cp8.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		cp8.UpdateEvent += delegate
		{
			int num39 = UILinkPointNavigator.Shortcuts.INGAMEOPTIONS_BUTTONS_RIGHT;
			if (num39 == 0)
			{
				num39 = 5;
			}
			if (UILinkPointNavigator.OverridePoint == -1 && cp8.CurrentPoint >= 2930 && cp8.CurrentPoint > 2930 + num39 - 1)
			{
				UILinkPointNavigator.ChangePoint(2930);
			}
			for (int num40 = 2930; num40 < 2930 + num39; num40++)
			{
				cp8.LinkMap[num40].Up = num40 - 1;
				cp8.LinkMap[num40].Down = num40 + 1;
			}
			cp8.LinkMap[2930].Up = -1;
			cp8.LinkMap[2930 + num39 - 1].Down = -2;
			HandleOptionsSpecials();
		};
		cp8.PageOnLeft = (cp8.PageOnRight = 1001);
		cp8.IsValidEvent += () => Main.ingameOptionsWindow;
		cp8.CanEnterEvent += () => Main.ingameOptionsWindow;
		UILinkPointNavigator.RegisterPage(cp8, 1002);
		UILinkPage cp7 = new UILinkPage();
		cp7.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		for (int num24 = 1550; num24 < 1558; num24++)
		{
			UILinkPoint uILinkPoint12 = new UILinkPoint(num24, enabled: true, -3, -4, -1, -2);
			switch (num24)
			{
			case 1551:
			case 1553:
			case 1555:
				uILinkPoint12.Up = uILinkPoint12.ID - 2;
				uILinkPoint12.Down = uILinkPoint12.ID + 2;
				uILinkPoint12.Right = uILinkPoint12.ID + 1;
				break;
			case 1552:
			case 1554:
			case 1556:
				uILinkPoint12.Up = uILinkPoint12.ID - 2;
				uILinkPoint12.Down = uILinkPoint12.ID + 2;
				uILinkPoint12.Left = uILinkPoint12.ID - 1;
				break;
			}
			cp7.LinkMap.Add(num24, uILinkPoint12);
		}
		cp7.LinkMap[1550].Down = 1551;
		cp7.LinkMap[1550].Right = 120;
		cp7.LinkMap[1550].Up = 307;
		cp7.LinkMap[1552].Right = 121;
		cp7.LinkMap[1554].Right = 121;
		cp7.LinkMap[1555].Down = 1570;
		cp7.LinkMap[1556].Down = 1570;
		cp7.LinkMap[1556].Right = 122;
		cp7.LinkMap[1557].Up = 1570;
		cp7.LinkMap[1557].Down = 308;
		cp7.LinkMap[1557].Right = 127;
		cp7.LinkMap.Add(1570, new UILinkPoint(1570, enabled: true, -3, -4, -1, -2));
		cp7.LinkMap[1570].Up = 1555;
		cp7.LinkMap[1570].Down = 1557;
		cp7.LinkMap[1570].Right = 126;
		for (int num25 = 0; num25 < 7; num25++)
		{
			cp7.LinkMap[1550 + num25].OnSpecialInteracts += value;
		}
		cp7.UpdateEvent += delegate
		{
			cp7.LinkMap[1551].Up = (Main.ShouldPVPDraw ? 1550 : (-1));
			cp7.LinkMap[1552].Up = (Main.ShouldPVPDraw ? 1550 : (-1));
			cp7.LinkMap[1570].Up = (Main.ShouldTeamSelectDraw ? 1555 : (-1));
			int iNFOACCCOUNT2 = UILinkPointNavigator.Shortcuts.INFOACCCOUNT;
			if (iNFOACCCOUNT2 > 0)
			{
				cp7.LinkMap[1570].Up = 1558 + (iNFOACCCOUNT2 - 1) / 2 * 2;
			}
			if (Main.ShouldTeamSelectDraw)
			{
				if (iNFOACCCOUNT2 >= 1)
				{
					cp7.LinkMap[1555].Down = 1558;
					cp7.LinkMap[1556].Down = 1558;
				}
				else
				{
					cp7.LinkMap[1555].Down = 1570;
					cp7.LinkMap[1556].Down = 1570;
				}
				if (iNFOACCCOUNT2 >= 2)
				{
					cp7.LinkMap[1556].Down = 1559;
				}
				else
				{
					cp7.LinkMap[1556].Down = 1570;
				}
			}
		};
		cp7.IsValidEvent += () => Main.playerInventory;
		cp7.PageOnLeft = 8;
		cp7.PageOnRight = 8;
		UILinkPointNavigator.RegisterPage(cp7, 16);
		UILinkPage cp6 = new UILinkPage();
		cp6.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		for (int num26 = 1558; num26 < 1570; num26++)
		{
			UILinkPoint uILinkPoint13 = new UILinkPoint(num26, enabled: true, -3, -4, -1, -2);
			uILinkPoint13.OnSpecialInteracts += value;
			switch (num26)
			{
			case 1559:
			case 1561:
			case 1563:
				uILinkPoint13.Up = uILinkPoint13.ID - 2;
				uILinkPoint13.Down = uILinkPoint13.ID + 2;
				uILinkPoint13.Right = uILinkPoint13.ID + 1;
				break;
			case 1560:
			case 1562:
			case 1564:
				uILinkPoint13.Up = uILinkPoint13.ID - 2;
				uILinkPoint13.Down = uILinkPoint13.ID + 2;
				uILinkPoint13.Left = uILinkPoint13.ID - 1;
				break;
			}
			cp6.LinkMap.Add(num26, uILinkPoint13);
		}
		cp6.UpdateEvent += delegate
		{
			int iNFOACCCOUNT = UILinkPointNavigator.Shortcuts.INFOACCCOUNT;
			if (UILinkPointNavigator.OverridePoint == -1 && cp6.CurrentPoint - 1558 >= iNFOACCCOUNT)
			{
				UILinkPointNavigator.ChangePoint(1558 + iNFOACCCOUNT - 1);
			}
			for (int num37 = 0; num37 < iNFOACCCOUNT; num37++)
			{
				bool flag2 = num37 % 2 == 0;
				int num38 = num37 + 1558;
				cp6.LinkMap[num38].Down = ((num37 < iNFOACCCOUNT - 2) ? (num38 + 2) : 1570);
				cp6.LinkMap[num38].Up = ((num37 > 1) ? (num38 - 2) : (Main.ShouldTeamSelectDraw ? (flag2 ? 1555 : 1556) : (-1)));
				cp6.LinkMap[num38].Right = ((flag2 && num37 + 1 < iNFOACCCOUNT) ? (num38 + 1) : (123 + num37 / 4));
				cp6.LinkMap[num38].Left = (flag2 ? (-3) : (num38 - 1));
			}
		};
		cp6.IsValidEvent += () => Main.playerInventory && UILinkPointNavigator.Shortcuts.INFOACCCOUNT > 0;
		cp6.PageOnLeft = 8;
		cp6.PageOnRight = 8;
		UILinkPointNavigator.RegisterPage(cp6, 17);
		UILinkPage cp5 = new UILinkPage();
		cp5.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		for (int num27 = 6000; num27 < 6012; num27++)
		{
			UILinkPoint uILinkPoint14 = new UILinkPoint(num27, enabled: true, -3, -4, -1, -2);
			switch (num27)
			{
			case 6000:
				uILinkPoint14.Right = 0;
				break;
			case 6001:
			case 6002:
				uILinkPoint14.Right = 10;
				break;
			case 6003:
			case 6004:
				uILinkPoint14.Right = 20;
				break;
			case 6005:
			case 6006:
				uILinkPoint14.Right = 30;
				break;
			default:
				uILinkPoint14.Right = 40;
				break;
			}
			cp5.LinkMap.Add(num27, uILinkPoint14);
		}
		cp5.UpdateEvent += delegate
		{
			int bUILDERACCCOUNT = UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT;
			if (UILinkPointNavigator.OverridePoint == -1 && cp5.CurrentPoint - 6000 >= bUILDERACCCOUNT)
			{
				UILinkPointNavigator.ChangePoint(6000 + bUILDERACCCOUNT - 1);
			}
			for (int num35 = 0; num35 < bUILDERACCCOUNT; num35++)
			{
				_ = num35 % 2;
				int num36 = num35 + 6000;
				cp5.LinkMap[num36].Down = ((num35 < bUILDERACCCOUNT - 1) ? (num36 + 1) : (-2));
				cp5.LinkMap[num36].Up = ((num35 > 0) ? (num36 - 1) : (-1));
			}
		};
		cp5.IsValidEvent += () => Main.playerInventory && UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 0;
		cp5.PageOnLeft = 8;
		cp5.PageOnRight = 8;
		UILinkPointNavigator.RegisterPage(cp5, 18);
		UILinkPage uILinkPage6 = new UILinkPage();
		uILinkPage6.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		uILinkPage6.LinkMap.Add(2806, new UILinkPoint(2806, enabled: true, 2805, 2807, -1, 2808));
		uILinkPage6.LinkMap.Add(2807, new UILinkPoint(2807, enabled: true, 2806, 2810, -1, 2809));
		uILinkPage6.LinkMap.Add(2808, new UILinkPoint(2808, enabled: true, 2813, 2809, 2806, -2));
		uILinkPage6.LinkMap.Add(2809, new UILinkPoint(2809, enabled: true, 2808, 2811, 2807, -2));
		uILinkPage6.LinkMap.Add(2810, new UILinkPoint(2810, enabled: true, 2807, -4, -1, 2811));
		uILinkPage6.LinkMap.Add(2811, new UILinkPoint(2811, enabled: true, 2809, -4, 2810, -2));
		uILinkPage6.LinkMap.Add(2805, new UILinkPoint(2805, enabled: true, -3, 2806, -1, 2813));
		uILinkPage6.LinkMap.Add(2813, new UILinkPoint(2813, enabled: true, -3, 2808, 2805, -2));
		uILinkPage6.LinkMap[2806].OnSpecialInteracts += value;
		uILinkPage6.LinkMap[2807].OnSpecialInteracts += value;
		uILinkPage6.LinkMap[2808].OnSpecialInteracts += value;
		uILinkPage6.LinkMap[2809].OnSpecialInteracts += value;
		uILinkPage6.LinkMap[2805].OnSpecialInteracts += value;
		uILinkPage6.LinkMap[2813].OnSpecialInteracts += value;
		uILinkPage6.CanEnterEvent += () => Main.clothesWindow;
		uILinkPage6.IsValidEvent += () => Main.clothesWindow;
		uILinkPage6.EnterEvent += delegate
		{
			Main.player[Main.myPlayer].releaseInventory = false;
		};
		uILinkPage6.LeaveEvent += delegate
		{
			Main.player[Main.myPlayer].LockGamepadTileInteractions();
		};
		uILinkPage6.PageOnLeft = 15;
		uILinkPage6.PageOnRight = 15;
		UILinkPointNavigator.RegisterPage(uILinkPage6, 14);
		UILinkPage uILinkPage7 = new UILinkPage();
		uILinkPage7.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		uILinkPage7.LinkMap.Add(2800, new UILinkPoint(2800, enabled: true, -3, -4, -1, 2801));
		uILinkPage7.LinkMap.Add(2801, new UILinkPoint(2801, enabled: true, -3, -4, 2800, 2802));
		uILinkPage7.LinkMap.Add(2802, new UILinkPoint(2802, enabled: true, -3, -4, 2801, 2812));
		uILinkPage7.LinkMap.Add(2812, new UILinkPoint(2812, enabled: true, -3, -4, 2802, 2803));
		uILinkPage7.LinkMap.Add(2803, new UILinkPoint(2803, enabled: true, -3, 2804, 2812, -2));
		uILinkPage7.LinkMap.Add(2804, new UILinkPoint(2804, enabled: true, 2803, -4, 2812, -2));
		uILinkPage7.LinkMap[2800].OnSpecialInteracts += value;
		uILinkPage7.LinkMap[2801].OnSpecialInteracts += value;
		uILinkPage7.LinkMap[2802].OnSpecialInteracts += value;
		uILinkPage7.LinkMap[2812].OnSpecialInteracts += value;
		uILinkPage7.LinkMap[2803].OnSpecialInteracts += value;
		uILinkPage7.LinkMap[2804].OnSpecialInteracts += value;
		uILinkPage7.UpdateEvent += delegate
		{
			Vector3 value27 = Main.rgbToHsl(Main.selColor);
			float interfaceDeadzoneX = PlayerInput.CurrentProfile.InterfaceDeadzoneX;
			float x = PlayerInput.GamepadThumbstickLeft.X;
			x = ((!(x < 0f - interfaceDeadzoneX) && !(x > interfaceDeadzoneX)) ? 0f : (MathHelper.Lerp(0f, 1f / 120f, (Math.Abs(x) - interfaceDeadzoneX) / (1f - interfaceDeadzoneX)) * (float)Math.Sign(x)));
			int currentPoint = UILinkPointNavigator.CurrentPoint;
			if (currentPoint == 2800)
			{
				Main.hBar = MathHelper.Clamp(Main.hBar + x, 0f, 1f);
			}
			if (currentPoint == 2801)
			{
				Main.sBar = MathHelper.Clamp(Main.sBar + x, 0f, 1f);
			}
			if (currentPoint == 2802)
			{
				Main.lBar = MathHelper.Clamp(Main.lBar + x, 0.15f, 1f);
			}
			if (currentPoint == 2812)
			{
				Main.player[Main.myPlayer].voicePitchOffset = MathHelper.Clamp(Main.player[Main.myPlayer].voicePitchOffset + x, -1f, 1f);
			}
			Vector3.Clamp(value27, Vector3.Zero, Vector3.One);
			if (x != 0f)
			{
				if (Main.clothesWindow)
				{
					Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
					switch (Main.selClothes)
					{
					case 0:
						Main.player[Main.myPlayer].shirtColor = Main.selColor;
						break;
					case 1:
						Main.player[Main.myPlayer].underShirtColor = Main.selColor;
						break;
					case 2:
						Main.player[Main.myPlayer].pantsColor = Main.selColor;
						break;
					case 3:
						Main.player[Main.myPlayer].shoeColor = Main.selColor;
						break;
					}
				}
				if (currentPoint != 2812)
				{
					SoundEngine.PlaySound(12);
				}
			}
			if (currentPoint == 2812)
			{
				bool flag = x != 0f;
				if (Main.WasDraggingPlayerAudio && !flag)
				{
					Main.player[Main.myPlayer].PlayHurtSound();
				}
				Main.WasDraggingPlayerAudio = flag;
			}
		};
		uILinkPage7.CanEnterEvent += () => Main.clothesWindow;
		uILinkPage7.IsValidEvent += () => Main.clothesWindow;
		uILinkPage7.EnterEvent += delegate
		{
			Main.player[Main.myPlayer].releaseInventory = false;
			Main.WasDraggingPlayerAudio = false;
		};
		uILinkPage7.LeaveEvent += delegate
		{
			Main.player[Main.myPlayer].LockGamepadTileInteractions();
		};
		uILinkPage7.PageOnLeft = 14;
		uILinkPage7.PageOnRight = 14;
		UILinkPointNavigator.RegisterPage(uILinkPage7, 15);
		UILinkPage cp4 = new UILinkPage();
		cp4.UpdateEvent += delegate
		{
			PlayerInput.GamepadAllowScrolling = true;
		};
		for (int num28 = 3000; num28 <= 4999; num28++)
		{
			cp4.LinkMap.Add(num28, new UILinkPoint(num28, enabled: true, -3, -4, -1, -2));
		}
		cp4.OnSpecialInteracts += () => (Main.InGameUI.CurrentState is UIBestiaryTest) ? (PlayerInput.BuildCommand(Lang.misc[82].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Language.GetText("UI.SwitchPage").Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]) + PlayerInput.BuildCommand(Lang.misc[53].Value, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]) + FancyUISpecialInstructions()) : (PlayerInput.BuildCommand(Lang.misc[53].Value, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]) + PlayerInput.BuildCommand(Lang.misc[82].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + FancyUISpecialInstructions());
		cp4.UpdateEvent += delegate
		{
			if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.Inventory)
			{
				FancyExit();
			}
		};
		cp4.EnterEvent += delegate
		{
			cp4.CurrentPoint = 3002;
		};
		cp4.CanEnterEvent += () => Main.MenuUI.IsVisible || Main.InGameUI.IsVisible;
		cp4.IsValidEvent += () => Main.MenuUI.IsVisible || Main.InGameUI.IsVisible;
		cp4.OnPageMoveAttempt += OnFancyUIPageMoveAttempt;
		UILinkPointNavigator.RegisterPage(cp4, 1004);
		UILinkPage cp3 = new UILinkPage();
		cp3.UpdateEvent += delegate
		{
			PlayerInput.GamepadAllowScrolling = true;
		};
		for (int num29 = 10000; num29 <= 11000; num29++)
		{
			cp3.LinkMap.Add(num29, new UILinkPoint(num29, enabled: true, -3, -4, -1, -2));
		}
		for (int num30 = 15000; num30 <= 15000; num30++)
		{
			cp3.LinkMap.Add(num30, new UILinkPoint(num30, enabled: true, -3, -4, -1, -2));
		}
		cp3.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]) + PlayerInput.BuildCommand(Lang.misc[53].Value, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]) + FancyUISpecialInstructions();
		cp3.UpdateEvent += delegate
		{
			if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.Inventory)
			{
				FancyExit();
			}
		};
		cp3.EnterEvent += delegate
		{
			cp3.CurrentPoint = 10000;
		};
		cp3.CanEnterEvent += CanEnterCreativeMenu;
		cp3.IsValidEvent += CanEnterCreativeMenu;
		cp3.OnPageMoveAttempt += OnFancyUIPageMoveAttempt;
		cp3.PageOnLeft = 8;
		cp3.PageOnRight = 0;
		UILinkPointNavigator.RegisterPage(cp3, 1005);
		UILinkPage uILinkPage8 = new UILinkPage();
		for (int num31 = 20000; num31 < 21000; num31++)
		{
			uILinkPage8.LinkMap.Add(num31, new UILinkPoint(num31, enabled: true, -3, -4, -1, -2));
		}
		uILinkPage8.CanEnterEvent += () => NewCraftingUI.Visible;
		uILinkPage8.IsValidEvent += () => NewCraftingUI.Visible;
		uILinkPage8.OnPageMoveAttempt += OnFancyUIPageMoveAttempt;
		uILinkPage8.PageOnLeft = 8;
		uILinkPage8.PageOnRight = 0;
		UILinkPointNavigator.RegisterPage(uILinkPage8, 24);
		UILinkPage cp2 = new UILinkPage();
		cp2.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		Func<string> value21 = () => PlayerInput.BuildCommand(Lang.misc[94].Value, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
		for (int num32 = 9000; num32 <= 9050; num32++)
		{
			UILinkPoint uILinkPoint15 = new UILinkPoint(num32, enabled: true, num32 + 10, num32 - 10, num32 - 1, num32 + 1);
			cp2.LinkMap.Add(num32, uILinkPoint15);
			uILinkPoint15.OnSpecialInteracts += value21;
		}
		cp2.UpdateEvent += delegate
		{
			int num33 = UILinkPointNavigator.Shortcuts.BUFFS_PER_COLUMN;
			if (num33 == 0)
			{
				num33 = 100;
			}
			for (int num34 = 0; num34 < 50; num34++)
			{
				cp2.LinkMap[9000 + num34].Up = ((num34 % num33 == 0) ? (-1) : (9000 + num34 - 1));
				if (cp2.LinkMap[9000 + num34].Up == -1)
				{
					if (num34 >= num33)
					{
						cp2.LinkMap[9000 + num34].Up = 184;
					}
					else
					{
						cp2.LinkMap[9000 + num34].Up = 189;
					}
				}
				cp2.LinkMap[9000 + num34].Down = (((num34 + 1) % num33 == 0 || num34 == UILinkPointNavigator.Shortcuts.BUFFS_DRAWN - 1) ? 308 : (9000 + num34 + 1));
				cp2.LinkMap[9000 + num34].Left = ((num34 < UILinkPointNavigator.Shortcuts.BUFFS_DRAWN - num33) ? (9000 + num34 + num33) : (-3));
				cp2.LinkMap[9000 + num34].Right = ((num34 < num33) ? (-4) : (9000 + num34 - num33));
			}
		};
		cp2.IsValidEvent += () => Main.playerInventory && Main.EquipPage == 2 && UILinkPointNavigator.Shortcuts.BUFFS_DRAWN > 0;
		cp2.PageOnLeft = 8;
		cp2.PageOnRight = 8;
		UILinkPointNavigator.RegisterPage(cp2, 19);
		UILinkPage uILinkPage9 = new UILinkPage();
		uILinkPage9.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		uILinkPage9.OnSpecialInteractsLate += () => ItemSlot.GetGamepadInstructions(35);
		UILinkPoint value22 = new UILinkPoint(12000, enabled: true, -3, 11001, -1, -2);
		uILinkPage9.LinkMap.Add(12000, value22);
		uILinkPage9.LinkMap[12000].OnSpecialInteracts += delegate
		{
			string text = "";
			if (Main.mouseItem.stack <= 0 || (Main.mouseItem.type == Main.bannerUI.FocusedItemType && Main.mouseItem.stack < Main.mouseItem.maxStack))
			{
				text += PlayerInput.BuildCommand(Language.GetTextValue("UI.GamepadClaimBanner"), PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"], PlayerInput.ProfileGamepadUI.KeyStatus["MouseRight"]);
			}
			return text;
		};
		uILinkPage9.ReachEndEvent += delegate(int current, int next)
		{
			bool value26 = next == -1;
			int yOffset2 = (next == -2).ToInt() - value26.ToInt();
			Main.bannerUI.NavigatePipsList(yOffset2);
		};
		uILinkPage9.EnterEvent += delegate
		{
			Main.PipsUseGrid = false;
			PlayerInput.LockGamepadButtons("MouseLeft");
		};
		uILinkPage9.CanEnterEvent += () => Main.playerInventory && Main.bannerUI.AnyAvailableBanners;
		uILinkPage9.IsValidEvent += () => Main.playerInventory && Main.bannerUI.AnyAvailableBanners;
		uILinkPage9.PageOnLeft = 23;
		uILinkPage9.PageOnRight = 0;
		UILinkPointNavigator.RegisterPage(uILinkPage9, 22);
		UILinkPage cp = new UILinkPage();
		cp.OnSpecialInteracts += () => PlayerInput.BuildCommand(Lang.misc[56].Value, PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]) + PlayerInput.BuildCommand(Lang.misc[64].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"], PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
		cp.OnSpecialInteractsLate += () => ItemSlot.GetGamepadInstructions(35);
		UILinkPoint value23 = new UILinkPoint(11100, enabled: true, -3, -4, -1, -2);
		cp.LinkMap.Add(11100, value23);
		cp.UpdateEvent += delegate
		{
			_ = UILinkPointNavigator.Shortcuts.CRAFT_IconsPerRow;
			_ = UILinkPointNavigator.Shortcuts.CRAFT_IconsPerColumn;
			cp.PageOnLeft = GetCornerWrapPageIdFromLeftToRight();
		};
		cp.ReachEndEvent += delegate(int current, int next)
		{
			bool value24 = next == -3;
			int xOffset = (next == -4).ToInt() - value24.ToInt();
			bool value25 = next == -1;
			int yOffset = (next == -2).ToInt() - value25.ToInt();
			Main.bannerUI.NavigatePipsGrid(xOffset, yOffset);
		};
		cp.EnterEvent += delegate
		{
			Main.PipsUseGrid = true;
			Main.bannerUI.ResetGridSelection();
		};
		cp.LeaveEvent += delegate
		{
			Main.PipsUseGrid = false;
		};
		cp.CanEnterEvent += () => Main.playerInventory && Main.bannerUI.AnyAvailableBanners;
		cp.IsValidEvent += () => Main.playerInventory && Main.PipsUseGrid && Main.bannerUI.AnyAvailableBanners;
		cp.PageOnLeft = 0;
		cp.PageOnRight = 22;
		UILinkPointNavigator.RegisterPage(cp, 23);
		UILinkPage uILinkPage10 = UILinkPointNavigator.Pages[UILinkPointNavigator.CurrentPage];
		uILinkPage10.CurrentPoint = uILinkPage10.DefaultPoint;
		uILinkPage10.Enter();
	}

	private static bool TryQuickCrafting(int startPoint, int pointOffset)
	{
		Player player = Main.player[Main.myPlayer];
		int num = Main.recStart + pointOffset;
		if (num >= MainnumAvailableRecipes)
		{
			return false;
		}
		bool result = false;
		int num2 = num - startPoint;
		Recipe recipe = Main.recipe[Main.availableRecipe[num2]];
		if (Main.mouseItem.type == 0 && recipe.createItem.maxStack > 1 && player.ItemSpace(recipe.createItem).CanTakeItemToPersonalInventory && !player.HasLockedInventory())
		{
			result = true;
			if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.Grapple)
			{
				SomeVarsForUILinkers.SequencedCraftingCurrent = recipe;
			}
			if (CanExecuteInputCommand() && PlayerInput.Triggers.Current.Grapple && Main.stackSplit <= 1)
			{
				ItemSlot.RefreshStackSplitCooldown();
				if (SomeVarsForUILinkers.SequencedCraftingCurrent == recipe)
				{
					CraftingRequests.CraftItem(recipe, 1, quickCraft: true);
				}
			}
		}
		return result;
	}

	private static bool CanEnterCreativeMenu()
	{
		if (Main.LocalPlayer.chest != -1)
		{
			return false;
		}
		if (Main.LocalPlayer.talkNPC != -1)
		{
			return false;
		}
		if (Main.playerInventory)
		{
			return Main.CreativeMenu.Enabled;
		}
		return false;
	}

	private static int GetCornerWrapPageIdFromLeftToRight()
	{
		return 8;
	}

	private static int GetCornerWrapPageIdFromRightToLeft()
	{
		if (Main.CreativeMenu.Enabled)
		{
			return 1005;
		}
		if (NewCraftingUI.Visible)
		{
			return 24;
		}
		if (Main.InPipBanner)
		{
			return 23;
		}
		TileEntity tileEntity = Main.LocalPlayer.tileEntityAnchor.GetTileEntity();
		if (tileEntity is TEDisplayDoll)
		{
			return 20;
		}
		if (tileEntity is TEHatRack)
		{
			return 21;
		}
		return 9;
	}

	private static void OnFancyUIPageMoveAttempt(int direction)
	{
		if (Main.MenuUI.CurrentState is UICharacterCreation uICharacterCreation)
		{
			uICharacterCreation.TryMovingCategory(direction);
		}
		if (UserInterface.ActiveInstance.CurrentState is UIBestiaryTest uIBestiaryTest)
		{
			uIBestiaryTest.TryMovingPages(direction);
		}
	}

	public static void FancyExit()
	{
		switch (UILinkPointNavigator.Shortcuts.BackButtonCommand)
		{
		case 1:
			SoundEngine.PlaySound(11);
			Main.menuMode = 0;
			break;
		case 2:
			SoundEngine.PlaySound(11);
			Main.menuMode = ((!Main.menuMultiplayer) ? 1 : 12);
			break;
		case 3:
			Main.menuMode = 0;
			IngameFancyUI.Close();
			break;
		case 4:
			SoundEngine.PlaySound(11);
			Main.menuMode = 11;
			break;
		case 5:
			SoundEngine.PlaySound(11);
			Main.menuMode = 11;
			break;
		case 6:
			Main.LocalPlayer.releaseInventory = false;
			UIVirtualKeyboard.Cancel();
			break;
		case 7:
			if (Main.MenuUI.CurrentState is IHaveBackButtonCommand haveBackButtonCommand)
			{
				haveBackButtonCommand.HandleBackButtonUsage();
			}
			break;
		}
	}

	public static string FancyUISpecialInstructions()
	{
		string text = "";
		int fANCYUI_SPECIAL_INSTRUCTIONS = UILinkPointNavigator.Shortcuts.FANCYUI_SPECIAL_INSTRUCTIONS;
		if (fANCYUI_SPECIAL_INSTRUCTIONS == 1)
		{
			if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.HotbarMinus)
			{
				UIVirtualKeyboard.CycleSymbols();
				PlayerInput.LockGamepadButtons("HotbarMinus");
				PlayerInput.SettingsForUI.TryRevertingToMouseMode();
			}
			text += PlayerInput.BuildCommand(Lang.menu[235].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"]);
			if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.MouseRight)
			{
				UIVirtualKeyboard.BackSpace();
				PlayerInput.LockGamepadButtons("MouseRight");
				PlayerInput.SettingsForUI.TryRevertingToMouseMode();
			}
			text += PlayerInput.BuildCommand(Lang.menu[236].Value, PlayerInput.ProfileGamepadUI.KeyStatus["MouseRight"]);
			if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.SmartCursor)
			{
				UIVirtualKeyboard.Write(" ");
				PlayerInput.LockGamepadButtons("SmartCursor");
				PlayerInput.SettingsForUI.TryRevertingToMouseMode();
			}
			text += PlayerInput.BuildCommand(Lang.menu[238].Value, PlayerInput.ProfileGamepadUI.KeyStatus["SmartCursor"]);
			if (UIVirtualKeyboard.CanSubmit)
			{
				if (CanExecuteInputCommand() && PlayerInput.Triggers.JustPressed.HotbarPlus)
				{
					UIVirtualKeyboard.Submit();
					PlayerInput.LockGamepadButtons("HotbarPlus");
					PlayerInput.SettingsForUI.TryRevertingToMouseMode();
				}
				text += PlayerInput.BuildCommand(Lang.menu[237].Value, PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]);
			}
		}
		return text;
	}

	public static void HandleOptionsSpecials()
	{
		switch (UILinkPointNavigator.Shortcuts.OPTIONS_BUTTON_SPECIALFEATURE)
		{
		case 1:
			Main.bgScroll = (int)HandleSliderHorizontalInput(Main.bgScroll, 0f, 100f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 1f);
			Main.caveParallax = 1f - (float)Main.bgScroll / 500f;
			break;
		case 2:
			Main.musicVolume = HandleSliderHorizontalInput(Main.musicVolume, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
			break;
		case 3:
			Main.soundVolume = HandleSliderHorizontalInput(Main.soundVolume, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
			break;
		case 4:
			Main.ambientVolume = HandleSliderHorizontalInput(Main.ambientVolume, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
			break;
		case 5:
		{
			float hBar = Main.hBar;
			float num3 = (Main.hBar = HandleSliderHorizontalInput(hBar, 0f, 1f));
			if (hBar != num3)
			{
				switch (Main.menuMode)
				{
				case 17:
					Main.player[Main.myPlayer].hairColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 18:
					Main.player[Main.myPlayer].eyeColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 19:
					Main.player[Main.myPlayer].skinColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 21:
					Main.player[Main.myPlayer].shirtColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 22:
					Main.player[Main.myPlayer].underShirtColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 23:
					Main.player[Main.myPlayer].pantsColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 24:
					Main.player[Main.myPlayer].shoeColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 25:
					Main.mouseColorSlider.Hue = num3;
					break;
				case 252:
					Main.mouseBorderColorSlider.Hue = num3;
					break;
				}
				SoundEngine.PlaySound(12);
			}
			break;
		}
		case 6:
		{
			float sBar = Main.sBar;
			float num2 = (Main.sBar = HandleSliderHorizontalInput(sBar, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX));
			if (sBar != num2)
			{
				switch (Main.menuMode)
				{
				case 17:
					Main.player[Main.myPlayer].hairColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 18:
					Main.player[Main.myPlayer].eyeColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 19:
					Main.player[Main.myPlayer].skinColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 21:
					Main.player[Main.myPlayer].shirtColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 22:
					Main.player[Main.myPlayer].underShirtColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 23:
					Main.player[Main.myPlayer].pantsColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 24:
					Main.player[Main.myPlayer].shoeColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 25:
					Main.mouseColorSlider.Saturation = num2;
					break;
				case 252:
					Main.mouseBorderColorSlider.Saturation = num2;
					break;
				}
				SoundEngine.PlaySound(12);
			}
			break;
		}
		case 7:
		{
			float lBar = Main.lBar;
			float min = 0.15f;
			if (Main.menuMode == 252)
			{
				min = 0f;
			}
			Main.lBar = HandleSliderHorizontalInput(lBar, min, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX);
			float lBar2 = Main.lBar;
			if (lBar != lBar2)
			{
				switch (Main.menuMode)
				{
				case 17:
					Main.player[Main.myPlayer].hairColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 18:
					Main.player[Main.myPlayer].eyeColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 19:
					Main.player[Main.myPlayer].skinColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 21:
					Main.player[Main.myPlayer].shirtColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 22:
					Main.player[Main.myPlayer].underShirtColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 23:
					Main.player[Main.myPlayer].pantsColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 24:
					Main.player[Main.myPlayer].shoeColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					break;
				case 25:
					Main.mouseColorSlider.Luminance = lBar2;
					break;
				case 252:
					Main.mouseBorderColorSlider.Luminance = lBar2;
					break;
				}
				SoundEngine.PlaySound(12);
			}
			break;
		}
		case 8:
		{
			float aBar = Main.aBar;
			float num4 = (Main.aBar = HandleSliderHorizontalInput(aBar, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX));
			if (aBar != num4)
			{
				int menuMode = Main.menuMode;
				if (menuMode == 252)
				{
					Main.mouseBorderColorSlider.Alpha = num4;
				}
				SoundEngine.PlaySound(12);
			}
			break;
		}
		case 9:
		{
			bool left = PlayerInput.Triggers.Current.Left;
			bool right = PlayerInput.Triggers.Current.Right;
			if (PlayerInput.Triggers.JustPressed.Left || PlayerInput.Triggers.JustPressed.Right)
			{
				SomeVarsForUILinkers.HairMoveCD = 0;
			}
			else if (SomeVarsForUILinkers.HairMoveCD > 0)
			{
				SomeVarsForUILinkers.HairMoveCD--;
			}
			if (SomeVarsForUILinkers.HairMoveCD == 0 && (left || right))
			{
				if (left)
				{
					Main.PendingPlayer.hair--;
				}
				if (right)
				{
					Main.PendingPlayer.hair++;
				}
				SomeVarsForUILinkers.HairMoveCD = 12;
			}
			int num = 51;
			if (Main.PendingPlayer.hair >= num)
			{
				Main.PendingPlayer.hair = 0;
			}
			if (Main.PendingPlayer.hair < 0)
			{
				Main.PendingPlayer.hair = num - 1;
			}
			break;
		}
		case 10:
			Main.GameZoomTarget = HandleSliderHorizontalInput(Main.GameZoomTarget, 1f, 2f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
			break;
		case 11:
			Main.UIScale = HandleSliderHorizontalInput(Main.UIScaleWanted, 0.5f, 2f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
			Main.temporaryGUIScaleSlider = Main.UIScaleWanted;
			break;
		case 12:
			Main.MapScale = HandleSliderHorizontalInput(Main.MapScale, 0.5f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.7f);
			break;
		}
	}
}
