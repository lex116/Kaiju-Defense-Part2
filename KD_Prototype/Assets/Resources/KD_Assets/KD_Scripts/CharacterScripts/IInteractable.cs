using UnityEngine;

public interface IInteractable
{
    void Activate(Unit_Human Activator);

    string ActivateText { get; }
}
