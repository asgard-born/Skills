using Configs;
using Skills;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

namespace Root
{
    public class EnterPoint : MonoBehaviour
    {
        [SerializeField] private SkillView[] _skillViews;
        [SerializeField] private SkillSetConfig _skillsConfig;
        [SerializeField] private HUDWindow _hudWindow;
        
        private void Start()
        {
            ValidateData();
            InitializeGameRoot();
        }

        private void InitializeGameRoot()
        {
            new GameRoot(
                new GameRoot.Ctx
                {
                    SkillViews = _skillViews,
                    SkillsConfig = _skillsConfig,
                    HUDWindow = _hudWindow,
                }).AddTo(this);
        }
        
        private void ValidateData()
        {
            Assert.IsNotNull(_skillsConfig, "Skills config cannot be null");
            Assert.IsNotNull(_skillViews, "Skill views cannot be null");
            Assert.IsTrue(_skillViews.Length > 1, "Skill views must has several elements");
            Assert.IsNotNull(_hudWindow, "HudWindow cannot be null");
        }
    }
}