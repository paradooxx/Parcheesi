using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    // private void Start()
    // {
    //     // allPlayers = new List<Player> { bluePlayer, redPlayer, greenPlayer, yellowPlayer };
    //     allPlayers = new List<Player> { bluePlayer, redPlayer };
    //     currentPlayer = bluePlayer;
    //     currentTurnText.text = "Current Turn : " + currentPlayer.playerType.ToString();
    //     currentTurnText.color = currentPlayer.playerColor;
    //     SetDicePositions(currentPlayer);

    //     foreach (Player player in allPlayers)
    //     {
    //         player.InitializePlayerPath();
    //     }
    // }

    public void InitializeGame(int playerCount)
    {
        if(playerCount < 1 || playerCount > 4)
        {
            //some error ui
            return;
        }

        allPlayers = new List<Player>();
        if (playerCount >= 1) allPlayers.Add(bluePlayer);
        if (playerCount >= 2) allPlayers.Add(greenPlayer);
        if (playerCount >= 3)
        {
            allPlayers.Remove(greenPlayer);
            allPlayers.Add(redPlayer); 
            allPlayers.Add(greenPlayer); 
        } 
        if (playerCount == 4) allPlayers.Add(yellowPlayer);

        if (!allPlayers.Contains(redPlayer)) redPlayer.gameObject.SetActive(false);
        if (!allPlayers.Contains(greenPlayer)) greenPlayer.gameObject.SetActive(false);
        if (!allPlayers.Contains(yellowPlayer)) yellowPlayer.gameObject.SetActive(false);

        foreach (Player player in allPlayers)
        {
            player.InitializePlayerPath();
        }

        currentPlayer = allPlayers[0];
        currentTurnText.text = "Current Turn : " + currentPlayer.playerType.ToString();
        currentTurnText.color = currentPlayer.playerColor;
        SetDicePositions(currentPlayer);
        Debug.Log($"{playerCount} players initialized for the game.");
        GameStateManager.Instance.SetState(GameState.Game);
    }

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
        diceRoll.RollDiceWithCallback((dice1, dice2) =>
        {
            dice1Result = dice1;
            dice2Result = dice2;
            diceResultSum = dice1Result + dice2Result;
            Debug.Log($"Dice Results: {dice1Result}, {dice2Result} : Sum: {diceResultSum}");
            availableDiceResults = new List<int> { dice1Result, dice2Result };
            MainDiceDeactive();

            if(!currentPlayer.isBot)
                ProcessDiceRoll();
            else
                ProcessDiceRollBot();
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
                if(!pawn.isInPlay && PawnsInPlay() == 0 && !pawn.mainPlayer.isBot)
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
            }
        }

        bool anyPawnInPlay = false;
        foreach (Pawn pawn in currentPlayer.GetComponentsInChildren<Pawn>())
        {
            if(pawn.isInPlay && !pawn.mainPlayer.isBot)
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
            EndTurn();
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

        if(CheckVictoryCondition() == true)
        {
           return; 
        }
        currentPlayerIndex = (currentPlayerIndex + 1) % allPlayers.Count;
        currentPlayer = allPlayers[currentPlayerIndex];

        SetDicePositions(currentPlayer);
        
        currentTurnText.text = "Current Turn : " + currentPlayer.playerType.ToString();
        currentTurnText.color = currentPlayer.playerColor;
        // diceButton.interactable = !currentPlayer.isPlayerAI;

        if(currentPlayer.isBot)
        {
            StartCoroutine(StartBotTurn());
        }
        else
        {
            mainDice.gameObject.GetComponent<Collider2D>().enabled = true;
        }
        // diceButton.interactable = true;
    }

    public void RemoveDiceResult(int diceValue)
    {
        availableDiceResults.Remove(diceValue);
    }
    
    #region BOT
    private IEnumerator StartBotTurn()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log($"AI Player {currentPlayer.name}'s turn starts.");
        RollDice();
        yield return new WaitForSeconds(2f);
    }

    private void ProcessDiceRollBot()
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
                if(!pawn.isInPlay && PawnsInPlay() == 0 && pawn.mainPlayer.isBot)
                {
                    pawn.EnterBoard();
                    DisablePawnSelection();
                    Invoke("RollDice", 2f);
                    return;
                }
            }
        }

        foreach (Pawn pawn in currentPlayer.GetComponentsInChildren<Pawn>())
        {
            if(pawn.isInPlay && pawn.mainPlayer.isBot)
            {
                BotMovement(pawn);
                EndTurn();
            }   
        }
    }

    private void BotMovement(Pawn pawn)
    {
        if (currentPlayer != pawn.mainPlayer)
        {
            Debug.Log($"Cannot move pawn: {name}, not the current player's turn.");
            return;
        }
        if(!pawn.isInPlay && (dice1Result == 5
                        || dice2Result == 5
                        || diceResultSum == 5))
        {
            pawn.EnterBoard();
            DisablePawnSelection();
            availableDiceResults.Clear();
            MainDiceActive();
            return;
        }

        List<int> availableResults = availableDiceResults;
        if(availableResults.Count > 0 && pawn.isInPlay)
        {
            Debug.Log($"Single tap detected. Moving pawn {name} with dice result: {availableResults[0]}");
            OnPawnSelected(pawn, availableResults[0]);
        }
        else
        {
            Debug.Log($"No available dice results for pawn {name}.");
        }

        if(pawn.isInPlay && availableDiceResults.Count > 0 && pawn.mainPlayer.isBot)
        {
            int steps = availableDiceResults[UnityEngine.Random.Range(0, availableDiceResults.Count)];
            OnPawnSelected(pawn, steps);
        }
    }

    private Pawn GetPawnToEnterBoard(int diceValue)
    {
        foreach (Pawn pawn in currentPlayer.GetComponentsInChildren<Pawn>())
        {
            if (!pawn.isInPlay && diceValue == 5)
            {
                return pawn;
            }
        }
        return null;
    }

    private Pawn GetBestPawnToMove(int diceValue)
    {
        Pawn bestPawn = null;
        int maxProgress = -1;

        foreach (Pawn pawn in currentPlayer.GetComponentsInChildren<Pawn>())
        {
            if (pawn.isInPlay)
            {
                int targetPositionIndex = pawn.currentPositionIndex + diceValue;

                if (targetPositionIndex < currentPlayer.playerPath.Count)
                {
                    int progress = targetPositionIndex;
                    if (progress > maxProgress)
                    {
                        maxProgress = progress;
                        bestPawn = pawn;
                    }
                }
            }
        }
        return bestPawn;
    }
    #endregion

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

    private bool CheckVictoryCondition()
    {
        // bool allPawnsAtVictoruPosition = false;
        int pawnsAtFinishLine =  0;
        foreach (Pawn pawn in currentPlayer.GetComponentsInChildren<Pawn>())
        {
            if (pawn.currentNode != null && pawn.currentNode.transform == currentPlayer.victoryPosition)
            {
                pawnsAtFinishLine ++;
                // if(pawn.GetComponent<Collider2D>().enabled)
                // {
                //     pawn.DisableMovementInteraction();
                // }
                // else
                //     allPawnsAtVictoruPosition = true;
            }
        }
        Debug.Log("PAWWWWWNSSSS at Finish: " + pawnsAtFinishLine);
        if (pawnsAtFinishLine == 4)
        {
            Debug.Log($"{currentPlayer} has won the game!");
            hasWon = true;
            //some game end logic
            GameEnd();
            return true;
        }
        return false;
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
        GameFinished();
    }
    
    private void GameFinished()
    {
        GameStateManager.Instance.SetState(GameState.Finish);
    }
}
