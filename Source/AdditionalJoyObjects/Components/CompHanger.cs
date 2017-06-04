using RimWorld;
using Verse;
using Verse.Sound;

namespace AdditionalJoyObjects {
  /// <summary>
  /// Minifies a minifiable object if the thing it's hanging on is missing
  /// </summary>
  public class CompHanger : ThingComp {

    private Map parentMap;

    /// <summary></summary>
    public CompProperties_Hanger Props {
      get { return (CompProperties_Hanger)props; }
    }


    public override void PostSpawnSetup(bool respawningAfterLoad) {
      base.PostSpawnSetup(respawningAfterLoad);

      parentMap = parent.Map;
    }


    /// <summary> Checks for a disconnection, then minifies an object if needed</summary>
    public override void CompTickRare() {
      base.CompTickRare();

      if (Props.hangingType == HangingType.None) {
        Log.Warning(parent.def.defName + " doesn't have a HangingType defined, please notify the mod author.");
      }

      if (Props.hangingType == HangingType.Wall) {
        // Get the tile behind this object
        IntVec3 c = parent.Position - parent.Rotation.FacingCell;
        // Minify this if the wall is missing
        Building edifice = c.GetEdifice(parentMap);
        if (edifice == null || (edifice.def != ThingDefOf.Wall && !edifice.def.building.isNaturalRock && (edifice.Faction != Faction.OfPlayer || (LinkFlags.Wall & edifice.def.graphicData.linkFlags) == LinkFlags.None))) {
          Minify();
        }
      }

      if (Props.hangingType == HangingType.Ceiling) {
        // Minify this if the ceiling is missing
        int occCells = 0;
        int roofCells = 0;
        foreach (IntVec3 current in parent.OccupiedRect()) {
          occCells++;
          if (!parentMap.roofGrid.Roofed(current)) {
            roofCells++;
          }
        }
        if (((float)(occCells - roofCells) / occCells) < 0.5f) {
          Minify();
        }
      }
    }


    /// <summary> Minifies the package</summary>
    public virtual void Minify() {
      MinifiedThing package = parent.MakeMinified();
      GenPlace.TryPlaceThing(package, parent.Position, parentMap, ThingPlaceMode.Near);
      SoundDef.Named("ThingUninstalled").PlayOneShot(new TargetInfo(parent.Position, parentMap));
    }
  }
}
