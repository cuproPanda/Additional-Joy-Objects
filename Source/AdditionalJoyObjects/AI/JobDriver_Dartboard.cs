using System.Collections.Generic;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;

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


    protected override IEnumerable<Toil> MakeNewToils() {
      //TargetA is the building
      //TargetB is the place to stand to watch
      //TargetC is the chair to sit in (can be null)

      // If the dartboard doesn't exist, end the job
      this.EndOnDespawnedOrNull<JobDriver_Dartboard>(TargetIndex.A, JobCondition.Incompletable);

      // Reserve the dartboard and the place to stand, minding the max participants
      yield return Toils_Reserve.Reserve(TargetIndex.A, CurJob.def.joyMaxParticipants);
      yield return Toils_Reserve.Reserve(TargetIndex.B);

      // If the place to stand has a sittable object, reserve it
      if (TargetC.HasThing)
        yield return Toils_Reserve.Reserve(TargetIndex.C);

      // Go to the standable area
      yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);

      // Info for the toil
      Toil play = new Toil();
      play.tickAction = () => WatchTickAction();
      play.defaultCompleteMode = ToilCompleteMode.Delay;
      play.defaultDuration = CurJob.def.joyDuration;
      play.AddFinishAction(() => JoyUtility.TryGainRecRoomThought(pawn));
      yield return play;
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
      if (targetCell.x == (thrower.Position.x - 1) || targetCell.z == (thrower.Position.z - 1)) {
        dart = DefDatabase<ThingDef>.GetNamed("AJO_Mote_GreenDart");
      }
      else if (targetCell.x == (thrower.Position.x + 1) || targetCell.z == (thrower.Position.z + 1)) {
        dart = DefDatabase<ThingDef>.GetNamed("AJO_Mote_RedDart");
      }
      else {
        dart = DefDatabase<ThingDef>.GetNamed("AJO_Mote_BlueDart");
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
