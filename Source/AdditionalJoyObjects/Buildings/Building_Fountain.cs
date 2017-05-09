using UnityEngine;
using RimWorld;
using Verse;

namespace AdditionalJoyObjects {
  [StaticConstructorOnStartup]
  internal class Building_Fountain : Building {

    private Graphic Anim0 = GraphicDatabase.Get<Graphic_Single>("Cupro/Object/Fountain/Fountain_anim0");
    private Graphic Anim1 = GraphicDatabase.Get<Graphic_Single>("Cupro/Object/Fountain/Fountain_anim1");
    private Graphic Anim2 = GraphicDatabase.Get<Graphic_Single>("Cupro/Object/Fountain/Fountain_anim2");

    private bool powered;
    private bool cycle;
    private bool dirty;

    private TickManager tickMan = Find.TickManager;
    private CompPowerTrader powerComp;
    private Building movingWater;

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
      Anim1.drawSize = new Vector2(1,3);
      Anim2.drawSize = new Vector2(1,3);
      Anim0.data.shaderType = ShaderType.CutoutComplex;
      Anim0.data.shaderType = ShaderType.CutoutComplex;
      Anim0.data.shaderType = ShaderType.CutoutComplex;

      // Get power info
      powerComp = GetComp<CompPowerTrader>();
      powered = powerComp.PowerOn;

      // Cache a copy of AJO_MovingWater to use
      movingWater = ThingMaker.MakeThing(ThingDef.Named("AJO_MovingWater")) as Building;
      movingWater.SetFactionDirect(Faction.OfPlayer);
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

      // Update the graphic on a 25 tick interval
      if (powered && tickMan.TicksGame % 25 == 0) {
        // Spawn the moving water placeholder, adding a beauty bonus
        if (!movingWater.Spawned) {
          GenSpawn.Spawn(movingWater, Position, Map);
          movingWater.SetPositionDirect(IntVec3.Invalid);
          movingWater.Position = Position;
        }

        // Cycle the graphic
        cycle = !cycle;
        dirty = true;
      }

      // Despawn the moving water placeholder, removing beauty bonus
      if (!powered && movingWater.Spawned && tickMan.TicksGame % 25 == 0) {
        movingWater.DeSpawn();
        dirty = true;
      }

      // If changes have been made, apply them
      if (dirty) {
        Map.mapDrawer.MapMeshDirty(Position, MapMeshFlag.Things);
      }
    }
  }
}
