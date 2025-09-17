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

        private readonly Ctx _ctx;

        public class Ctx
        {
            public SkillView[] SkillViews;
            public SkillSetConfig SkillsConfig;
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

            _onEarnScoresClick = AddUnsafe(new ReactiveCommand<int>());
            _onForgetAllClick = AddUnsafe(new ReactiveCommand());
        }

        private void InitializeScores()
        {
            AddUnsafe(new ScoresCalculator(new ScoresCalculator.Ctx
            {
                Scores = _scores,
                OnEarnScoresClick = _onEarnScoresClick
            }));
        }

        private void InitializeSkills()
        {
            AddUnsafe(new SkillsRoot(new SkillsRoot.Ctx
            {
                SkillViews = _ctx.SkillViews,
                SkillsSetConfig = _ctx.SkillsConfig,

                Scores = _scores.ToReadOnlyReactiveProperty(),
            }));
        }

        private void InitializeUI()
        {
            AddUnsafe(new UIRoot(new UIRoot.Ctx
            {
                HUDWindow = _ctx.HUDWindow,
                OnEarnScoresClick = _onEarnScoresClick
            }));
        }
    }
}