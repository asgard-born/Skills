using Configs;
using Extensions;
using UniRx;

namespace Skills
{
    public class SkillPm : BaseDisposable
    {
        public class Ctx
        {
            public SkillConfig Config;
            
            public ReadOnlyReactiveProperty<int> Scores;
            public ReactiveCommand OnSkillSelected;
            public ReactiveCommand<SkillStatus> UpdateSkillStatus;
        }

        private Ctx _ctx;

        public SkillPm(Ctx ctx)
        {
            _ctx = ctx;

            AddUnsafe(_ctx.OnSkillSelected.Subscribe(_ => SendStatus()));
            SendStatus();
        }

        private void SendStatus()
        {
            var status = new SkillStatus
            {
                CanBeForgotten = _ctx.Config.IsLearned,
                CanBeLearned = !_ctx.Config.IsLearned && _ctx.Config.Cost <= _ctx.Scores.Value
            };

            _ctx.UpdateSkillStatus?.Execute(status);
        }
    }
}