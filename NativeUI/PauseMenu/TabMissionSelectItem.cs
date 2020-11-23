using System;
using System.Collections.Generic;
using System.Drawing;
using Font = CitizenFX.Core.UI.Font;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;

namespace NativeUI.PauseMenu
{
    public delegate void OnItemSelect(MissionInformation selectedItem);

    public class MissionInformation
    {
        public MissionInformation(string name, IEnumerable<Tuple<string, string>> info)
        {
            Name = name;
            ValueList = new List<Tuple<string, string>>(info);
        }

        public MissionInformation(string name, string description, IEnumerable<Tuple<string, string>> info)
        {
            Name = name;
            Description = description;
            ValueList = new List<Tuple<string, string>>(info);
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public MissionLogo Logo { get; set; }
        public List<Tuple<string, string>> ValueList { get; set; }
    }

    public class MissionLogo
    {
        /// <summary>
        /// Create a logo from an external picture.
        /// </summary>
        /// <param name="filepath">Path to the picture</param>
        public MissionLogo(string filepath)
        {
            FileName = filepath;
            IsGameTexture = false;
        }

        /// <summary>
        /// Create a mission logo from a game texture.
        /// </summary>
        /// <param name="textureDict">Name of the texture dictionary</param>
        /// <param name="textureName">Name of the texture.</param>
        public MissionLogo(string textureDict, string textureName)
        {
            FileName = textureName;
            DictionaryName = textureDict;
            IsGameTexture = true;
        }

        internal bool IsGameTexture;
        internal string FileName { get; set; }
        internal string DictionaryName { get; set; }
    }

    public class TabMissionSelectItem : TabItem
    {
        protected internal float _add = 0;
        public TabMissionSelectItem(string name, IEnumerable<MissionInformation> list) : base(name)
        {
            base.FadeInWhenFocused = true;
            base.DrawBg = false;

            _noLogo = new Sprite("gtav_online", "rockstarlogo256", new PointF(), new SizeF(512, 256));
            _maxItem = MaxItemsPerView;
            _minItem = 0;

            CanBeFocused = true;

            Heists = new List<MissionInformation>(list);
        }

        public event OnItemSelect OnItemSelect;

        public List<MissionInformation> Heists { get; set; }
        public int Index { get; set; }

        protected const int MaxItemsPerView = 15;
        protected int _minItem;
        protected int _maxItem;
        protected Sprite _noLogo { get; set; }

        public override void ProcessControls()
        {
            if (!Focused) return;
            if (Heists.Count == 0) return;
            if (JustOpened)
            {
                JustOpened = false;
                return;
            }

            if (Game.IsControlJustPressed(0, Control.PhoneSelect))
            {
                Game.PlaySound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                OnItemSelect?.Invoke(Heists[Index]);
            }

            if (Game.IsControlJustPressed(0, Control.FrontendUp) || Game.IsControlJustPressed(0, Control.MoveUpOnly))
            {
                Index = (1000 - (1000 % Heists.Count) + Index - 1) % Heists.Count;
                Game.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");

                if (Heists.Count <= MaxItemsPerView) return;

                if (Index < _minItem)
                {
                    _minItem--;
                    _maxItem--;
                }

                if (Index == Heists.Count - 1)
                {
                    _minItem = Heists.Count - MaxItemsPerView;
                    _maxItem = Heists.Count;
                }
            }

            else if (Game.IsControlJustPressed(0, Control.FrontendDown) || Game.IsControlJustPressed(0, Control.MoveDownOnly))
            {
                Index = (1000 - (1000 % Heists.Count) + Index + 1) % Heists.Count;
                Game.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");

                if (Heists.Count <= MaxItemsPerView) return;

                if (Index >= _maxItem)
                {
                    _maxItem++;
                    _minItem++;
                }

                if (Index == 0)
                {
                    _minItem = 0;
                    _maxItem = MaxItemsPerView;
                }
            }
        }

        public override void Draw()
        {
            base.Draw();
            if (Heists.Count == 0) return;


            var alpha = Focused ? 120 : 30;
            var blackAlpha = Focused ? 200 : 100;
            var fullAlpha = Focused ? 255 : 150;
            var activeWidth = Resolution.Width - SafeSize.X * 2;
            var itemSize = new SizeF((int)activeWidth - (_add + 515), 40);
            var counter = 0;
            for (int i = _minItem; i < Math.Min(Heists.Count, _maxItem); i++)
            {
                new UIResRectangle(SafeSize.AddPoints(new PointF(0, 43 * counter)), itemSize, (Index == i && Focused) ? Color.FromArgb(fullAlpha, Colors.White) : Color.FromArgb(blackAlpha, Colors.Black)).Draw();
                new UIResText(Heists[i].Name, SafeSize.AddPoints(new PointF(6, 5 + 43 * counter)), 0.35f, Color.FromArgb(fullAlpha, (Index == i && Focused) ? Colors.Black : Colors.White)).Draw();
                counter++;
            }

            if (Heists[Index].Logo == null || string.IsNullOrEmpty(Heists[Index].Logo.FileName))
            {
                _noLogo.Position = new PointF((int)Resolution.Width - SafeSize.X - (512 + _add), SafeSize.Y);
                _noLogo.Color = Color.FromArgb(blackAlpha, 0, 0, 0);
                _noLogo.Draw();
            }
            else if (Heists[Index].Logo != null && Heists[Index].Logo.FileName != null && !Heists[Index].Logo.IsGameTexture)
            {
                //var target = Heists[Index].Logo.FileName;
                //Sprite.DrawTexture(target, new PointF((int)Resolution.Width - SafeSize.X - 512, SafeSize.Y), new SizeF(512, 256));
            }
            else if (Heists[Index].Logo != null && Heists[Index].Logo.FileName != null &&
                     Heists[Index].Logo.IsGameTexture)
            {
                var newLogo = new Sprite(Heists[Index].Logo.DictionaryName, Heists[Index].Logo.FileName, new PointF((int)Resolution.Width - SafeSize.X - (512 + _add), SafeSize.Y), new SizeF(512, 256))
                {
                    Color = Color.FromArgb(blackAlpha, 0, 0, 0)
                };
                newLogo.Draw();
            }

            new UIResRectangle(new PointF((int)Resolution.Width - SafeSize.X - (512 + _add), SafeSize.Y + 256), new SizeF(512, 40), Color.FromArgb(fullAlpha, Colors.Black)).Draw();
            new UIResText(Heists[Index].Name, new PointF((int)Resolution.Width - SafeSize.X - (4 + _add), SafeSize.Y + 260), 0.5f, Color.FromArgb(fullAlpha, Colors.White),
                Font.HouseScript, Alignment.Right).Draw();

            for (int i = 0; i < Heists[Index].ValueList.Count; i++)
            {
                new UIResRectangle(new PointF((int)Resolution.Width - SafeSize.X - (512 + _add), SafeSize.Y + 256 + 40 + (40 * i)),
                    new SizeF(512, 40), i % 2 == 0 ? Color.FromArgb(alpha, 0, 0, 0) : Color.FromArgb(blackAlpha, 0, 0, 0)).Draw();
                var text = Heists[Index].ValueList[i].Item1;
                var label = Heists[Index].ValueList[i].Item2;


                new UIResText(text, new PointF((int)Resolution.Width - SafeSize.X - (506 + _add), SafeSize.Y + 260 + 42 + (40 * i)), 0.35f, Color.FromArgb(fullAlpha, Colors.White)).Draw();
                new UIResText(label, new PointF((int)Resolution.Width - SafeSize.X - (6 + _add), SafeSize.Y + 260 + 42 + (40 * i)), 0.35f, Color.FromArgb(fullAlpha, Colors.White), Font.ChaletLondon, Alignment.Right).Draw();
            }

            if (!string.IsNullOrEmpty(Heists[Index].Description))
            {
                var propLen = Heists[Index].ValueList.Count;
                new UIResRectangle(new PointF((int)Resolution.Width - SafeSize.X - (512 + _add), SafeSize.Y + 256 + 42 + 40 * propLen),
                    new SizeF(512, 2), Color.FromArgb(fullAlpha, Colors.White)).Draw();
                new UIResText(Heists[Index].Description,
                    new PointF((int)Resolution.Width - SafeSize.X - (508 + _add), SafeSize.Y + 256 + 45 + 40 * propLen + 4), 0.35f,
                    Color.FromArgb(fullAlpha, Colors.White)) { Wrap = (508 + _add) }.Draw();
                new UIResRectangle(new PointF((int)Resolution.Width - SafeSize.X - (512 + _add), SafeSize.Y + 256 + 44 + 40 * propLen),
                    new SizeF(512, 45 * (int)(ScreenTools.GetTextWidth(Heists[Index].Description, Font.ChaletLondon, 0.35f) / 500)),
                    Color.FromArgb(blackAlpha, 0, 0, 0)).Draw();
            }
        }
    }
}