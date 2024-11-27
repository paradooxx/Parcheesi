using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{
    public int currentPositionIndex = -1;
    public bool isInPlay => currentPositionIndex >= 0;

    public Player mainPlayer;
    public Node currentNode;

    [SerializeField, Range(0, 3)] private int pawnIndex;

    private void Start()
    {
        mainPlayer = GetComponentInParent<Player>();
    }

    private void OnMouseDown() 
    {
        if(GameManager.Instance.currentPlayer == mainPlayer)
        {
            Debug.Log($"Pawn clicked: {name}");
            GameManager.Instance.OnPawnSelected(this, GameManager.Instance.diceResultSum);
        }
        else
        {
            Debug.Log($"Cannot move pawn: {name}, not the current player's turn.");
        }
    }

    public void MovePawn(int steps, System.Action onMoveComplete)
    {
        if(!isInPlay)
        {
            Debug.Log("NotInPlay");
            return;
        }
        StartCoroutine(MovePawnCoroutine(steps, onMoveComplete));
    }

    private IEnumerator MovePawnCoroutine(int steps, System.Action onMoveComplete)
    {
        Debug.Log($"Starting to move pawn: {name} for {steps} steps.");
        for (int i = 0; i < steps; i++)
        {
            Node nextNode = GameManager.Instance.GetNextNodeForPlayer(this);

            if (nextNode != null && nextNode.CanPawnLand(this))
            {
                Debug.Log($"Moving to node: {nextNode.nodeIndex}");
                nextNode.TryEliminatePawn(this);
                nextNode.AddPawn(this);

                currentPositionIndex++;
                currentNode = nextNode;

                // Smooth movement
                Vector3 startPosition = transform.position;
                Vector3 endPosition = nextNode.transform.position;
                float elapsedTime = 0f;
                float moveDuration = 0.5f;

                while (elapsedTime < moveDuration)
                {
                    transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                transform.position = endPosition; // Ensure final position is exact
            }
            else
            {
                Debug.Log($"Cannot move pawn to node. NextNode is null or cannot land on node.");
                break; // Stop moving if the node is invalid
            }
        }
        Debug.Log($"Finished moving pawn: {name}");
        onMoveComplete?.Invoke();
    }


    public void EnterBoard()
    {
        if(!isInPlay)
        {
            currentPositionIndex = 0;
            Node startNode = mainPlayer.playerPath[currentPositionIndex];
            startNode.AddPawn(this);
            transform.position = startNode.transform.position;
        }
    }

    public void ResetToHomePosition()
    {
        currentPositionIndex = -1;
        currentNode?.RemovePawn(this);
        currentNode = null;
        transform.position = mainPlayer.homePositions[pawnIndex].transform.position;
    }
}
