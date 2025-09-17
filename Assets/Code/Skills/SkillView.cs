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
        [SerializeField] private SkillType[] _neighbors;
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
        public SkillType[] Neighbors => _neighbors;

        public class Ctx
        {
            public SkillConfig Config;
            public ReactiveCommand OnSkillSelected;
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

            if (_isBase)
            {
                SetSelectedState();
            }
            else
            {
                UpdateLearnedState();
            }
        }

        private void Awake()
        {
            _skillButton.onClick.AddListener(SetSelectedState);
        }

        private void SetSelectedState()
        {
            _isSelected = true;

            if (_isBase)
            {
                _learnBtn.gameObject.SetActive(false);
                _forgetBtn.gameObject.SetActive(false);
            }

            _selectedImage.gameObject.SetActive(true);
            _ctx.OnSkillSelected?.Execute();
        }

        private void SetUnselectedState(SkillType skillType)
        {
            if (_skillType != skillType || !_isSelected || _isBase) return;

            _isSelected = false;
            _learnBtn.gameObject.SetActive(false);
            _forgetBtn.gameObject.SetActive(false);
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

            _learnBtn.gameObject.SetActive(status.CanBeLearned);
            _forgetBtn.gameObject.SetActive(status.CanBeForgotten);
        }

        private void OnDestroy()
        {
            _skillButton.onClick.RemoveListener(SetSelectedState);
        }

        private void UpdateLearnedState()
        {
            Color color = _mainImage.color;
            color.a = _ctx.Config.IsLearned ? 1 : .5f;
            _mainImage.color = color;
        }
    }
}