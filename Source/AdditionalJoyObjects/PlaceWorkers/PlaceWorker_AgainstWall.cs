using RimWorld;
using Verse;

namespace AdditionalJoyObjects {
  /// <summary>
  /// Only allows an object to be placed against a wall
  /// <para>(Checks the cell behind this object)</para>
  /// </summary>
  public class PlaceWorker_AgainstWall : PlaceWorker {


		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null) {

      // Get the tile behind this object
      IntVec3 c = loc - rot.FacingCell;
      // Determine if the tile is an edifice
      Building edifice = c.GetEdifice(map);

      // Don't place outside of the map
      if (!c.InBounds(map) || !loc.InBounds(map)) {
        return false;
      }

        // Only allow placing on a natural or constructed wall
      if (edifice == null || edifice.def == null || (edifice.def != ThingDefOf.Wall && !edifice.def.building.isNaturalRock &&
          ((edifice.Faction == null || edifice.Faction != Faction.OfPlayer) ||
          edifice.def.graphicData == null || edifice.def.graphicData.linkFlags == 0 || (LinkFlags.Wall & edifice.def.graphicData.linkFlags) == LinkFlags.None))) {
        return new AcceptanceReport("AJO_MustBePlacedOnWall".Translate(new object[] { checkingDef.LabelCap }));
      }

      // Otherwise, accept placing
      return true;
    }
  }
}

