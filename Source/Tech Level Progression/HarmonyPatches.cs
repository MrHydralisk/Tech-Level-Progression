using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using System.Reflection;
using RimWorld;
using UnityEngine;

namespace TechLevelProgression
{
    [StaticConstructorOnStartup]
    public class HarmonyPatches
    {
        private static readonly Type patchType;

        private static AccessTools.FieldRef<object, Dictionary<ResearchProjectDef, float>> progress;

        static HarmonyPatches()
        {
            patchType = typeof(HarmonyPatches);
            progress = AccessTools.FieldRefAccess<Dictionary<ResearchProjectDef, float>>(typeof(ResearchManager), "progress");
            Harmony val = new Harmony("rimworld.mrhydralisk.techlevelprogression");
            val.Patch((MethodBase)AccessTools.Method(typeof(ResearchManager), "ReapplyAllMods", (Type[])null, (Type[])null), (HarmonyMethod)null, new HarmonyMethod(patchType, "ReapplyAllMods_Postfix", (Type[])null), (HarmonyMethod)null, (HarmonyMethod)null);
            val.Patch(AccessTools.Method(typeof(ResearchProjectDef), "CostFactor"), prefix: new HarmonyMethod(patchType, "RPD_CostFactor_Prefix"));
            val.Patch(AccessTools.Property(typeof(ResearchProjectDef), "CostApparent").GetGetMethod(), postfix: new HarmonyMethod(patchType, "RPD_CostApparent_Postfix"));
            val.Patch(AccessTools.Property(typeof(ResearchProjectDef), "ProgressApparent").GetGetMethod(), postfix: new HarmonyMethod(patchType, "RPD_CostApparent_Postfix"));
        }

        public static void ReapplyAllMods_Postfix(ResearchManager __instance)
        {
            TechLevel currTechLevel = Faction.OfPlayer.def.techLevel;
            IEnumerable<TechLevel> leftTechLevels = Enum.GetValues(typeof(TechLevel)).Cast<TechLevel>().OrderByDescending((TechLevel tl) => tl);
            bool isAnimalTL = true;
            if (!TechLevelProgressionMod.Settings.TechLevelDecrease)
            {
                isAnimalTL = false;
                leftTechLevels = leftTechLevels.Where((TechLevel tl) => (int)tl > (int)currTechLevel);
            }
            string tlprogress = "";
            foreach (TechLevel tl in leftTechLevels)
            {
                int currR = -1, allR = -1;
                ref Dictionary<ResearchProjectDef, float> reference = ref progress.Invoke((object)__instance);
                currR = reference.Count(rp => rp.Key.techLevel == tl && rp.Key.IsFinished);
                allR = DefDatabase<ResearchProjectDef>.AllDefs.Count((ResearchProjectDef rpd) => rpd.techLevel == tl);
                tlprogress += "\n" + tl.ToStringSafe() + " progress " + currR.ToStringSafe() + " / " + allR.ToStringSafe();
                if ((currR > 0) && (allR > 0) && (currR / (float)allR >= (TechLevelProgressionMod.Settings.TechLevelPrecise ? TechLevelProgressionMod.Settings.ResearchPercentPrecise[(int)tl] : TechLevelProgressionMod.Settings.ResearchPercent)))
                {
                    isAnimalTL = false;
                    Faction.OfPlayer.def.techLevel = (TechLevel)Mathf.Min((byte)TechLevelProgressionMod.Settings.TechLevelRange.max, (byte)tl);
                    if ((int)tl > (int)currTechLevel)
                        Find.LetterStack.ReceiveLetter("TechLevelProgression.Letters.TLIncreased.Title".Translate(), "TechLevelProgression.Letters.TLIncreased.Text".Translate(Faction.OfPlayer.def.techLevel.ToStringSafe()), LetterDefOf.NeutralEvent);
                    break;
                }
            }
            if (isAnimalTL)
            {
                Faction.OfPlayer.def.techLevel = (TechLevel)Mathf.Max((byte)TechLevelProgressionMod.Settings.TechLevelRange.min, (byte)TechLevel.Animal);
            }
            Log.Message("Current tech level " + Faction.OfPlayer.def.techLevel.ToStringSafe() + tlprogress);
        }

        public static bool RPD_CostFactor_Prefix(ref float __result, ResearchProjectDef __instance, TechLevel researcherTechLevel)
        {
            if (__instance.techLevel == TechLevel.Undefined)
            {
                __result = 1f;
                return false;
            }
            int techLevelDiff = (int)researcherTechLevel - (int)__instance.techLevel;
            if (techLevelDiff > 0)
            {
                __result = Mathf.Pow(TechLevelProgressionMod.Settings.ResearchTechLevelBelowMult, techLevelDiff);
            }
            else if (techLevelDiff < 0)
            {
                __result = Mathf.Pow(1 + TechLevelProgressionMod.Settings.ResearchTechLevelAboveMult, -techLevelDiff);
            }
            else
            {
                __result = 1f;
            }
            return false;
        }

        public static void RPD_CostApparent_Postfix(ref float __result, ResearchProjectDef __instance)
        {
            __result = Mathf.Round(__result);
        }
    }
}
