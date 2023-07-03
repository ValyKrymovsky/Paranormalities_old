using UnityEngine;

public interface IInteractionPopup
{
    public void SpawnPopup(GameObject _popupObject);
    public void TurnOffPopup();
    public void TurnOnPopup();
    public void DestroyPopup();
}

