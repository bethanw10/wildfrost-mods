using System.Collections;

namespace HadesFrost.StatusEffects
{
    internal class StatusInstantIncreaseCounter : StatusEffectInstant
    {
        public override IEnumerator Process()
        {
            target.counter.current += count;
            yield return base.Process();
        }
    }
}