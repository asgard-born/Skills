using Extensions;
using UniRx;

namespace Scores
{
    public class ScoresCalculator : BaseDisposable
    {
        private readonly Ctx _ctx;

        public class Ctx
        {
            public ReactiveCommand<int> OnEarnScoresClick;
            public ReactiveProperty<int> Scores;
        }

        public ScoresCalculator(Ctx ctx)
        {
            _ctx = ctx;
            AddUnsafe(ctx.OnEarnScoresClick.Subscribe(TryEarnScores));
        }

        private void TryEarnScores(int scores)
        {
            // Можно добавить проверки, например

            AddScores(scores);
        }

        private void AddScores(int scores)
        {
            _ctx.Scores.Value += scores;
        }
    }
}