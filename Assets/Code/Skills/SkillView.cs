using Configs;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Skills
{
    [RequireComponent(typeof(CanvasGroup), typeof(Button), typeof(Image))]
    public class SkillView : MonoBehaviour
    {
        [SerializeField] private SkillType _skillType;
        [SerializeField] private SkillType[] _neighbors;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Button _skillButton;
        [SerializeField] private Button _learnBtn;
        [SerializeField] private Button _forgetBtn;
        [SerializeField] private Image _skillImage;

        private Ctx _ctx;
        private bool _isBase;
        private bool _isActive;
        private bool _isLearned;
        private int _cost;

        public SkillType SkillType => _skillType;
        public SkillType[] Neighbors => _neighbors;
        public bool IsActive => _isActive;

        public class Ctx
        {
            public ReactiveCommand<SkillType> LearnSkill;
            public ReactiveCommand<(SkillType, int)> OnSkillLearned;
            public ReactiveCommand<(SkillType, int)> OnSkillForget;
            public SkillConfig Config;
        }

        public void Initialize(Ctx ctx)
        {
            _ctx = ctx;
            _cost = ctx.Config.Cost;
            _isBase = ctx.Config.IsBase;

            if (_isBase)
            {
                SetActiveState();
            }
        }

        private void Awake()
        {
            _skillButton.onClick.AddListener(SetActiveState);
            SetInactiveState();
        }

        private void SetInactiveState()
        {
        }

        private void SetActiveState()
        {
            _isActive = true;

            if (_isBase)
            {
                _learnBtn.gameObject.SetActive(false);
                _forgetBtn.gameObject.SetActive(false);
            }
        }

        public void OnLearnButtonClick()
        {
        }

        public void OnForgetButtonClick()
        {
        }

        public void UpdateStatus()
        {
            // _costText.text = $"Изучить: {ctx.config.Cost.ToString()}";
            // _canvasGroup.alpha = isActive ? 1 : .5f;
            // _learnBtn.gameObject.SetActive(!_isLearned);
            // _forgetBtn.gameObject.SetActive(_isLearned);
        }

        private void OnDestroy()
        {
            _skillButton.onClick.RemoveListener(SetActiveState);
        }
    }
}