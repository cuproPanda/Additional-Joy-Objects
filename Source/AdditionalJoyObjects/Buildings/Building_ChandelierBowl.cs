using Verse;

namespace AdditionalJoyObjects {

  internal class Building_ChandelierBowl : Building {

    public override void TickRare() {
      base.TickRare();

      Building_Chandelier chand = Position.GetThingList(Map).Find(c => c is Building_Chandelier) as Building_Chandelier;

      if (Spawned && chand == null) {
        // If the chandelier is missing, the glass should be too
        Destroy();
      }
    }
  }
}
