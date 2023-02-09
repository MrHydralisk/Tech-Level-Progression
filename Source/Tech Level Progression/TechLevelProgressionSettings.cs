using Verse;

namespace TechLevelProgression
{
    public class TechLevelProgressionSettings : ModSettings
    {
        public float ResearchPercent = 0.75f;
        public bool TechLevelDecrease = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ResearchPercent, "ResearchPercent", defaultValue: 0.75f);
            Scribe_Values.Look(ref TechLevelDecrease, "TechLevelDecrease", defaultValue: true);
        }
    }
}
