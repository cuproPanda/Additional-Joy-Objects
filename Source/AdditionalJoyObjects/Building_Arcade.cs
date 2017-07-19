using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace AdditionalJoyObjects {

	public class Building_Arcade : Building, IAssignableBuilding {

		public List<Pawn> owners = new List<Pawn>();

		public bool Unowned {
			get { return owners.Count <= 0; }
		}

		public IEnumerable<Pawn> AssigningCandidates {
			get {
				if (!Spawned) {
					return Enumerable.Empty<Pawn>();
				}
				return Map.mapPawns.FreeColonists;
			}
		}

		public IEnumerable<Pawn> AssignedPawns {
			get {
				return owners;
			}
		}

		public int MaxAssignedPawnsCount {
			get {
				return 5;
			}
		}

		private int OwnerInspectCount {
			get {
				if (owners.Count > 3) {
					return 3;
				}
				return owners.Count;
			}
		}

		private bool PlayerCanSeeOwners {
			get {
				if (Faction == Faction.OfPlayer) {
					return true;
				}
				for (int i = 0; i < this.owners.Count; i++) {
					if (owners[i].Faction == Faction.OfPlayer || owners[i].HostFaction == Faction.OfPlayer) {
						return true;
					}
				}
				return false;
			}
		}


		public void TryAssignPawn(Pawn pawn) {
			if (!owners.Contains(pawn)) {
				owners.Add(pawn);
			}
		}


		public void TryUnassignPawn(Pawn pawn) {
			if (owners.Contains(pawn)) {
				owners.Remove(pawn);
			}
		}


		public override void SpawnSetup(Map map, bool respawningAfterLoad) {
			base.SpawnSetup(map, respawningAfterLoad);
		}


		public override void DeSpawn() {
			RemoveAllOwners();
			base.DeSpawn();
		}


		public override void ExposeData() {
			base.ExposeData();
			if (Scribe.mode == LoadSaveMode.Saving) {
				owners.RemoveAll((Pawn x) => x.Destroyed);
			}
			Scribe_Collections.Look(ref owners, "owners", LookMode.Reference);
			if (Scribe.mode == LoadSaveMode.PostLoadInit) {
				SortOwners();
			}
		}


		public override IEnumerable<Gizmo> GetGizmos() {
			foreach (Gizmo g in base.GetGizmos()) {
				yield return g;
			}
			yield return new Command_Action {
				defaultLabel = "CommandBedSetOwnerLabel".Translate(),
				icon = ContentFinder<Texture2D>.Get("UI/Commands/AssignOwner", true),
				defaultDesc = Static.CommandDesc_SetArcadeOwner,
				action = delegate {
					Find.WindowStack.Add(new Dialog_AssignBuildingOwner(this));
				},
				hotKey = KeyBindingDefOf.Misc3
			};
		}


		public override string GetInspectString() {
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.GetInspectString());
			stringBuilder.AppendLine();
			if (PlayerCanSeeOwners) {
				stringBuilder.AppendLine("ForColonistUse".Translate());
				if (owners.Count == 0) {
					stringBuilder.AppendLine("Owner".Translate() + ": " + "Nobody".Translate().ToLower());
				}
				else if (owners.Count == 1) {
					stringBuilder.AppendLine("Owner".Translate() + ": " + owners[0].Label);
				}
				else {
					stringBuilder.Append("Owners".Translate() + ": ");
					bool conjugate = false;
					for (int i = 0; i < OwnerInspectCount; i++) {
						if (conjugate) {
							stringBuilder.Append(", ");
						}
						conjugate = true;
						stringBuilder.Append(owners[i].LabelShort);
					}
					if (owners.Count > 3) {
						stringBuilder.Append($" (+ {owners.Count - 3})");
					}
					stringBuilder.AppendLine();
				}
			}
			return stringBuilder.ToString().TrimEndNewlines();
		}


		public void SortOwners() {
			owners.SortBy((Pawn x) => x.thingIDNumber);
		}


		private void RemoveAllOwners() {
			for (int i = owners.Count - 1; i >= 0; i--) {
				owners[i].ownership.UnclaimBed();
			}
		}
	}
}
