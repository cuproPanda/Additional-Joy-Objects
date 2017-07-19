using System.Collections.Generic;

using Verse;

namespace AdditionalJoyObjects {

  public class RoomRoleWorker_Library : RoomRoleWorker {

    public override float GetScore(Room room) {
      int num = 0;
      List<Thing> containedAndAdjacentThings = room.ContainedAndAdjacentThings;

      for (int i = 0; i < containedAndAdjacentThings.Count; i++) {
        Thing thing = containedAndAdjacentThings[i];
        if (thing.def.category == ThingCategory.Building) {
          if (thing.def == AjoDefOf.AJO_BookRack || thing.def == AjoDefOf.AJO_ReadingTable) {
            num++;
          }
          else if (thing.def == AjoDefOf.AJO_Bookshelf) {
            num += 2;
          }
          else if (thing.def == AjoDefOf.AJO_Bookcase) {
            num += 3;
          }
        }
      }
      return num * 3.5f;
    }
  }
}
