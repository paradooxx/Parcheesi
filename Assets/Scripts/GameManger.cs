using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("PlayerPaths")]
    public List<Node> bluePlayerPath;
    public List<Node> redPlayerPath;
    public List<Node> greenPlayerPath;
    public List<Node> yellowPlayerPath;

    [Header("Victory Positions")]
    public Transform blueVictoryPosition;
    public Transform redVictoryPosition;
    public Transform greenVictoryPosition;
    public Transform yellowVictoryPosition;

    [Header("Players")]
    public Player bluePlayer;
    public Player redPlayer;
    public Player greenPlayer;
    public Player yellowPlayer;

    [SerializeField] DiceRoll diceRoll;
    
    public int dice1Result, dice2Result, diceResultSum;
    public Player currentPlayer;
    private int currentPlayerIndex = 0;
    private List<Player> allPlayers;

    [SerializeField] private TMP_Text currentTurnText;

    private bool hasWon;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        allPlayers = new List<Player> { bluePlayer, redPlayer, greenPlayer, yellowPlayer };
        currentPlayer = bluePlayer;
        currentTurnText.text = "Current Turn : " + currentPlayer.ToString();

        foreach (Player player in allPlayers)
        {
            player.InitializePlayerPath();
        }
    }

    public void RollDice()
    {
        diceRoll.RollDiceWithCallback((dice1, dice2) =>
        {
            dice1Result = dice1;
            dice2Result = dice2;
            diceResultSum = dice1Result + dice2Result;
            Debug.Log($"Dice Results: {dice1Result}, {dice2Result} : Sum: {diceResultSum}");

            ProcessDiceRoll();
        }); 
    }

    private bool CanPlayerMakeMove()
    {
        foreach (Pawn pawn in currentPlayer.GetComponentsInChildren<Pawn>())
        {
            if (!pawn.isInPlay && (dice1Result == 5 || dice2Result == 5 || diceResultSum == 5)) return true; // Can enter board
            if (pawn.isInPlay) return true; // Can move
        }
        return false;
    }
    
    private void ProcessDiceRoll()
    {
        if (!CanPlayerMakeMove())
        {
            Debug.Log($"{currentPlayer} cannot make a move. Skipping turn.");
            EndTurn();
            return;
        }
        if(dice1Result == 5 || dice2Result == 5 || diceResultSum == 5)
        {
            foreach (Pawn pawn in currentPlayer.GetComponentsInChildren<Pawn>())
            {
                if(!pawn.isInPlay)
                {
                    pawn.EnterBoard();
                    return;
                }
            }
        }
        EnablePawnSelection();
    }

    private void EnablePawnSelection()
    {
        foreach(Pawn pawn in currentPlayer.GetComponentsInChildren<Pawn>())
        {
            pawn.GetComponent<Collider2D>().enabled = true;
        }
    }

    public void OnPawnSelected(Pawn selectedPawn, int steps)
    {
        Debug.Log($"Moving pawn: {selectedPawn.name} for {steps} steps.");
        selectedPawn.MovePawn(steps, () => { EndTurn(); });
    }

    private void EndTurn()
    {
        foreach(Pawn pawn in currentPlayer.GetComponentsInChildren<Pawn>())
        {
            pawn.GetComponent<Collider2D>().enabled = false;
        }

        currentPlayerIndex = (currentPlayerIndex + 1) % allPlayers.Count;
        currentPlayer = allPlayers[currentPlayerIndex];

        currentTurnText.text = "Current Turn : " + currentPlayer.ToString();

        CheckVictoryCondition();
    }

    public Node GetNextNodeForPlayer(Pawn pawn)
    {
        if (pawn == null)
        {
            Debug.LogError("Player is null. Cannot determine next node.");
            return null;
        }

        int currentIndex = pawn.currentPositionIndex;

        // Ensure the player has not reached the end of their path
        if (currentIndex + 1 < pawn.mainPlayer.playerPath.Count)
        {
            Node nextNode = pawn.mainPlayer.playerPath[currentIndex + 1];
            Debug.Log($"Next node for player {pawn.name}: {nextNode.nodeIndex}");
            return nextNode;
        }
        else
        {
            return null; // Optionally return the victory position node
        }
    }

    private void CheckVictoryCondition()
    {
        foreach (Pawn pawn in currentPlayer.GetComponentsInChildren<Pawn>())
        {
            if (pawn.currentNode != null && pawn.currentNode == currentPlayer.victoryPosition)
            {
                hasWon = true;
                break;
            }
            if (pawn.currentNode != null && pawn.currentNode != currentPlayer.victoryPosition)
            {
                hasWon = false;
                break;
            }
        }

        if (hasWon)
        {
            Debug.Log($"{currentPlayer} has won the game!");
            // Implement game-over logic here
        }
    }
}
