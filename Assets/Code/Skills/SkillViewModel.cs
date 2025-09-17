using Configs;
using Extensions;
using UniRx;

namespace Skills
{
    public class SkillViewModel : BaseDisposable
    {
        private readonly Ctx _ctx;

        public SkillType Type { get; }
        public SkillType[] Neighbors { get; }
        public int Cost { get; }
        public bool IsBase { get; }
        public bool IsLearned { get; set; }
        public bool CanBeLearned { get; private set; }
        public bool CanBeForgotten { get; private set; }

        public class Ctx
        {
            public SkillConfig Config;
            public ReactiveCommand<SkillType> SkillSelectionGlobal;
            public ReactiveCommand<SkillStatus> UpdateViewStatus;
            public ReactiveCommand<(SkillType, int)> OnSkillLearned;
            public ReactiveCommand<(SkillType, int)> OnSkillForgotten;
        }

        public SkillViewModel(Ctx ctx)
        {
            _ctx = ctx;

            Type = _ctx.Config.Type;
            IsBase = _ctx.Config.IsBase;
            Cost = _ctx.Config.Cost;
            Neighbors = _ctx.Config.Neighbors;
            IsLearned = _ctx.Config.IsLearned;

        }

        public void UpdateStatus(SkillStatus status)
        {
            if (Type != status.Type)
            {
                return;
            }

            IsLearned = status.IsLearned;
            CanBeLearned = status.CanBeLearned;
            CanBeForgotten = status.CanBeForgotten;

            _ctx.UpdateViewStatus?.Execute(status);
        }
    }
}