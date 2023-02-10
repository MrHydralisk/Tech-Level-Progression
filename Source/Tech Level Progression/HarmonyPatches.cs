using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using System.Reflection;
using RimWorld;

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
                if ((currR > 0) && (allR > 0) && (currR / (float)allR >= TechLevelProgressionMod.Settings.ResearchPercent))
                {
                    isAnimalTL = false;
                    Faction.OfPlayer.def.techLevel = tl;
                    if ((int)tl > (int)currTechLevel)
                        Find.LetterStack.ReceiveLetter("TechLevelProgression.Letters.TLIncreased.Title".Translate(), "TechLevelProgression.Letters.TLIncreased.Text".Translate(Faction.OfPlayer.def.techLevel.ToStringSafe()), LetterDefOf.NeutralEvent);
                    break;
                }
            }
            if (isAnimalTL)
            {
                Faction.OfPlayer.def.techLevel = TechLevel.Animal;
            }
            Log.Message("Current tech level " + Faction.OfPlayer.def.techLevel.ToStringSafe() + tlprogress);
        }
    }
}
