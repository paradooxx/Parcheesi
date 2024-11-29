using UnityEngine;
using UnityEngine.Events;

public class DiceRollClick : MonoBehaviour
{
    [SerializeField] private GameObject border;

    public UnityEvent onButtonClicked;

    private Collider2D diceCollider;

    private void Start()
    {
        diceCollider = GetComponent<Collider2D>();
    }

    private void OnMouseDown()
    {
        onButtonClicked?.Invoke();
    }

    private void Update()
    {
        if(diceCollider.enabled)
        {
            border.SetActive(true);
        }
        else
        {
            border.SetActive(false);
        }
    }
}
