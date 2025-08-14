using RimWorld;
using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace TechLevelProgression
{
    public class TechLevelProgressionMod : Mod
    {
        public static TechLevelProgressionSettings Settings { get; private set; }

        private float prevHeight = float.MaxValue;
        private Vector2 scrollPos;

        public TechLevelProgressionMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<TechLevelProgressionSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            Rect rect = new Rect(0f, 0f, inRect.width - 20f, prevHeight);
            Widgets.BeginScrollView(inRect, ref scrollPos, rect);
            Listing_Standard options = new Listing_Standard();
            options.Begin(rect);
            options.Label("TechLevelProgression.Settings.ResearchPercent.Label".Translate(Settings.ResearchPercent.ToStringPercent()));
            Settings.ResearchPercent = Mathf.Round(options.Slider(Settings.ResearchPercent, 0.01f, 1f) * 100f) / 100f;
            options.GapLine();
            options.CheckboxLabeled("TechLevelProgression.Settings.TechLevelPrecise.Label".Translate().RawText, ref Settings.TechLevelPrecise);
            foreach (TechLevel tl in Enum.GetValues(typeof(TechLevel)))
            {
                int TLResearchCount = GetTLResearchCount(tl);
                options.Label("TechLevelProgression.Settings.ResearchPercentPrecise.Label".Translate(tl.ToStringSafe(), Settings.ResearchPercentPrecise[(int)tl].ToStringPercent(), Mathf.CeilToInt(Settings.ResearchPercentPrecise[(int)tl] * TLResearchCount).ToString(), TLResearchCount.ToStringSafe()));
                Settings.ResearchPercentPrecise[(int)tl] = Mathf.Round(options.Slider(Settings.ResearchPercentPrecise[(int)tl], 0.01f, 1f) * 100f) / 100f;
            }
            options.GapLine();
            options.CheckboxLabeled("TechLevelProgression.Settings.TechLevelDecrease.Label".Translate().RawText, ref Settings.TechLevelDecrease);
            options.Label("TechLevelProgression.Settings.TechLevelRange.Label".Translate(((TechLevel)Settings.TechLevelRange.min).ToStringSafe(), ((TechLevel)Settings.TechLevelRange.max).ToStringSafe()));
            options.IntRange(ref Settings.TechLevelRange, (int)TechLevel.Animal, (int)TechLevel.Archotech);
            options.GapLine();
            options.Label("TechLevelProgression.Settings.ResearchTechLevelAboveMult.Label".Translate(Settings.ResearchTechLevelAboveMult.ToStringPercent()));
            Settings.ResearchTechLevelAboveMult = Mathf.Round(options.Slider(Settings.ResearchTechLevelAboveMult, 0.01f, 5f) * 100f) / 100f;
            options.Label("TechLevelProgression.Settings.ResearchTechLevelBelowMult.Label".Translate(Settings.ResearchTechLevelBelowMult.ToStringPercent()));
            Settings.ResearchTechLevelBelowMult = Mathf.Round(options.Slider(Settings.ResearchTechLevelBelowMult, 0.01f, 1f) * 100f) / 100f;
            prevHeight = options.CurHeight;
            options.End();
            Widgets.EndScrollView();
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
