using UnityEngine;

public class GameManger : MonoBehaviour
{
    [SerializeField] DiceRoll diceRoll;

    public void RollDice()
    {
        int[] diceRollResults = diceRoll.RollDiceReturnResult();

        Debug.Log($"Dice Results: {diceRollResults[0]}, {diceRollResults[1]}"); 
    }
}
