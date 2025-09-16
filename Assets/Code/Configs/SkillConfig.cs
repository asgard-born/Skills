using System;
using Skills;

namespace Configs
{
    [Serializable]
    public class SkillConfig
    {
        public SkillType Type;
        public int Cost;
        public bool IsLearned;
        public string Name;
        public bool IsBase;
    }
}