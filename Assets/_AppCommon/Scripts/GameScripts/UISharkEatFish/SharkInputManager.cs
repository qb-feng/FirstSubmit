using UnityEngine;
using UnityEngine.EventSystems;

public class SharkInputManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public SharkPlayerManager sharkPlayerManager;         //A link to the SharkPlayerManager


    public  void OnPointerDown(PointerEventData eventData)
    {
        sharkPlayerManager.UpdateInput(true);

    }

    public  void OnPointerUp(PointerEventData eventData)
    {
        sharkPlayerManager.UpdateInput(false);
    }
}
