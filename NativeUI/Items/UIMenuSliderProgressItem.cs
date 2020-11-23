using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NativeUI
{
	public class UIMenuSliderProgressItem : UIMenuItem
	{
		protected internal Sprite _arrowLeft;
		protected internal Sprite _arrowRight;
		protected internal bool Pressed;
		protected internal UIMenuGridAudio Audio;
		protected internal UIResRectangle _rectangleBackground;
		protected internal UIResRectangle _rectangleSlider;
		protected internal UIResRectangle _rectangleDivider;

		protected internal int _value = 0;
		protected internal int _max;
		protected internal int _multiplier = 5;
		protected internal bool Divider;
		protected internal Color SliderColor;
		protected internal Color BackgroundSliderColor;

		public UIMenuSliderProgressItem(string text, int maxCount, int startIndex, bool divider = false) : this(text, maxCount, startIndex, "", divider)
		{
			_max = maxCount;
			_value = startIndex;
		}

		public UIMenuSliderProgressItem(string text, int maxCount, int startIndex, string description, bool divider = false) : this(text, maxCount, startIndex, description, Color.FromArgb(255, 57, 119, 200), Color.FromArgb(255, 4, 32, 57), divider)
		{
			_max = maxCount;
			_value = startIndex;
		}

		public UIMenuSliderProgressItem(string text, int maxCount, int startIndex, string description, Color sliderColor, Color backgroundSliderColor, bool divider = false) : base(text, description)
		{
			_max = maxCount;
			_value = startIndex;
			_arrowLeft = new Sprite("commonmenu", "arrowleft", new PointF(0, 105), new SizeF(25, 25));
			_arrowRight = new Sprite("commonmenu", "arrowright", new PointF(0, 105), new SizeF(25, 25));
			SliderColor = sliderColor;
			BackgroundSliderColor = backgroundSliderColor;
			_rectangleBackground = new UIResRectangle(new PointF(0, 0), new SizeF(150, 10), BackgroundSliderColor);
			_rectangleSlider = new UIResRectangle(new PointF(0, 0), new SizeF(75, 10), SliderColor);
			if (divider)
				_rectangleDivider = new UIResRectangle(new Point(0, 0), new Size(2, 20), Colors.WhiteSmoke);
			else
				_rectangleDivider = new UIResRectangle(new Point(0, 0), new Size(2, 20), Color.Transparent);
			float offset = _rectangleBackground.Size.Width / _max * _value;
			_rectangleSlider.Size = new SizeF(offset, _rectangleSlider.Size.Height);
			Audio = new UIMenuGridAudio("CONTINUOUS_SLIDER", "HUD_FRONTEND_DEFAULT_SOUNDSET", 0);
		}

		public override void Position(int y)
		{
			base.Position(y);
			_rectangleBackground.Position = new PointF(250f + base.Offset.X + Parent.WidthOffset, y + 158.5f + base.Offset.Y);
			_rectangleSlider.Position = new PointF(250f + base.Offset.X + Parent.WidthOffset, y + 158.5f + base.Offset.Y);
			_rectangleDivider.Position = new PointF(323.5f + base.Offset.X + Parent.WidthOffset, y + 153 + base.Offset.Y);
			_arrowLeft.Position = new PointF(225 + base.Offset.X + Parent.WidthOffset, y + 150.5f + base.Offset.Y);
			_arrowRight.Position = new PointF(400 + base.Offset.X + Parent.WidthOffset, y + 150.5f + base.Offset.Y);
		}

		public int Value
		{
			get
			{
				float offset = _rectangleBackground.Size.Width / _max * _value;
				_rectangleSlider.Size = new SizeF(offset, _rectangleSlider.Size.Height);
				return _value;
			}
			set
			{
				if (value > _max)
					_value = _max;
				else if (value < 0)
					_value = 0;
				else
					_value = value;
				SliderProgressChanged(Value);
			}
		}

		public int Multiplier
		{
			get => _multiplier;
			set =>_multiplier = value;
		}

		/// <summary>
		/// Triggered when the slider is changed.
		/// </summary>
		public event ItemSliderProgressEvent OnSliderChanged;

		internal virtual void SliderProgressChanged(int value)
		{
			OnSliderChanged?.Invoke(this, value);
		}

		public async void Functions()
		{
			if (ScreenTools.IsMouseInBounds(new PointF(_rectangleBackground.Position.X, _rectangleBackground.Position.Y), new SizeF(150f, _rectangleBackground.Size.Height)))
			{
				if (API.IsDisabledControlPressed(0, 24))
				{
					if (!Pressed)
					{
						Pressed = true;
						Audio.Id = API.GetSoundId();
						API.PlaySoundFrontend(Audio.Id, Audio.Slider, Audio.Library, true);
					}
							await BaseScript.Delay(0);
							float CursorX = API.GetDisabledControlNormal(0, 239) * Resolution.Width;
							var Progress = CursorX - _rectangleSlider.Position.X;
							Value = (int)Math.Round(_max * ((Progress >= 0f && Progress <= 150f) ? Progress : (Progress < 0) ? 0 : 150f) / 150f);
							SliderProgressChanged(Value);
				}
				else
				{
					API.StopSound(Audio.Id);
					API.ReleaseSoundId(Audio.Id);
					Pressed = false;
				}
			}
			else if (ScreenTools.IsMouseInBounds(_arrowLeft.Position, _arrowLeft.Size))
			{
				if (API.IsDisabledControlPressed(0, 24))
				{
					Value -= Multiplier;
					SliderProgressChanged(Value);
				}
			}
			else if (ScreenTools.IsMouseInBounds(_arrowRight.Position, _arrowRight.Size))
			{
				if (API.IsDisabledControlPressed(0, 24))
				{
					Value += Multiplier;
					SliderProgressChanged(Value);
				}
			}
			else
			{
				API.StopSound(Audio.Id);
				API.ReleaseSoundId(Audio.Id);
				Pressed = false;
			}
		}

		/// <summary>
		/// Draw item.
		/// </summary>
		public override async Task Draw()
		{
			base.Draw();
			_arrowLeft.Color = Enabled ? Selected ? Colors.Black : Colors.WhiteSmoke : Color.FromArgb(163, 159, 148);
			_arrowRight.Color = Enabled ? Selected ? Colors.Black : Colors.WhiteSmoke : Color.FromArgb(163, 159, 148);
			_arrowLeft.Draw();
			_arrowRight.Draw();
			_rectangleBackground.Draw();
			_rectangleSlider.Draw();
			_rectangleDivider.Draw();
			Functions();
		}
	}
}
