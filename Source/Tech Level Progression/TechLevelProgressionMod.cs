using UnityEngine;
using RimWorld;
using Verse;
using System.Linq;
using System;

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
            options.CheckboxLabeled("TechLevelProgression.Settings.TechLevelDecrease.Label".Translate().RawText, ref Settings.TechLevelDecrease);
            options.Label("TechLevelProgression.Settings.TechLevelRange.Label".Translate(((TechLevel)Settings.TechLevelRange.min).ToStringSafe(), ((TechLevel)Settings.TechLevelRange.max).ToStringSafe()));
            options.IntRange(ref Settings.TechLevelRange, (int)TechLevel.Animal, (int)TechLevel.Archotech);
            options.CheckboxLabeled("TechLevelProgression.Settings.TechLevelPrecise.Label".Translate().RawText, ref Settings.TechLevelPrecise);
            foreach(TechLevel tl in Enum.GetValues(typeof(TechLevel)))
            {
                int TLResearchCount = GetTLResearchCount(tl);
                options.Label("TechLevelProgression.Settings.ResearchPercentPrecise.Label".Translate(tl.ToStringSafe(), Settings.ResearchPercentPrecise[(int)tl].ToStringPercent(), Mathf.CeilToInt(Settings.ResearchPercentPrecise[(int)tl] * TLResearchCount).ToString(), TLResearchCount.ToStringSafe()));
                Settings.ResearchPercentPrecise[(int)tl] = options.Slider(Settings.ResearchPercentPrecise[(int)tl], 0.01f, 1f);
            }
            options.End();
        }

        public int GetTLResearchCount(TechLevel tl)
        {
            int count = 0;
            count = DefDatabase<ResearchProjectDef>.AllDefs.Count((ResearchProjectDef rpd) => rpd.techLevel == tl);
            return count;
        }

        public override string SettingsCategory()
        {
            return "TechLevelProgression.Settings.Title".Translate().RawText;
        }
    }
}
