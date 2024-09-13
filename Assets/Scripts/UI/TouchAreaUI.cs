using UnityEngine;
using UnityEngine.EventSystems;

public class TouchAreaUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    

    // This method is called when the touch begins
    public void OnPointerDown(PointerEventData eventData)
    {
        print("Should Start");
        GameStarter.Instance.shouldStartGame = true;
       
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       
    }
}
