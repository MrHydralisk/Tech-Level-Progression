using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TechLevelProgression
{
    public class TechLevelProgressionMod : Mod
    {
        public static TechLevelProgressionSettings Settings { get; private set; }
        public static TechLevelProgressionMod Instance { get; private set; }

        public Dictionary<TechLevel, TechLevelResearch> researchProjectsTechLevel
        {
            get
            {
                if (researchProjectsTechLevelCached.NullOrEmpty())
                {
                    researchProjectsTechLevelCached = new Dictionary<TechLevel, TechLevelResearch>();
                    foreach (TechLevel tl in Enum.GetValues(typeof(TechLevel)))
                    {
                        researchProjectsTechLevelCached.Add(tl, new TechLevelResearch(tl));
                    }
                    IEnumerable<ResearchProjectDef> researchProjectDefs = DefDatabase<ResearchProjectDef>.AllDefs;
                    allDefs = researchProjectDefs.Count();
                    foreach (var group in researchProjectDefs.GroupBy((ResearchProjectDef rpd) => rpd.techLevel))
                    {
                        researchProjectsTechLevelCached.SetOrAdd(group.Key, new TechLevelResearch(group.ToList()));
                    }
                }
                return researchProjectsTechLevelCached;
            }
        }
        private Dictionary<TechLevel, TechLevelResearch> researchProjectsTechLevelCached;
        public int allDefs;

        private float prevHeight = float.MaxValue;
        private Vector2 scrollPos;

        private Texture2D modIcon;

        public Texture2D ModIcon
        {
            get
            {
                if (modIcon.NullOrBad())
                {
                    modIcon = ContentFinder<Texture2D>.Get("Icons/TLP_ModIcon");
                }
                return modIcon;
            }
        }

        public TechLevelProgressionMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<TechLevelProgressionSettings>();
            Instance = this;
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
            foreach (TechLevel tl in researchProjectsTechLevel.Keys)
            {
                TechLevelResearch techLevelResearch = researchProjectsTechLevel[tl];
                options.Label("TechLevelProgression.Settings.ResearchPercentPrecise.Label".Translate(tl.ToStringSafe(), Settings.ResearchPercentPrecise[(int)tl].ToStringPercent(), Mathf.CeilToInt(Settings.ResearchPercentPrecise[(int)tl] * techLevelResearch.countAll).ToString(), techLevelResearch.countAll));
                Settings.ResearchPercentPrecise[(int)tl] = Mathf.Round(options.Slider(Settings.ResearchPercentPrecise[(int)tl], 0.01f, 1f) * 100f) / 100f;
            }
            options.GapLine();
            options.CheckboxLabeled("TechLevelProgression.Settings.TechLevelScenario.Label".Translate().RawText, ref Settings.TechLevelScenario);
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

        public void DrawTechLevelProgress(Rect rect)
        {
            if (Widgets.ButtonImage(rect, ModIcon, tooltip: "TechLevelProgression.TechLevelProgressReport.Tooltip".Translate()))
            {
                List<string> progressReport = new List<string>();
                progressReport.Add("TechLevelProgression.TechLevelProgressReport.CurrentTechLevel".Translate(GameComponent_TechLevelProgression.Instance.currTechLevel.ToStringSafe()));
                progressReport.Add("TechLevelProgression.TechLevelProgressReport.TechLevelReports".Translate());
                foreach (TechLevel tl in researchProjectsTechLevel.Keys)
                {
                    TechLevelResearch techLevelResearch = researchProjectsTechLevel[tl];
                    progressReport.Add("TechLevelProgression.TechLevelProgressReport.TechLevelReport".Translate(tl.ToStringSafe(), techLevelResearch.countFinished, techLevelResearch.countAll, Mathf.CeilToInt((TechLevelProgressionMod.Settings.TechLevelPrecise ? TechLevelProgressionMod.Settings.ResearchPercentPrecise[(int)tl] : TechLevelProgressionMod.Settings.ResearchPercent) * techLevelResearch.countAll)));
                }
                Find.WindowStack.Add(new Dialog_MessageBox(String.Join("\n", progressReport)));
            }
        }

        public override string SettingsCategory()
        {
            return "TechLevelProgression.Settings.Title".Translate().RawText;
        }
    }
}
