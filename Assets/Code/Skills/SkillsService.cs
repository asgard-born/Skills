using System.Collections.Generic;
using Extensions;
using UniRx;

namespace Skills
{
    public class SkillsService : BaseDisposable
    {
        private SkillView _selectedSkill;
        
        public class Ctx
        {
            public List<SkillPm> Skills;
            public List<SkillView> Views;

            public ReactiveCommand<SkillType> OnSkillSelected;
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
            AddUnsafe(_ctx.OnSkillSelected.Subscribe(OnSkillSelected));
            AddUnsafe(_ctx.UnselectSkill.Subscribe(UnselectSkill));
        }

        private void UnselectSkill(SkillType skillType)
        {
            
        }
        private void OnSkillSelected(SkillType skillType)
        {
            var status = new SkillStatus
            {
                CanBeLearned = true,
                CanBeForgotten = false
            };

            _ctx.UpdateSkillStatus?.Execute(status);
        }
    }
}