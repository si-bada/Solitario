using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    #region Script Parameters
    public Toggle Music;
    public Toggle SFX;
    public Toggle Draw1;
    public Toggle Draw3;
    public Button RemoveAds;
    #endregion

    #region Unity Methods

    private void Start()
    {
        InitSettings();
    }
    #endregion

    #region Methods
    public void InitSettings()
    {
        if(AudioManager.Instance != null)
        {
            Music.onValueChanged.AddListener((value) => AudioManager.Instance.MuteMusic(value));
            SFX.onValueChanged.AddListener((value) => AudioManager.Instance.MuteSFX(value));
        }
        Draw1.onValueChanged.AddListener((value) => OnToggleDrawOne(value));
        Draw3.onValueChanged.AddListener((value) => OnToggleDrawThree(value));
        if (PlayerPrefs.HasKey("Music"))
        {
            Music.isOn = PlayerPrefs.GetInt("Music") == 0 ? true : false;
        }

        if (PlayerPrefs.HasKey("SFX"))
        {
            SFX.isOn = PlayerPrefs.GetInt("SFX") == 0 ? true : false;
        }
        if (PlayerPrefs.HasKey("DrawMode"))
        {
            Draw1.isOn = PlayerPrefs.GetInt("DrawMode") == 1 ? true : false;
        }
        if (PlayerPrefs.HasKey("DrawMode"))
        {
            Draw3.isOn = PlayerPrefs.GetInt("DrawMode") == 3 ? true : false;
        }
    }
    public void OnToggleDrawOne(bool value)
    {
        if (value)
        {
            PlayerPrefs.SetInt("DrawMode", 1);
            GameManager.Instance.DrawMode = DrawMode.One;
        }
    }
    public void OnToggleDrawThree(bool value)
    {
        if (value)
        {
            PlayerPrefs.SetInt("DrawMode", 3);
            GameManager.Instance.DrawMode = DrawMode.Three;
        }
    }
    #endregion
}
