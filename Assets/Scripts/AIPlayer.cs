using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 控制AI的难度算法
/// </summary>
public class AIPlayer
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        None
    }

    private readonly string _aiSide;
    private readonly string _playerSide;

    public AIPlayer(string aiSide)
    {
        _aiSide = aiSide;
        _playerSide = (aiSide == "X") ? "O" : "X";
    }

    public int GetMove(Difficulty difficulty, string[] board)
    {
        // // 根据难度选择不同的AI移动策略
        switch (difficulty)
        {
            case Difficulty.Easy:
                return GetRandomMove(board);
            case Difficulty.Medium:
                return GetMediumMove(board);
            case Difficulty.Hard:
                return GetBestMove(board);
            default:
                return -1;
        }
    }

    private int GetRandomMove(string[] board)
    {
        // // 简单难度：随机选择一个空格子
        List<int> emptyCells = new List<int>();
        for (int i = 0; i < board.Length; i++)
        {
            if (string.IsNullOrEmpty(board[i]))
            {
                emptyCells.Add(i);
            }
        }

        return (emptyCells.Count > 0) ? emptyCells[Random.Range(0, emptyCells.Count)] : -1;
    }

    private int GetMediumMove(string[] board)
    {
        // // 中等难度：先尝试获胜，再阻止玩家获胜，最后随机移动

        // // 检查AI是否能在下一步获胜
        for (int i = 0; i < board.Length; i++)
        {
            if (string.IsNullOrEmpty(board[i]))
            {
                board[i] = _aiSide;
                if (CheckWin(board, _aiSide))
                {
                    board[i] = "";
                    return i;
                }

                board[i] = "";
            }
        }

        // // 检查是否需要阻止玩家在下一步获胜
        for (int i = 0; i < board.Length; i++)
        {
            if (string.IsNullOrEmpty(board[i]))
            {
                board[i] = _playerSide;
                if (CheckWin(board, _playerSide))
                {
                    board[i] = "";
                    return i;
                }

                board[i] = "";
            }
        }

        // // 如果没有特殊情况，随机移动
        return GetRandomMove(board);
    }

    private int GetBestMove(string[] board)
    {
        // // 困难难度：使用极小极大算法找出最佳移动
        int bestScore = int.MinValue;
        int move = -1;

        for (int i = 0; i < board.Length; i++)
        {
            if (string.IsNullOrEmpty(board[i]))
            {
                // // 尝试每个可能的移动
                board[i] = _aiSide;
                int score = Minimax(board, false);
                board[i] = "";

                // // 选择得分最高的移动
                if (score > bestScore)
                {
                    bestScore = score;
                    move = i;
                }
            }
        }

        return move;
    }

    private int Minimax(string[] board, bool isMaximizing)
    {
        // // 递归实现极小极大算法

        // // 检查终止条件
        if (CheckWin(board, _aiSide)) return 1;
        if (CheckWin(board, _playerSide)) return -1;
        if (IsBoardFull(board)) return 0;

        // // 递归计算每个可能移动的分数
        int bestScore = isMaximizing ? int.MinValue : int.MaxValue;

        for (int i = 0; i < board.Length; i++)
        {
            if (string.IsNullOrEmpty(board[i]))
            {
                // // 尝试移动
                board[i] = isMaximizing ? _aiSide : _playerSide;
                int score = Minimax(board, !isMaximizing);
                board[i] = "";

                // // 更新最佳分数
                bestScore = isMaximizing ? Mathf.Max(score, bestScore) : Mathf.Min(score, bestScore);
            }
        }

        return bestScore;
    }

    private bool CheckWin(string[] board, string side)
    {
        // // 检查行是否有三连一致
        for (int i = 0; i < 3; i++)
        {
            if (board[i * 3] == side && board[i * 3 + 1] == side && board[i * 3 + 2] == side)
                return true;
        }

        // // 检查列是否有三连一致
        for (int i = 0; i < 3; i++)
        {
            if (board[i] == side && board[i + 3] == side && board[i + 6] == side)
                return true;
        }

        // // 检查对角线是否有三连一致
        if (board[0] == side && board[4] == side && board[8] == side)
            return true;
        if (board[2] == side && board[4] == side && board[6] == side)
            return true;

        return false;
    }

    private bool IsBoardFull(string[] board)
    {
        // // 检查棋盘是否已满（平局判断）
        foreach (string cell in board)
        {
            if (string.IsNullOrEmpty(cell)) return false;
        }

        return true;
    }
}