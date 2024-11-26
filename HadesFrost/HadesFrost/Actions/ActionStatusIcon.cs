namespace HadesFrost.ButtonStatuses
{
    public class ActionStatusIcon : StatusIcon
    {
        public ButtonAnimator Animator;
        public ActionButton HexButton;

        private IStatusAction statusEffect;

        public override void Assign(Entity entity)
        {
            base.Assign(entity);
            SetText();
            onValueDown.AddListener(delegate { Ping(); });
            onValueUp.AddListener(delegate { Ping(); });
            afterUpdate.AddListener(SetText);
            onValueDown.AddListener(CheckDestroy);

            var effect = entity.FindStatus(type);

            if (effect is IStatusAction actionEffect)
            {
                this.statusEffect = actionEffect;
                actionEffect.ButtonCreate(this);
                HexButton.onClick.AddListener(statusEffect.RunButtonClicked);
                onDestroy.AddListener(DisableDragBlocker);
            }
        }

        private void DisableDragBlocker()
        {
            HexButton.DisableDragBlocking();
        }
    }
}