using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace NativeUI
{
	public class UIMenuProgressItem : UIMenuItem
	{
		protected UIResRectangle _background;
		protected int Items;
		protected bool Pressed;
		protected bool Counter;
		protected float _max = 407.5f;
		protected UIMenuGridAudio Audio;
		protected UIResRectangle _bar;
		protected int _index;
		public int Index
		{
			get { return _index; }
			set
			{
				if (value > Items)
					_index = Items;
				else if (value < 0)
					_index = 0;
				else
					_index = value;
				if (Counter)
					SetRightLabel(_index + "/" + Items);
				else
					SetRightLabel("" + _index);

				UpdateBar();
			}
		}

		protected void UpdateBar()
		{
			_bar.Size = new SizeF(Index *(_max / Items), _bar.Size.Height);
		}

		public event OnProgressChanged OnProgressChanged;
		public event OnProgressSelected OnProgressSelected;

		public UIMenuProgressItem(string text, string description, int maxItems, int index, bool counter) : base(text, description)
		{
			Items = maxItems;
			_index = index;
			Counter = counter;
			_max = 407.5f;
			Audio = new UIMenuGridAudio("CONTINUOUS_SLIDER", "HUD_FRONTEND_DEFAULT_SOUNDSET", 0);
			_background = new UIResRectangle(new PointF(0, 0), new SizeF(415, 14), Color.FromArgb(255, 0, 0, 0));
			_bar = new UIResRectangle(new PointF(0, 0), new SizeF(407.5f, 7.5f));
			if (Counter)
				SetRightLabel(_index + "/" + Items);
			else
				SetRightLabel(""+_index);

			UpdateBar();
		}

		public void ProgressChanged(UIMenu Menu, UIMenuProgressItem Item, int index)
		{
			OnProgressChanged?.Invoke(Menu, Item, index);
		}

		public void ProgressSelected(UIMenu Menu, UIMenuProgressItem Item, int index)
		{
			OnProgressSelected?.Invoke(Menu, Item, index);
		}

		public override void SetRightBadge(BadgeStyle badge)
		{
			throw new Exception("UIMenuProgressItem cannot have a right badge.");
		}

		public override void Position(int y)
		{
			_rectangle.Position = new PointF(Offset.X, y + 144 + Offset.Y);
			_selectedSprite.Position = new PointF(0 + Offset.X, y + 144 + Offset.Y);
			_text.Position = new PointF(8 + Offset.X, y + 141.5f + Offset.Y);

			_labelText.Position = new PointF(420 + Offset.X, y + 141.5f + Offset.Y);

			_max = 407.5f + Parent.WidthOffset;
			_background.Size = new SizeF(415f + Parent.WidthOffset, 14f);
			_background.Position = new PointF(8f + Offset.X, 170f + y + Offset.Y);
			_bar.Position = new PointF(11.75f + Offset.X, 172.5f + y + Offset.Y);
		}

		public void CalculateProgress(float CursorX)
		{
			var Progress = CursorX - _bar.Position.X;
			Index = (int)Math.Round(Items * ((Progress >= 0f && Progress <= _max ) ? Progress : (Progress < 0) ? 0 : _max) / _max);
		}

		public async void Functions()
		{
			if (ScreenTools.IsMouseInBounds(new PointF(_bar.Position.X, _bar.Position.Y - 7.5f), new SizeF(_max, _bar.Size.Height + 19)))
			{
				if (API.IsDisabledControlPressed(0, 24))
				{
					if (!Pressed)
					{
						Pressed = true;
						Audio.Id = API.GetSoundId();
						API.PlaySoundFrontend(Audio.Id, Audio.Slider, Audio.Library, true);
						while (API.IsDisabledControlPressed(0, 24) && ScreenTools.IsMouseInBounds(new PointF(_bar.Position.X, _bar.Position.Y - 7.5f), new SizeF(_max, _bar.Size.Height + 19)))
						{
							await BaseScript.Delay(0);
							float CursorX = API.GetDisabledControlNormal(0, 239) * Resolution.Width;
							CalculateProgress(CursorX);
							Parent.ProgressChange(this, _index);
							ProgressChanged(Parent, this, _index);

						}
						API.StopSound(Audio.Id);
						API.ReleaseSoundId(Audio.Id);
						Pressed = false;
					}
				}
			}
		}

		public async override Task Draw()
		{
			base.Draw();
			if (Selected)
			{
				_background.Color = Colors.Black;
				_bar.Color = Colors.White;
			}
			else
			{
				_background.Color = Colors.White;
				_bar.Color = Colors.Black;
			}
			Functions();
			_background.Draw();
			_bar.Draw();
		}
	}
}
