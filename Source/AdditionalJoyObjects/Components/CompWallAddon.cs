using RimWorld;
using Verse;

namespace AdditionalJoyObjects {
  /// <summary>
  /// Allows objects to die when a wall is destroyed
  /// </summary>
  public class CompWallAddon : ThingComp {
    /// <summary></summary>
    public override void CompTickRare() {

      Building edifice = parent.Position.GetEdifice(parent.Map);
      if (edifice == null || (edifice.def != ThingDefOf.Wall && !edifice.def.building.isNaturalRock && (edifice.Faction != Faction.OfPlayer || (LinkFlags.Wall & edifice.def.graphicData.linkFlags) == LinkFlags.None))) {
        parent.Destroy(DestroyMode.KillFinalize);
      }
    }
  }
}
