using System;
using System.Collections.Generic;
using System.Drawing;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using Font = CitizenFX.Core.UI.Font;

namespace NativeUI.PauseMenu
{
    public class TabView
    {
        public TabView(string title)
        {
            Title = title;
            SubTitle = "";
            SideStringTop  = "";
            SideStringMiddle  = "";
            SideStringBottom  = "";
            Tabs = new List<TabItem>();
            Index = 0;
            Name = Game.Player.Name;
            TemporarilyHidden = false;
            CanLeave = true;
        }
        public TabView(string title, string subtitle)
        {
            Title = title;
            SubTitle = subtitle;
            SideStringTop = "";
            SideStringMiddle = "";
            SideStringBottom = "";
            Tabs = new List<TabItem>();
            Index = 0;
            Name = Game.Player.Name;
            TemporarilyHidden = false;
            CanLeave = true;
        }

        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string SideStringTop { get; set; }
        public string SideStringMiddle { get; set; }
        public string SideStringBottom { get; set; }
        public Tuple<string, string> HeaderPicture { internal get; set; }
        public Sprite Photo { get; set; }
        public string Name { get; set; }
        public string Money { get; set; }
        public string MoneySubtitle { get; set; }
        public List<TabItem> Tabs { get; set; }
        public int FocusLevel { get; set; }
        public bool TemporarilyHidden { get; set; }
        public bool CanLeave { get; set; }
        public bool HideTabs { get; set; }
        public bool DisplayHeader = true;
        
        protected readonly SizeF Resolution = ScreenTools.ResolutionMaintainRatio;
        internal bool _loaded;
        internal readonly static string _browseTextLocalized = Game.GetGXTEntry("HUD_INPUT1C");

        public event EventHandler OnMenuClose;

        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (value)
                {
                    API.SetPauseMenuActive(true);
                    Screen.Effects.Start(ScreenEffect.FocusOut, 800);
                    API.TransitionToBlurred(700);
                    InstructionalButtonsHandler.InstructionalButtons.Enabled = true;
                    List<InstructionalButton> buttons = new List<InstructionalButton>()
                    {
                        new InstructionalButton(Control.PhoneSelect, UIMenu._selectTextLocalized),
                        new InstructionalButton(Control.PhoneCancel, UIMenu._backTextLocalized),
                        new InstructionalButton(Control.FrontendRb, ""),
                        new InstructionalButton(Control.FrontendLb, _browseTextLocalized),
                    };
                    InstructionalButtonsHandler.InstructionalButtons.SetInstructionalButtons(buttons);
                }
                else
                {
                    API.SetPauseMenuActive(false);
                    Screen.Effects.Start(ScreenEffect.FocusOut, 500);
                    API.TransitionFromBlurred(400);
                    InstructionalButtonsHandler.InstructionalButtons.Enabled = false;
                }
                _visible = value;
            }
        }
        public void AddTab(TabItem item)
        {
            Tabs.Add(item);
            item.Parent = this;
        }

        public int Index;
        private bool _visible;

        private Scaleform _sc;
        private Scaleform _header;
        public void ShowInstructionalButtons()
        {
            if (_sc == null)
            {
                _sc = new Scaleform("instructional_buttons");
            }

            _sc.CallFunction("CLEAR_ALL");
            _sc.CallFunction("TOGGLE_MOUSE_BUTTONS", 0);
            _sc.CallFunction("CREATE_CONTAINER");


            _sc.CallFunction("SET_DATA_SLOT", 0, API.GetControlInstructionalButton(2, (int)Control.PhoneSelect, 0), UIMenu._selectTextLocalized);
            _sc.CallFunction("SET_DATA_SLOT", 1, API.GetControlInstructionalButton(2, (int)Control.PhoneCancel, 0), UIMenu._backTextLocalized);

            _sc.CallFunction("SET_DATA_SLOT", 2, API.GetControlInstructionalButton(2, (int)Control.FrontendRb, 0), "");
            _sc.CallFunction("SET_DATA_SLOT", 3, API.GetControlInstructionalButton(2, (int)Control.FrontendLb, 0), _browseTextLocalized);
        }

        public async void ShowHeader()
		{
            if (_header == null)
                _header = new Scaleform("pause_menu_header");
            while (!_header.IsLoaded) await BaseScript.Delay(0);
            if (String.IsNullOrEmpty(SubTitle) || String.IsNullOrWhiteSpace(SubTitle))
                _header.CallFunction("SET_HEADER_TITLE", Title);
            else
            {
                _header.CallFunction("SET_HEADER_TITLE", Title, false, SubTitle);
                _header.CallFunction("SHIFT_CORONA_DESC", true);
            }
            if (HeaderPicture != null)
                _header.CallFunction("SET_CHAR_IMG", HeaderPicture.Item1, HeaderPicture.Item2, true);
            else
			{
                int mugshot = API.RegisterPedheadshot(API.PlayerPedId());
                while (!API.IsPedheadshotReady(mugshot)) await BaseScript.Delay(1);
                string Txd = API.GetPedheadshotTxdString(mugshot);
                HeaderPicture = new Tuple<string, string>(Txd, Txd);
                API.ReleasePedheadshotImgUpload(mugshot);
                _header.CallFunction("SET_CHAR_IMG", HeaderPicture.Item1, HeaderPicture.Item2, true);
            }
            _header.CallFunction("SET_HEADING_DETAILS", SideStringTop, SideStringMiddle, SideStringBottom, false);
            _header.CallFunction("BUILD_MENU");
            _header.CallFunction("adjustHeaderPositions");
            _header.CallFunction("SHOW_HEADING_DETAILS", true);
            _header.CallFunction("SHOW_MENU", true);
            _loaded = true;
        }

        public void DrawInstructionalButton(int slot, Control control, string text)
        {
            _sc.CallFunction("SET_DATA_SLOT", slot, API.GetControlInstructionalButton(2, (int)control, 0), text);
        }

        public void ProcessControls()
        {
            if (!Visible || TemporarilyHidden) return;
            API.DisableAllControlActions(0);

            if (Game.IsControlJustPressed(2, Control.PhoneLeft) && FocusLevel == 0)
            {
                Tabs[Index].Active = false;
                Tabs[Index].Focused = false;
                Tabs[Index].Visible = false;
                Index = (1000 - (1000 % Tabs.Count) + Index - 1) % Tabs.Count;
                Tabs[Index].Active = true;
                Tabs[Index].Focused = false;
                Tabs[Index].Visible = true;

                Game.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
            }

            else if (Game.IsControlJustPressed(2, Control.PhoneRight) && FocusLevel == 0)
            {
                Tabs[Index].Active = false;
                Tabs[Index].Focused = false;
                Tabs[Index].Visible = false;
                Index = (1000 - (1000 % Tabs.Count) + Index + 1) % Tabs.Count;
                Tabs[Index].Active = true;
                Tabs[Index].Focused = false;
                Tabs[Index].Visible = true;

                Game.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
            }

            else if (Game.IsControlJustPressed(2, Control.FrontendAccept) && FocusLevel == 0)
            {
                if (Tabs[Index].CanBeFocused)
                {
                    Tabs[Index].Focused = true;
                    Tabs[Index].JustOpened = true;
                    FocusLevel = 1;
                }
                else
                {
                    Tabs[Index].JustOpened = true;
                    Tabs[Index].OnActivated();
                }

                Game.PlaySound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");

            }

            else if (Game.IsControlJustPressed(2, Control.PhoneCancel))
            {
                if (FocusLevel == 1)
                {
                    Tabs[Index].Focused = false;
                    FocusLevel = 0;
                    Game.PlaySound("BACK", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                }
                else if (FocusLevel == 0 && CanLeave)
				{
                    Visible = false;
                    Game.PlaySound("BACK", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                    OnMenuClose?.Invoke(this, EventArgs.Empty);
                    _loaded = false;
                    _header.CallFunction("REMOVE_MENU", true);
                    _header.Dispose();
                    _header = null;
                }
            }

            if (!HideTabs)
            {
                if (Game.IsControlJustPressed(0, Control.FrontendLb))
                {
                    Tabs[Index].Active = false;
                    Tabs[Index].Focused = false;
                    Tabs[Index].Visible = false;
                    Index = (1000 - (1000 % Tabs.Count) + Index - 1) % Tabs.Count;
                    Tabs[Index].Active = true;
                    Tabs[Index].Focused = false;
                    Tabs[Index].Visible = true;

                    FocusLevel = 0;

                    Game.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                }

                else if (Game.IsControlJustPressed(0, Control.FrontendRb))
                {
                    Tabs[Index].Active = false;
                    Tabs[Index].Focused = false;
                    Tabs[Index].Visible = false;
                    Index = (1000 - (1000 % Tabs.Count) + Index + 1) % Tabs.Count;
                    Tabs[Index].Active = true;
                    Tabs[Index].Focused = false;
                    Tabs[Index].Visible = true;

                    FocusLevel = 0;

                    Game.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                }
            }

            if (Tabs.Count > 0) Tabs[Index].ProcessControls();
        }

        public void RefreshIndex()
        {
            foreach (var item in Tabs)
            {
                item.Focused = false;
                item.Active = false;
                item.Visible = false;
            }

            Index = (1000 - (1000 % Tabs.Count)) % Tabs.Count;
            Tabs[Index].Active = true;
            Tabs[Index].Focused = false;
            Tabs[Index].Visible = true;
            FocusLevel = 0;
        }

        public void Draw()
        {
            if (!Visible || TemporarilyHidden) return;
            ShowInstructionalButtons();
            API.HideHudAndRadarThisFrame();
            API.ShowCursorThisFrame();


            var safe = new PointF(300, SubTitle != null && SubTitle != ""? 205 : 195);
            if (!HideTabs)
            {
                /*
                new UIResText(Title, new PointF(safe.X, safe.Y - 80), 1f, Colors.White, Font.ChaletComprimeCologne, Alignment.Left)
                {
                    Shadow = true,
                }.Draw();

                if (Photo == null)
                    new Sprite("char_multiplayer", "char_multiplayer", new PointF((int)Resolution.Width - safe.X - 64, safe.Y - 90), new SizeF(75, 75)).Draw();
                else
                {
                    Photo.Position = new PointF((int)Resolution.Width - safe.X - 100, safe.Y - 90);
                    Photo.Size = new SizeF(75, 75);
                    Photo.Draw();
                }

                new UIResText(Name, new PointF((int)Resolution.Width - safe.X - 106, safe.Y - 98), 0.5f, Colors.White, Font.ChaletComprimeCologne, Alignment.Right)
                {
                    Shadow = true,
                }.Draw();

                string t = Money;
                if (string.IsNullOrEmpty(Money))
                {
                    t = DateTime.Now.ToString();
                }


                new UIResText(t, new PointF((int)Resolution.Width - safe.X - 106, safe.Y - 70), 0.5f, Colors.White,
                    Font.ChaletComprimeCologne, Alignment.Right)
                {
                    Shadow = true,
                }.Draw();

                string subt = MoneySubtitle;
                if (string.IsNullOrEmpty(MoneySubtitle))
                {
                    subt = "";
                }

                new UIResText(subt, new PointF((int)Resolution.Width - safe.X - 106, safe.Y - 44), 0.5f, Colors.White,
                    Font.ChaletComprimeCologne, Alignment.Right)
                {
                    Shadow = true,
                }.Draw();
                */
                for (int i = 0; i < Tabs.Count; i++)
                {
                    var activeSize = Resolution.Width - 2 * safe.X;
                    activeSize -= 5;
                    float tabWidth = ((int)activeSize / Tabs.Count) - 1.95f;
                    Game.EnableControlThisFrame(0, Control.CursorX);
                    Game.EnableControlThisFrame(0, Control.CursorY);

                    var hovering = ScreenTools.IsMouseInBounds(safe.AddPoints(new PointF((tabWidth + 5) * i, 0)),
                        new SizeF(tabWidth, 40));

                    var tabColor = Tabs[i].Active ? Colors.White : hovering ? Color.FromArgb(100, 50, 50, 50) : Colors.Black;
                    new UIResRectangle(safe.AddPoints(new PointF((tabWidth + 5) * i, 0)), new SizeF(tabWidth, 40), Color.FromArgb(Tabs[i].Active ? 255 : 200, tabColor)).Draw();
                    if (Tabs[i].Active)
                        new UIResRectangle(safe.SubtractPoints(new PointF(-((tabWidth + 5) * i), 10)), new SizeF(tabWidth, 10), Colors.DodgerBlue).Draw();

                    new UIResText(Tabs[i].Title.ToUpper(), safe.AddPoints(new PointF((tabWidth / 2) + (tabWidth + 5) * i, 5)), 0.3f, Tabs[i].Active ? Colors.Black : Colors.White, Font.ChaletLondon, Alignment.Center).Draw();

                    if (hovering && Game.IsControlJustPressed(0, Control.CursorAccept) && !Tabs[i].Active)
                    {
                        Tabs[Index].Active = false;
                        Tabs[Index].Focused = false;
                        Tabs[Index].Visible = false;
                        Index = (1000 - (1000 % Tabs.Count) + i) % Tabs.Count;
                        Tabs[Index].Active = true;
                        Tabs[Index].Focused = true;
                        Tabs[Index].Visible = true;
                        Tabs[Index].JustOpened = true;

                        if (Tabs[Index].CanBeFocused)
                            FocusLevel = 1;
                        else
                            FocusLevel = 0;

                        Game.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                    }
                }
            }
            Tabs[Index].Draw();

            _sc.CallFunction("DRAW_INSTRUCTIONAL_BUTTONS", -1);
            _sc.Render2D();
			if (DisplayHeader)
			{
                if(!_loaded)
                    ShowHeader();
                API.DrawScaleformMovie(_header.Handle, 0.501f, 0.162f, 0.6782f, 0.145f, 255, 255, 255, 255, 0);
            }
        }
    }

}