using System;
using System.Drawing;
using System.Threading.Tasks;

namespace NativeUI
{
	public enum UIMenuCheckboxStyle
	{
		Cross,
		Tick
	}

	public class UIMenuCheckboxItem : UIMenuItem
    {
        protected internal Sprite _checkedSprite;

        /// <summary>
        /// Triggered when the checkbox state is changed.
        /// </summary>
        public event ItemCheckboxEvent CheckboxEvent;

		public UIMenuCheckboxStyle Style { get; set; }

		/// <summary>
		/// Checkbox item with a toggleable checkbox.
		/// </summary>
		/// <param name="text">Item label.</param>
		/// <param name="check">Boolean value whether the checkbox is checked.</param>
		public UIMenuCheckboxItem(string text, bool check) : this(text, check, "")
        {
        }

        /// <summary>
        /// Checkbox item with a toggleable checkbox.
        /// </summary>
        /// <param name="text">Item label.</param>
        /// <param name="check">Boolean value whether the checkbox is checked.</param>
        /// <param name="description">Description for this item.</param>
        public UIMenuCheckboxItem(string text, bool check, string description) : this(text, UIMenuCheckboxStyle.Tick, check, description, Color.Transparent, Color.FromArgb(255, 255, 255, 255))
        {
        }

		/// <summary>
		/// Checkbox item with a toggleable checkbox.
		/// </summary>
		/// <param name="text">Item label.</param>
		/// <param name="style">CheckBox style (Tick or Cross).</param>
		/// <param name="check">Boolean value whether the checkbox is checked.</param>
		/// <param name="description">Description for this item.</param>
		public UIMenuCheckboxItem(string text, UIMenuCheckboxStyle style, bool check, string description) : this(text, style, check, description, Color.Transparent, Color.FromArgb(255, 255, 255, 255))
		{
		}

		/// <summary>
		/// Checkbox item with a toggleable checkbox.
		/// </summary>
		/// <param name="text">Item label.</param>
		/// <param name="style">CheckBox style (Tick or Cross).</param>
		/// <param name="check">Boolean value whether the checkbox is checked.</param>
		/// <param name="description">Description for this item.</param>
		/// <param name="mainColor">Main item color.</param>
		/// <param name="highlightColor">Highlight item color.</param>
		public UIMenuCheckboxItem(string text, UIMenuCheckboxStyle style, bool check, string description, Color mainColor, Color highlightColor) : base(text, description, mainColor, highlightColor)
		{
			const int y = 0;
			Style = style;
			_checkedSprite = new Sprite("commonmenu", "shop_box_blank", new PointF(410, y + 95), new SizeF(50, 50));
			Checked = check;
		}


		/// <summary>
		/// Change or get whether the checkbox is checked.
		/// </summary>
		public bool Checked { get; set; }

		/// <summary>
		/// Change item's position.
		/// </summary>
		/// <param name="y">New Y value.</param>
		public override void Position(int y)
        {
            base.Position(y);
            _checkedSprite.Position = new PointF(380 + Offset.X + Parent.WidthOffset, y + 138 + Offset.Y);
        }


        /// <summary>
        /// Draw item.
        /// </summary>
        public override async Task Draw()
        {
            base.Draw();
            _checkedSprite.Position = new PointF(380 + Offset.X + Parent.WidthOffset, _checkedSprite.Position.Y);
            _checkedSprite.TextureName = Selected ? (Checked ? (Style == UIMenuCheckboxStyle.Tick ? "shop_box_tickb" : "shop_box_crossb") : "shop_box_blankb") : Checked ? (Style == UIMenuCheckboxStyle.Tick ? "shop_box_tick" : "shop_box_cross") : "shop_box_blank";
			_checkedSprite.Draw();
        }

        public void CheckboxEventTrigger()
        {
            CheckboxEvent?.Invoke(this, Checked);
        }

        public override void SetRightBadge(BadgeStyle badge)
        {
            throw new Exception("UIMenuCheckboxItem cannot have a right badge.");
        }

        public override void SetRightLabel(string text)
        {
            throw new Exception("UIMenuListItem cannot have a right label.");
        }
    }
}