using System;
using System.Collections;

internal class StatusInstantIncreaseCounter : StatusEffectInstant
{
    public override IEnumerator Process()
    {
        target.counter.current += count;
        yield return base.Process();
    }
}