using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject selectPlayerPanel;
    [SerializeField] private GameObject gameFinishedPanel;
    [SerializeField] private GameObject selectBotPanel;

    [SerializeField] private GameObject mainBg;

    [SerializeField] private TMP_Text winText;

    private GameObject activePanel;
    private List<GameObject> unactivePanels;

    private void OnEnable() => GameStateManager.OnStateChange += ManagePanels;

    private void OnDisable() => GameStateManager.OnStateChange -= ManagePanels;

    private void Awake()
    {
        unactivePanels = new List<GameObject> { mainMenuPanel, selectPlayerPanel, gameFinishedPanel, selectBotPanel };
        HideAllPanels();
        mainBg.SetActive(true);
    }

    private void ManagePanels(GameState state)
    {
        switch(state)
        {
            case GameState.MainMenu:
                SetActivePanel(mainMenuPanel);
                break;
            case GameState.SelectPlayer:
                SetActivePanel(selectPlayerPanel);
                mainBg.SetActive(false);
                break;
            case GameState.SelectBot:
                SetActivePanel(selectBotPanel);
                mainBg.SetActive(false);
                break;
            case GameState.Finish:
                SetActivePanel(gameFinishedPanel);
                mainBg.SetActive(false);
                winText.text = "Winner: " + GameManager.Instance.currentPlayer.playerType.ToString();
                break;
            case GameState.Game:
                HideAllPanels();
                mainBg.SetActive(false);
                break;
        }
    }

    private void DisableActivePanel()
    {
        if(activePanel == null) return;
        unactivePanels.Add(activePanel);
        activePanel.SetActive(false);
        activePanel = null;
    }

    private void SetActivePanel(GameObject panel)
    {
        DisableActivePanel();
        unactivePanels.Remove(panel);
        activePanel = panel;
        panel.SetActive(true);
    }

    private void HideAllPanels()
    {
        DisableActivePanel();
        foreach (GameObject panel in unactivePanels)
        {
            panel.SetActive(false);
        }
    }

    public void PlayervPlayer()
    {
        SetActivePanel(selectPlayerPanel);
    }

    public void PlayervComputer()
    {
        SetActivePanel(selectBotPanel);
    }


    //use in UI to get to main menu
    public void CloseButton()
    {
        SetActivePanel(mainMenuPanel);
    }
}
