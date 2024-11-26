// Decompiled with JetBrains decompiler
// Type: StatusEffectDestroyAfterUse
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C9F0F28-E1DA-4288-A489-EB0A2F4123E8
// Assembly location: C:\Users\bess\source\repos\wildfrost-mods\HadesFrost\HadesFrost\bin\Debug\Assembly-CSharp-Publicized.dll

namespace HadesFrost.StatusEffects
{
    public class StatusEffectUses : StatusEffectData
    {
        public bool subbed;
        public bool destroy;

        public void OnDestroy() => Unsub();

        public void Sub()
        {
            Events.OnActionPerform += CheckAction;
            subbed = true;
        }

        public void Unsub()
        {
            if (!subbed)
            {
                return;
            }

            Events.OnActionPerform -= CheckAction;
            subbed = false;
        }

        public override bool RunCardPlayedEvent(Entity entity, Entity[] targets)
        {
            if (entity == target && !target.silenced)
            {
                Sub();
                if (count <= 1)
                {
                    destroy = true;
                }
                else
                {
                    count -= 1;

                    this.target.display.promptUpdateDescription = true;
                    this.target.PromptUpdate();
                }
            }

            return false;
        }

        public void CheckAction(PlayAction action)
        {
            if (!(action is ActionReduceUses actionReduceUses) || !(actionReduceUses.entity == target))
            {
                return;
            }

            Unsub();
            if (!destroy)
            {
                return;
            }

            target.alive = false;
            ActionQueue.Stack(new ActionConsume(target));
        }
    }
}