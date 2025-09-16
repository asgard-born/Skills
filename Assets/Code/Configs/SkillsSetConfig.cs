using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "Skills_Config", menuName = "Configs/Skills_Config")]
    public class SkillsSetConfig : ScriptableObject
    {
        [SerializeField] private SkillConfig[] _skills;

        public SkillConfig[] SkillConfigs => _skills;
    }
}