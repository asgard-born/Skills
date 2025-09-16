using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HudWindow : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoresValue;
        [SerializeField] private Button _earnButton;
        [SerializeField] private int _scoresPerClick = 100;

        private Ctx _ctx;

        public class Ctx
        {
            public ReactiveCommand<int> OnEarnScoresClick;
            public ReactiveProperty<int> Scores;
        }

        public void Initialize(Ctx ctx)
        {
            _ctx = ctx;

            _ctx.Scores.Subscribe(UpdateScores);
        }

        private void Awake()
        {
            _earnButton.onClick.AddListener(OnEarnButtonClick);
        }

        private void OnEarnButtonClick()
        {
            _ctx.OnEarnScoresClick.Execute(_scoresPerClick);
        }

        private void UpdateScores(int scores)
        {
            _scoresValue.text = scores.ToString();
        }

        private void OnDestroy()
        {
            _earnButton.onClick.RemoveListener(OnEarnButtonClick);
        }
    }
}