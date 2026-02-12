
public interface IInteractor 
{
    // If true, the interactor will automatically call Interact() on an interactable when entering its trigger
    bool AutoInteract { get;}

    void OnInteractableEnter(IInteracable interacable);
    void OnInteractableExit(IInteracable interacable);
}
