using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HadesFrost.Actions
{
    public class ActionButton : Button
    {
        private ActionStatusIcon Icon => GetComponent<ActionStatusIcon>();

        private static ActionButton dragBlocker;

        private Entity Entity => Icon?.target;

        public override void OnPointerEnter(PointerEventData eventData)
        {
            dragBlocker = this;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            DisableDragBlocking();
        }

        public void DisableDragBlocking()
        {
            if (dragBlocker == this)
            {
                dragBlocker = null;
            }
        }

        public static void DisableDrag(ref Entity entity, ref bool shouldDrag)
        {
            if (dragBlocker == null || entity != dragBlocker.Entity)
            {
                return;
            }
            shouldDrag = false;
        }
    }
}