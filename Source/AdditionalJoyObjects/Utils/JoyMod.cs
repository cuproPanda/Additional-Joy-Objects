using System;
using UnityEngine;
using Verse;

namespace AdditionalJoyObjects {

  public sealed class JoyMod : Mod {


    public JoyMod(ModContentPack content) : base(content) {
      GetSettings<JoySettings>();
    }


    public override string SettingsCategory() {
      return Static.ModName;
    }


    public override void DoSettingsWindowContents(Rect rect) {

			Listing_Standard list = new Listing_Standard() {
				ColumnWidth = rect.width
			};
			list.Begin(rect);
      list.Gap();
      {
				Rect leftRect = list.GetRect(Text.LineHeight).LeftHalf().Rounded();

				Widgets.CheckboxLabeled(leftRect, Static.Label_ArcadeJoyBool, ref JoySettings.ArcadesGiveXp);
				if (Mouse.IsOver(leftRect)) {
					Widgets.DrawHighlight(leftRect);
				}
				TooltipHandler.TipRegion(leftRect, Static.ToolTip_ArcadeJoyBool);
			}
			list.Gap();
			{
				Rect fullRect = list.GetRect(Text.LineHeight);
				Rect leftRect = fullRect.LeftHalf().Rounded();
				Rect rightRect = fullRect.RightHalf().Rounded();

				Widgets.Label(leftRect, "AJO_Label_ArcadeJoyGain".Translate(JoySettings.ArcadeXpMultiplier));
				if (Mouse.IsOver(leftRect)) {
					Widgets.DrawHighlight(leftRect);
				}
				TooltipHandler.TipRegion(leftRect, Static.ToolTip_ArcadeJoyGain);
				IntSlider(rightRect, ref JoySettings.ArcadeXpMultiplier, 1, 3);
			}
			list.End();
    }


		public static void IntSlider(Rect rect, ref int value, int minValue, int maxValue, int roundTo = 1) {
			value = RoundToAsInt(roundTo, Widgets.HorizontalSlider(
					new Rect(rect.xMin + rect.height + 10f, rect.y, rect.width - ((rect.height * 2) + 20f), rect.height),
					value, minValue, maxValue, true));
		}


		private static int RoundToAsInt(int factor, float num) {
			return (int)(Math.Round(num / (double)factor, 0) * factor);
		}
	}
}
