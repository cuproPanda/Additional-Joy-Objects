using UnityEngine;
using RimWorld;
using Verse;

namespace AdditionalJoyObjects {

  public class JobDriver_Dartboard : JobDriver_WatchBuilding {
    // Interval at which darts will be thrown
    private const int throwDartInterval = 400;

    public JobDriver_Dartboard() { }
    // If 400 ticks have passed, throw a dart
    protected override void WatchTickAction() {
      if (pawn.IsHashIntervalTick(throwDartInterval)) {
        ThrowDart(pawn, TargetA.Cell);
      }
      base.WatchTickAction();
    }


    // Process the dart
    public static void ThrowDart(Pawn thrower, IntVec3 targetCell) {

      // If the pawn can't spawn motes, do nothing
      if (!thrower.Position.ShouldSpawnMotesAt(thrower.Map) || thrower.Map.moteCounter.SaturatedLowPriority) {
        return;
      }

      ThingDef dart;

      // Speed for the dart
      float num = 20f;
      // The target to throw the dart to
      Vector3 vector = targetCell.ToVector3Shifted();
      //vector.y = thrower.DrawPos.y;

      // Set dart color based on player
      if (targetCell.x == thrower.Position.x - 1 || targetCell.z == thrower.Position.z - 1) {
        dart = AjoDefOf.AJO_Mote_GreenDart;
      }
      else if (targetCell.x == thrower.Position.x + 1 || targetCell.z == thrower.Position.z + 1) {
        dart = AjoDefOf.AJO_Mote_RedDart;
      }
      else {
        dart = AjoDefOf.AJO_Mote_BlueDart;
      }

      // Transforms for the dart
      MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(dart, null);
      moteThrown.Scale = 1f;
      moteThrown.rotationRate = 0f;
      moteThrown.exactPosition = thrower.DrawPos;
      moteThrown.exactRotation = (vector - moteThrown.exactPosition).AngleFlat();
      moteThrown.SetVelocity((vector - moteThrown.exactPosition).AngleFlat(), num);
      moteThrown.MoveAngle = (vector - moteThrown.exactPosition).AngleFlat();
      moteThrown.airTimeLeft = (moteThrown.exactPosition - vector).MagnitudeHorizontal() / num;

      // Throw the dart
      GenSpawn.Spawn(moteThrown, thrower.Position, thrower.Map);
    }
  }
}
