using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 控制井字棋游戏的主要逻辑，管理游戏状态、回合和AI行为
/// </summary>
public class GameController : MonoBehaviour
{
    public TextMeshProUGUI[] gridCells;

    public TextMeshProUGUI turnText;
    public Button restartButton;
    
    public TextMeshProUGUI playerXText;
    public TextMeshProUGUI playerOText;
    
    // 添加玩家选择按钮引用
    public Button playerXButton;
    public Button playerOButton;

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
    
    private SoundManager _soundManager;

    private void Start()
    {
        // // 获取SoundManager单例实例
        _soundManager = SoundManager.Instance;
        if (_soundManager == null)
        {
            Debug.LogWarning("SoundManager not found!");
        }

        // // 设置棋盘格子的控制器引用并初始化重新开始按钮
        SetControllerReferenceOnButtons();
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        // // 根据游戏模式初始化游戏
        if (currentGameMode == AIPlayer.Difficulty.None)
        {
            // // 双人游戏模式，直接开始
            _waitingForPlayerSelection = false;
            // 在双人模式下，不设置_humanPlayer，这样会导致文本显示问题
            InitializeGame();
        }
        else
        {
            // // AI对战模式，等待玩家选择X或O
            _waitingForPlayerSelection = true;
            _humanPlayer = null;
            _aiPlayerSide = null;
            turnText.text = "开始游戏或选择玩家";
            SetAllButtonsEnabled(true);

            UpdatePlayerSelectionText();
            UpdatePlayerSelectionButtons();
        }
    }
    
    private void UpdatePlayerSelectionText()
    {
        // // 更新玩家选择UI文本
        if (_waitingForPlayerSelection)
        {
            if (playerXText != null)
                playerXText.text = "X（选择）";
                
            if (playerOText != null)
                playerOText.text = "O（选择）";
        }
        else if (currentGameMode == AIPlayer.Difficulty.None)
        {
            // 双人模式下，两边都显示为玩家
            if (playerXText != null)
                playerXText.text = "X（玩家）";
                
            if (playerOText != null)
                playerOText.text = "O（玩家）";
        }
        else
        {
            // AI模式下，根据玩家选择显示玩家或电脑
            if (playerXText != null)
                playerXText.text = _humanPlayer == playerX ? "X（玩家）" : "X（电脑）";
                
            if (playerOText != null)
                playerOText.text = _humanPlayer == playerO ? "O（玩家）" : "O（电脑）";
        }
    }
    
    // 新增方法：更新玩家选择按钮状态
    private void UpdatePlayerSelectionButtons()
    {
        if (playerXButton != null)
        {
            playerXButton.interactable = _waitingForPlayerSelection;
        }
        
        if (playerOButton != null)
        {
            playerOButton.interactable = _waitingForPlayerSelection;
        }
    }

    private void InitializeGame()
    {
        // // 重置棋盘格子和游戏状态
        foreach (var singleCell in gridCells)
        {
            singleCell.text = "";
            singleCell.color = Color.white;
        }

        _isGameOver = false;
        _turnCount = 0;
        _gameStarted = true;

        if (currentGameMode != AIPlayer.Difficulty.None)
        {
            // // AI对战模式的初始化
            if (_humanPlayer == playerO)
            {
                // // 如果玩家选择O，AI先行
                _currentPlayer = _aiPlayerSide;
                turnText.text = "电脑思考中...";
                SetAllButtonsEnabled(false);
            }
            else
            {
                // // 如果玩家选择X，玩家先行
                _currentPlayer = _humanPlayer;
                turnText.text = $"轮到您了 ({_humanPlayer})";
                SetAllButtonsEnabled(true);
            }

            // // 创建AI实例并执行AI移动（如果AI先行）
            _aiPlayerInstance = new AIPlayer(_aiPlayerSide);

            if (_currentPlayer == _aiPlayerSide)
            {
                Invoke(nameof(PerformAIMove), 0.5f);
            }
        }
        else
        {
            // // 双人游戏模式的初始化
            _currentPlayer = playerX;
            turnText.text = $"轮到 {_currentPlayer} 了";
            _aiPlayerInstance = null;
            SetAllButtonsEnabled(true);
        }
        
        // 更新UI文本和按钮状态（放在最后，确保所有设置完成后再更新UI）
        UpdatePlayerSelectionText();
        UpdatePlayerSelectionButtons();
    }

    private void SetAllButtonsEnabled(bool isButtonEnabled)
    {
        // // 设置所有棋盘格子的按钮启用/禁用状态
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
        // // 为每个格子设置控制器引用并添加点击监听器
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
        // // 返回当前玩家的符号（X或O）
        if (_waitingForPlayerSelection)
        {
            return "";
        }

        return _currentPlayer;
    }

    public void EndTurn()
    {
        // // 处理回合结束逻辑
        if (_waitingForPlayerSelection)
        {
            return;
        }

        if (_isGameOver) return;
        _turnCount++;

        // // 播放落子音效
        if (_soundManager != null)
        {
            _soundManager.PlayPlacementSound();
        }

        // // 检查是否有玩家获胜
        if (CheckWin())
        {
            string winMessage;
            if (currentGameMode == AIPlayer.Difficulty.None)
            {
                // // 双人模式的胜利消息
                winMessage = _currentPlayer + "赢了！";
                if (_soundManager != null)
                {
                    _soundManager.PlayVictorySound();
                }
            }
            else
            {
                // // AI模式的胜利消息
                if (_currentPlayer == _humanPlayer)
                {
                    winMessage = "您赢了！";
                    if (_soundManager != null)
                    {
                        _soundManager.PlayVictorySound();
                    }
                }
                else
                {
                    winMessage = "电脑赢了！";
                    if (_soundManager != null)
                    {
                        _soundManager.PlayDefeatSound();
                    }
                }
            }

            HandleGameOver(winMessage);
            return;
        }

        // // 检查是否平局
        if (_turnCount >= 9)
        {
            if (_soundManager != null)
            {
                _soundManager.PlayDrawSound();
            }

            HandleGameOver("平局！");
            return;
        }

        // // 切换玩家
        ChangeSides();

        if (currentGameMode != AIPlayer.Difficulty.None && _currentPlayer == _aiPlayerSide)
        {
            // // 如果是AI的回合，执行AI移动
            turnText.text = "电脑思考中...";
            SetAllButtonsEnabled(false);
            Invoke("PerformAIMove", 0.5f);
        }
        else
        {
            // // 如果是玩家的回合，更新UI
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
        // // 只启用空白格子的按钮
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
        // // 执行AI移动
        if (_isGameOver || _aiPlayerInstance == null) return;

        // // 将当前棋盘状态转换为字符串数组供AI使用
        string[] board = new string[gridCells.Length];
        for (int i = 0; i < gridCells.Length; i++)
        {
            board[i] = gridCells[i].text;
        }

        // // 获取AI决定的移动位置
        int moveIndex = _aiPlayerInstance.GetMove(currentGameMode, board);

        if (moveIndex != -1)
        {
            // // 在AI选择的位置落子
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
        // // 切换当前玩家（X和O之间）
        _currentPlayer = (_currentPlayer == playerX) ? playerO : playerX;
    }

    private int[] GetWinningCells()
    {
        // // 检查行
        for (int i = 0; i < 3; i++)
        {
            if (!string.IsNullOrEmpty(gridCells[i * 3].text) &&
                gridCells[i * 3].text == gridCells[i * 3 + 1].text &&
                gridCells[i * 3 + 1].text == gridCells[i * 3 + 2].text)
            {
                return new[] { i * 3, i * 3 + 1, i * 3 + 2 };
            }
        }

        // // 检查列
        for (int i = 0; i < 3; i++)
        {
            if (!string.IsNullOrEmpty(gridCells[i].text) &&
                gridCells[i].text == gridCells[i + 3].text &&
                gridCells[i + 3].text == gridCells[i + 6].text)
            {
                return new[] { i, i + 3, i + 6 };
            }
        }

        // // 检查对角线
        if (!string.IsNullOrEmpty(gridCells[0].text) &&
            gridCells[0].text == gridCells[4].text &&
            gridCells[4].text == gridCells[8].text)
        {
            return new[] { 0, 4, 8 };
        }

        if (!string.IsNullOrEmpty(gridCells[2].text) &&
            gridCells[2].text == gridCells[4].text &&
            gridCells[4].text == gridCells[6].text)
        {
            return new[] { 2, 4, 6 };
        }

        return null;
    }

    private bool CheckWin()
    {
        // // 检查是否有玩家获胜
        return GetWinningCells() != null;
    }

    private void HandleGameOver(string message)
    {
        // // 处理游戏结束状态
        _isGameOver = true;
        turnText.text = message;

        SetAllButtonsEnabled(false);
        
        if (!message.Contains("平局"))
        {
            // // 高亮显示获胜的格子
            HighlightWinningCells();
        }
        else
        {
            // // 平局时显示所有格子为暗色
            DimAllCells();
        }
    }

    private void HighlightWinningCells()
    {
        // // 高亮显示获胜的三个格子
        int[] winningCells = GetWinningCells();
        if (winningCells == null) return;
        
        Color highlightColor = new Color(1f, 1f, 1f); // 白色高亮
        Color dimColor = new Color(0.3f, 0.3f, 0.3f); // 灰色暗化
        
        // // 先将所有格子设为暗色
        foreach (var cell in gridCells)
        {
            cell.color = dimColor;
        }
        
        // // 再将获胜的格子设为高亮色
        foreach (int index in winningCells)
        {
            gridCells[index].color = highlightColor;
        }
    }
    
    private void DimAllCells()
    {
        // // 将所有非空格子设为暗色
        Color dimColor = new Color(0.3f, 0.3f, 0.3f); // 灰色暗化

        foreach (var cell in gridCells)
        {
            if (!string.IsNullOrEmpty(cell.text))
            {
                cell.color = dimColor;
            }
        }
    }

    public void RestartGame()
    {
        // // 重新开始游戏，重置所有状态
        foreach (var singleCell in gridCells)
        {
            singleCell.text = "";
            singleCell.color = Color.white;
        }

        _isGameOver = false;
        _turnCount = 0;

        if (currentGameMode == AIPlayer.Difficulty.None)
        {
            // // 双人模式直接开始
            _waitingForPlayerSelection = false;
            _humanPlayer = null;
            _aiPlayerSide = null;
            InitializeGame();
        }
        else
        {
            // // AI模式等待玩家选择
            _waitingForPlayerSelection = true;
            _humanPlayer = null;
            _aiPlayerSide = null;
            turnText.text = "开始游戏或选择玩家";
            SetAllButtonsEnabled(true);
            
            // UI更新放在状态设置后
            UpdatePlayerSelectionText();
            UpdatePlayerSelectionButtons();
        }
    }

    public void SelectPlayer(string player)
    {
        SelectPlayer(player, -1);
    }

    public void SelectPlayer(string player, int firstMoveIndex)
    {
        // // 玩家选择X或O
        if (!_waitingForPlayerSelection) return;

        _humanPlayer = player;
        _aiPlayerSide = (_humanPlayer == playerX) ? playerO : playerX;
        _waitingForPlayerSelection = false;
        
        UpdatePlayerSelectionText();
        // 更新玩家选择按钮状态
        UpdatePlayerSelectionButtons();
        
        InitializeGame();

        // // 如果提供了第一步移动的位置，直接在该位置落子
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
        // // 处理难度变更
        currentGameMode = (AIPlayer.Difficulty)(index);
        Debug.Log(currentGameMode);
        if (_gameStarted)
        {
            RestartGame();
        }
    }
}