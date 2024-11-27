using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRoll : MonoBehaviour
{
    [Header("Dice GameObjects")]
    [SerializeField] private GameObject dice1;
    [SerializeField] private GameObject dice2;

    [Header("Dice Sprites")]
    [SerializeField] private Sprite[] diceSprites;
    private SpriteRenderer diceRenderer1;
    private SpriteRenderer diceRenderer2;

    [Header("Dice Animation Settings")]
    [SerializeField] private float animationTime = 2.0f;
    [SerializeField] private float diceSpriteChangeTime = 0.2f;

    private void Start()
    {
        diceRenderer1 = dice1.GetComponent<SpriteRenderer>();
        diceRenderer2 = dice2.GetComponent<SpriteRenderer>();
    }

    public void RollDice()
    {
        StartCoroutine(RollDiceAnimation());
    }

    private IEnumerator RollDiceAnimation()
    {
        float elapsedTime = 0f;

        while(elapsedTime < animationTime)
        {
            diceRenderer1.sprite = diceSprites[Random.Range(0, diceSprites.Length)];
            diceRenderer2.sprite = diceSprites[Random.Range(0, diceSprites.Length)];

            yield return new WaitForSeconds(diceSpriteChangeTime);
            elapsedTime += diceSpriteChangeTime;
        }

        int diceResult1 = Random.Range(1, 7);
        int diceResult2 = Random.Range(1, 7);

        diceRenderer1.sprite = diceSprites[diceResult1 - 1];
        diceRenderer2.sprite = diceSprites[diceResult2 - 1];
    }
}
