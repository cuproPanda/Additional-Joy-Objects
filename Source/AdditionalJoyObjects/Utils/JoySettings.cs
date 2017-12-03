using Verse;

namespace AdditionalJoyObjects {

  public class JoySettings : ModSettings {

    internal static bool ArcadesGiveXp = true;
    internal static int ArcadeXpMultiplier = 1;


    public override void ExposeData() {
      base.ExposeData();
      Scribe_Values.Look(ref ArcadesGiveXp, "ArcadesGiveXp", true);
      Scribe_Values.Look(ref ArcadeXpMultiplier, "ArcadeXpMultiplier", 1);
    }
  }
}
