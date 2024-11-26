using System.Collections;

namespace HadesFrost.Actions
{
    public interface IStatusAction
    {
        void ButtonCreate(ActionStatusIcon icon);

        void RunButtonClicked();

        IEnumerator ButtonClicked();
    }
}