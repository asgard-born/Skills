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

        private readonly List<SkillViewModel> _models = new();

        private ReactiveCommand<SkillType> _onSkillSelected;
        private ReactiveCommand<SkillType> _onLearnSkillClicked;
        private ReactiveCommand<SkillType> _unselectSkill;

        public class Ctx
        {
            public SkillView[] SkillViews;
            public SkillsConfigs SkillsConfigs;

            public ReadOnlyReactiveProperty<int> Scores;
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
            AddUnsafe(_unselectSkill = new ReactiveCommand<SkillType>());
        }

        private void InitializeSkills()
        {
            foreach (SkillConfig config in _ctx.SkillsConfigs.Configs)
            {
                SkillView view = _ctx.SkillViews.FirstOrDefault(s => s.SkillType == config.Type);

                if (view == null)
                {
                    Debug.LogError($"There is no view with type {config.Type.ToString()}");
                    continue;
                }

                ReactiveCommand<SkillStatus> updateViewStatus;
                AddUnsafe(updateViewStatus = new ReactiveCommand<SkillStatus>());

                view.Initialize(new SkillView.Ctx
                {
                    Config = config,
                    OnViewSkillSelected = _onSkillSelected,
                    UpdateStatus = updateViewStatus,
                    UnselectSkill = _unselectSkill,
                    OnLearnSkillClicked = _onLearnSkillClicked,
                });

                SkillViewModel viewModel = new SkillViewModel(new SkillViewModel.Ctx
                {
                    Config = config,
                    SkillSelectionGlobal = _onSkillSelected,
                    OnSkillLearned = _ctx.OnSkillLearned,
                    OnSkillForgotten = _ctx.OnSkillForgotten,
                });

                AddUnsafe(viewModel);

                _models.Add(viewModel);
            }
        }

        private void InitializeService()
        {
            AddUnsafe(new SkillsService(new SkillsService.Ctx
            {
                Models = _models,
                OnSkillSelected = _onSkillSelected,
                UnselectSkill = _unselectSkill,
                OnLearnSkillClicked = _onLearnSkillClicked,
            }));
        }
    }
}