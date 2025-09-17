using Configs;
using Extensions;
using UniRx;

namespace Skills
{
    public class SkillPm : BaseDisposable
    {
        private readonly Ctx _ctx;

        public class Ctx
        {
            public SkillConfig Config;

            public ReadOnlyReactiveProperty<int> Scores;
            public ReactiveCommand OnViewSkillSelected;
            public ReactiveCommand<SkillType> SkillSelectionBus;
            public ReactiveCommand<SkillStatus> UpdateSkillStatus;
        }

        public SkillPm(Ctx ctx)
        {
            _ctx = ctx;

            AddUnsafe(_ctx.OnViewSkillSelected.Subscribe(_ => OnViewSkillSelected()));
            SendStatusToView();
        }

        private void OnViewSkillSelected()
        {
            _ctx.SkillSelectionBus?.Execute(_ctx.Config.Type);
            SendStatusToView();
        }

        private void SendStatusToView()
        {
            var status = new SkillStatus
            {
                IsLearned = _ctx.Config.IsLearned,
                CanBeForgotten = _ctx.Config.IsLearned && !_ctx.Config.IsBase,
                CanBeLearned = !_ctx.Config.IsLearned && _ctx.Config.Cost <= _ctx.Scores.Value
            };

            _ctx.UpdateSkillStatus?.Execute(status);
        }
    }
}