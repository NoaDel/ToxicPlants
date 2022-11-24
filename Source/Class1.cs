using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;

namespace ToxicPlants
{
    internal class RefuelWorkGiverUtility
    {
        public static bool CanRefuel(Pawn pawn, Thing t, bool forced = false)
        {
            CompRefuelable compRefuelable = t.TryGetComp<CompRefuelable>();
            if (compRefuelable == null || compRefuelable.IsFull || (!forced && !compRefuelable.allowAutoRefuel))
            {
                return false;
            }
            if (compRefuelable.FuelPercentOfMax > 0f && !compRefuelable.Props.allowRefuelIfNotEmpty)
            {
                return false;
            }
            if (!forced && !compRefuelable.ShouldAutoRefuelNow)
            {
                return false;
            }
            if (t.IsForbidden(pawn) || !pawn.CanReserve(t, 1, -1, null, forced))
            {
                return false;
            }
            if (t.Faction != pawn.Faction || t.def == ToxicPlants_DefOf.Plant_BloodRose)
            {
                return false;
            }
            CompActivable compActivable = t.TryGetComp<CompActivable>();
            if (compActivable != null && compActivable.Props.cooldownPreventsRefuel && compActivable.OnCooldown)
            {
                JobFailReason.Is(compActivable.Props.onCooldownString.CapitalizeFirst());
                return false;
            }
            return true;
        }
    }
}
