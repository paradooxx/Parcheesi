using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] private GameState defaultGameState;

    public static GameStateManager Instance { get; private set; }
    public static GameState currentGameState { get; private set; }

    public static event UnityAction<GameState> OnStateChange;

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
        SetState(defaultGameState);
    }

    public void SetState(GameState gameState)
    {
        currentGameState = gameState;
        OnStateChange?.Invoke(gameState);
    }
} 

public enum GameState
{
    MainMenu,
    SelectPlayer,
    SelectBot,
    Game,
    Finish
}
