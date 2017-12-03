using Verse;

namespace AdditionalJoyObjects {

  internal class PlaceWorker_Chandelier : PlaceWorker {


		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null) {
      // Don't allow placing on big things
      foreach (IntVec3 c in GenAdj.CellsOccupiedBy(loc, rot, checkingDef.Size)) {
        if (c.GetEdifice(map) != null) {
          if (c.GetEdifice(map).def.blockWind == true || c.GetEdifice(map).def.holdsRoof == true) {
            return new AcceptanceReport("AJO_ObjectTooTall".Translate(new object[] { c.GetEdifice(map).LabelCap, checkingDef.LabelCap }));
          }
        }
        if (c.GetThingList(map).Find(ch => ch is Building_Chandelier) != null) {
          return new AcceptanceReport("IdenticalThingExists".Translate());
        }
      }

      // Otherwise, accept placing
      return true;
    }
  }
}
