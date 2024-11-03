using System.Collections;

namespace HadesFrost.ButtonStatuses
{
    public interface IStatusHex
    {
        void ButtonCreate(HexStatusIcon icon);

        void RunButtonClicked();

        IEnumerator ButtonClicked();
    }
}