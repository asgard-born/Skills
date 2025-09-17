using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "Skills_Config", menuName = "Configs/Skills_Config")]
    public class SkillsConfigs : ScriptableObject
    {
        [SerializeField] private SkillConfig[] _skills;

        public SkillConfig[] Configs => _skills;
    }
}