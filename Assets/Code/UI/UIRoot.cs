using Extensions;
using UniRx;

namespace UI
{
    public class UIRoot : BaseDisposable
    {
        public struct Ctx
        {
            public HUDWindow HUDWindow;

            public ReadOnlyReactiveProperty<int> Scores;
            public ReactiveCommand<int> OnEarnScoresClick;
            public ReactiveCommand OnForgetAllClick;
        }

        public UIRoot(Ctx ctx)
        {
            ctx.HUDWindow.Initialize(new HUDWindow.Ctx
            {
                Scores = ctx.Scores,
                OnEarnScoresClick = ctx.OnEarnScoresClick,
                OnForgetAllClick = ctx.OnForgetAllClick
            });
        }
    }
}