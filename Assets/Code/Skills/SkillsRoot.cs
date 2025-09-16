using System.Collections.Generic;
using System.Linq;
using Configs;
using Extensions;
using UniRx;
using UnityEngine;

namespace Skills
{
    public class SkillsRoot : BaseDisposable
    {
        private readonly Ctx _ctx;
        
        private List<SkillPm> _models;
        
        private ReactiveCommand<SkillType> _onSkillSelected;
        private ReactiveCommand<SkillStatus> _updateSkillStatus;
        private ReactiveCommand<SkillType> _onLearnSkillClicked;

        public class Ctx
        {
            public SkillView[] SkillViews;
            public SkillSetConfig SkillsSetConfig;

            public ReactiveCommand<(SkillType, int)> OnSkillLearned;
            public ReactiveCommand<(SkillType, int)> OnSkillForgotten;
        }

        public SkillsRoot(Ctx ctx)
        {
            _ctx = ctx;

            InitializeRx();
            InitializeSkills();
            InitializeService();
        }

        private void InitializeRx()
        {
            AddUnsafe(_onSkillSelected = new ReactiveCommand<SkillType>());
            AddUnsafe(_onLearnSkillClicked = new ReactiveCommand<SkillType>());
        }

        private void InitializeSkills()
        {
            foreach (SkillConfig config in _ctx.SkillsSetConfig.SkillConfigs)
            {
                SkillView view = _ctx.SkillViews.FirstOrDefault(s => s.SkillType == config.Type);

                if (view == null)
                {
                    Debug.LogError($"There is no view with type {config.Type.ToString()}");
                    continue;
                }

                view.Initialize(new SkillView.Ctx
                {
                    Config = config,
                    OnSkillSelected = _onSkillSelected,
                    OnLearnSkillClicked = _onLearnSkillClicked,
                    OnSkillLearned = _ctx.OnSkillLearned,
                    OnSkillForgotten = _ctx.OnSkillForgotten,
                });

                SkillPm model = new SkillPm(new SkillPm.Ctx
                {
                    Config = config
                });

                AddUnsafe(model);

                _models.Add(model);
            }
        }

        private void InitializeService()
        {
            AddUnsafe(new SkillsService(new SkillsService.Ctx
            {
                Skills = _models,
                OnSkillSelected =_onSkillSelected,
                OnLearnSkillClicked = _onLearnSkillClicked,
                UpdateSkillStatus = _updateSkillStatus,
                OnSkillLearned = _ctx.OnSkillLearned,
                OnSkillForgotten = _ctx.OnSkillForgotten
            }));
        }
    }
}