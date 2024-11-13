using System.Collections;

namespace Piri
{
    public class StatusEffectApplyXWhenAllyOrSelfDebuffed : StatusEffectApplyX
    {
        private string[] debuffs = { "snow", "frost", "vim", "ink", "demonize", "haze", "overload", "shroom" };

        public override void Init()
        {
            OnApplyStatus += CheckStatus;
        }

        public override bool RunApplyStatusEvent(StatusEffectApply apply)
        {
            return target.enabled &&
                apply.target.owner == target.owner &&
                debuffs.Contains(apply.effectData.type) &&
                Battle.IsOnBoard(target) &&
                Battle.IsOnBoard(apply.target);
        }

        public IEnumerator CheckStatus(StatusEffectApply apply) => Run(GetTargets(targets: new Entity[1]
        {
            apply.target
        }), apply.count);
    }
}