using System.Collections.Generic;
using Extensions;
using UniRx;

namespace Skills
{
    public class SkillsService : BaseDisposable
    {
        private SkillType _lastSelectedSkill;

        public class Ctx
        {
            public List<SkillPm> Skills;

            public ReactiveCommand<SkillType> SkillSelectionBus;
            public ReactiveCommand<SkillType> UnselectSkill;
            public ReactiveCommand<SkillType> OnLearnSkillClicked;
            public ReactiveCommand<SkillStatus> UpdateSkillStatus;
            public ReactiveCommand<(SkillType, int)> OnSkillLearned;
            public ReactiveCommand<(SkillType, int)> OnSkillForgotten;
        }

        private Ctx _ctx;

        public SkillsService(Ctx ctx)
        {
            _ctx = ctx;
            AddUnsafe(_ctx.SkillSelectionBus.Subscribe(OnSkillSelected));
        }

        private void OnSkillSelected(SkillType skillType)
        {
            _ctx.UnselectSkill?.Execute(_lastSelectedSkill);
            _lastSelectedSkill = skillType;
        }
    }
}