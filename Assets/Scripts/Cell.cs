using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI buttonText;

    private string _playerSide;
    private GameController _gameController;

    private void Start()
    {
        if (button != null && _gameController != null)
        {
            button.onClick.AddListener(() => SetCell());
        }
    }

    public void SetCell(string side = null)
    {
        if (_gameController == null) return;

        _playerSide = side ?? _gameController.GetPlayerSide();

        if (string.IsNullOrEmpty(_playerSide))
        {
            if (buttonText.text == "")
            {
                _gameController.SelectPlayer("X", GetButtonIndex());
            }

            return;
        }

        if (!string.IsNullOrEmpty(buttonText.text)) return;

        buttonText.text = _playerSide;
        button.enabled = false;
        _gameController.EndTurn();
    }

    private int GetButtonIndex()
    {
        TextMeshProUGUI[] allCells = _gameController.gridCells;
        for (int i = 0; i < allCells.Length; i++)
        {
            if (allCells[i] == buttonText)
            {
                return i;
            }
        }

        return -1;
    }

    public void SetControllerReference(GameController gameController)
    {
        _gameController = gameController;
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => SetCell());
        }
    }
}