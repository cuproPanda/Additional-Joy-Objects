﻿using System.Collections.Generic;

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
      
      Building bookcase = TargetB.Thing as Building;

      // For larger bookcases, give more joy
      // The pawn is able to choose a book they like more, so they get more joy
      int bookOptionsBuff = 0;
      if (bookcase != null) {
        if (bookcase.def.defName == "AJO_Bookshelf") {
          bookOptionsBuff = 1;
        }
        else if (bookcase.def.defName == "AJO_Bookcase") {
          bookOptionsBuff = 2;
        }
      }

      // TargetIndex.A is the reading table
      // TargetIndex.B is the bookcase
      // TargetIndex.C is the chair

      // Set fail conditions
      this.FailOnBurningImmobile(TargetIndex.A);
      this.FailOnBurningImmobile(TargetIndex.B);
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
      getBook.handlingFacing = true;
      getBook.tickAction = () => {
        choosing = true;
        base.WatchTickAction();
        pawn.Drawer.rotator.FaceCell(TargetB.Cell);
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
          pawn.needs.joy.GainJoy(bookOptionsBuff * 0.000144f, joyKind);
        }
      };
      read.defaultCompleteMode = ToilCompleteMode.Delay;
      read.defaultDuration = CurJob.def.joyDuration;
      read.AddFinishAction(() => {
        Room room = pawn.GetRoom();
        if (room != null) {
          int scoreStageIndex = RoomStatDefOf.Impressiveness.GetScoreStageIndex(room.GetStat(RoomStatDefOf.Impressiveness));
          if (ThoughtDef.Named("AJO_ReadInImpressiveLibrary").stages[scoreStageIndex] != null) {
            pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(ThoughtDef.Named("AJO_ReadInImpressiveLibrary"), scoreStageIndex));
          }
        }
      });
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