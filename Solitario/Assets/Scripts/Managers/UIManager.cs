using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    #region static
    public static UIManager Instance;
    #endregion

    #region getters
    public int Score
    {
        get
        {
            return currentScore;
        }
    }
    #endregion

    #region Script Parameters
    //to change orientation
    public Transform[] MenuButtons;
    public Transform[] TextLabels;

    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI TimerText;
    public TextMeshProUGUI MovesText;

    public UIWindow[] UIWindows; 

    [Header("Landscape Parents")]
    public Transform TextInfoLandscape;
    public Transform UiButtonsLandscape;

    [Header("Portrait Parents")]
    public Transform TextInfoPortrait;
    public Transform UiButtonsPortrait;
    #endregion

    #region Fields
    private int currentScore = 0;
    private float currentTimer = 0;
    private int currentMoves = 0;
    private bool pauseTimer = true;
    private bool canUndo = true;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        InitEvents();
    }

    private void Update()
    {
        HandleTimer();
    }
    #endregion

    #region Methods
    public void OnPauseButton()
    {
        OpenUIWindow(WindowUI.Pause);
        GameManager.Instance.UpdateGameState(GameState.Pause);
        AudioManager.Instance.Play("Click_SFX");
    }

    public void OnUndoButton()
    {
        if (canUndo)
        {
            GameManager.Instance.UndoCommand();
            Invoke(nameof(ResetUndoButton), 0.43f);
            canUndo = false;
        }
    }

    public void OnRestartButton()
    {
        GameManager.Instance.OnGameSceneButton();
    }

    public void OnHomeButton()
    {
        GameManager.Instance.OnHomeSceneButton();
    }

    public void OpenUIWindow(WindowUI window)
    {
        for (int i = 0; i < UIWindows.Length; i++)
        {
            if (UIWindows[i].window == window)
            {
                UIWindows[i].OpenWindow();
                return;
            }
        }
    }
    public static string GetTimerString(float timer)
    {
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer - minutes * 60);
        string timerString = string.Format("{0:00}:{1:00}", minutes, seconds);

        return timerString;
    }
    #endregion

    #region Implementations
    private void HandleTimer()
    {
        if (pauseTimer)
            return;

        currentTimer += Time.deltaTime;

        TimerText.text = "TIME\n" + GetTimerString(currentTimer);
    }
    private void InitEvents()
    {
        EventsManager.Instance.OnCardsDealed.AddListener((List<CardData> cardDataList) =>
        {
            pauseTimer = false;
            GameManager.Instance.UpdateGameState(GameState.Started);
        });

        EventsManager.Instance.OnCommand.AddListener(() =>
        {
            currentMoves++;
            MovesText.text = "MOVES\n" + currentMoves.ToString();
        });

        EventsManager.Instance.OnScore.AddListener((score) =>
        {
            currentScore += score;

            if (currentScore < 0)
                currentScore = 0;

            ScoreText.text = "POINTS\n" + currentScore.ToString();
        });

        EventsManager.Instance.OnUndoScore.AddListener((score) =>
        {
            currentScore -= score;

            if (currentScore < 0)
                currentScore = 0;

            ScoreText.text = "POINTS\n" + currentScore.ToString();
        });

        EventsManager.Instance.OnGameStateChanged.AddListener((gameState) =>
        {
            switch (gameState)
            {
                case GameState.Started:
                    pauseTimer = false;
                    break;
                case GameState.Pause:
                    pauseTimer = true;
                    break;
            }
        });

        EventsManager.Instance.OnOrientationChanged.AddListener((ScreenOrientation) =>
        {
            switch (ScreenOrientation)
            {
                case ScreenOrientation.Portrait:

                    TextInfoLandscape.GetComponent<Image>().enabled = false;
                    TextInfoPortrait.GetComponent<Image>().enabled = true;

                    for (int i = 0; i < TextLabels.Length; i++)
                    {
                        Transform infoTextLabel = TextLabels[i];
                        infoTextLabel.SetParent(TextInfoPortrait);
                    }
                    for (int i = 0; i < MenuButtons.Length; i++)
                    {
                        Transform uiButton = MenuButtons[i];
                        uiButton.SetParent(UiButtonsPortrait);
                    }
                    break;

                case ScreenOrientation.PortraitUpsideDown:

                    TextInfoLandscape.GetComponent<Image>().enabled = false;
                    TextInfoPortrait.GetComponent<Image>().enabled = true;

                    for (int i = 0; i < TextLabels.Length; i++)
                    {
                        Transform infoTextLabel = TextLabels[i];
                        infoTextLabel.SetParent(TextInfoPortrait);
                    }
                    for (int i = 0; i < MenuButtons.Length; i++)
                    {
                        Transform uiButton = MenuButtons[i];
                        uiButton.SetParent(UiButtonsPortrait);
                    }
                    break;

                case ScreenOrientation.LandscapeLeft:

                    TextInfoLandscape.GetComponent<Image>().enabled = true;
                    TextInfoPortrait.GetComponent<Image>().enabled = false;

                    for (int i = 0; i < TextLabels.Length; i++)
                    {
                        Transform infoTextLabel = TextLabels[i];
                        infoTextLabel.SetParent(TextInfoLandscape);
                    }
                    for (int i = 0; i < MenuButtons.Length; i++)
                    {
                        Transform uiButton = MenuButtons[i];
                        uiButton.SetParent(UiButtonsLandscape);
                    }
                    break;

                case ScreenOrientation.LandscapeRight:

                    TextInfoLandscape.GetComponent<Image>().enabled = true;
                    TextInfoPortrait.GetComponent<Image>().enabled = false;

                    for (int i = 0; i < TextLabels.Length; i++)
                    {
                        Transform infoTextLabel = TextLabels[i];
                        infoTextLabel.SetParent(TextInfoLandscape);
                    }
                    for (int i = 0; i < MenuButtons.Length; i++)
                    {
                        Transform uiButton = MenuButtons[i];
                        uiButton.SetParent(UiButtonsLandscape);
                    }
                    break;
            }
        });

        EventsManager.Instance.OnGameWon.AddListener((UnityEngine.Events.UnityAction)(() =>
        {
            OpenUIWindow(WindowUI.Win);
        }));
    }

    private void ResetUndoButton()
    {
        canUndo = true;
    }

    #endregion

}
