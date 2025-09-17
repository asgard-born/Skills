using Extensions;
using UniRx;

namespace Scores
{
    public class ScoresCalculator : BaseDisposable
    {
        private readonly Ctx _ctx;

        public class Ctx
        {
            public ReactiveProperty<int> Scores;
            public ReactiveCommand<int> OnEarnScoresClick;
        }

        public ScoresCalculator(Ctx ctx)
        {
            _ctx = ctx;
            AddUnsafe(ctx.OnEarnScoresClick.Subscribe(AddScores));
        }

        private void AddScores(int scores)
        {
            _ctx.Scores.Value += scores;
        }
    }
}