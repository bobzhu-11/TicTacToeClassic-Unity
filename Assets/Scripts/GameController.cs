using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI[] gridCells;

    public TextMeshProUGUI turnText;
    public TextMeshProUGUI gameOverText;
    public TMP_Dropdown difficultyDropdown;
    public Button restartButton;

    private string _currentPlayer;
    private string _humanPlayer;
    private string _aiPlayerSide;

    private AIPlayer _aiPlayerInstance;
    public AIPlayer.Difficulty currentGameMode = AIPlayer.Difficulty.Easy;

    private int _turnCount;
    public string playerX = "X";
    public string playerO = "O";
    private bool _isGameOver;
    private bool _gameStarted;
    private bool _waitingForPlayerSelection;

    private void Start()
    {
        SetControllerReferenceOnButtons();
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        if (currentGameMode == AIPlayer.Difficulty.None)
        {
            _waitingForPlayerSelection = false;
            _humanPlayer = null;
            InitializeGame();
        }
        else
        {
            _waitingForPlayerSelection = true;
            turnText.text = "开始游戏或选择玩家";
            gameOverText.text = "";
            SetAllButtonsEnabled(true);
        }
    }

    private void InitializeGame()
    {
        foreach (var singleCell in gridCells)
        {
            singleCell.text = "";
        }

        _isGameOver = false;
        _turnCount = 0;
        _gameStarted = true;
        gameOverText.text = "";

        if (currentGameMode != AIPlayer.Difficulty.None)
        {
            if (_humanPlayer == playerO)
            {
                _currentPlayer = _aiPlayerSide;
                turnText.text = "AI思考中...";
                SetAllButtonsEnabled(false);
            }
            else
            {
                _currentPlayer = _humanPlayer;
                turnText.text = $"轮到您了 ({_humanPlayer})";
                SetAllButtonsEnabled(true);
            }

            _aiPlayerInstance = new AIPlayer(_aiPlayerSide);

            if (_currentPlayer == _aiPlayerSide)
            {
                Invoke(nameof(PerformAIMove), 0.5f);
            }
        }
        else
        {
            _currentPlayer = playerX;
            turnText.text = $"轮到 {_currentPlayer} 了";
            _aiPlayerInstance = null;
            SetAllButtonsEnabled(true);
        }
    }

    private void SetAllButtonsEnabled(bool isButtonEnabled)
    {
        foreach (var singleCell in gridCells)
        {
            Cell cellComponent = singleCell.GetComponentInParent<Cell>();
            if (cellComponent != null && cellComponent.button != null)
            {
                cellComponent.button.enabled = isButtonEnabled;
            }
        }
    }

    private void SetControllerReferenceOnButtons()
    {
        for (int i = 0; i < gridCells.Length; i++)
        {
            Cell cellComponent = gridCells[i].GetComponentInParent<Cell>();
            if (cellComponent != null)
            {
                cellComponent.SetControllerReference(this);
                Button button = cellComponent.button;
                if (button != null)
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => cellComponent.SetCell());
                }
            }
            else
            {
                Debug.LogError($"Cell {i} is missing a Cell component!");
            }
        }
    }

    public string GetPlayerSide()
    {
        if (_waitingForPlayerSelection)
        {
            return "";
        }

        return _currentPlayer;
    }

    public void EndTurn()
    {
        if (_waitingForPlayerSelection)
        {
            return;
        }

        if (_isGameOver) return;
        _turnCount++;

        if (CheckWin())
        {
            string winMessage;
            if (currentGameMode == AIPlayer.Difficulty.None)
            {
                winMessage = _currentPlayer + "赢了！";
            }
            else
            {
                winMessage = (_currentPlayer == _humanPlayer) ? "您赢了！" : "AI赢了！";
            }

            HandleGameOver(winMessage);
            return;
        }

        if (_turnCount >= 9)
        {
            HandleGameOver("平局！");
            return;
        }

        ChangeSides();

        if (currentGameMode != AIPlayer.Difficulty.None && _currentPlayer == _aiPlayerSide)
        {
            turnText.text = "AI思考中...";
            SetAllButtonsEnabled(false);
            Invoke("PerformAIMove", 0.5f);
        }
        else
        {
            if (currentGameMode == AIPlayer.Difficulty.None)
            {
                turnText.text = $"轮到 {_currentPlayer} 了";
            }
            else
            {
                turnText.text = $"轮到您了 ({_humanPlayer})";
            }

            SetEmptyCellsEnabled(true);
        }
    }

    private void SetEmptyCellsEnabled(bool isButtonEnabled)
    {
        for (int i = 0; i < gridCells.Length; i++)
        {
            Cell cellComponent = gridCells[i].GetComponentInParent<Cell>();
            if (cellComponent != null && cellComponent.button != null)
            {
                if (string.IsNullOrEmpty(gridCells[i].text))
                {
                    cellComponent.button.enabled = isButtonEnabled;
                }
                else
                {
                    cellComponent.button.enabled = false;
                }
            }
        }
    }

    private void PerformAIMove()
    {
        if (_isGameOver || _aiPlayerInstance == null) return;

        string[] board = new string[gridCells.Length];
        for (int i = 0; i < gridCells.Length; i++)
        {
            board[i] = gridCells[i].text;
        }

        int moveIndex = _aiPlayerInstance.GetMove(currentGameMode, board);

        if (moveIndex != -1)
        {
            Cell cell = gridCells[moveIndex].GetComponentInParent<Cell>();
            if (cell != null)
            {
                cell.SetCell(_currentPlayer);
            }
        }

        if (!_isGameOver)
        {
            SetEmptyCellsEnabled(true);
        }
    }

    private void ChangeSides()
    {
        _currentPlayer = (_currentPlayer == playerX) ? playerO : playerX;
    }

    private bool CheckWin()
    {
        // 检查行
        for (int i = 0; i < 3; i++)
        {
            if (!string.IsNullOrEmpty(gridCells[i * 3].text) &&
                gridCells[i * 3].text == gridCells[i * 3 + 1].text &&
                gridCells[i * 3 + 1].text == gridCells[i * 3 + 2].text)
            {
                return true;
            }
        }

        // 检查列
        for (int i = 0; i < 3; i++)
        {
            if (!string.IsNullOrEmpty(gridCells[i].text) &&
                gridCells[i].text == gridCells[i + 3].text &&
                gridCells[i + 3].text == gridCells[i + 6].text)
            {
                return true;
            }
        }

        // 检查对角线
        if (!string.IsNullOrEmpty(gridCells[0].text) &&
            gridCells[0].text == gridCells[4].text &&
            gridCells[4].text == gridCells[8].text)
        {
            return true;
        }

        if (!string.IsNullOrEmpty(gridCells[2].text) &&
            gridCells[2].text == gridCells[4].text &&
            gridCells[4].text == gridCells[6].text)
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

        SetAllButtonsEnabled(false);
    }

    public void RestartGame()
    {
        foreach (var singleCell in gridCells)
        {
            singleCell.text = "";
        }

        _isGameOver = false;
        _turnCount = 0;

        if (currentGameMode == AIPlayer.Difficulty.None)
        {
            _waitingForPlayerSelection = false;
            _humanPlayer = null;
            _aiPlayerSide = null;
            InitializeGame();
        }
        else
        {
            _waitingForPlayerSelection = true;
            _humanPlayer = null;
            _aiPlayerSide = null;
            turnText.text = "开始游戏或选择玩家";
            gameOverText.text = "";
            SetAllButtonsEnabled(true);
        }
    }

    public void SelectPlayer(string player)
    {
        SelectPlayer(player, -1);
    }

    public void SelectPlayer(string player, int firstMoveIndex)
    {
        if (!_waitingForPlayerSelection) return;

        _humanPlayer = player;
        _aiPlayerSide = (_humanPlayer == playerX) ? playerO : playerX;
        _waitingForPlayerSelection = false;

        InitializeGame();

        if (firstMoveIndex >= 0 && firstMoveIndex < gridCells.Length)
        {
            Cell cell = gridCells[firstMoveIndex].GetComponentInParent<Cell>();
            if (cell != null && string.IsNullOrEmpty(gridCells[firstMoveIndex].text))
            {
                gridCells[firstMoveIndex].text = _humanPlayer;
                cell.button.enabled = false;
                EndTurn();
            }
        }
    }

    public void OnDifficultyChanged(int index)
    {
        currentGameMode = (AIPlayer.Difficulty)(index);
        Debug.Log(currentGameMode);
        if (_gameStarted)
        {
            RestartGame();
        }
    }
}