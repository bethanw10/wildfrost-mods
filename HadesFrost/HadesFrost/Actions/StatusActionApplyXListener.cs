using System.Collections;

namespace HadesFrost.Actions
{
    public class StatusActionApplyXListener : StatusEffectApplyX
    {
        public IEnumerator Run()
        {
            yield return Run(GetTargets());
        }
    }
}