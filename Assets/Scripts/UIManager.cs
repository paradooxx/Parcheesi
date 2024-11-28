using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject selectPlayerPanel;
    [SerializeField] private GameObject gameFinishedPanel;

    private GameObject activePanel;
    private List<GameObject> unactivePanels;

    private void OnEnable() => GameStateManager.OnStateChange += ManagePanels;

    private void OnDisable() => GameStateManager.OnStateChange -= ManagePanels;

    private void Awake()
    {
        unactivePanels = new List<GameObject> { mainMenuPanel, selectPlayerPanel };
        HideAllPanels();
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
                break;
            case GameState.Finish:
                SetActivePanel(gameFinishedPanel);
                break;
            case GameState.Game:
                HideAllPanels();
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

    //use in UI to get to main menu
    public void CloseButton()
    {
        SetActivePanel(mainMenuPanel);
    }
}
