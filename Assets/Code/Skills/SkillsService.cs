using System.Collections.Generic;
using System.Linq;
using Extensions;
using UniRx;

namespace Skills
{
    public class SkillsService : BaseDisposable
    {
        private SkillType _lastSelectedSkill;
        private readonly Ctx _ctx;

        private readonly HashSet<SkillType> _articulationPointsCache = new();

        public class Ctx
        {
            public List<SkillViewModel> Models;

            public ReadOnlyReactiveProperty<int> Scores;
            public ReactiveCommand<SkillType> OnSkillSelected;
            public ReactiveCommand<SkillType> UnselectSkill;
            public ReactiveCommand<SkillType> OnLearnSkillClicked;
            public ReactiveCommand<SkillType> OnForgetSkillClicked;
            public ReactiveCommand<(SkillType, int)> OnSkillLearned;
            public ReactiveCommand<(SkillType, int)> OnSkillForgotten;
        }

        public SkillsService(Ctx ctx)
        {
            _ctx = ctx;

            AddUnsafe(_ctx.Scores.Subscribe(_ => OnScoreChanged()));
            AddUnsafe(_ctx.OnSkillSelected.Subscribe(OnSkillSelected));
            AddUnsafe(_ctx.OnLearnSkillClicked.Subscribe(OnLearnSkillClicked));
            AddUnsafe(_ctx.OnForgetSkillClicked.Subscribe(OnForgetSkill));

            RecalculateArticulationPoints();
        }

        private void OnScoreChanged()
        {
            SkillViewModel selectedModel = GetModel(_lastSelectedSkill);

            if (selectedModel != null)
            {
                selectedModel.UpdateStatus(new SkillStatus
                {
                    Type = selectedModel.Type,
                    IsLearned = selectedModel.IsLearned,
                    CanBeLearned = CanBeLearned(selectedModel),
                    CanBeForgotten = selectedModel.CanBeForgotten
                });
            }
        }

        private void OnSkillSelected(SkillType skillType)
        {
            _ctx.UnselectSkill?.Execute(_lastSelectedSkill);
            _lastSelectedSkill = skillType;

            SkillViewModel model = GetModel(skillType);

            RecalculateArticulationPoints();

            model.UpdateStatus(new SkillStatus
            {
                Type = model.Type,
                IsLearned = model.IsLearned,
                CanBeLearned = CanBeLearned(model),
                CanBeForgotten = CanBeForgotten(model)
            });
        }

        private void OnLearnSkillClicked(SkillType type)
        {
            SkillViewModel model = GetModel(type);
            if (model == null || !CanBeLearned(model)) return;

            _ctx.OnSkillLearned?.Execute((type, model.Cost));

            RecalculateArticulationPoints();

            model.UpdateStatus(new SkillStatus
            {
                Type = model.Type,
                IsLearned = true,
                CanBeLearned = CanBeLearned(model),
                CanBeForgotten = CanBeForgotten(model)
            });
        }

        private void OnForgetSkill(SkillType type)
        {
            SkillViewModel model = GetModel(type);
            if (model == null || !CanBeForgotten(model)) return;

            _ctx.OnSkillForgotten?.Execute((type, model.Cost));

            RecalculateArticulationPoints();

            model.UpdateStatus(new SkillStatus
            {
                Type = model.Type,
                IsLearned = false,
                CanBeLearned = CanBeLearned(model),
                CanBeForgotten = CanBeForgotten(model)
            });
        }

        private SkillViewModel GetModel(SkillType type)
        {
            return _ctx.Models.FirstOrDefault(m => m.Type == type);
        }

        private bool CanBeLearned(SkillViewModel viewModel)
        {
            if (viewModel.IsLearned) return false;
            if (_ctx.Scores.Value < viewModel.Cost) return false;

            return viewModel.Neighbors.Any(n => GetModel(n)?.IsLearned == true);
        }

        private bool CanBeForgotten(SkillViewModel viewModel)
        {
            if (!viewModel.IsLearned) return false;
            if (viewModel.IsBase) return false;

            return !_articulationPointsCache.Contains(viewModel.Type);
        }

        private void RecalculateArticulationPoints()
        {
            _articulationPointsCache.Clear();

            var visited = new HashSet<SkillType>();
            var disc = new Dictionary<SkillType, int>();
            var low = new Dictionary<SkillType, int>();
            var parent = new Dictionary<SkillType, SkillType>();
            int time = 0;

            void DepthFirstSearch(SkillViewModel model)
            {
                visited.Add(model.Type);
                disc[model.Type] = low[model.Type] = ++time;
                int children = 0;

                foreach (SkillType nType in model.Neighbors)
                {
                    SkillViewModel neighbor = GetModel(nType);
                    if (neighbor == null || !neighbor.IsLearned) continue;

                    if (!visited.Contains(neighbor.Type))
                    {
                        children++;
                        parent[neighbor.Type] = model.Type;
                        DepthFirstSearch(neighbor);

                        low[model.Type] = System.Math.Min(low[model.Type], low[neighbor.Type]);

                        if (!parent.ContainsKey(model.Type) && children > 1)
                            _articulationPointsCache.Add(model.Type);

                        if (parent.ContainsKey(model.Type) && low[neighbor.Type] >= disc[model.Type])
                            _articulationPointsCache.Add(model.Type);
                    }
                    else if (parent.ContainsKey(model.Type) && neighbor.Type != parent[model.Type])
                    {
                        low[model.Type] = System.Math.Min(low[model.Type], disc[neighbor.Type]);
                    }
                }
            }

            SkillViewModel baseSkill = _ctx.Models.FirstOrDefault(s => s.IsBase);

            if (baseSkill != null && baseSkill.IsLearned)
            {
                DepthFirstSearch(baseSkill);
            }
        }

        public List<SkillType> ForgetAllSkills()
        {
            var order = new List<SkillType>();

            while (true)
            {
                // Берём навыки, которые можно забыть
                var removable = _ctx.Models
                    .Where(m => m.IsLearned && !m.IsBase && CanBeForgotten(m))
                    .ToList();

                if (removable.Count == 0)
                    break;

                foreach (var model in removable)
                {
                    var status = new SkillStatus
                    {
                        Type = model.Type,
                        IsLearned = false,
                        CanBeLearned = CanBeLearned(model),
                        CanBeForgotten = false
                    };

                    model.UpdateStatus(status);

                    _ctx.OnSkillForgotten?.Execute((model.Type, model.Cost));

                    order.Add(model.Type);
                }

                RecalculateArticulationPoints();
            }

            foreach (var model in _ctx.Models)
            {
                var status = new SkillStatus
                {
                    Type = model.Type,
                    IsLearned = model.IsLearned,
                    CanBeLearned = CanBeLearned(model),
                    CanBeForgotten = CanBeForgotten(model)
                };

                model.UpdateStatus(status);
            }

            return order;
        }
    }
}