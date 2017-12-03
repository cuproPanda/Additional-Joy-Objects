using RimWorld;

namespace AdditionalJoyObjects {

	public class JobDriver_PlayArcade : JobDriver_WatchBuilding {

		public override void Notify_Starting() {
			if (JoySettings.ArcadesGiveXp) {
				job.def.joyXpPerTick = 0.01f * JoySettings.ArcadeXpMultiplier;
			}
			else {
				job.def.joyXpPerTick = 0;
			}
			base.Notify_Starting();
		}
	}
}
