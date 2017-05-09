using RimWorld;
using Verse;

namespace AdditionalJoyObjects {
  
  internal class Building_Chandelier : Building {

    public override void TickRare() {
      base.TickRare();

      Building_ChandelierBowl bowl = Position.GetThingList(Map).Find(b => b is Building_ChandelierBowl) as Building_ChandelierBowl;

      // Spawn the correct size bowl to match this chandelier
      if (Spawned && bowl == null) {

        int size = def.Size.x;
        string bowlName = "ChanPart_Bowl_" + size + "x" + size;

        if (ThingDef.Named(bowlName) == null) {
          Log.Warning("AdditionalJoyObjects:: Could not determine what chandelier bowl to spawn. Spawning a 1x1. Enjoy your weirdly small chandelier bowl!");
          bowl = ThingMaker.MakeThing(ThingDef.Named("ChanPart_Bowl_1x1")) as Building_ChandelierBowl;
        }
        else {
          bowl = ThingMaker.MakeThing(ThingDef.Named(bowlName)) as Building_ChandelierBowl;
        }

        bowl.SetFactionDirect(Faction.OfPlayer);
        GenSpawn.Spawn(bowl, Position, Map);
      }
    }
  }
}
