using System.Collections;

namespace HadesFrost.ButtonStatuses
{
    public class StatusHexApplyXListener : StatusEffectApplyX
    {
        public IEnumerator Run()
        {
            yield return Run(GetTargets());
        }
    }
}