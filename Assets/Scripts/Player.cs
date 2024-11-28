using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerType playerType;
    public Color playerColor;
    public List<Node> playerPath;
    public Transform victoryPosition;

    public List<Transform> homePositions;
    public Pawn[] pawns;

    public int currentPathIndex = 0;
    public int currentPositionIndex = -1;
    public bool isInPlay => currentPositionIndex >= 0;

    public Node currentNode;

    public bool isPlayerAI;

    private void InitializePawns()
    {
        if (homePositions.Count != pawns.Length)
        {
            Debug.LogError("Home positions and pawns count mismatch!");
            return;
        }

        // Set each pawn to its respective home position
        for (int i = 0; i < pawns.Length; i++)
        {
            if(!gameObject.activeSelf)
            {
                pawns[i].gameObject.SetActive(false);
            }
            else
            {
                pawns[i].transform.position = homePositions[i].position;
                pawns[i].ResetToHomePosition(); // Ensure the pawn is reset properl
            }
        }
    }

    public void InitializePlayerPath()
    {
        int totalPath = GameManager.Instance.bluePlayerPath.Count;
        switch (playerType)
        {
            case PlayerType.Blue:
                playerPath = GameManager.Instance.bluePlayerPath;
                victoryPosition = GameManager.Instance.bluePlayerPath[totalPath - 1].transform;
                break;

            case PlayerType.Red:
                playerPath = GameManager.Instance.redPlayerPath;
                victoryPosition = GameManager.Instance.redPlayerPath[totalPath - 1].transform;
                break;

            case PlayerType.Green:
                playerPath = GameManager.Instance.greenPlayerPath;
                victoryPosition = GameManager.Instance.greenPlayerPath[totalPath - 1].transform;
                break;

            case PlayerType.Yellow:
                playerPath = GameManager.Instance.yellowPlayerPath;
                victoryPosition = GameManager.Instance.yellowPlayerPath[totalPath - 1].transform;
                break;

            default:
                Debug.LogError("Invalid PlayerType for Player initialization!");
                break;
        }
        InitializePawns(); // Initialize pawns for the active player
    }

    // public void MovePlayer(int dice1, int dice2, System.Action onMoveComplete)
    // {
    //     int totalSteps = dice1 + dice2;

    //     if()
    // }

}

public enum PlayerType
{
    Blue, Red, Green, Yellow
}

public enum HomePositions
{
    Position0, Position1, Position2, Position3
}
