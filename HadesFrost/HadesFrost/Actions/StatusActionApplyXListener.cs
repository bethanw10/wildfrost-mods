using System.Collections;

namespace HadesFrost.ButtonStatuses
{
    public class StatusActionApplyXListener : StatusEffectApplyX
    {
        public IEnumerator Run()
        {
            yield return Run(GetTargets());
        }
    }
}