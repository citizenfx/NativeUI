using CitizenFX.Core.UI;
using System;

namespace NativeUI
{
	public class StringMeasurer
	{
		/// <summary>
		/// Measures width of a 0.35 scale string.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		[Obsolete("Use Screen.GetTextWidth instead.", true)]
		public static int MeasureString(string input) => (int)ScreenTools.GetTextWidth(input, Font.ChaletLondon, 1f);

		[Obsolete("Use Screen.GetTextWidth instead.", true)]
		public static float MeasureString(string input, Font font, float scale) => ScreenTools.GetTextWidth(input, font, scale);
	}
}