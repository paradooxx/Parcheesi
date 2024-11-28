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

    private float lastClickTime = 0f;
    private float doubleClickThreshold = 0.3f;

    private void Start()
    {
        mainPlayer = GetComponentInParent<Player>();

        if(mainPlayer.isBot)
        {
            DisableMovementInteraction();
        }
    }

    private void OnMouseDown() 
    {
        if (GameManager.Instance.currentPlayer != mainPlayer || mainPlayer.isBot)
        {
            Debug.Log($"Cannot move pawn: {name}, not the current player's turn.");
            return;
        }
        if(!isInPlay && (GameManager.Instance.dice1Result == 5
                        || GameManager.Instance.dice2Result == 5
                        || GameManager.Instance.diceResultSum == 5))
        {
            EnterBoard();
            GameManager.Instance.DisablePawnSelection();
            GameManager.Instance.availableDiceResults.Clear();
            GameManager.Instance.MainDiceActive();
            return;
        }
        else if(isInPlay)
        {
            EnableMovementInteraction();
        }
        else if(!isInPlay)
        {
            DisableMovementInteraction();
        }

        List<int> availableResults = GameManager.Instance.availableDiceResults;
        if(availableResults.Count > 0 && isInPlay)
        {
            Debug.Log($"Single tap detected. Moving pawn {name} with dice result: {availableResults[0]}");
            GameManager.Instance.OnPawnSelected(this, availableResults[0]);
        }
        else
        {
            Debug.Log($"No available dice results for pawn {name}.");
        }

        if(isInPlay && GameManager.Instance.availableDiceResults.Count > 0 && mainPlayer.isBot)
        {
            int steps = GameManager.Instance.availableDiceResults[Random.Range(0, GameManager.Instance.availableDiceResults.Count)];
            GameManager.Instance.OnPawnSelected(this, steps);
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

        int stepsToVictory = mainPlayer.playerPath.Count - currentPositionIndex - 1;
        
        if(steps > stepsToVictory)
        {
            Debug.Log($"Cannot move pawn {name} for {steps} steps. Only {stepsToVictory} steps left to victory position.");
            onMoveComplete?.Invoke();
            yield break;
        }
        for (int i = 0; i < steps; i++)
        {
            Node nextNode = GameManager.Instance.GetNextNodeForPlayer(this);

            if (nextNode != null)
            {
                if(nextNode.IsBlockedForOtherPlayer(this))
                {
                    Debug.Log($"Node {nextNode.nodeIndex} is blocked for pawn {name}. Stopping one step before.");
                    break;
                }
                bool added = nextNode.AddPawn(this);
                if(!added)
                {
                    Debug.Log($"Pawn {name} cannot move to node {nextNode.nodeIndex} (node is full).");
                    break;
                }

                if(currentNode != null)
                {
                    currentNode.RemovePawn(this);
                }

                Debug.Log($"Moving to node: {nextNode.nodeIndex}");
                nextNode.TryEliminatePawn(this);
                nextNode.AddPawn(this);

                currentPositionIndex++;
                currentNode = nextNode;

                // for smooth movement
                Vector3 startPosition = transform.position;
                Vector3 endPosition = nextNode.transform.position;
                float elapsedTime = 0f;
                float moveDuration = 0.1f;

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
            Node startNode = mainPlayer.playerPath[0];
            bool added = startNode.AddPawn(this);
            if(!added)
            {
                Debug.Log($"Cannot enter board. Start node {startNode.nodeIndex} is full.");

                bool canMoveOtherPawns = false;
                foreach(Pawn pawn in mainPlayer.GetComponentsInChildren<Pawn>())
                {
                    if(pawn.isInPlay)
                    {
                        pawn.EnableMovementInteraction();
                        canMoveOtherPawns = true;
                    }
                }
                if(canMoveOtherPawns == false)
                {
                    Debug.Log($"No other pawns available to move for {mainPlayer.name}. Ending turn.");
                    GameManager.Instance.EndTurn();
                }
                return;
            }
            DisableMovementInteraction();
            currentPositionIndex = 0;
            currentNode = startNode;
            transform.position = startNode.transform.position;
            StartCoroutine(PlayEnterBoardAnimation());
            Debug.Log($"Pawn {name} entered the board at Node {startNode.nodeIndex}.");
        }
    }

    private IEnumerator PlayEnterBoardAnimation()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 enlargedScale = originalScale * 1.5f;

        float animationDuration = 0.3f;
        float elapsedTime = 0f;

        //scaling up
        while (elapsedTime < animationDuration / 2)
        {
            transform.localScale = Vector3.Lerp(originalScale, enlargedScale, elapsedTime / (animationDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;

        //scaling down
        while (elapsedTime < animationDuration / 2)
        {
            transform.localScale = Vector3.Lerp(enlargedScale, originalScale, elapsedTime / (animationDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
    }

    
    public void EnableMovementInteraction()
    {
        GetComponent<Collider2D>().enabled = true;
    }

    public void DisableMovementInteraction()
    {
        GetComponent<Collider2D>().enabled = false;
    }

    public void ResetToHomePosition()
    {
        currentPositionIndex = -1;
        currentNode?.RemovePawn(this);
        currentNode = null;
        transform.position = mainPlayer.homePositions[pawnIndex].transform.position;
    }
}
