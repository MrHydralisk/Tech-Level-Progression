using UnityEngine;
using RimWorld;
using Verse;

namespace TechLevelProgression
{
    public class TechLevelProgressionMod : Mod
    {
        public static TechLevelProgressionSettings Settings { get; private set; }

        public TechLevelProgressionMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<TechLevelProgressionSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            Listing_Standard options = new Listing_Standard();
            options.Begin(inRect);
            options.Label("TechLevelProgression.Settings.ResearchPercent.Label".Translate(Settings.ResearchPercent.ToStringPercent()));
            Settings.ResearchPercent = options.Slider(Settings.ResearchPercent, 0.01f, 1f);
            options.End();
        }

        public override string SettingsCategory()
        {
            return "TechLevelProgression.Settings.Title".Translate().RawText;
        }
    }
}
