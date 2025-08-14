using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TechLevelProgression
{
    public class GameComponent_TechLevelProgression : GameComponent
    {
        public static GameComponent_TechLevelProgression Instance;

        public TechLevel currTechLevel = TechLevel.Animal;

        public GameComponent_TechLevelProgression(Game game)
        {
            Instance = this;
        }

        public override void LoadedGame()
        {
            base.LoadedGame();
            if (Find.FactionManager.OfPlayer.def.techLevel != currTechLevel)
            {
                ChangeTechLevel(currTechLevel);
            }
            UpdateTechLevel();
        }

        public override void StartedNewGame()
        {
            base.StartedNewGame();
            InitializeTechLevelProgression();
            UpdateTechLevel();
        }

        public void UpdateTechLevel()
        {
            IEnumerable<TechLevel> leftTechLevels = TechLevelProgressionMod.Instance.researchProjectsTechLevel.Keys.OrderByDescending((TechLevel tl) => tl);
            TechLevel newTL = TechLevel.Animal;
            if (!TechLevelProgressionMod.Settings.TechLevelDecrease)
            {
                newTL = TechLevel.Undefined;
                leftTechLevels = leftTechLevels.Where((TechLevel tl) => (int)tl > (int)currTechLevel);
            }
            foreach (TechLevel tl in leftTechLevels)
            {
                TechLevelResearch techLevelResearch = TechLevelProgressionMod.Instance.researchProjectsTechLevel[tl];
                int currR = techLevelResearch.countFinished;
                if ((currR > 0) && (techLevelResearch.countAll > 0) && (currR >= Mathf.CeilToInt((TechLevelProgressionMod.Settings.TechLevelPrecise ? TechLevelProgressionMod.Settings.ResearchPercentPrecise[(int)tl] : TechLevelProgressionMod.Settings.ResearchPercent) * techLevelResearch.countAll)))
                {
                    newTL = tl;
                    break;
                }
            }
            if (newTL != TechLevel.Undefined)
            {
                ChangeTechLevel(newTL);
            }
        }

        public void InitializeTechLevelProgression()
        {
            if (TechLevelProgressionMod.Settings.TechLevelScenario)
            {
                currTechLevel = Find.FactionManager.OfPlayer.def.techLevel;
            }
        }

        public void ChangeTechLevel(TechLevel newTechLevel)
        {
            TechLevel newTL = (TechLevel)Mathf.Clamp((byte)newTechLevel, TechLevelProgressionMod.Settings.TechLevelRange.min, TechLevelProgressionMod.Settings.TechLevelRange.max);
            if ((int)newTL > (int)currTechLevel)
            {
                Find.LetterStack.ReceiveLetter("TechLevelProgression.Letters.TLIncreased.Title".Translate(), "TechLevelProgression.Letters.TLIncreased.Text".Translate(newTL.ToStringSafe()), LetterDefOf.NeutralEvent);
            }
            currTechLevel = newTL;
            Faction.OfPlayer.def.techLevel = currTechLevel;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref currTechLevel, "currTechLevel", TechLevel.Animal);
        }
    }
}
