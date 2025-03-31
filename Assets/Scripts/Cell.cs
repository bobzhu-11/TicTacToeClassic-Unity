using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI buttonText;

    private string _playerSide;
    private GameController _gameController;

    public void SetCell()
    {
        if (_gameController == null)
        {
            Debug.LogError("GameController reference is missing in Cell.");
            return;
        }
    
        _playerSide = _gameController.GetPlayerSide();
        buttonText.text = _playerSide;
        button.enabled = false;
        _gameController.EndTurn();
    }

    public void SetControllerReference(GameController gameController)
    {
        _gameController = gameController;
    }
}
