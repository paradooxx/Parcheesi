using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerType playerType;

    private bool isInStar;

    [SerializeField] private HomePositions thisPlayerStartPositionIndex;
    [SerializeField] private List<Transform> homePositions;
    [SerializeField] private List<Transform> pathPoints;

    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform endPosition;
    private Transform currentPosition;

    private int startHomePosition => (int)thisPlayerStartPositionIndex;

    private void Start()
    {
        transform.position = homePositions[startHomePosition].position;
    }
}

public enum PlayerType
{
    Blue, Red, Green, Yellow
}

public enum HomePositions
{
    Position0, Position1, Position2, Position3
}
