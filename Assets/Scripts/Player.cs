using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerType playerType;
    public List<Node> playerPath;
    public Transform victoryPosition;

    public List<Transform> homePositions;
    public Pawn[] pawns;

    public int currentPathIndex = 0;
    public int currentPositionIndex = -1;
    public bool isInPlay => currentPositionIndex >= 0;

    public Node currentNode;

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
            pawns[i].transform.position = homePositions[i].position;
            pawns[i].ResetToHomePosition(); // Ensure the pawn is reset properly
        }
    }

    public void InitializePlayerPath()
    {
        switch (playerType)
        {
            case PlayerType.Blue:
                playerPath = GameManager.Instance.bluePlayerPath;
                victoryPosition = GameManager.Instance.blueVictoryPosition;
                break;

            case PlayerType.Red:
                playerPath = GameManager.Instance.redPlayerPath;
                victoryPosition = GameManager.Instance.redVictoryPosition;
                break;

            case PlayerType.Green:
                playerPath = GameManager.Instance.greenPlayerPath;
                victoryPosition = GameManager.Instance.greenVictoryPosition;
                break;

            case PlayerType.Yellow:
                playerPath = GameManager.Instance.yellowPlayerPath;
                victoryPosition = GameManager.Instance.yellowVictoryPosition;
                break;

            default:
                Debug.LogError("Invalid PlayerType for Player initialization!");
                break;

        }
        InitializePawns();
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
