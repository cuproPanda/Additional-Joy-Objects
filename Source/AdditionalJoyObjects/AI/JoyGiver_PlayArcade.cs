using RimWorld;
using Verse;
using Verse.AI;

namespace AdditionalJoyObjects {

	public class JoyGiver_PlayArcade : JoyGiver_InteractBuilding {

		protected override Job TryGivePlayJob(Pawn pawn, Thing t) {
			Building_Arcade arcade = t as Building_Arcade;
			if (t != null) {
				if ((arcade.owners.NullOrEmpty() || arcade.owners.Contains(pawn)) && t.InteractionCell.Standable(t.Map) && !t.IsForbidden(pawn) && !t.InteractionCell.IsForbidden(pawn) && pawn.Map.reservationManager.CanReserve(pawn, t.InteractionCell)) {
					return new Job(def.jobDef, t, t.InteractionCell);
				}
			}
			return null;
		}
	}
}
