using System.Collections;

namespace HadesFrost.ButtonStatuses
{
    public interface IStatusAction
    {
        void ButtonCreate(ActionStatusIcon icon);

        void RunButtonClicked();

        IEnumerator ButtonClicked();
    }
}