using Verse;

namespace AdditionalJoyObjects {
  /// <summary>
  /// Restrict buildings to being placed under a roof
  /// </summary>
  public class PlaceWorker_Roofed : PlaceWorker {
    /// <summary></summary>
    public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null) {

      foreach (IntVec3 current in GenAdj.CellsOccupiedBy(loc, rot, checkingDef.Size)) {
        if (!map.roofGrid.Roofed(current)) {
          return new AcceptanceReport ("AJO_NeedsRoof".Translate(new object[] { checkingDef.LabelCap }));
        }
      }

      return true;
    }
  }
}
