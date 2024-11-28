using UnityEngine;
using UnityEngine.Events;

public class DiceRollClick : MonoBehaviour
{
    public UnityEvent onButtonClicked;

    private void OnMouseDown()
    {
        onButtonClicked?.Invoke();
    }
}
