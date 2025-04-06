using System.Collections.Generic;
using UnityEngine;

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

        return GetRandomMove(board);
    }

    private int GetBestMove(string[] board)
    {
        int bestScore = int.MinValue;
        int move = -1;

        for (int i = 0; i < board.Length; i++)
        {
            if (string.IsNullOrEmpty(board[i]))
            {
                board[i] = _aiSide;
                int score = Minimax(board, false);
                board[i] = "";

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
        if (CheckWin(board, _aiSide)) return 1;
        if (CheckWin(board, _playerSide)) return -1;
        if (IsBoardFull(board)) return 0;

        int bestScore = isMaximizing ? int.MinValue : int.MaxValue;

        for (int i = 0; i < board.Length; i++)
        {
            if (string.IsNullOrEmpty(board[i]))
            {
                board[i] = isMaximizing ? _aiSide : _playerSide;
                int score = Minimax(board, !isMaximizing);
                board[i] = "";

                bestScore = isMaximizing ? Mathf.Max(score, bestScore) : Mathf.Min(score, bestScore);
            }
        }

        return bestScore;
    }

    private bool CheckWin(string[] board, string side)
    {
        for (int i = 0; i < 3; i++)
        {
            if (board[i * 3] == side && board[i * 3 + 1] == side && board[i * 3 + 2] == side)
                return true;
        }

        for (int i = 0; i < 3; i++)
        {
            if (board[i] == side && board[i + 3] == side && board[i + 6] == side)
                return true;
        }

        if (board[0] == side && board[4] == side && board[8] == side)
            return true;
        if (board[2] == side && board[4] == side && board[6] == side)
            return true;

        return false;
    }

    private bool IsBoardFull(string[] board)
    {
        foreach (string cell in board)
        {
            if (string.IsNullOrEmpty(cell)) return false;
        }

        return true;
    }
}