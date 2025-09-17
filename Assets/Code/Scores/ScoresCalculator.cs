using Extensions;
using Skills;
using UniRx;
using UnityEngine;

namespace Scores
{
    public class ScoresCalculator : BaseDisposable
    {
        private readonly Ctx _ctx;

        public class Ctx
        {
            public ReactiveProperty<int> Scores;
            public ReactiveCommand<int> OnEarnScoresClick;
            public ReactiveCommand<(SkillType, int)> OnSkillLearned;
            public ReactiveCommand<(SkillType, int)> OnSkillForgotten;
        }

        public ScoresCalculator(Ctx ctx)
        {
            _ctx = ctx;
            AddUnsafe(ctx.OnEarnScoresClick.Subscribe(AddScores));
            AddUnsafe(ctx.OnSkillLearned.Subscribe(t => RemoveScores(t.Item2)));
            AddUnsafe(ctx.OnSkillForgotten.Subscribe(t => AddScores(t.Item2)));
        }

        private void AddScores(int scores)
        {
            _ctx.Scores.Value += scores;
        }

        private void RemoveScores(int scores)
        {
            if (_ctx.Scores.Value - scores < 0)
            {
                Debug.LogError("Scores cannot be less than zero!");
                _ctx.Scores.Value = 0;
                return;
            }

            _ctx.Scores.Value -= scores;
        }
    }
}