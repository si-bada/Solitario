using UnityEngine;

public class PilesManager : MonoBehaviour
{

    #region static
    public static PilesManager Instance;
    #endregion

    #region Script Parameters
    public PileHandler[] TablePiles;
    public PileHandler[] AcesPiles;
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
    #endregion
}
