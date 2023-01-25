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
            val.Patch((MethodBase)AccessTools.Method(typeof(ResearchManager), "FinishProject", new Type[] { typeof(ResearchProjectDef), typeof(bool), typeof(Pawn)
#if v1_4
                , typeof(bool)
#endif
            }, (Type[])null), (HarmonyMethod)null, new HarmonyMethod(patchType, "FinishProject_Postfix", (Type[])null), (HarmonyMethod)null, (HarmonyMethod)null);
        }

        public static void FinishProject_Postfix(ResearchProjectDef proj, bool doCompletionDialog, Pawn researcher,
#if v1_4
            bool doCompletionLetter,
#endif
            ResearchManager __instance)
        {
            TechLevel currTechLevel = Faction.OfPlayer.def.techLevel;
            IEnumerable<TechLevel> leftTechLevels = Enum.GetValues(typeof(TechLevel)).Cast<TechLevel>().Where((TechLevel tl) => (int)tl > (int)currTechLevel).OrderByDescending((TechLevel tl) => tl);
            foreach (TechLevel tl in leftTechLevels)
            {
                int currR = -1, allR = -1;
                ref Dictionary<ResearchProjectDef, float> reference = ref progress.Invoke((object)__instance);
                currR = reference.Count(rp => rp.Key.techLevel == tl && rp.Key.IsFinished);
                allR = DefDatabase<ResearchProjectDef>.AllDefs.Count((ResearchProjectDef rpd) => rpd.techLevel == tl);
                if ((currR > 0) && (allR > 0) && (currR / (float)allR >= TechLevelProgressionMod.Settings.ResearchPercent))
                {
                    Faction.OfPlayer.def.techLevel = tl;
                    Find.LetterStack.ReceiveLetter("TechLevelProgression.Letters.TLIncreased.Title".Translate(), "TechLevelProgression.Letters.TLIncreased.Text".Translate(Faction.OfPlayer.def.techLevel.ToStringSafe()), LetterDefOf.NeutralEvent);
                    break;
                }
            }
        }
    }
}
