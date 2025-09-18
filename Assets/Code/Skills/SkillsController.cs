using System.Collections.Generic;
using System.Linq;
using Extensions;
using UniRx;

namespace Skills
{
    public class SkillsController : BaseDisposable
    {
        private SkillType _lastSelectedSkill = SkillType.None;
        private readonly Ctx _ctx;

        private readonly HashSet<SkillType> _articulationPointsCache = new();

        public class Ctx
        {
            public List<SkillModel> Models;

            public ReadOnlyReactiveProperty<int> Scores;
            public ReactiveCommand<SkillType> OnSkillSelected;
            public ReactiveCommand<SkillType> OnLearnSkillClicked;
            public ReactiveCommand<SkillType> OnForgetSkillClicked;
            public ReactiveCommand<(SkillType, int)> OnSkillLearned;
            public ReactiveCommand<(SkillType, int)> OnSkillForgotten;
            public ReactiveCommand OnForgetAllSkillsClicked;
        }

        public SkillsController(Ctx ctx)
        {
            _ctx = ctx;

            AddUnsafe(_ctx.Scores.Subscribe(_ => OnScoreChanged()));
            AddUnsafe(_ctx.OnSkillSelected.Subscribe(OnSkillSelected));
            AddUnsafe(_ctx.OnLearnSkillClicked.Subscribe(OnLearnSkillClicked));
            AddUnsafe(_ctx.OnForgetSkillClicked.Subscribe(OnForgetSkillClicked));
            AddUnsafe(_ctx.OnForgetAllSkillsClicked.Subscribe(_ => ForgetAllSkills()));

            RecalculateArticulationPoints();
            InitAllModels();
        }

        private void InitAllModels()
        {
            foreach (SkillModel model in _ctx.Models)
            {
                model.UpdateStatus(new SkillStatus
                {
                    Type = model.Type,
                    IsLearned = model.IsLearned,
                    IsSelected = model.IsSelected,
                    CanBeLearned = CanBeLearned(model),
                    CanBeForgotten = CanBeForgotten(model)
                });
            }
        }

        private void OnScoreChanged()
        {
            SkillModel selectedModel = GetModel(_lastSelectedSkill);

            if (selectedModel != null)
            {
                selectedModel.UpdateStatus(new SkillStatus
                {
                    Type = selectedModel.Type,
                    IsLearned = selectedModel.IsLearned,
                    IsSelected = true,
                    CanBeLearned = CanBeLearned(selectedModel),
                    CanBeForgotten = selectedModel.CanBeForgotten
                });
            }
        }

        private void OnSkillSelected(SkillType skillType)
        {
            if (skillType == _lastSelectedSkill) return;

            if (_lastSelectedSkill != SkillType.None)
            {
                SkillModel lastSelected = GetModel(_lastSelectedSkill);

                lastSelected.UpdateStatus(new SkillStatus
                {
                    Type = lastSelected.Type,
                    IsLearned = lastSelected.IsLearned,
                    IsSelected = false,
                    CanBeLearned = lastSelected.CanBeLearned,
                    CanBeForgotten = lastSelected.CanBeForgotten
                });
            }

            _lastSelectedSkill = skillType;

            SkillModel model = GetModel(skillType);

            model.UpdateStatus(new SkillStatus
            {
                Type = model.Type,
                IsLearned = model.IsLearned,
                IsSelected = true,
                CanBeLearned = CanBeLearned(model),
                CanBeForgotten = CanBeForgotten(model)
            });
        }

        private void OnLearnSkillClicked(SkillType type)
        {
            SkillModel model = GetModel(type);
            if (model == null || !CanBeLearned(model)) return;

            _ctx.OnSkillLearned?.Execute((type, model.Cost));

            model.UpdateStatus(new SkillStatus
            {
                Type = model.Type,
                IsLearned = true,
                IsSelected = model.IsSelected,
                CanBeLearned = false,
                CanBeForgotten = !model.IsBase
            });

            RecalculateArticulationPoints();
        }

        private void OnForgetSkillClicked(SkillType type)
        {
            SkillModel model = GetModel(type);
            if (model == null || !CanBeForgotten(model)) return;

            _ctx.OnSkillForgotten?.Execute((type, model.Cost));

            model.IsLearned = false;

            model.UpdateStatus(new SkillStatus
            {
                Type = model.Type,
                IsLearned = model.IsLearned,
                IsSelected = model.IsSelected,
                CanBeLearned = CanBeLearned(model),
                CanBeForgotten = false
            });

            RecalculateArticulationPoints();
        }

        private SkillModel GetModel(SkillType type)
        {
            return _ctx.Models.FirstOrDefault(m => m.Type == type);
        }

        private bool CanBeLearned(SkillModel model)
        {
            if (model.IsLearned) return false;
            if (_ctx.Scores.Value < model.Cost) return false;

            return model.Neighbors.Any(n => GetModel(n)?.IsLearned == true);
        }

        private bool CanBeForgotten(SkillModel model)
        {
            if (!model.IsLearned) return false;
            if (model.IsBase) return false;

            return !_articulationPointsCache.Contains(model.Type);
        }

        private void RecalculateArticulationPoints()
        {
            _articulationPointsCache.Clear();

            var visited = new HashSet<SkillType>();
            var disc = new Dictionary<SkillType, int>();
            var low = new Dictionary<SkillType, int>();
            var parent = new Dictionary<SkillType, SkillType>();
            int time = 0;

            void DepthFirstSearch(SkillModel model)
            {
                visited.Add(model.Type);
                disc[model.Type] = low[model.Type] = ++time;
                int children = 0;

                foreach (SkillType nType in model.Neighbors)
                {
                    SkillModel neighbor = GetModel(nType);
                    if (neighbor == null || !neighbor.IsLearned) continue;

                    if (!visited.Contains(neighbor.Type))
                    {
                        children++;
                        parent[neighbor.Type] = model.Type;
                        DepthFirstSearch(neighbor);

                        low[model.Type] = System.Math.Min(low[model.Type], low[neighbor.Type]);

                        if (!parent.ContainsKey(model.Type) && children > 1)
                        {
                            _articulationPointsCache.Add(model.Type);
                        }

                        if (parent.ContainsKey(model.Type) && low[neighbor.Type] >= disc[model.Type])
                        {
                            _articulationPointsCache.Add(model.Type);
                        }
                    }
                    else if (parent.ContainsKey(model.Type) && neighbor.Type != parent[model.Type])
                    {
                        low[model.Type] = System.Math.Min(low[model.Type], disc[neighbor.Type]);
                    }
                }
            }

            SkillModel baseSkill = _ctx.Models.FirstOrDefault(s => s.IsBase);

            if (baseSkill != null && baseSkill.IsLearned)
            {
                DepthFirstSearch(baseSkill);
            }
        }

        private void ForgetAllSkills()
        {
            var order = new List<SkillType>();

            while (true)
            {
                var removable = _ctx.Models
                    .Where(m => m.IsLearned && !m.IsBase && CanBeForgotten(m))
                    .ToList();

                if (removable.Count == 0)
                {
                    break;
                }

                foreach (var model in removable)
                {
                    var status = new SkillStatus
                    {
                        Type = model.Type,
                        IsLearned = false,
                        IsSelected = model.IsSelected,
                        CanBeLearned = CanBeLearned(model),
                        CanBeForgotten = false
                    };

                    model.UpdateStatus(status);

                    _ctx.OnSkillForgotten?.Execute((model.Type, model.Cost));

                    order.Add(model.Type);
                }

                RecalculateArticulationPoints();
            }
        }
    }
}