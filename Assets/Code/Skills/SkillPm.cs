using Configs;
using Extensions;

namespace Skills
{
    public class SkillPm : BaseDisposable
    {
        public class Ctx
        {
            public SkillConfig Config;
        }

        private Ctx _ctx;

        public SkillPm(Ctx ctx)
        {
            _ctx = ctx;
        }
    }
}