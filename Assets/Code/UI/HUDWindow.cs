using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HUDWindow : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoresValue;
        [SerializeField] private Button _earnButton;
        [SerializeField] private Button _forgetAllButton;
        [SerializeField] private int _scoresPerClick = 100;

        private Ctx _ctx;

        public class Ctx
        {
            public ReadOnlyReactiveProperty<int> Scores;
            public ReactiveCommand<int> OnEarnScoresClick;
            public ReactiveCommand OnForgetAllClick;
        }

        public void Initialize(Ctx ctx)
        {
            _ctx = ctx;

            _ctx.Scores.Subscribe(UpdateScores);
        }

        private void Awake()
        {
            _earnButton.onClick.AddListener(EarnClickHandle);
            _forgetAllButton.onClick.AddListener(ForgetAllClickHandle);
        }

        private void EarnClickHandle()
        {
            _ctx.OnEarnScoresClick?.Execute(_scoresPerClick);
        }

        private void ForgetAllClickHandle()
        {
            _ctx.OnForgetAllClick?.Execute();
        }

        private void UpdateScores(int scores)
        {
            _scoresValue.text = scores.ToString();
        }

        private void OnDestroy()
        {
            _earnButton.onClick.RemoveListener(ForgetAllClickHandle);
        }
    }
}