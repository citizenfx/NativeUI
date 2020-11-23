using System;
using System.Collections.Generic;
using System.Drawing;
using Font = CitizenFX.Core.UI.Font;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;

namespace NativeUI.PauseMenu
{
    public class TabInteractiveListItem : TabItem
    {
        public TabInteractiveListItem(string name, IEnumerable<UIMenuItem> items) : base(name)
        {
            DrawBg = false;
            CanBeFocused = true;
            Items = new List<UIMenuItem>(items);
            IsInList = true;
            _maxItem = MaxItemsPerView;
            _minItem = 0;
        }

        public List<UIMenuItem> Items { get; set; }
        public int Index { get; set; }
        public bool IsInList { get; set; }
        protected const int MaxItemsPerView = 15;
        protected int _minItem;
        protected int _maxItem;

        public void MoveDown()
        {
            Index = (1000 - (1000 % Items.Count) + Index + 1) % Items.Count;

            if (Items.Count <= MaxItemsPerView) return;

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

        public void MoveUp()
        {
            Index = (1000 - (1000 % Items.Count) + Index - 1) % Items.Count;

            if (Items.Count <= MaxItemsPerView) return;

            if (Index < _minItem)
            {
                _minItem--;
                _maxItem--;
            }

            if (Index == Items.Count - 1)
            {
                _minItem = Items.Count - MaxItemsPerView;
                _maxItem = Items.Count;
            }
        }

        public void RefreshIndex()
        {
            Index = 0;
            _maxItem = MaxItemsPerView;
            _minItem = 0;
        }


        public override void ProcessControls()
        {
            if (!Visible) return;
            if (JustOpened)
            {
                JustOpened = false;
                return;
            }

            if (!Focused) return;

            if (Items.Count == 0) return;


            if (Game.IsControlJustPressed(0, Control.FrontendAccept) && Focused && Items[Index] is UIMenuCheckboxItem)
            {
                Game.PlaySound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                ((UIMenuCheckboxItem)Items[Index]).Checked = !((UIMenuCheckboxItem)Items[Index]).Checked;
                ((UIMenuCheckboxItem)Items[Index]).CheckboxEventTrigger();
            }
            else if (Game.IsControlJustPressed(0, Control.FrontendAccept) && Focused)
            {
                Game.PlaySound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                Items[Index].ItemActivate(null);
            }

            if (Game.IsControlJustPressed(0, Control.FrontendLeft) && Focused)
            {
                Game.PlaySound("NAV_LEFT_RIGHT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
				if (Items[Index] is UIMenuListItem lIt)
				{
					lIt.Index--;
					lIt.ListChangedTrigger(lIt.Index);
				}
				else if (Items[Index] is UIMenuSliderItem slIt)
				{
                    slIt.Value -= slIt.Multiplier;
                    slIt.SliderChanged(slIt.Value);
                }
                else if (Items[Index] is UIMenuSliderProgressItem slPIt)
                {
                    slPIt.Value --;
                    slPIt.SliderProgressChanged(slPIt.Value);
                }
            }

            if (Game.IsControlJustPressed(0, Control.FrontendRight) && Focused)
            {
                Game.PlaySound("NAV_LEFT_RIGHT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                if (Items[Index] is UIMenuListItem lIt)
                {
                    lIt.Index++;
                    lIt.ListChangedTrigger(lIt.Index);
                }
                else if (Items[Index] is UIMenuSliderItem slIt)
                {
                    slIt.Value += slIt.Multiplier;
                    slIt.SliderChanged(slIt.Value);
                }
                else if (Items[Index] is UIMenuSliderProgressItem slPIt)
                {
                    slPIt.Value++;
                    slPIt.SliderProgressChanged(slPIt.Value);
                }
            }

            if (Game.IsControlJustPressed(0, Control.FrontendUp) || Game.IsControlJustPressed(0, Control.MoveUpOnly) || Game.IsControlJustPressed(0, Control.CursorScrollUp))
            {
                Game.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                MoveUp();
            }

            else if (Game.IsControlJustPressed(0, Control.FrontendDown) || Game.IsControlJustPressed(0, Control.MoveDownOnly) || Game.IsControlJustPressed(0, Control.CursorScrollDown))
            {
                Game.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                MoveDown();
            }
        }

        public override void Draw()
        {
            if (!Visible) return;
            base.Draw();

            var alpha = Focused ? 120 : 30;
            var blackAlpha = Focused ? 200 : 100;
            var fullAlpha = Focused ? 250 : 150;

            var submenuWidth = (BottomRight.X - TopLeft.X);
            var itemSize = new SizeF(submenuWidth, 40);

            int i = 0;
            for (int c = _minItem; c < Math.Min(Items.Count, _maxItem); c++)
            {
                var hovering = ScreenTools.IsMouseInBounds(SafeSize.AddPoints(new PointF(0, (itemSize.Height + 3) * i)),
                    itemSize);

                var hasLeftBadge = Items[c].LeftBadge != UIMenuItem.BadgeStyle.None;
                var hasRightBadge = Items[c].RightBadge != UIMenuItem.BadgeStyle.None;

                var hasBothBadges = hasRightBadge && hasLeftBadge;
                var hasAnyBadge = hasRightBadge || hasLeftBadge;



                new UIResRectangle(SafeSize.AddPoints(new PointF(0, (itemSize.Height + 3) * i)), itemSize, (Index == c && Focused) ? Color.FromArgb(fullAlpha, Colors.White) : Focused && hovering ? Color.FromArgb(100, 50, 50, 50) : Color.FromArgb(blackAlpha, Colors.Black)).Draw();
                new UIResText(Items[c].Text, SafeSize.AddPoints(new PointF((hasBothBadges ? 60 : hasAnyBadge ? 30 : 6), 5 + (itemSize.Height + 3) * i)), 0.35f, Color.FromArgb(fullAlpha, (Index == c && Focused) ? Colors.Black : Colors.White)).Draw();

                if (hasLeftBadge && !hasRightBadge)
                {
                    new Sprite(UIMenuItem.BadgeToSpriteLib(Items[c].LeftBadge),
                        UIMenuItem.BadgeToSpriteName(Items[c].LeftBadge, (Index == c && Focused)), SafeSize.AddPoints(new PointF(-2, 1 + (itemSize.Height + 3) * i)), new SizeF(40, 40), 0f,
                        UIMenuItem.BadgeToColor(Items[c].LeftBadge, (Index == c && Focused))).Draw();
                }

                if (!hasLeftBadge && hasRightBadge)
                {
                    new Sprite(UIMenuItem.BadgeToSpriteLib(Items[c].RightBadge),
                        UIMenuItem.BadgeToSpriteName(Items[c].RightBadge, (Index == c && Focused)), SafeSize.AddPoints(new PointF(-2, 1 + (itemSize.Height + 3) * i)), new SizeF(40, 40), 0f,
                        UIMenuItem.BadgeToColor(Items[c].RightBadge, (Index == c && Focused))).Draw();
                }

                if (hasLeftBadge && hasRightBadge)
                {
                    new Sprite(UIMenuItem.BadgeToSpriteLib(Items[c].LeftBadge),
                        UIMenuItem.BadgeToSpriteName(Items[c].LeftBadge, (Index == c && Focused)), SafeSize.AddPoints(new PointF(-2, 1 + (itemSize.Height + 3) * i)), new SizeF(40, 40), 0f,
                        UIMenuItem.BadgeToColor(Items[c].LeftBadge, (Index == c && Focused))).Draw();

                    new Sprite(UIMenuItem.BadgeToSpriteLib(Items[c].RightBadge),
                        UIMenuItem.BadgeToSpriteName(Items[c].RightBadge, (Index == c && Focused)), SafeSize.AddPoints(new PointF(25, 1 + (itemSize.Height + 3) * i)), new SizeF(40, 40), 0f,
                        UIMenuItem.BadgeToColor(Items[c].RightBadge, (Index == c && Focused))).Draw();
                }

                if (!string.IsNullOrEmpty(Items[c].RightLabel))
                {
                    new UIResText(Items[c].RightLabel,
                        SafeSize.AddPoints(new PointF(BottomRight.X - SafeSize.X - 5, 5 + (itemSize.Height + 3) * i)),
                        0.35f, Color.FromArgb(fullAlpha, (Index == c && Focused) ? Colors.Black : Colors.White),
                        Font.ChaletLondon, Alignment.Right).Draw();
                }

                if (Items[c] is UIMenuCheckboxItem convItem)
                {
                    convItem.Selected = c == Index && Focused;
                    convItem._checkedSprite.Position = SafeSize.AddPoints(new PointF(BottomRight.X - SafeSize.X - 60, -5 + (itemSize.Height + 3) * i));
                    convItem._checkedSprite.TextureName = convItem.Selected ? (convItem.Checked ? (convItem.Style == UIMenuCheckboxStyle.Tick ? "shop_box_tickb" : "shop_box_crossb") : "shop_box_blankb") : convItem.Checked ? (convItem.Style == UIMenuCheckboxStyle.Tick ? "shop_box_tick" : "shop_box_cross") : "shop_box_blank";
                    convItem._checkedSprite.Draw();
                }
                else if (Items[c] is UIMenuListItem listItem)
                {
                    var yoffset = 5;
                    var basePos =
                        SafeSize.AddPoints(new PointF(BottomRight.X - SafeSize.X - 30, yoffset + (itemSize.Height + 3) * i));

                    listItem._arrowLeft.Position = basePos;
                    listItem._arrowRight.Position = basePos;
                    listItem._itemText.Position = basePos;
                    listItem._itemText.Color = Colors.White;
                    listItem._itemText.Font = Font.ChaletLondon;
                    listItem._itemText.Alignment = Alignment.Left;
                    listItem._itemText.TextAlignment = Alignment.Right;

                    string caption = listItem.Items[listItem.Index].ToString();
					float offset = ScreenTools.GetTextWidth(caption, listItem._itemText.Font, listItem._itemText.Scale);

                    var selected = c == Index && Focused;
                    listItem.Selected = selected;

                    listItem._itemText.Color = listItem.Enabled ? listItem.Selected ? Colors.Black : Colors.WhiteSmoke : Color.FromArgb(163, 159, 148);

                    listItem._itemText.Caption = caption;

                    listItem._arrowLeft.Color = listItem.Enabled ? listItem.Selected ? Colors.Black : Colors.WhiteSmoke : Color.FromArgb(163, 159, 148);
                    listItem._arrowRight.Color = listItem.Enabled ? listItem.Selected ? Colors.Black : Colors.WhiteSmoke : Color.FromArgb(163, 159, 148);

                    listItem._arrowLeft.Position = SafeSize.AddPoints(new PointF(BottomRight.X - SafeSize.X - 60 - (int)offset, yoffset + (itemSize.Height + 3) * i));
                    if (listItem.Selected)
                    {
                        listItem._arrowLeft.Draw();
                        listItem._arrowRight.Draw();
                        listItem._itemText.Position = SafeSize.AddPoints(new PointF(BottomRight.X - SafeSize.X - 30, yoffset + (itemSize.Height + 3) * i));
                    }
                    else
                        listItem._itemText.Position = SafeSize.AddPoints(new PointF(BottomRight.X - SafeSize.X - 5, yoffset + (itemSize.Height + 3) * i));
                    listItem._itemText.Draw();
                }
                else if (Items[c] is UIMenuSliderItem sliderItem)
                {
                    var yoffset = 15;
                    var basePos = SafeSize.AddPoints(new PointF(BottomRight.X - SafeSize.X - 210, yoffset + (itemSize.Height + 3) * i));
                    sliderItem._rectangleBackground.Position = basePos;
                    sliderItem._rectangleBackground.Size = new SizeF(200f, 10);

                    sliderItem._rectangleSlider.Position = basePos;
                    sliderItem._rectangleSlider.Size = new SizeF(100f, 10);

                    sliderItem._rectangleDivider.Position = SafeSize.AddPoints(new PointF(BottomRight.X - SafeSize.X - 110, (yoffset - 5) + (itemSize.Height + 3) * i));
                    sliderItem._rectangleDivider.Size = new SizeF(2, 20);
                    if (sliderItem.Divider)
                        sliderItem._rectangleDivider.Color = Colors.WhiteSmoke;
                    sliderItem.Selected = c == Index && Focused;

                    sliderItem._rectangleSlider.Position = new PointF(basePos.X + (sliderItem._value / (float)sliderItem._max * 100f), sliderItem._rectangleSlider.Position.Y);
                    sliderItem._rectangleBackground.Draw();
                    sliderItem._rectangleSlider.Draw();
                    if (sliderItem.Divider)
                        sliderItem._rectangleDivider.Draw();
                }
                else if (Items[c] is UIMenuSliderProgressItem sliderProgressItem)
                {
                    var yoffset = 15;
                    var basePos = SafeSize.AddPoints(new PointF(BottomRight.X - SafeSize.X - 210, yoffset + (itemSize.Height + 3) * i));
                    sliderProgressItem._rectangleBackground.Position = basePos;
                    sliderProgressItem._rectangleBackground.Size = new SizeF(200, 10);

                    sliderProgressItem._rectangleSlider.Position = basePos;

                    sliderProgressItem._rectangleDivider.Position = SafeSize.AddPoints(new PointF(BottomRight.X - SafeSize.X - 100, (yoffset - 5) + (itemSize.Height + 3f) * i));
                    sliderProgressItem._rectangleDivider.Size = new SizeF(2, 20);

                    if (sliderProgressItem.Divider)
                        sliderProgressItem._rectangleDivider.Color = Colors.WhiteSmoke;
                    sliderProgressItem.Selected = c == Index && Focused;

                    sliderProgressItem._rectangleBackground.Draw();
                    sliderProgressItem._rectangleSlider.Draw();
                    sliderProgressItem._rectangleDivider.Draw();

                    if (ScreenTools.IsMouseInBounds(new PointF(sliderProgressItem._rectangleBackground.Position.X, sliderProgressItem._rectangleBackground.Position.Y-5), new SizeF(200f, sliderProgressItem._rectangleBackground.Size.Height)))
                    {
                        if (API.IsDisabledControlPressed(0, 24))
                        {
                            if (!sliderProgressItem.Pressed)
                            {
                                sliderProgressItem.Pressed = true;
                                sliderProgressItem.Audio.Id = API.GetSoundId();
                                API.PlaySoundFrontend(sliderProgressItem.Audio.Id, sliderProgressItem.Audio.Slider, sliderProgressItem.Audio.Library, true);
                            }
                            float CursorX = API.GetDisabledControlNormal(0, 239) * Resolution.Width;
                            var Progress = CursorX - sliderProgressItem._rectangleSlider.Position.X;
                            sliderProgressItem.Value = (int)Math.Round(sliderProgressItem._max * ((Progress >= 0f && Progress <= 200f) ? Progress : (Progress < 0) ? 0 : 200f) / 200f);
                            sliderProgressItem.SliderProgressChanged(sliderProgressItem.Value);
                        }
                        else
                        {
                            API.StopSound(sliderProgressItem.Audio.Id);
                            API.ReleaseSoundId(sliderProgressItem.Audio.Id);
                            sliderProgressItem.Pressed = false;
                        }
                    }
                    else
                    {
                        API.StopSound(sliderProgressItem.Audio.Id);
                        API.ReleaseSoundId(sliderProgressItem.Audio.Id);
                        sliderProgressItem.Pressed = false;
                    }
                }

                if (Focused && hovering && Game.IsControlJustPressed(0, Control.CursorAccept))
                {
                    bool open = Index == c;
                    Index = (1000 - (1000 % Items.Count) + c) % Items.Count;
                    if (!open)
                        Game.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                    else
                    {
                        if (Items[Index] is UIMenuCheckboxItem)
                        {
                            Game.PlaySound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                            ((UIMenuCheckboxItem)Items[Index]).Checked = !((UIMenuCheckboxItem)Items[Index]).Checked;
                            ((UIMenuCheckboxItem)Items[Index]).CheckboxEventTrigger();
                        }
                        else
                        {
                            Game.PlaySound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                            Items[Index].ItemActivate(null);
                        }
                    }
                }
                i++;
            }
        }
    }
}