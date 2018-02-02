using UnityEngine;

public interface IInteractable
{
    string DisplayText { get; }
    void Activate(MasterInventory_Class inventory);
}
