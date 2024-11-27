using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public List<int> availableDiceResults;
    public Player currentPlayer;
    private int currentPlayerIndex = 0;
    private List<Player> allPlayers;

    [SerializeField] private TMP_Text currentTurnText;

    private bool hasWon;

    public Button diceButton;

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

    //can remove after setting player in ui
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

    public void InitializeGame(int playerCount)
    {
        if(playerCount < 2 || playerCount > 4)
        {
            //some error ui
            return;
        }

        allPlayers = new List<Player>();
        if (playerCount >= 1) allPlayers.Add(bluePlayer);
        if (playerCount >= 2) allPlayers.Add(redPlayer);
        if (playerCount >= 3) allPlayers.Add(greenPlayer);
        if (playerCount == 4) allPlayers.Add(yellowPlayer);

        if (!allPlayers.Contains(greenPlayer)) greenPlayer.gameObject.SetActive(false);
        if (!allPlayers.Contains(yellowPlayer)) yellowPlayer.gameObject.SetActive(false);

        foreach (Player player in allPlayers)
        {
            player.InitializePlayerPath();
        }

        currentPlayer = allPlayers[0];
        currentTurnText.text = "Current Turn : " + currentPlayer.ToString();

        Debug.Log($"{playerCount} players initialized for the game.");
    }

    public void RollDice()
    {
        diceButton.interactable = false;
        diceRoll.RollDiceWithCallback((dice1, dice2) =>
        {
            dice1Result = dice1;
            dice2Result = dice2;
            diceResultSum = dice1Result + dice2Result;
            Debug.Log($"Dice Results: {dice1Result}, {dice2Result} : Sum: {diceResultSum}");
            availableDiceResults = new List<int> { dice1Result, dice2Result };
            ProcessDiceRoll();
        }); 
    }

    public bool UseDiceResult(int value)
    {
        if(availableDiceResults.Contains(value))
        {
            availableDiceResults.Remove(value);
            return true;
        }
        return false;
    }

    public bool AreDiceResultsAvailable()
    {
        return availableDiceResults.Count > 0;
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
                    diceButton.interactable = true;
                    DisablePawnSelection();
                    return;
                }
            }
        }
        EnablePawnSelection();
        // DisableDiceSelection();
    }

    public void EnablePawnSelection()
    {
        foreach(Pawn pawn in currentPlayer.GetComponentsInChildren<Pawn>())
        {
            pawn.GetComponent<Collider2D>().enabled = true;
        }
    }

    public void DisablePawnSelection()
    {
        foreach(Pawn pawn in currentPlayer.GetComponentsInChildren<Pawn>())
        {
            pawn.GetComponent<Collider2D>().enabled = false;
        }
    }

    public void OnPawnSelected(Pawn selectedPawn, int steps)
    {
        Debug.Log($"Moving pawn: {selectedPawn.name} for {steps} steps.");

        if(UseDiceResult(steps))
        {
            selectedPawn.MovePawn(steps, () => 
            {
                if(!AreDiceResultsAvailable())
                {
                    EndTurn();
                }
                else
                {
                    EnablePawnSelection();
                }
            });
        }
        else
        {
            Debug.Log($"Cannot move pawn {selectedPawn.name} for {steps} steps. Dice result unavailable.");
        }
        selectedPawn.MovePawn(steps, () => { EndTurn(); });
    }

    private void EndTurn()
    {
        DisablePawnSelection();

        currentPlayerIndex = (currentPlayerIndex + 1) % allPlayers.Count;
        currentPlayer = allPlayers[currentPlayerIndex];

        currentTurnText.text = "Current Turn : " + currentPlayer.ToString();
        diceButton.interactable = true;
        
        availableDiceResults.Clear();
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
            return null;
        }
    }

    private void CheckVictoryCondition()
    {
        bool allPawnsAtVictoruPosition = true;
        foreach (Pawn pawn in currentPlayer.GetComponentsInChildren<Pawn>())
        {
            if (pawn.currentNode == null && pawn.currentNode != currentPlayer.victoryPosition)
            {
                allPawnsAtVictoruPosition = false;
                break;
            }
        }

        if (allPawnsAtVictoruPosition)
        {
            Debug.Log($"{currentPlayer} has won the game!");
            hasWon = true;
            //some game end logic
        }
    }
}
