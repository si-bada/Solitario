using UnityEngine;

public class AceCardsHandler : MonoBehaviour
{
    #region Script Parameters
    public Transform[] AcePilesTransform = null;
    public Transform LandscapeParent = null;
    public Transform PortraitParent = null;
    #endregion

    #region Unity Methods
    private void Start()
    {
        InitEvents();
    }
    #endregion

    #region Implementations
    private void InitEvents()
    {
        EventsManager.Instance.OnOrientationChanged.AddListener(HandleEventScreenOrientationChange);
    }

    private void HandleEventScreenOrientationChange(ScreenOrientation ScreenOrientation)
    {
        switch (ScreenOrientation)
        {
            case ScreenOrientation.Portrait:
                for (int i = 0; i < AcePilesTransform.Length; i++)
                {
                    Transform tablePileTransform = AcePilesTransform[i];
                    tablePileTransform.SetParent(PortraitParent);
                }
                break;

            case ScreenOrientation.PortraitUpsideDown:
                for (int i = 0; i < AcePilesTransform.Length; i++)
                {
                    Transform tablePileTransform = AcePilesTransform[i];
                    tablePileTransform.SetParent(PortraitParent);
                }
                break;

            case ScreenOrientation.LandscapeLeft:
                for (int i = 0; i < AcePilesTransform.Length; i++)
                {
                    Transform tablePileTransform = AcePilesTransform[i];
                    tablePileTransform.SetParent(LandscapeParent);
                }
                break;

            case ScreenOrientation.LandscapeRight:
                for (int i = 0; i < AcePilesTransform.Length; i++)
                {
                    Transform tablePileTransform = AcePilesTransform[i];
                    tablePileTransform.SetParent(LandscapeParent);
                }
                break;
        }
    }
    #endregion
}
