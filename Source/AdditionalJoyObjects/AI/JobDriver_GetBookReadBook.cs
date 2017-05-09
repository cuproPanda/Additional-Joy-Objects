using System.Collections.Generic;

using RimWorld;
using Verse;
using Verse.AI;

namespace AdditionalJoyObjects {

  public class JobDriver_GetBookReadBook : JobDriver_WatchBuilding {

    private List<string> bookNames = DefDatabase<BookNameDef>.GetNamed("BookNames").bookNames;
    private int name;
    private bool choosing = false;
    private bool reading = false;

    protected override IEnumerable<Toil> MakeNewToils() {

      Pawn actor = GetActor();
      Building bookcase = TargetB.Thing as Building;

      // For larger bookcases, give more joy
      // The pawn is able to choose a book they like more, so they get more joy
      int bookOptionsBuff = 0;
      if (bookcase != null) {
        if (bookcase.def == ThingDef.Named("AJO_Bookshelf")) {
          bookOptionsBuff = 1;
        }
        if (bookcase.def == ThingDef.Named("AJO_Bookcase")) {
          bookOptionsBuff = 2;
        }
      }

      // TargetIndex.A is the reading table
      // TargetIndex.B is the bookcase
      // TargetIndex.C is the chair

      // Set fail conditions
      this.FailOnBurningImmobile(TargetIndex.A);
      this.FailOnBurningImmobile(TargetIndex.B);
      this.FailOnDespawnedOrNull(TargetIndex.A);
      this.FailOnDespawnedOrNull(TargetIndex.B);
      this.FailOnDestroyedOrNull(TargetIndex.A);
      this.FailOnDestroyedOrNull(TargetIndex.B);

      // Reserve the reading table, bookcase, and chair
      yield return Toils_Reserve.Reserve(TargetIndex.A);
      yield return Toils_Reserve.Reserve(TargetIndex.B);
      yield return Toils_Reserve.Reserve(TargetIndex.C);

      // Go to the bookcase
      yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.InteractionCell);

      // Get a book
      Toil getBook = new Toil();
      getBook.tickAction = () => {
        choosing = true;
        base.WatchTickAction();
        actor.Drawer.rotator.FaceCell(TargetB.Cell);
      };
      getBook.defaultCompleteMode = ToilCompleteMode.Delay;
      getBook.defaultDuration = 60;
      getBook.AddFinishAction(() => {
        // Get the randomized name of the book being read
        name = Rand.Range(0, bookNames.Count);
        reading = true;
        choosing = false;
      });
      yield return getBook;

      // We no longer need the bookcase to be reserved; let other pawns use it
      yield return Toils_Reserve.Release(TargetIndex.B);

      // Sit at the reading table
      yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.InteractionCell);

      // Read the book
      Toil read = new Toil();
      JoyKindDef joyKind = DefDatabase<JobDef>.GetNamed("AJO_ReadBook").joyKind;
      read.socialMode = RandomSocialMode.Off;
      read.tickAction = () => {
        base.WatchTickAction();
        // Give extra joy based on the variety of books
        if (bookcase != null) {
          actor.needs.joy.GainJoy(bookOptionsBuff * 0.000144f, joyKind);
        }
      };
      read.defaultCompleteMode = ToilCompleteMode.Delay;
      read.defaultDuration = CurJob.def.joyDuration;
      read.AddFinishAction(() => JoyUtility.TryGainRecRoomThought(pawn));
      yield return read;

      // Return the book
      yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.ClosestTouch);
    }


    public override string GetReport() {
      if (choosing) {
        return "AJO_Choosing".Translate();
      }
      if (reading) {
        return ("AJO_Reading".Translate() + " '" + bookNames[name] + "'.");
      }
      return base.GetReport();
    }
  }
}