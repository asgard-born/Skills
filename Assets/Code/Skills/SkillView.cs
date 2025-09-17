using Configs;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Skills
{
    [RequireComponent(typeof(Button))]
    public class SkillView : MonoBehaviour
    {
        [SerializeField] private SkillType _skillType;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private Button _skillButton;
        [SerializeField] private Button _learnBtn;
        [SerializeField] private Button _forgetBtn;
        [SerializeField] private Image _mainImage;
        [SerializeField] private Image _selectedImage;

        private Ctx _ctx;
        private bool _isBase;
        private bool _isSelected;
        private bool _isLearned;

        public SkillType SkillType => _skillType;

        public class Ctx
        {
            public SkillConfig Config;
            public ReactiveCommand OnViewSkillSelected;
            public ReactiveCommand<SkillType> UnselectSkill;
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
            _ctx.UnselectSkill.Subscribe(SetUnselectedState).AddTo(this);

            _skillButton.onClick.AddListener(SetSelectedState);
            _learnBtn.onClick.AddListener(OnLearnButtonClick);

            if (!_isBase)
            {
                _forgetBtn.onClick.AddListener(OnForgetButtonClick);
            }
        }

        private void SetSelectedState()
        {
            _isSelected = true;

            if (!_isBase)
            {
                _learnBtn.gameObject.SetActive(false);
                _forgetBtn.gameObject.SetActive(false);
            }

            _selectedImage.gameObject.SetActive(true);
            _ctx.OnViewSkillSelected?.Execute();
        }

        private void SetUnselectedState(SkillType skillType)
        {
            if (_skillType != skillType || !_isSelected) return;

            _isSelected = false;

            _learnBtn.gameObject.SetActive(false);

            if (!_isBase)
            {
                _forgetBtn.gameObject.SetActive(false);
            }

            _selectedImage.gameObject.SetActive(false);
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

            if (_isSelected)
            {
                _learnBtn.gameObject.SetActive(status.CanBeLearned);

                if (!_isBase)
                {
                    _forgetBtn.gameObject.SetActive(status.CanBeForgotten);
                }
            }

            Color color = _mainImage.color;
            color.a = status.IsLearned ? 1 : .5f;
            _mainImage.color = color;
        }

        private void OnDestroy()
        {
            _skillButton.onClick.RemoveListener(SetSelectedState);
            _learnBtn.onClick.RemoveListener(OnLearnButtonClick);

            if (!_isBase)
            {
                _forgetBtn.onClick.RemoveListener(OnForgetButtonClick);
            }
        }
    }
}