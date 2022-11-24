using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;
using System;
using System.Collections.Generic;
using static HarmonyLib.Code;
using System.Reflection.Emit;
using Verse.AI;
using System.Diagnostics.Eventing.Reader;
using System.Security.Cryptography;

namespace ToxicPlants
{
    [StaticConstructorOnStartup]
    public static class ToxicPlants
    {
        static ToxicPlants()
        {
            var harmony = new Harmony("ac.mwsad.patch");
            harmony.PatchAll();


        }
        public static float GetToxicResistance(Pawn pawn)
        {
            float test = pawn.GetStatValue(StatDefOf.ToxicResistance);
            return test;
        }

    }
    public class ToxicPlantModExtension : DefModExtension
    {
        public float ToxicAmount;
    }
    [HarmonyPatch(typeof(TendUtility), "DoTend")]
    public static class Heal_Patch
    {
        public static void Postfix(Pawn patient, Medicine medicine)
        {
            if ((medicine != null) && (patient != null) && (medicine.def.HasModExtension<ToxicPlantModExtension>() == true))
            {
                HealthUtility.AdjustSeverity(patient, HediffDefOf.ToxicBuildup, (0.2f * Mathf.Max(1f - ToxicPlants.GetToxicResistance(patient), 0f)));
            }
        }
    }
    [HarmonyPatch(typeof(Recipe_InstallArtificialBodyPart), "ApplyOnPawn")]
    public static class Surgery_Patch
    {
        public static void Postfix(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (pawn != null)
            {
                for (var i = 0; i < ingredients.Count; i++)
            {
                Log.Message(Convert.ToString(ingredients[i].def));
            }
                HealthUtility.AdjustSeverity(pawn, HediffDefOf.ToxicBuildup, (0.2f * Mathf.Max(1f - pawn.GetStatValue(StatDefOf.ToxicResistance), 0f)));
            
            }
        }
    }
    [HarmonyPatch(typeof(Recipe_Surgery), "CheckSurgeryFail")]
    public static class Surgery1_Patch
    {
        public static void Postfix(Pawn surgeon, Pawn patient, List<Thing> ingredients, BodyPartRecord part, Bill bill)
        {
            Log.Message("test2");
            if (patient != null)
            {
                Log.Message("test2");
            }
        }
    }
    [HarmonyPatch(typeof(Recipe_ExtractHemogen), "ApplyOnPawn")]
    public static class Surgery2_Patch
    {
        public static void Postfix(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            Log.Message("test3");
            if (pawn != null)
            {
                Log.Message("test3");
            }
        }
    }
    [HarmonyPatch(typeof(RefuelWorkGiverUtility))]
    [HarmonyPatch("CanRefuel")]
    public static class refuel_Patch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> insts)
        {
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ThingDefOf), "test"));
            yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(ToxicPlants_DefOf), nameof(ToxicPlants_DefOf.Plant_BloodRose)));
            yield return new CodeInstruction(OpCodes.Bne_Un_S);
            foreach (CodeInstruction inst in insts)
            {
                if (inst.Calls(AccessTools.PropertyGetter(typeof(Thing), nameof(Thing.Faction))))
                {
                    // Thing I don't know how to do yet
                }
                yield return inst;
            }
        }
    }
}
