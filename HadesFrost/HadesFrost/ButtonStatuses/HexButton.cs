using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HadesFrost.ButtonStatuses
{
    public class HexButton : Button
    {
        private HexStatusIcon Icon => GetComponent<HexStatusIcon>();

        private static HexButton DragBlocker;

        private Entity Entity => Icon?.target;

        public override void OnPointerEnter(PointerEventData eventData)
        {
            DragBlocker = this;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            DisableDragBlocking();
        }

        public void DisableDragBlocking()
        {
            if (DragBlocker == this)
            {
                DragBlocker = null;
            }
        }

        public static void DisableDrag(ref Entity entity, ref bool shouldDrag)
        {
            if (DragBlocker == null || entity != DragBlocker.Entity)
            {
                return;
            }
            shouldDrag = false;
        }
    }
}