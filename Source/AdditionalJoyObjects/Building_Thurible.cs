using System.Collections.Generic;
using System.Text;

using RimWorld;
using Verse;

namespace AdditionalJoyObjects {
  public class Building_Thurible : Building {

    public CompRefuelable refuelableComp;     // This building's CompRefuelable
    public CompQuality qualityComp;           // Needed for inspect string data
    private string curScent;                  // Title of the current scent produced. Just for appearances.
    private List<string> fluffScent = DefDatabase<ThuribleScentDef>.GetNamed("ScentNames").ScentNames;
    private bool emptied;                     // Boolean for determining when the thurible runs out of fuel
    private float enjoyment = 0.005f;         // How much joy to add every TickRare (0.005f = 0.5%)

    private static List<IntVec3> mistableCells = new List<IntVec3>();
    public static List<IntVec3> MistableCellsAround(IntVec3 pos) {
      mistableCells.Clear();
      if (!pos.InBounds()) {
        return mistableCells;
      }
      Region region = pos.GetRegion();
      if (region == null) {
        return mistableCells;
      }
      RegionTraverser.BreadthFirstTraverse(region, (Region from, Region r) => r.portal == null, delegate (Region r) {
        foreach (IntVec3 current in r.Cells) {
          if (current.InHorDistOf(pos, 10.5f)) {
            mistableCells.Add(current);
          }
        }
        return false;
      });
      return mistableCells;
    }


    // Handle saving
    public override void ExposeData() {
      base.ExposeData();
      Scribe_Values.LookValue(ref curScent, "fluffScent", "Unscented");
    }


    // Do the base setup and add references
    public override void SpawnSetup() {
      base.SpawnSetup();
      refuelableComp = GetComp<CompRefuelable>();
      qualityComp = GetComp<CompQuality>();
    }


    public override void TickRare() {
      base.TickRare();

      // If fuel ran out
      if (refuelableComp.FuelPercent <= 0) {
        emptied = true;
      }
      // If fuel ran out, and has been refilled
      if (emptied && refuelableComp.FuelPercent > 0) {
        // Change the scent listed
        curScent = NewScent();
        // Reset the fuel check
        emptied = false;
      }

      // Only search for pawns if there is fuel loaded and the thurible is not outside (or in an unroofed area)
      if (refuelableComp.HasFuel && !Position.UsesOutdoorTemperature()) {
        // Find the current room
        Room room = RoomQuery.RoomAt(this.Position);
        int radius = 0;
        // A radius of 349 should give a 10-tile circular radius
        // (number of tiles in the radial pattern)
        while (radius < 349) {
          // Get the tiles within the radius range, starting from the middle
          IntVec3 r = this.Position + GenRadial.RadialPattern[radius];
          // If we are still in the same room, and under a roof
          if (RoomQuery.RoomAt(r) == room && Find.RoofGrid.Roofed(r)) {

            List<Pawn> pawnsInRange = new List<Pawn>();
            pawnsInRange.Add((Pawn) r.GetThingList().Find(pawn => pawn is Pawn) as Pawn);

            for (int t = 0; t < pawnsInRange.Count; t++) {
              // If a pawn is found, cache it as p
              if (pawnsInRange[t] is Pawn) {
                Pawn p = pawnsInRange[t] as Pawn;
                if (p.IsColonist && p.needs.joy != null) {
                  // Add joy, but not above 100%
                  p.needs.joy.CurLevel += enjoyment;
                  // Use fuel and throw a puff of smoke/dust/vapor
                  refuelableComp.Notify_UsedThisTick();
                  MoteThrower.ThrowDustPuff(base.Position, 0.6f); 
                }
              }
            }
          }
          // Increment the radius, moving to the next tile
          radius++;
        }
      }
    }

    // Since the thurible uses TickRare(), the normal fuel
    // measurements don't work properly. This is a simple solution.
    public override string GetInspectString() {
      StringBuilder stringBuilder = new StringBuilder();
      // Manually add the quality report
      stringBuilder.AppendLine(qualityComp.CompInspectStringExtra());
      // Show current fuel level as a percentage of 100, since the max fuel is only 1
      stringBuilder.Append("AJO_Tank".Translate() + ": ");
      stringBuilder.Append((refuelableComp.FuelPercent * 100).ToStringDecimalIfSmall());
      stringBuilder.AppendLine("%");
      // Get a random scent
      stringBuilder.Append("AJO_Scent".Translate() + ": ");
      if (emptied) {
        stringBuilder.AppendLine("none"); 
      }
      if (!emptied) {
        stringBuilder.AppendLine(curScent);
      }
      // If the thurible is somewhere that is exposed to the outdoors
      if (Position.UsesOutdoorTemperature()) {
        stringBuilder.AppendLine("AJO_ClosedRoom".Translate());
      }
      return stringBuilder.ToString();
    }

    // Generate a random number and choose which scent to display
    private string NewScent() {
      int tmp = Rand.Range(0, fluffScent.Count);
      return fluffScent[tmp];
    }
  }
}
