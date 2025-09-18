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

        private readonly List<SkillModel> _models = new();

        private ReactiveCommand<SkillType> _onViewSelectClick;
        private ReactiveCommand<SkillType> _onLearnSkillClicked;
        private ReactiveCommand<SkillType> _onForgetSkillClicked;

        public class Ctx
        {
            public SkillView[] SkillViews;
            public SkillsConfigs SkillsConfigs;

            public ReadOnlyReactiveProperty<int> Scores;
            public ReactiveCommand<(SkillType, int)> OnSkillLearned;
            public ReactiveCommand<(SkillType, int)> OnSkillForgotten;
            public ReactiveCommand OnForgetAllClick;
        }

        public SkillsRoot(Ctx ctx)
        {
            _ctx = ctx;

            InitializeRx();
            InitializeSkills();
            InitializeController();
        }

        private void InitializeRx()
        {
            AddUnsafe(_onViewSelectClick = new ReactiveCommand<SkillType>());
            AddUnsafe(_onLearnSkillClicked = new ReactiveCommand<SkillType>());
            AddUnsafe(_onForgetSkillClicked = new ReactiveCommand<SkillType>());
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
                    OnViewSelectClick = _onViewSelectClick,
                    UpdateStatus = updateViewStatus,
                    OnLearnSkillClicked = _onLearnSkillClicked,
                    OnForgetSkillClicked = _onForgetSkillClicked
                });

                SkillModel model = new SkillModel(new SkillModel.Ctx
                {
                    Config = config,
                    UpdateViewStatus = updateViewStatus,
                });

                AddUnsafe(model);
                _models.Add(model);
            }
        }

        private void InitializeController()
        {
            AddUnsafe(new SkillsController(new SkillsController.Ctx
            {
                Models = _models,

                Scores = _ctx.Scores,
                OnViewSelectClick = _onViewSelectClick,
                OnLearnSkillClicked = _onLearnSkillClicked,
                OnForgetSkillClicked = _onForgetSkillClicked,
                OnSkillLearned = _ctx.OnSkillLearned,
                OnSkillForgotten = _ctx.OnSkillForgotten,
                OnForgetAllSkillsClicked = _ctx.OnForgetAllClick
            }));
        }
    }
}