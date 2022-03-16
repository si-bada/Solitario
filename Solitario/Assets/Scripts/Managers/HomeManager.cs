using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    #region Static
    public static HomeManager Instance;
    #endregion

    #region Script Parameters
    public CardUI[] CardsPortrait;
    public CardUI[] CardsLandscape;
    public GameObject[] GoldSymbolsPortrait;
    public GameObject[] GoldSymbolsLanscape;
    public Button StartGame;
    public GameObject HomePortrait;
    public GameObject HomeLandscape;
    #endregion

    #region Fields
    private CardUI[] Cards;
    private GameObject[] GoldSymbols;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    private void Start()
    {
        StartUI();

        StartGame.onClick.AddListener(() => GameManager.Instance.OnGameSceneButton());
    }
    #endregion

    #region Methods
    public void StartUI()
    {
#if UNITY_EDITOR
        Debug.LogWarning("Device Orientation Check does not work In Editor Mode - PORTRAIT MODE BY DEFAULT");
        HomePortrait.SetActive(true);
        HomeLandscape.SetActive(false);
        Cards = CardsPortrait;
        GoldSymbols = GoldSymbolsPortrait;
#else
        if (GameManager.Instance.CurrentDeviceOrientation == ScreenOrientation.Portrait || GameManager.Instance.CurrentDeviceOrientation == ScreenOrientation.PortraitUpsideDown)
        {
            HomePortrait.SetActive(true);
            HomeLandscape.SetActive(false);
            Cards = CardsPortrait;
            GoldSymbols = GoldSymbolsPortrait;
        }
        else
        {
            HomePortrait.SetActive(false);
            HomeLandscape.SetActive(true);
            Cards = CardsLandscape;
            GoldSymbols = GoldSymbolsLanscape;
        }
#endif

        StartCoroutine(FlipCards());
    }
    #endregion

    #region Implementations
    private IEnumerator FlipCards()
    {
        for (int i = 0; i < Cards.Length; i++)
        {
            CardUI cardui = Cards[i];

            cardui.FlipCard(CardSide.Front);

            yield return new WaitForSeconds(0.2f);
        }

        for (int i = 0; i < GoldSymbols.Length; i++)
        {
            GameObject goldenSuit = GoldSymbols[i];

            iTween.ScaleTo(goldenSuit, Vector3.one, 0.2f);

            yield return new WaitForSeconds(0.1f);

            iTween.PunchScale(goldenSuit, Vector3.one * 1.5f, 1.5f);

            yield return new WaitForSeconds(0.2f);
        }
    }
    #endregion
}
