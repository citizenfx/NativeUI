using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NativeUI
{
	public class UIMenuSliderProgressItem  : UIMenuItem
	{
		protected Sprite _arrowLeft;
		protected Sprite _arrowRight;

		protected UIResRectangle _rectangleBackground;
		protected UIResRectangle _rectangleSlider;
		protected UIResRectangle _rectangleDivider;

		protected int _value = 0;
		protected int _max;
		protected int _multiplier = 5;

		public UIMenuSliderProgressItem(string text, int maxCount, int startIndex, bool divider = false) : this(text, maxCount, startIndex, "", divider)
		{
			_max = maxCount;
			_value = startIndex;
		}

		public UIMenuSliderProgressItem(string text, int maxCount, int startIndex, string description, bool divider = false) : this (text, maxCount, startIndex, description, Color.FromArgb(255, 57, 119, 200), Color.FromArgb(255, 4, 32, 57), divider)
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
			_rectangleBackground = new UIResRectangle(new PointF(0, 0), new SizeF(150, 10), backgroundSliderColor);
			_rectangleSlider = new UIResRectangle(new PointF(0, 0), new SizeF(75, 10), sliderColor);
			_value = startIndex;
			if (divider)
				_rectangleDivider = new UIResRectangle(new Point(0, 0), new Size(2, 20), Colors.WhiteSmoke);
			else
				_rectangleDivider = new UIResRectangle(new Point(0, 0), new Size(2, 20), Color.Transparent);
			float offset = _rectangleBackground.Size.Width / _max * _value;
			_rectangleSlider.Size = new SizeF(offset, _rectangleSlider.Size.Height);
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
				SliderProgressChanged();
			}
		}

		/// <summary>
		/// Triggered when the slider is changed.
		/// </summary>
		public event ItemSliderProgressEvent OnSliderChanged;

		internal virtual void SliderProgressChanged()
		{
			OnSliderChanged?.Invoke(this, Value);
		}


		/// <summary>
		/// Draw item.
		/// </summary>
		public override async Task Draw()
		{
			base.Draw();
			_arrowLeft.Color = Enabled ? Selected ? Colors.Black : Colors.WhiteSmoke : Color.FromArgb(163, 159, 148);
			_arrowRight.Color = Enabled ? Selected ? Colors.Black : Colors.WhiteSmoke : Color.FromArgb(163, 159, 148);
			if (Selected)
			{
				_arrowLeft.Draw();
				_arrowRight.Draw();
			}
			else
			{

			}
			_rectangleBackground.Draw();
			_rectangleSlider.Draw();
			_rectangleDivider.Draw();
		}


	}
}
