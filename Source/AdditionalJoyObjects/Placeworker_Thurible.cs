using Verse;

namespace AdditionalJoyObjects {

  public class Placeworker_Thurible : PlaceWorker {
    public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot) {
      if (!center.UsesOutdoorTemperature()) {
        GenDraw.DrawFieldEdges(Building_Thurible.MistableCellsAround(center)); 
      }
    }
  }
}
