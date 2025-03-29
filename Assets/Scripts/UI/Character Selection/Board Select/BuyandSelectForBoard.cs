using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyandSelectForBoard : MonoBehaviour
{
    public BoardSelectUI defaultBoard;
    BoardSelectUI selectedBoard;

    private void Start()
    {
        selectedBoard = defaultBoard;
    }

    private void OnEnable()
    {
        Actions.onSelectBoards += SelectedBoard;
    }

    private void OnDisable()
    {
        Actions.onSelectBoards -= SelectedBoard;
    }
    private void SelectedBoard(BoardSelectUI v)
    {
        selectedBoard = v;
    }

    public void Buy()
    {
        if (PlayerCollectibleManager.instance.GetGoldAmount()>= selectedBoard.price)
        {
            PlayerCollectibleManager.instance.AddCoin(-selectedBoard.price);
            selectedBoard.isOwned = true;

            selectedBoard.CheckButtons();
        }
        else
            print("Not enough currency");
    }

    public void Select()
    {

        if (Actions.onBoardSelectPressed != null)
        {
            print("Main Select Run 'n boards " + selectedBoard.name);
            Actions.onBoardSelectPressed.Invoke(selectedBoard);
            selectedBoard.CheckButtons();

        }
        else
        {
            Debug.LogWarning("onSelectCharacters event is not assigned or has no listeners.");
        }
    }

}
