using UnityEngine;

namespace Skills
{
    public class SkillView : MonoBehaviour
    {
        [SerializeField] private SkillType _skillType;
        [SerializeField] private SkillType[] _neighbors;

        private int _cost;
        private string _name;
        private bool _isBase;

        public SkillType SkillType => _skillType;
        public SkillType[] Neighbors => _neighbors;
    }
}