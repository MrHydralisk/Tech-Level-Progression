using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TechLevelProgression
{
    public class TechLevelResearch
    {
        public List<ResearchProjectDef> researchProjects = new List<ResearchProjectDef>();

        public int countAll;
        public int countFinished => researchProjects.Count((ResearchProjectDef rpd) => rpd.IsFinished);
        public TechLevel techLevel;

        public TechLevelResearch(TechLevel techLevel)
        {
            countAll = 0;
            this.techLevel = techLevel;
        }

        public TechLevelResearch(List<ResearchProjectDef> researchProjects)
        {
            this.researchProjects = researchProjects;
            countAll = researchProjects.Count();
            techLevel = researchProjects.FirstOrDefault()?.techLevel ?? TechLevel.Undefined;
        }
    }
}
