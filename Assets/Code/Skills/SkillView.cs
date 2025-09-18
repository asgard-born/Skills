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
        [SerializeField] private TextMeshProUGUI _skillText;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private Button _skillButton;
        [SerializeField] private Button _learnBtn;
        [SerializeField] private Button _forgetBtn;
        [SerializeField] private Image _mainImage;
        [SerializeField] private Image _selectedImage;

        private Ctx _ctx;
        private bool _isBase;
        private bool _isSelected;

        public SkillType SkillType => _skillType;

        public class Ctx
        {
            public SkillConfig Config;
            public ReactiveCommand<SkillType> OnViewSelectClick;
            public ReactiveCommand<SkillType> OnLearnSkillClicked;
            public ReactiveCommand<SkillStatus> UpdateStatus;
            public ReactiveCommand<SkillType> OnForgetSkillClicked;
        }

        public void Initialize(Ctx ctx)
        {
            _ctx = ctx;
            _isBase = ctx.Config.IsBase;
            _skillText.text = ctx.Config.Name;
            _ctx.UpdateStatus.Subscribe(UpdateStatus).AddTo(this);

            _skillButton.onClick.AddListener(OnSelectClick);
            _learnBtn.onClick.AddListener(OnLearnButtonClick);

            if (!_isBase)
            {
                _forgetBtn.onClick.AddListener(OnForgetButtonClick);
            }
        }

        private void OnSelectClick()
        {
            _ctx.OnViewSelectClick?.Execute(_skillType);
        }

        private void OnLearnButtonClick()
        {
            _ctx.OnLearnSkillClicked?.Execute(_skillType);
        }

        private void OnForgetButtonClick()
        {
            _ctx.OnForgetSkillClicked?.Execute(_skillType);
        }

        private void UpdateStatus(SkillStatus viewStatus)
        {
            _costText.text = $"Изучить: {_ctx.Config.Cost.ToString()}";
            SetSelectedState(viewStatus.IsSelected, viewStatus);

            Color color = _mainImage.color;
            color.a = viewStatus.IsLearned ? 1 : .5f;
            _mainImage.color = color;
        }

        private void SetSelectedState(bool isSelected, SkillStatus viewStatus)
        {
            _isSelected = isSelected;
            _selectedImage.gameObject.SetActive(isSelected);

            if (_isSelected)
            {
                _learnBtn.gameObject.SetActive(viewStatus.CanBeLearned);

                if (!_isBase)
                {
                    _forgetBtn.gameObject.SetActive(viewStatus.CanBeForgotten);
                }
            }
            else
            {
                _learnBtn.gameObject.SetActive(false);

                if (!_isBase)
                {
                    _forgetBtn.gameObject.SetActive(false);
                }
            }
        }

        private void OnDestroy()
        {
            _skillButton.onClick.RemoveListener(OnSelectClick);
            _learnBtn.onClick.RemoveListener(OnLearnButtonClick);

            if (!_isBase)
            {
                _forgetBtn.onClick.RemoveListener(OnForgetButtonClick);
            }
        }
    }
}