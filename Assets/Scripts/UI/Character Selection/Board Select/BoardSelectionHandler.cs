using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSelectionHandler : MonoBehaviour
{
    BoardSelectUI[] boards;
    public GameObject displayBoard;
    public GameObject playerBoardRef;


    GameObject[] _playerBoardModels;
    GameObject[] _boardDisplayModels;
    private void Awake()
    {
        playerBoardRef = GameObject.FindWithTag("SkateModel");
        print(playerBoardRef.name);
        _playerBoardModels = new GameObject[playerBoardRef.transform.childCount];
        _boardDisplayModels = new GameObject[displayBoard.transform.childCount];

        boards = new BoardSelectUI[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            boards[i] = transform.GetChild(i).GetComponent<BoardSelectUI>();
        }

        for (int i = 0; i < playerBoardRef.transform.childCount; i++)
        {
            _playerBoardModels[i] = playerBoardRef.transform.GetChild(i).gameObject;
        }
        for (int i = 0; i < displayBoard.transform.childCount; i++)
        {
            _boardDisplayModels[i] = displayBoard.transform.GetChild(i).gameObject;
        }
    }

    private void OnEnable()
    {
        CheckSelected();
        playerBoardRef.SetActive(false);
        Actions.onSelectBoards += SelectBoardUI;
        Actions.onBoardSelectPressed += SelectPlayerBoard;
        Actions.onBoardHovered += SelectDisplayBoard;
    }
    void CheckSelected()
    {
        BoardSelectUI activBoard = FindSelected();

        SelectBoardUI(activBoard);
        SelectDisplayBoard(activBoard.modelRef.transform);
    }
    BoardSelectUI FindSelected()
    {
        foreach (var item in boards)
        {
            if (item.isSelected == true)
            {
                return item;
            }
        }

        return null;
    }
    public void SelectBoardUI(BoardSelectUI v)
    {
        for (int i = 0; i < boards.Length; i++)
        {
            boards[i].selectedMark.SetActive(false);
        }

        v.selectedMark.SetActive(true);
        v.CheckButtons();
    }

    void SelectPlayerBoard(BoardSelectUI v)
    {

        Transform selectedModel = v.modelRef.transform;
        foreach (var item in _playerBoardModels)
        {
            item.SetActive(false);

            if (selectedModel.name == item.name)
            {
                item.SetActive(true);
                SetChoosenBoard(v);
            }
        }


    }

    void SelectDisplayBoard(Transform selectedModel)
    {

        foreach (var item in _boardDisplayModels)
        {
            item.SetActive(false);

            if (selectedModel.name == item.name)
            {
                item.SetActive(true);

            }
        }

    }
    void SetChoosenBoard(BoardSelectUI v)
    {
        for (int i = 0; i < boards.Length; i++)
        {
            boards[i].isSelected = false;
        }

        print("SetChoosenBoard true "+ v.name);
        v.isSelected = true;
    }
}
