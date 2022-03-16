using UnityEngine;

public class UIWindow : MonoBehaviour
{
    #region Script Parameters
    public WindowUI window = WindowUI.Nothing;
    public GameObject LandscapeMainPanel = null;
    public GameObject PortraitMainPanel = null;
    public Settings PortraitSettings;
    #endregion

    #region Unity Methods
    public void Start()
    {
        EventsManager.Instance.OnOrientationChanged.AddListener((ScreenOrientation) =>
        {
            if (LandscapeMainPanel.activeSelf || PortraitMainPanel.activeSelf)
            {
                switch (GameManager.Instance.CurrentDeviceOrientation)
                {
                    case ScreenOrientation.Portrait:

                        LandscapeMainPanel.SetActive(false);
                        PortraitMainPanel.SetActive(true);
                        break;

                    case ScreenOrientation.PortraitUpsideDown:

                        LandscapeMainPanel.SetActive(false);
                        PortraitMainPanel.SetActive(true);
                        break;

                    case ScreenOrientation.LandscapeLeft:

                        PortraitMainPanel.SetActive(false);
                        LandscapeMainPanel.SetActive(true);
                        break;

                    case ScreenOrientation.LandscapeRight:

                        PortraitMainPanel.SetActive(false);
                        LandscapeMainPanel.SetActive(true);
                        break;
                }
            }         
        });
        if(PortraitSettings != null)
        {
            PortraitSettings.InitSettings();
        }
    }
    #endregion
    #region Methods

    public void OpenWindow()
    {
        switch (GameManager.Instance.CurrentDeviceOrientation)
        {
            case ScreenOrientation.Portrait:

                PortraitMainPanel.SetActive(true);
                break;

            case ScreenOrientation.PortraitUpsideDown:

                PortraitMainPanel.SetActive(true);
                break;

            case ScreenOrientation.LandscapeLeft:

                LandscapeMainPanel.SetActive(true);
                break;

            case ScreenOrientation.LandscapeRight:

                LandscapeMainPanel.SetActive(true);
                break;

            default:

                PortraitMainPanel.SetActive(true);
                break;
        }
    }

    public void OnCloseWindowButton()
    {
        switch (window)
        {
            case WindowUI.Pause:
                GameManager.Instance.UpdateGameState(GameState.Started);
                break;
        }

        AudioManager.Instance.Play("Click_SFX");
        LandscapeMainPanel.SetActive(false);
        PortraitMainPanel.SetActive(false);
    }

    public void OnClickBackToMenu()
    {
        GameManager.Instance.OnHomeSceneButton();
    }

    public void OnClickQuitGame()
    {
        Application.Quit();
    }
    #endregion
}
