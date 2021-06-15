using System;
using System.Drawing;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using NativeUI;
using NativeUI.PauseMenu;
using CitizenFX.Core.Native;
using System.Linq;

public class MenuExample : BaseScript
{
	private bool ketchup = false;
	private string dish = "Banana";
	private MenuPool _menuPool;

	public void AddMenuKetchup(UIMenu menu)
	{
		var newitem = new UIMenuCheckboxItem("Add ketchup?", UIMenuCheckboxStyle.Cross, ketchup, "Do you wish to add ketchup?");
		menu.AddItem(newitem);
		menu.OnCheckboxChange += (sender, item, checked_) =>
		{
			if (item == newitem)
			{
				ketchup = checked_;
				Screen.ShowNotification("~r~Ketchup status: ~b~" + ketchup);
			}
		};
	}

	public void AddMenuFoods(UIMenu menu)
	{
		var foods = new List<dynamic>
		{
			"Banana",
			"Apple",
			"Pizza",
			"Quartilicious",
			0xF00D, // Dynamic!
        };
		var newitem = new UIMenuListItem("Food", foods, 0);
		menu.AddItem(newitem);
		menu.OnListChange += (sender, item, index) =>
		{
			if (item == newitem)
			{
				dish = item.Items[index].ToString();
				Screen.ShowNotification("Preparing ~b~" + dish + "~w~...");
			}

		};
	}

	public void HeritageMenu(UIMenu menu)
	{
		var heritagemenu = _menuPool.AddSubMenu(menu, "Heritage Menu");
		var heritageWindow = new UIMenuHeritageWindow(0, 0);
		heritagemenu.AddWindow(heritageWindow);
		List<dynamic> momfaces = new List<dynamic>() { "Hannah", "Audrey", "Jasmine", "Giselle", "Amelia", "Isabella", "Zoe", "Ava", "Camilla", "Violet", "Sophia", "Eveline", "Nicole", "Ashley", "Grace", "Brianna", "Natalie", "Olivia", "Elizabeth", "Charlotte", "Emma", "Misty" };
		List<dynamic> dadfaces = new List<dynamic>() { "Benjamin", "Daniel", "Joshua", "Noah", "Andrew", "Joan", "Alex", "Isaac", "Evan", "Ethan", "Vincent", "Angel", "Diego", "Adrian", "Gabriel", "Michael", "Santiago", "Kevin", "Louis", "Samuel", "Anthony", "Claude", "Niko", "John" };
		List<dynamic> lista = new List<dynamic>();
		for (int i = 0; i < 101; i++) lista.Add(i);
		var mom = new UIMenuListItem("Mamma", momfaces, 0);
		var dad = new UIMenuListItem("Papà", dadfaces, 0);
		var newItem = new UIMenuSliderHeritageItem("Heritage Slider", "This is Useful on heritage", true);
		heritagemenu.AddItem(mom);
		heritagemenu.AddItem(dad);
		heritagemenu.AddItem(newItem);
		int MomIndex = 0;
		int DadIndex = 0;
		heritagemenu.OnListChange += (_sender, _listItem, _newIndex) =>
		{
			if (_listItem == mom)
			{
				MomIndex = _newIndex;
				heritageWindow.Index(MomIndex, DadIndex);
			}
			else if (_listItem == dad)
			{
				DadIndex = _newIndex;
				heritageWindow.Index(MomIndex, DadIndex);
			}
			// This way the heritage window changes only if you change a list item!
		};

		heritagemenu.OnSliderChange += (_sender, _item, _newIndex) =>
		{
			if (_item == newItem)
			{
				Screen.ShowNotification("Wow the slider changed! Who do i look like??");
			}
		};
	}

	public void AddScaleformMenu(UIMenu menu)
	{
		var scaleformMenu = _menuPool.AddSubMenu(menu, "Scaleforms Showdown");
		UIMenuItem showSimplePopup = new UIMenuItem("Show PopupWarning example", "You can customize it to your needs");
		UIMenuItem showPopupButtons = new UIMenuItem("Show PopupWarning with buttons", "It waits until a button has been pressed!");
		UIMenuListItem customInstr = new UIMenuListItem("SavingNotification", Enum.GetNames(typeof(LoadingSpinnerType)).Cast<dynamic>().ToList(), 0, "InstructionalButtons now give you the ability to dynamically edit, add, remove, customize your buttons, you can even use them outside the menu ~y~without having to run multiple instances of the same scaleform~w~, aren't you happy??");
		UIMenuItem customInstr2 = new UIMenuItem("Add a random InstructionalButton!", "InstructionalButtons now give you the ability to dynamically edit, add, remove, customize your buttons, you can even use them outside the menu ~y~without having to run multiple instances of the same scaleform~w~, aren't you happy??");
		scaleformMenu.AddItem(showSimplePopup);
		scaleformMenu.AddItem(showPopupButtons);
		scaleformMenu.AddItem(customInstr);
		scaleformMenu.AddItem(customInstr2);

		scaleformMenu.OnItemSelect += async (sender, item, index) =>
		{
			if(item == showSimplePopup)
			{
				PopupWarningThread.Warning.ShowWarning("This is the title", "This is the subtitle", "This is the prompt.. you have 6 seconds left", "This is the error message, NativeUI Ver. 3.0");
				await Delay(1000);
				for (int i=5; i > -1; i--)
				{
					PopupWarningThread.Warning.UpdateWarning("This is the title", "This is the subtitle", $"This is the prompt.. you have {i} seconds left", "This is the error message, NativeUI Ver. 3.0");
					await Delay(1000);
				}
				PopupWarningThread.Warning.Dispose();
			}
			else if (item == showPopupButtons)
			{
				List<InstructionalButton> buttons = new List<InstructionalButton>()
				{
					new InstructionalButton(Control.FrontendAccept, "Accept only with Keyboard", PadCheck.Keyboard),
					new InstructionalButton(Control.FrontendY, "Cancel only with GamePad", PadCheck.Controller),
					new InstructionalButton(Control.FrontendX, Control.Detonate, "This will change button if you're using gamepad or keyboard"),
					new InstructionalButton(new List<Control> { Control.MoveUpOnly, Control.MoveLeftOnly , Control.MoveDownOnly , Control.MoveRightOnly }, "Woow multiple buttons at once??")
				};
				PopupWarningThread.Warning.ShowWarningWithButtons("This is the title", "This is the subtitle", "This is the prompt, press any button", buttons, "This is the error message, NativeUI Ver. 3.0");
				PopupWarningThread.Warning.OnButtonPressed += (button) =>
				{
					Debug.WriteLine($"You pressed a Button => {button.Text}");
				};
			}
			else if (item == customInstr2)
			{
				if (InstructionalButtonsHandler.InstructionalButtons.ControlButtons.Count >= 6) return;
				InstructionalButtonsHandler.InstructionalButtons.AddInstructionalButton(new InstructionalButton((Control)new Random().Next(0, 250), "I'm a new button look at me!"));
			}
		};

		customInstr.OnListSelected += (item, index) =>
		{
			if (InstructionalButtonsHandler.InstructionalButtons.IsSaving) return;
			InstructionalButtonsHandler.InstructionalButtons.AddSavingText((LoadingSpinnerType)(index + 1), "I'm a saving text", 3000);
		};
	}

	public void AddMenuCook(UIMenu menu)
	{
		var newitem = new UIMenuItem("Cook!", "Cook the dish with the appropiate ingredients and ketchup.");
		newitem.SetLeftBadge(UIMenuItem.BadgeStyle.Star);
		newitem.SetRightBadge(UIMenuItem.BadgeStyle.Tick);
		menu.AddItem(newitem);
		menu.OnItemSelect += (sender, item, index) =>
		{
			if (item == newitem)
			{
				string output = ketchup ? "You have ordered ~b~{0}~w~ ~r~with~w~ ketchup." : "You have ordered ~b~{0}~w~ ~r~without~w~ ketchup.";
				Screen.ShowSubtitle(String.Format(output, dish));
			}
		};

		menu.OnIndexChange += (sender, index) =>
		{
			if (sender.MenuItems[index] == newitem)
				newitem.SetLeftBadge(UIMenuItem.BadgeStyle.None);
		};

		var colorItem = new UIMenuItem("UIMenuItem with Colors", "~b~Look!!~r~I can be colored ~y~too!!~w~", Color.FromArgb(150, 185, 230, 185), Color.FromArgb(170, 174, 219, 242));
		menu.AddItem(colorItem);

		var foods = new List<dynamic>
		{
			"Banana",
			"Apple",
			"Pizza",
			"Quartilicious",
			0xF00D, // Dynamic!
        };

		var BlankItem = new UIMenuSeparatorItem();
		menu.AddItem(BlankItem);

		var colorListItem = new UIMenuListItem("Colored ListItem.. Really?", foods, 0, "~b~Look!!~r~I can be colored ~y~too!!~w~", Color.FromArgb(150, 185, 230, 185), Color.FromArgb(170, 174, 219, 242));
		menu.AddItem(colorListItem);

		var Slider = new UIMenuSliderItem("Slider Item", "Cool!", true); // by default max is 100 and multipler 5 = 20 steps.
		menu.AddItem(Slider);

		var SliderProgress = new UIMenuSliderProgressItem("Slider Progress Item", 10, 0);
		menu.AddItem(SliderProgress);

		var Progress = new UIMenuProgressItem("Progress Item", "descriptiom", 10 , 0, true);
		menu.AddItem(Progress);

		var listPanelItem1 = new UIMenuListItem("Change Color", new List<object> { "Example", "example2" }, 0);
		var ColorPanel = new UIMenuColorPanel("Color Panel Example", UIMenuColorPanel.ColorPanelType.Hair);
		// you can choose between hair palette or makeup palette
		menu.AddItem(listPanelItem1);
		listPanelItem1.AddPanel(ColorPanel);

		var listPanelItem2 = new UIMenuListItem("Change Percentage", new List<object> { "Example", "example2" }, 0);
		var PercentagePanel = new UIMenuPercentagePanel("Percentage Panel Example", "0%", "100%");
		// You can change every text in this Panel
		menu.AddItem(listPanelItem2);
		listPanelItem2.AddPanel(PercentagePanel);

		var listPanelItem3 = new UIMenuListItem("Change Grid Position", new List<object> { "Example", "example2" }, 0);
		var GridPanel = new UIMenuGridPanel("Up", "Left", "Right", "Down", new System.Drawing.PointF(.5f, .5f));
		// you can choose the text in every position and where to place the starting position of the cirlce
		// the other 2 grids panel available are not listed as they work the same way but in only 2 direction (horizontally or vertically)
		// and to use them you must stream the stream folder provided with NativeUI project
		menu.AddItem(listPanelItem3);
		listPanelItem3.AddPanel(GridPanel);

		var listPanelItem4 = new UIMenuListItem("Look at Statistics", new List<object> { "Example", "example2" }, 0);
		var statistics = new UIMenuStatisticsPanel();
		statistics.AddStatistics("Look at this!");
		statistics.AddStatistics("I'm a statistic too!");
		statistics.AddStatistics("Am i not?!");
		//you can add as menu statistics you want 
		statistics.SetPercentage(0, 10f);
		statistics.SetPercentage(1, 50f);
		statistics.SetPercentage(2, 100f);
		//and you can get / set their percentage
		menu.AddItem(listPanelItem4);
		listPanelItem4.AddPanel(statistics);

		UIMenuItem PauseMenu = new UIMenuItem("Open custom pauseMenu");
		menu.AddItem(PauseMenu);
		PauseMenu.Activated += (_submenu, item) =>
		{
			_menuPool.CloseAllMenus();
			OpenCustomPauseMenu();
		};

		// THERE ARE NO EVENTS FOR PANELS.. WHEN YOU CHANGE WHAT IS CHANGABLE THE LISTITEM WILL DO WHATEVER YOU TELL HIM TO DO

		menu.OnListChange += (sender, item, index) =>
		{
			if (item == listPanelItem1)
			{
				Screen.ShowNotification("Selected color " + ((item.Panels[0] as UIMenuColorPanel).CurrentSelection + 1) + "...");
				item.Description = "Selected color " + ((item.Panels[0] as UIMenuColorPanel).CurrentSelection + 1) + "...";
				item.Parent.UpdateDescription(); // this is neat.. this will update the description of the item without refresh index.. try it by changing color
			}
			else if (item == listPanelItem2)
			{
				Screen.ShowSubtitle("Percentage = " + (item.Panels[0] as UIMenuPercentagePanel).Percentage + "...");
			}
			else if (item == listPanelItem3)
			{
				Screen.ShowSubtitle("GridPosition = " + (item.Panels[0] as UIMenuGridPanel).CirclePosition + "...");
			}
		};
	}

	public void AddMenuAnotherMenu(UIMenu menu)
	{
		var submenu = _menuPool.AddSubMenu(menu, "Another Menu");
		for (int i = 0; i < 20; i++)
			submenu.AddItem(new UIMenuItem("PageFiller", "Sample description that takes more than one line. Moreso, it takes way more than two lines since it's so long. Wow, check out this length!"));
	}

	public void HandleMenuEvents(UIMenu menu)
	{
		menu.OnMenuStateChanged += (oldMenu, newMenu, state) =>
		{
			if(state == MenuState.Opened)
			{
				Debug.WriteLine($"{newMenu.Title.Caption} just opened!");
			}
			else if (state == MenuState.ChangeForward)
			{
				Debug.WriteLine($"{oldMenu.Title.Caption} => {newMenu.Title.Caption}");
			}
			else if (state == MenuState.ChangeBackward)
			{
				Debug.WriteLine($"{newMenu.Title.Caption} <= {oldMenu.Title.Caption}");
			}
			else if (state == MenuState.Closed)
			{
				Debug.WriteLine($"{oldMenu.Title.Caption} just closed!");
			}
		};
	}


	public MenuExample()
	{
		_menuPool = new MenuPool();
		var mainMenu = new UIMenu("Native UI", "~b~NATIVEUI SHOWCASE", true); // true means add menu Glare scaleform to the menu
		_menuPool.Add(mainMenu);
		HeritageMenu(mainMenu);
		AddScaleformMenu(mainMenu);
		AddMenuKetchup(mainMenu);
		AddMenuFoods(mainMenu);
		AddMenuCook(mainMenu);
		AddMenuAnotherMenu(mainMenu);
		HandleMenuEvents(mainMenu);
		_menuPool.RefreshIndex();

		Tick += async () =>
		{
			_menuPool.ProcessMenus();
			if (Game.IsControlJustPressed(0, Control.SelectCharacterMichael)) // Our menu on/off switch
				mainMenu.Visible = !mainMenu.Visible;
		};
	}

	public async void OpenCustomPauseMenu()
	{
		TabView MenuContainer = new TabView("This is the title", "Subtitle");
		MenuContainer.SideStringTop = "Player_Name";
		MenuContainer.SideStringMiddle = "Middle_String";
		MenuContainer.SideStringBottom = "Bottom_string";
		MenuContainer.DisplayHeader = true;
		_menuPool.AddPauseMenu(MenuContainer);


		TabItem Item1 = new TabItem("simple TabItem");

		TabTextItem Item2 = new TabTextItem("TabTextItem", "This is the Title inside", "With a cool text to be added where you can write whatever you want");

		TabItemSimpleList Item3 = new TabItemSimpleList("TabItemSimpleList", new Dictionary<string, string>()
		{
			["Item 1"] = "subItem 1",
			["Item 2"] = "subItem 2",
			["Item 3"] = "subItem 3",
			["Item 4"] = "subItem 4",
			["Item 5"] = "subItem 5",
			["Item 6"] = "subItem 6"
		});


		List<UIMenuItem> items = new List<UIMenuItem>()
		{
			new UIMenuItem("Item 1"),
			new UIMenuCheckboxItem("Item 2", true),
			new UIMenuListItem("Item 3", new List<dynamic>(){"Item1", 2, 3.0999 }, 0),
			new UIMenuSliderItem("Item 4", "", true),
			new UIMenuSliderProgressItem("Item 5", 20, 0),
		};

		TabInteractiveListItem Item4 = new TabInteractiveListItem("TabInteractiveListItem", items);
		List<MissionInformation> info = new List<MissionInformation>()
		{
			new MissionInformation("Mission 1", new List<Tuple<string, string>>()
			{
				new Tuple<string, string>("item 1", "description 1"),
				new Tuple<string, string>("item 2", "description 2"),
				new Tuple<string, string>("item 3", "description 3"),
				new Tuple<string, string>("item 4", "description 4"),
				new Tuple<string, string>("item 5", "description 5"),
			}),
			new MissionInformation("Mission 2", new List<Tuple<string, string>>()
			{
				new Tuple<string, string>("item 1", "description 1"),
				new Tuple<string, string>("item 2", "description 2"),
				new Tuple<string, string>("item 3", "description 3"),
				new Tuple<string, string>("item 4", "description 4"),
				new Tuple<string, string>("item 5", "description 5"),
			}),
		};
		TabSubmenuItem Item5 = new TabSubmenuItem("TabSubmenuItem", new List<TabItem>()
		{
			new TabItem("simple TabItem"),
			new TabTextItem("TabTextItem", "This is the Title inside", "With a cool text to be added where you can write whatever you want"),
			new TabItemSimpleList("TabItemSimpleList", new Dictionary<string, string>()
			{
				["Item 1"] = "subItem 1",
				["Item 2"] = "subItem 2",
				["Item 3"] = "subItem 3",
				["Item 4"] = "subItem 4",
				["Item 5"] = "subItem 5",
				["Item 6"] = "subItem 6"
			}),
			new TabMissionSelectItem("Mission tab", info),
			new TabInteractiveListItem("TabInteractiveListItem", items)
		});
		TabMissionSelectItem Item6 = new TabMissionSelectItem("Mission tab", info);

		MenuContainer.AddTab(Item1);
		MenuContainer.AddTab(Item2);
		MenuContainer.AddTab(Item3);
		MenuContainer.AddTab(Item4);
		MenuContainer.AddTab(Item5);
		MenuContainer.AddTab(Item6);
		// this way we can choose which tab is the defualt one
		Item1.Active = true;
		Item1.Focused = true;
		Item1.Visible = true;
		MenuContainer.Visible = true;
		// items have events exactly the same as UIMenuItems and you can handle TabInteractiveListItem items just like that
	}
}
