using System.Collections.Generic;
using Extensions;
using UniRx;

namespace Skills
{
    public class SkillsService : BaseDisposable
    {
        public class Ctx
        {
            public List<SkillPm> Skills;

            public ReactiveCommand<SkillType> OnSkillSelected;
            public ReactiveCommand<SkillType> OnLearnSkillClicked;
            public ReactiveCommand<(SkillType, int)> OnSkillLearned;
            public ReactiveCommand<(SkillType, int)> OnSkillForgotten;
        }

        private Ctx _ctx;

        public SkillsService(Ctx ctx)
        {
            _ctx = ctx;
        }
        
        private void UpdateStatus(SkillType skillType)
        {
            var status = new SkillStatus
            {
                CanBeLearned = true,
                CanBeForgotten = false
            };
        }
    }
}