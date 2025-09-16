using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "Skills_Config", menuName = "Configs/Skills_Config")]
    public class SkillSetConfig : ScriptableObject
    {
        [SerializeField] private SkillConfig[] _skills;

        public SkillConfig[] SkillConfigs => _skills;
    }
}