using Configs;
using Extensions;
using Scores;
using Skills;
using UI;
using UniRx;

namespace Root
{
    public class GameRoot : BaseDisposable
    {
        private ReactiveProperty<int> _scores;
        private ReactiveCommand<int> _onEarnScoresClick;
        private ReactiveCommand _onForgetAllClick;
        private ReactiveCommand<(SkillType, int)> _onSkillLearned;
        private ReactiveCommand<(SkillType, int)> _onSkillForgotten;

        private readonly Ctx _ctx;

        public class Ctx
        {
            public SkillView[] SkillViews;
            public SkillsConfigs SkillsConfig;
            public HUDWindow HUDWindow;
        }

        public GameRoot(Ctx ctx)
        {
            _ctx = ctx;

            InitializeRx();
            InitializeScores();
            InitializeSkills();
            InitializeUI();
        }

        private void InitializeRx()
        {
            _scores = AddUnsafe(new ReactiveProperty<int>());

            AddUnsafe(_onEarnScoresClick = new ReactiveCommand<int>());
            AddUnsafe(_onForgetAllClick = new ReactiveCommand());
            AddUnsafe(_onSkillLearned = new ReactiveCommand<(SkillType, int)>());
            AddUnsafe(_onSkillForgotten = new ReactiveCommand<(SkillType, int)>());
        }

        private void InitializeScores()
        {
            AddUnsafe(new ScoresCalculator(new ScoresCalculator.Ctx
            {
                Scores = _scores,
                OnEarnScoresClick = _onEarnScoresClick,
                OnSkillLearned = _onSkillLearned,
                OnSkillForgotten = _onSkillForgotten
            }));
        }

        private void InitializeSkills()
        {
            AddUnsafe(new SkillsRoot(new SkillsRoot.Ctx
            {
                SkillViews = _ctx.SkillViews,
                SkillsConfigs = _ctx.SkillsConfig,
                OnSkillLearned = _onSkillLearned,
                OnSkillForgotten = _onSkillForgotten,
                Scores = _scores.ToReadOnlyReactiveProperty(),
            }));
        }

        private void InitializeUI()
        {
            AddUnsafe(new UIRoot(new UIRoot.Ctx
            {
                HUDWindow = _ctx.HUDWindow,

                Scores = _scores.ToReadOnlyReactiveProperty(),
                OnEarnScoresClick = _onEarnScoresClick,
                OnForgetAllClick = _onForgetAllClick
            }));
        }
    }
}