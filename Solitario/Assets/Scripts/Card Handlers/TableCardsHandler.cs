using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableCardsHandler : MonoBehaviour
{
    #region Sciprt Parameters
    public CardUI CardUIPrefab;
    public Transform DeckTransform;
    public Transform[] TablePilesTransform;
    public Transform LandscapeParent;
    public Transform PortraitParent;
    #endregion

    #region Unity Methods
    private void Start()
    {
        InitEvents(); 
    }
    #endregion

    #region Implementations
    private void InitCardsUI(List<CardData> cardsData)
    {
        StartCoroutine(SpawnCards(cardsData));
    }

    private IEnumerator SpawnCards(List<CardData> cardsData)
    {
        int currentRow = 0;

        // cycle each row (7)
        while (currentRow < 7)
        {
            // get the number of cards to spawn for each row, knowing the starting total (28)
            int cardsToInstantiate = 28 - (28 - (currentRow + 1));

            for (int i = 0; i < cardsToInstantiate; i++)
            {
                CardUI cardui = Instantiate(CardUIPrefab, DeckTransform);

                // Save the column transform reference inside each Card script
                Transform columnTransform = TablePilesTransform[currentRow];
                cardui.UpdateParent(columnTransform);

                GameObject spawnPosition = Instantiate(new GameObject("spawnPos", typeof(RectTransform)), columnTransform);
                spawnPosition.GetComponent<RectTransform>().sizeDelta = cardui.GetComponent<RectTransform>().sizeDelta;

                cardui.SetCardData(cardsData[0], CardArea.Table);

                // Remove the spawned card from the cards data list in order to let the DeckManager handle the remaining cards
                cardsData.RemoveAt(0);
           
                // Set the last spawned card to be facing its front
                if (i == cardsToInstantiate - 1)
                {
                    cardui.FlipCard(CardSide.Front);
                    cardui.EnableRaycast(true);
                }

                yield return new WaitForSeconds(0.01f);
                iTween.MoveTo(cardui.gameObject, spawnPosition.transform.position, 1.5f);
                yield return new WaitForSeconds(0.01f);
                cardui.transform.SetParent(spawnPosition.transform);

                AudioManager.Instance.PlayOneShot("SpawnCard_SFX");

                // Time to wait to next card to spawn
                yield return new WaitForSeconds(0.1f);
            }

            currentRow++;
        }

        EventsManager.Instance.OnCardsDealed.Invoke(cardsData);
    }

    private void InitEvents()
    {
        EventsManager.Instance.OnShuffleEnded.AddListener(HandleEventShuffleEnded);
        EventsManager.Instance.OnOrientationChanged.AddListener(HandleEventScreenOrientationChange);
    }

    private void HandleEventShuffleEnded(List<CardData> cardsData)
    {
        InitCardsUI(cardsData);
    }

    private void HandleEventScreenOrientationChange(ScreenOrientation ScreenOrientation)
    {
        switch (ScreenOrientation)
        {
            case ScreenOrientation.Portrait:
                for (int i = 0; i < TablePilesTransform.Length; i++)
                {
                    Transform tablePileTransform = TablePilesTransform[i];
                    tablePileTransform.SetParent(PortraitParent);
                }
                break;

            case ScreenOrientation.PortraitUpsideDown:
                for (int i = 0; i < TablePilesTransform.Length; i++)
                {
                    Transform tablePileTransform = TablePilesTransform[i];
                    tablePileTransform.SetParent(PortraitParent);
                }
                break;

            case ScreenOrientation.LandscapeLeft:
                for (int i = 0; i < TablePilesTransform.Length; i++)
                {
                    Transform tablePileTransform = TablePilesTransform[i];
                    tablePileTransform.SetParent(LandscapeParent);
                }
                break;

            case ScreenOrientation.LandscapeRight:
                for (int i = 0; i < TablePilesTransform.Length; i++)
                {
                    Transform tablePileTransform = TablePilesTransform[i];
                    tablePileTransform.SetParent(LandscapeParent);
                }
                break;
        }
    }
    #endregion
}


