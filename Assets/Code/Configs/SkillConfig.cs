using System;
using Skills;

namespace Configs
{
    [Serializable]
    public struct SkillConfig
    {
        public SkillType Type;
        public int Cost;
        public string Name;
        public bool IsBase;
    }
}