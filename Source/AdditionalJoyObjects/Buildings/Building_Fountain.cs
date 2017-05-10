using UnityEngine;
using RimWorld;
using Verse;

namespace AdditionalJoyObjects {
  [StaticConstructorOnStartup]
  internal class Building_Fountain : Building {

    private static readonly Graphic Anim0 = GraphicDatabase.Get<Graphic_Single>("Cupro/Object/Fountain/Fountain_anim0");
    private static readonly Graphic Anim1 = GraphicDatabase.Get<Graphic_Single>("Cupro/Object/Fountain/Fountain_anim1");
    private static readonly Graphic Anim2 = GraphicDatabase.Get<Graphic_Single>("Cupro/Object/Fountain/Fountain_anim2");

    private bool powered;
    private bool cycle;
    private bool dirty;

    private TickManager tickMan = Find.TickManager;
    private CompPowerTrader powerComp;
    private ThingDef movingWaterDef;
    private Building newMovingWater;
    private Building curMovingWater;

    // Cycle the graphic between animations
    private Graphic cycleGraphic {
      get {
        return cycle ? Anim1 : Anim2;
      }
    }

    public override Graphic Graphic {
      get {
        if (powerComp == null || !powered) {
          return Anim0;
        }
        return cycleGraphic;
      }
    }


    public override void SpawnSetup(Map map, bool respawningAfterLoad) {
      base.SpawnSetup(map, respawningAfterLoad);

      // Define graphic data
      Anim0.drawSize = new Vector2(1,3);
      Anim0.data.shaderType = ShaderType.CutoutComplex;
      Anim0.color = DrawColor;

      Anim1.drawSize = new Vector2(1,3);
      Anim1.data.shaderType = ShaderType.CutoutComplex;
      Anim1.color = DrawColor;

      Anim2.drawSize = new Vector2(1,3);
      Anim2.data.shaderType = ShaderType.CutoutComplex;
      Anim2.color = DrawColor;

      // Get power info
      powerComp = GetComp<CompPowerTrader>();
      powered = powerComp.PowerOn;

      // Cache a copy of AJO_MovingWater to use
      movingWaterDef = ThingDef.Named("AJO_MovingWater");
      newMovingWater = ThingMaker.MakeThing(movingWaterDef) as Building;
      newMovingWater.SetFactionDirect(Faction.OfPlayer);
    }


    public override void Tick() {
      base.Tick();

      // Reset the dirty bool
      dirty = false;

      // If the power status has changed, update the graphic
      if (powered != powerComp.PowerOn) {
        powered = powerComp.PowerOn;
        dirty = true;
      }

      // Handle interval processing
      if (tickMan.TicksGame % 25 == 0) {
        curMovingWater = Position.GetThingList(Map).Find(w => w.def == movingWaterDef) as Building;

        // Update the graphic
        if (powered) {
          // Spawn the moving water placeholder, adding a beauty bonus
          if (curMovingWater == null) {
            GenSpawn.Spawn(newMovingWater, Position, Map);
          }

          // Cycle the graphic
          cycle = !cycle;
          dirty = true;
        }

        // Destroy the moving water placeholder, removing beauty bonus
        if (!powered && curMovingWater != null) {
          curMovingWater.Destroy();
          dirty = true;
        }
      }

      // If changes have been made, apply them
      if (dirty) {
        Map.mapDrawer.MapMeshDirty(Position, MapMeshFlag.Things);
      }
    }
  }
}
