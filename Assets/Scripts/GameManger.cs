using System;
using System.Collections;
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

    // [Header("Victory Positions")]
    // public Transform blueVictoryPosition;
    // public Transform redVictoryPosition;
    // public Transform greenVictoryPosition;
    // public Transform yellowVictoryPosition;

    [Header("Players")]
    public Player bluePlayer;
    public Player redPlayer;
    public Player greenPlayer;
    public Player yellowPlayer;

    [Header("Dice Positions")]
    [SerializeField] private List<Transform> dicePositions;
    [SerializeField] private Transform mainDice;
    [SerializeField] DiceRoll diceRoll;
    
    public int dice1Result, dice2Result, diceResultSum;
    public List<int> availableDiceResults;
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

    //can remove after setting player in ui
    private void Start()
    {
        // allPlayers = new List<Player> { bluePlayer, redPlayer, greenPlayer, yellowPlayer };
        allPlayers = new List<Player> { bluePlayer, greenPlayer };
        currentPlayer = bluePlayer;
        currentTurnText.text = "Current Turn : " + currentPlayer.playerType.ToString();
        currentTurnText.color = currentPlayer.playerColor;
        SetDicePositions(currentPlayer);

        foreach (Player player in allPlayers)
        {
            player.InitializePlayerPath();
        }
    }

    // public void InitializeGame(int playerCount)
    // {
    //     if(playerCount < 2 || playerCount > 4)
    //     {
    //         //some error ui
    //         return;
    //     }

    //     allPlayers = new List<Player>();
    //     if (playerCount >= 1) allPlayers.Add(bluePlayer);
    //     if (playerCount >= 2) allPlayers.Add(greenPlayer);
    //     if (playerCount >= 3) allPlayers.Add(redPlayer);
    //     if (playerCount == 4) allPlayers.Add(yellowPlayer);

    //     if (!allPlayers.Contains(redPlayer)) redPlayer.gameObject.SetActive(false);
    //     if (!allPlayers.Contains(yellowPlayer)) yellowPlayer.gameObject.SetActive(false);

    //     foreach (Player player in allPlayers)
    //     {
    //         player.InitializePlayerPath();
    //     }

    //     currentPlayer = allPlayers[0];
    //     currentTurnText.text = "Current Turn : " + currentPlayer.playerType.ToString();
    //     SetDicePositions(currentPlayer);
    //     Debug.Log($"{playerCount} players initialized for the game.");
    //     GameStateManager.Instance.SetState(GameState.Game);
    // }

    private void SetDicePositions(Player currentPlayer)
    {
        if(currentPlayer == bluePlayer)
        {
            mainDice.rotation = Quaternion.Euler(0, 0, 0);
            mainDice.position = dicePositions[0].position;
        }
        else if(currentPlayer == redPlayer)
        {
            mainDice.rotation = Quaternion.Euler(0, 0, -180);
            mainDice.position = dicePositions[1].position;
        }
        else if(currentPlayer == greenPlayer)
        {
            mainDice.rotation = Quaternion.Euler(0, 0, -180);
            mainDice.position = dicePositions[2].position;
        }
        else if(currentPlayer == yellowPlayer)
        {
            mainDice.rotation = Quaternion.Euler(0, 0, 0);
            mainDice.position = dicePositions[3].position;
        }
    }

    public void RollDice()
    {
        // diceButton.interactable = false;
        diceRoll.RollDiceWithCallback((dice1, dice2) =>
        {
            dice1Result = dice1;
            dice2Result = dice2;
            diceResultSum = dice1Result + dice2Result;
            Debug.Log($"Dice Results: {dice1Result}, {dice2Result} : Sum: {diceResultSum}");
            availableDiceResults = new List<int> { dice1Result, dice2Result };
            MainDiceDeactive();
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

    private void ProcessDiceRoll()
    {
        if (!CanPlayerMakeMove())
        {
            Debug.Log($"{currentPlayer} cannot make a move. Skipping turn.");
            Invoke("EndTurn", 1.5f);
            return;
        }
        if(dice1Result == 5 || dice2Result == 5 || diceResultSum == 5)
        {
            foreach (Pawn pawn in currentPlayer.GetComponentsInChildren<Pawn>())
            {
                // pawn.DisableMovementInteraction();
                Debug.Log("PPPPPPPPPPPPPLAYS: " + PawnsInPlay());
                if(!pawn.isInPlay && PawnsInPlay() == 0)
                {
                    pawn.EnterBoard();
                    DisablePawnSelection();
                    MainDiceActive();
                    return;
                }
                else
                {
                    EnablePawnSelection();
                }
                // if(pawn.isInPlay)
                // if(!pawn.isInPlay)
                // {
                //     pawn.EnterBoard();
                //     mainDice.gameObject.GetComponent<Collider2D>().enabled = true;
                //     return;
                // }
            }
        }

        bool anyPawnInPlay = false;
        foreach (Pawn pawn in currentPlayer.GetComponentsInChildren<Pawn>())
        {
            if(pawn.isInPlay)
            {
                pawn.EnableMovementInteraction();
                anyPawnInPlay = true;
            }
            
        }
        if(!anyPawnInPlay)
        {
            EndTurn();
        }
        else
        {
            EnablePawnSelection();
            MainDiceDeactive();
        }
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

    public void MainDiceActive()
    {
        mainDice.gameObject.GetComponent<Collider2D>().enabled = true;
    }

    public void MainDiceDeactive()
    {
        mainDice.gameObject.GetComponent<Collider2D>().enabled = false;
    }

    public void OnPawnSelected(Pawn selectedPawn, int steps)
    {
        Debug.Log($"Moving pawn: {selectedPawn.name} for {steps} steps.");

        int stepsToVictory = selectedPawn.mainPlayer.playerPath.Count - selectedPawn.currentPositionIndex - 1;

        if(steps > stepsToVictory)
        {
            Debug.Log($"Cannot move pawn {selectedPawn.name} for {steps} steps. Only {stepsToVictory} steps left to victory position.");
            return;
        }
        
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
        // selectedPawn.MovePawn(steps, () => { EndTurn(); });
    }

    public int PawnsInPlay()
    {
        int pawnInPlay = 0;
        foreach (Pawn pawn in currentPlayer.GetComponentsInChildren<Pawn>())
        {
            if(pawn.isInPlay)
            {
                pawnInPlay ++;
            }
        }
        return pawnInPlay;
    }

    public void EndTurn()
    {
        DisablePawnSelection();
        availableDiceResults.Clear();

        currentPlayerIndex = (currentPlayerIndex + 1) % allPlayers.Count;
        currentPlayer = allPlayers[currentPlayerIndex];

        CheckVictoryCondition();
        SetDicePositions(currentPlayer);
        
        currentTurnText.text = "Current Turn : " + currentPlayer.playerType.ToString();
        currentTurnText.color = currentPlayer.playerColor;
        // diceButton.interactable = !currentPlayer.isPlayerAI;
        mainDice.gameObject.GetComponent<Collider2D>().enabled = true;
        // diceButton.interactable = true;
    }

    public Node GetNextNodeForPlayer(Pawn pawn)
    {
        if (pawn == null)
        {
            Debug.LogError("Player is null. Cannot determine next node.");
            return null;
        }

        int currentIndex = pawn.currentPositionIndex;

        // check if player has not reached the end of their path
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
        bool allPawnsAtVictoruPosition = false;
        foreach (Pawn pawn in currentPlayer.GetComponentsInChildren<Pawn>())
        {
            if (pawn.currentNode != null && pawn.currentNode == currentPlayer.victoryPosition)
            {
                allPawnsAtVictoruPosition = true;
                break;
            }
        }

        if (allPawnsAtVictoruPosition)
        {
            Debug.Log($"{currentPlayer} has won the game!");
            hasWon = true;
            //some game end logic
            GameEnd();
        }
    }

    private void GameEnd()
    {
        Debug.Log($"Game Over! {currentPlayer.name} is the winner!");

        foreach (Player player in allPlayers)
        {
            foreach(Pawn pawn in player.GetComponentsInChildren<Pawn>())
            {
                pawn.DisableMovementInteraction();
            }
        }
        Debug.Log("Game ended");
        //some UI Message
        Invoke("GoToMainMenu", 2f);
    }
    
    private void GoToMainMenu()
    {
        GameStateManager.Instance.SetState(GameState.MainMenu);
    }
}
