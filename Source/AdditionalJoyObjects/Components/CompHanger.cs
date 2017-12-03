using RimWorld;
using Verse;
using Verse.Sound;

namespace AdditionalJoyObjects {
  /// <summary>
  /// Minifies a minifiable object if the thing it's hanging on is missing
  /// </summary>
  public class CompHanger : ThingComp {

    /// <summary></summary>
    public CompProperties_Hanger Props {
      get { return (CompProperties_Hanger)props; }
    }


    /// <summary> Checks for a disconnection, then minifies an object if needed</summary>
    public override void CompTickRare() {
      base.CompTickRare();

      if (Props.hangingType == HangingType.None) {
        Log.Warning(parent.def.defName + " doesn't have a hangingType defined, please notify the mod author.");
      }

      if (Props.hangingType == HangingType.Wall) {
        // Get the tile behind this object
        IntVec3 c = parent.Position - parent.Rotation.FacingCell;
        // Minify this if the wall is missing
        Building edifice = c.GetEdifice(parent.Map);
        if (edifice == null || edifice.def == null || (edifice.def != ThingDefOf.Wall &&
          ((edifice.Faction == null || edifice.Faction != Faction.OfPlayer) ||
          edifice.def.graphicData == null || edifice.def.graphicData.linkFlags == 0 || (LinkFlags.Wall & edifice.def.graphicData.linkFlags) == LinkFlags.None))) {
          Minify();
        }
      }

      if (Props.hangingType == HangingType.Ceiling) {
        // Minify this if the ceiling is missing
        int occCells = 0;
        int unroofedCells = 0;
        foreach (IntVec3 current in parent.OccupiedRect()) {
          occCells++;
          if (!parent.Map.roofGrid.Roofed(current)) {
            unroofedCells++;
          }
          if (current.GetEdifice(parent.Map) == null || current.GetEdifice(parent.Map).def == null) {
            continue;
          }
          if (current.GetEdifice(parent.Map).def.blockWind == true || current.GetEdifice(parent.Map).def.holdsRoof == true) {
            Minify();
          }
        }
        if (((float)(occCells - unroofedCells) / occCells) < 0.5f) {
          Minify();
        }
      }
    }


    /// <summary> Minifies the package</summary>
    public virtual void Minify() {
      MinifiedThing package = parent.MakeMinified();
      GenPlace.TryPlaceThing(package, parent.Position, parent.Map, ThingPlaceMode.Near);
      SoundDef.Named("ThingUninstalled").PlayOneShot(new TargetInfo(parent.Position, parent.Map));
    }
  }
}
