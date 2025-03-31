using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI[] gridCells;
    
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI gameOverText;
    
    private string _currentPlayer;
    
    private int _turnCount;
    public string playerX = "X";
    public string playerO = "O";
    private bool _isGameOver;
    
    private void Start()
    {
        SetControllerReferenceOnButtons();
        InitializeGame();
    }

    private void InitializeGame()
    {
        turnText.text = "开始游戏或选择玩家";
        gameOverText.text = "";
        _currentPlayer = playerX;
        _isGameOver = false;
        _turnCount = 0;
    }

    private void SetControllerReferenceOnButtons()
    {
        for (int i = 0; i < gridCells.Length; i++)
        {
            Cell cellComponent = gridCells[i].GetComponentInParent<Cell>();
            if (cellComponent != null)
            {
                cellComponent.SetControllerReference(this);
            }
            else
            {
                Debug.LogError($"Cell {i} is missing a Cell component!");
            }
        }
    }

    public string GetPlayerSide()
    {
        return _currentPlayer;
    }

    public void EndTurn()
    {
        if (_isGameOver) return;
        _turnCount++;
        
        //检测胜利
        if (CheckWin())
        {
            HandleGameOver(_currentPlayer + "赢了！");
            return;
        }
        //检测平局
        if (_turnCount >= 9)
        {
            HandleGameOver("平局！");
            return;
        }
        
        ChangeSides();
    }

    private void ChangeSides()
    {
        _currentPlayer = (_currentPlayer == playerX) ? playerO : playerX;
        turnText.text = $"轮到 {_currentPlayer} 了";
    }

    private bool CheckWin()
    {
        // 行
        for (int i = 0; i < 3; i++)
        {
            if (gridCells[i*3].text == _currentPlayer && 
                gridCells[i*3+1].text == _currentPlayer && 
                gridCells[i*3+2].text == _currentPlayer)
            {
                return true;
            }
        }
        
        // 列
        for (int i = 0; i < 3; i++)
        {
            if (gridCells[i].text == _currentPlayer && 
                gridCells[i+3].text == _currentPlayer && 
                gridCells[i+6].text == _currentPlayer)
            {
                return true;
            }
        }
        
        // 对角线
        if (gridCells[0].text == _currentPlayer && 
            gridCells[4].text == _currentPlayer && 
            gridCells[8].text == _currentPlayer)
        {
            return true;
        }
        
        if (gridCells[2].text == _currentPlayer && 
            gridCells[4].text == _currentPlayer && 
            gridCells[6].text == _currentPlayer)
        {
            return true;
        }
        
        return false;
    }

    private void HandleGameOver(string message)
    {
        _isGameOver = true;
        turnText.text = "游戏结束";

        gameOverText.text = message;
        
        
        foreach (var singleCell in gridCells)
        {
            singleCell.GetComponentInParent<Button>().enabled = false;
        }
        
    }

    public void RestartGame()
    {
        InitializeGame();
        
        foreach (var singleCell in gridCells)
        {
            singleCell.text = "";
            Button button = singleCell.GetComponentInParent<Button>();
            if (button != null)
            {
                button.enabled = true;
            }
        }
        
    }
}

