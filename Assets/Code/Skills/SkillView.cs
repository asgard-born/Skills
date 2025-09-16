using Configs;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Skills
{
    [RequireComponent(typeof(Button), typeof(Image))]
    public class SkillView : MonoBehaviour
    {
        [SerializeField] private SkillType _skillType;
        [SerializeField] private SkillType[] _neighbors;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private Button _skillButton;
        [SerializeField] private Button _learnBtn;
        [SerializeField] private Button _forgetBtn;
        [SerializeField] private Image _mainImage;

        private Ctx _ctx;
        private bool _isBase;
        private bool _isActive;
        private bool _isLearned;

        public SkillType SkillType => _skillType;
        public SkillType[] Neighbors => _neighbors;
        public bool IsActive => _isActive;

        public class Ctx
        {
            public SkillConfig Config;
            public ReactiveCommand<SkillType> OnSkillSelected;
            public ReactiveCommand<SkillType> OnLearnSkillClicked;
            public ReactiveCommand<SkillStatus> UpdateSkillStatus;
            public ReactiveCommand<(SkillType, int)> OnSkillLearned;
            public ReactiveCommand<(SkillType, int)> OnSkillForgotten;
        }

        public void Initialize(Ctx ctx)
        {
            _ctx = ctx;
            _isBase = ctx.Config.IsBase;
            _ctx.UpdateSkillStatus.Subscribe(UpdateStatus).AddTo(this);

            if (_isBase)
            {
                SetActiveState();
            }
            else
            {
                SetLearnedView();
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

            _ctx.OnSkillSelected?.Execute(_ctx.Config.Type);
        }

        private void OnLearnButtonClick()
        {
        }

        private void OnForgetButtonClick()
        {
        }

        private void UpdateStatus(SkillStatus status)
        {
            _costText.text = $"Изучить: {_ctx.Config.Cost.ToString()}";

            _learnBtn.gameObject.SetActive(status.CanBeLearned);
            _forgetBtn.gameObject.SetActive(status.CanBeForgotten);
        }

        private void OnDestroy()
        {
            _skillButton.onClick.RemoveListener(SetActiveState);
        }

        private void SetLearnedView()
        {
            Color color = _mainImage.color;
            color.a = _ctx.Config.IsLearned ? 1 : .5f;
            _mainImage.color = color;
        }
    }
}