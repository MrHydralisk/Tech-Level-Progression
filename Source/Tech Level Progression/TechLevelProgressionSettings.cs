using RimWorld;
using System.Collections.Generic;
using Verse;

namespace TechLevelProgression
{
    public class TechLevelProgressionSettings : ModSettings
    {
        public float ResearchPercent = 0.75f;
        public List<float> ResearchPercentPrecise = new List<float>() { 0.25f, 0.25f, 0.5f, 0.5f, 0.6f, 0.65f, 0.75f, 0.75f};
        public bool TechLevelDecrease = true;
        public bool TechLevelPrecise = false;
        public IntRange TechLevelRange = new IntRange((int)TechLevel.Animal, (int)TechLevel.Archotech);

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ResearchPercent, "ResearchPercent", defaultValue: 0.75f);
            Scribe_Collections.Look(ref ResearchPercentPrecise, "ResearchPercentPrecise", LookMode.Value);
            Scribe_Values.Look(ref TechLevelDecrease, "TechLevelDecrease", defaultValue: true);
            Scribe_Values.Look(ref TechLevelPrecise, "TechLevelPrecise", defaultValue: false);
            Scribe_Values.Look(ref TechLevelRange, "TechLevelRange", defaultValue: new IntRange((int)TechLevel.Animal, (int)TechLevel.Archotech));
        }
    }
}
