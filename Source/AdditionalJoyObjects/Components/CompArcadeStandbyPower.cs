using System.Collections.Generic;
using System.Linq;

using RimWorld;
using Verse;

namespace AdditionalJoyObjects {

  internal class CompArcadeStandbyPower : ThingComp {

    private List<Thing> pawnsInCell;
    private CompPowerTrader powerComp;
    private CompFlickable flickableComp;
    private float idlePower = 5f;
    private float fullPower;


    public override void PostSpawnSetup(bool respawningAfterLoad) {
      base.PostSpawnSetup(respawningAfterLoad);

      powerComp = parent.GetComp<CompPowerTrader>();
      flickableComp = parent.GetComp<CompFlickable>();
      fullPower = powerComp.Props.basePowerConsumption;
    }


    public override void CompTick() {
      base.CompTick();

      // Determine the power usage based on if the arcade is being used
      if (parent.Spawned) {
        powerComp.PowerOutput = InUse() ? -fullPower : -idlePower; 
      }
    }


    private bool InUse() {
      pawnsInCell = parent.InteractionCell.GetThingList(parent.Map).Where(p => p is Pawn).ToList();

      // If a pawn is using this arcade, comsume full power
      // Otherwise, only use standby power
      if (flickableComp.SwitchIsOn && !parent.IsBrokenDown()) {
        for (int p = 0; p < pawnsInCell.Count; p++) {
          if (pawnsInCell[p] is Pawn) {
            Pawn pawn = pawnsInCell[p] as Pawn;
            if (pawn.CurJob != null && (pawn.CurJob.targetA != null && pawn.CurJob.targetA.Thing == parent) || (pawn.CurJob.targetB != null && pawn.CurJob.targetB.Thing == parent)) {
              return true;
            }
          }
        } 
      }
      return false;
    }
  }
}
