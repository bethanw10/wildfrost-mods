namespace HadesFrost.ButtonStatuses
{
    public class HexStatusIcon : StatusIcon
    {
        public ButtonAnimator Animator;
        public HexButton HexButton;

        private IStatusHex hexStatusEffect;

        public override void Assign(Entity entity)
        {
            Utils.Utils.Log("assign");

            base.Assign(entity);
            SetText();
            onValueDown.AddListener(delegate { Ping(); });
            onValueUp.AddListener(delegate { Ping(); });
            afterUpdate.AddListener(SetText);
            onValueDown.AddListener(CheckDestroy);

            var effect = entity.FindStatus(type);
            Utils.Utils.Log("is token?");

            if (effect is IStatusHex hexEffect)
            {
                Utils.Utils.Log("adding listener");
                this.hexStatusEffect = hexEffect;
                hexEffect.ButtonCreate(this);
                HexButton.onClick.AddListener(this.hexStatusEffect.RunButtonClicked);
                onDestroy.AddListener(DisableDragBlocker);
            }
        }

        private void DisableDragBlocker()
        {
            HexButton.DisableDragBlocking();
        }
    }
}