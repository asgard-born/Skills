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

        private readonly List<SkillPm> _models = new();

        private ReactiveCommand<SkillType> _skillSelectionBus;
        private ReactiveCommand<SkillType> _unselectSkill;
        private ReactiveCommand<SkillStatus> _updateSkillStatus;
        private ReactiveCommand<SkillType> _onLearnSkillClicked;

        public class Ctx
        {
            public SkillView[] SkillViews;
            public SkillSetConfig SkillsSetConfig;

            public ReadOnlyReactiveProperty<int> Scores;
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
            AddUnsafe(_skillSelectionBus = new ReactiveCommand<SkillType>());
            AddUnsafe(_unselectSkill = new ReactiveCommand<SkillType>());
            AddUnsafe(_updateSkillStatus = new ReactiveCommand<SkillStatus>());
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

                ReactiveCommand onViewSkillSelected;
                ReactiveCommand<SkillType> onLearnSkillClicked;
                ReactiveCommand<SkillStatus> updateSkillStatus;
                ReactiveCommand<(SkillType, int)> onSkillLearned;
                ReactiveCommand<(SkillType, int)> onSkillForgotten;

                AddUnsafe(onViewSkillSelected = new ReactiveCommand());
                AddUnsafe(onLearnSkillClicked = new ReactiveCommand<SkillType>());
                AddUnsafe(updateSkillStatus = new ReactiveCommand<SkillStatus>());
                AddUnsafe(onSkillLearned = new ReactiveCommand<(SkillType, int)>());
                AddUnsafe(onSkillForgotten = new ReactiveCommand<(SkillType, int)>());

                view.Initialize(new SkillView.Ctx
                {
                    Config = config,
                    OnViewSkillSelected = onViewSkillSelected,
                    UpdateSkillStatus = updateSkillStatus,
                    UnselectSkill = _unselectSkill,
                    OnLearnSkillClicked = onLearnSkillClicked,
                    OnSkillLearned = onSkillLearned,
                    OnSkillForgotten = onSkillForgotten,
                });

                SkillPm model = new SkillPm(new SkillPm.Ctx
                {
                    Config = config,

                    Scores = _ctx.Scores,
                    OnViewSkillSelected = onViewSkillSelected,
                    UpdateSkillStatus = updateSkillStatus,
                    SkillSelectionBus = _skillSelectionBus,
                    // OnSkillLearned = onSkillLearned,
                    // OnSkillForgotten = onSkillForgotten,
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
                SkillSelectionBus = _skillSelectionBus,
                UnselectSkill = _unselectSkill,
                OnLearnSkillClicked = _onLearnSkillClicked,
                UpdateSkillStatus = _updateSkillStatus
            }));
        }
    }
}