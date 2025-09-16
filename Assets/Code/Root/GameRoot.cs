using Configs;
using Extensions;
using Skills;
using UniRx;

namespace Root
{
    public class GameRoot : BaseDisposable
    {
        private ReactiveProperty<int> _scores;

        private ReactiveCommand<int> _onEarnScoresClick;
        private readonly Ctx _ctx;

        public class Ctx
        {
            public SkillView[] SkillViews;
            public SkillSetConfig SkillsConfig;
        }

        public GameRoot(Ctx ctx)
        {
            _ctx = ctx;
            
            InitializeRx();
            InitializeSkills();
        }

        private void InitializeRx()
        {
            _scores = AddUnsafe(new ReactiveProperty<int>());

            _onEarnScoresClick = AddUnsafe(new ReactiveCommand<int>());
        }

        private void InitializeSkills()
        {
            AddUnsafe(new SkillsRoot(new SkillsRoot.Ctx
            {
                SkillViews = _ctx.SkillViews,
                SkillsSetConfig = _ctx.SkillsConfig
            }));
        }
    }
}