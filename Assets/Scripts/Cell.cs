using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 管理棋盘 
/// </summary>
public class Cell : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI buttonText;

    private string _playerSide;
    private GameController _gameController;

    private void Start()
    {
        // // 初始化时添加按钮点击监听器
        if (button != null && _gameController != null)
        {
            button.onClick.AddListener(() => SetCell());
        }
    }

    public void SetCell(string side = null)
    {
        // // 在单元格中放置玩家符号（X或O）
        if (_gameController == null) return;

        _playerSide = side ?? _gameController.GetPlayerSide();

        if (string.IsNullOrEmpty(_playerSide))
        {
            // // 如果还没有选择玩家方，这个格子的点击会触发玩家选择
            if (buttonText.text == "")
            {
                _gameController.SelectPlayer("X", GetButtonIndex());
            }

            return;
        }

        // // 如果格子已经有内容则不做任何操作
        if (!string.IsNullOrEmpty(buttonText.text)) return;

        // // 设置格子内容并禁用按钮
        buttonText.text = _playerSide;
        button.enabled = false;
        // // 通知GameController结束当前回合
        _gameController.EndTurn();
    }

    private int GetButtonIndex()
    {
        // // 获取当前格子在棋盘中的索引
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
        // // 设置对GameController的引用，并重新添加点击监听器
        _gameController = gameController;
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => SetCell());
        }
    }
}