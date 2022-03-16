using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Press L for Landscape Mode")]
    [Header("Press P for Portrait Mode")]

    #region static
    public static GameManager Instance;
    private static float deviceOrientationCheckDelay = 0.5f;
    private static bool keepChecking = true;
    #endregion

    #region getters
    public GameState GameState
    {
        get
        {
            return currentGameState;
        }
    }
    public DrawMode DrawMode
    {
        get
        {
            return currentDrawMode;
        }
        set
        {
            currentDrawMode = value;
        }
    }
    public ScreenOrientation CurrentDeviceOrientation
    {
        get
        {
            return currentDeviceOrientation;
        }
    }
    public MoveSystem MoveSystem
    {
        get
        {
            return moveSystem;
        }
    }
    public CommandSystem CommandSystem
    {
        get
        {
            return commandSystem;
        }
    }
    #endregion

    #region Fields
    private GameState currentGameState = GameState.Home;
    private DrawMode currentDrawMode = DrawMode.One;
    private ScreenOrientation currentDeviceOrientation;
    private int completedAcePileCount = 0;
    private MoveSystem moveSystem;
    private CommandSystem commandSystem;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
        currentDeviceOrientation = Screen.orientation;

    }
    private void Start()
    {
        DontDestroyOnLoad(this);

        HandleUnityLog();
        InitSystems();

        StartCoroutine(CheckForDeviceOrientationChange(0.5f));
    }
    private void Update()
    {
        // Editor debug inputs
        if (Input.GetKeyDown(KeyCode.L))
        {
            EventsManager.Instance.OnOrientationChanged.Invoke(ScreenOrientation.LandscapeRight);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            EventsManager.Instance.OnOrientationChanged.Invoke(ScreenOrientation.Portrait);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            UpdateCompletedAcePileCount(OperationType.Add);
        }
    }
    private void OnDestroy()
    {
        keepChecking = false;
    }
    #endregion

    #region Methods
    public void UndoCommand()
    {
        commandSystem.UndoCommand();
    }
    public void UpdateCompletedAcePileCount(OperationType operationType)
    {
        switch (operationType)
        {
            case OperationType.Add:
                completedAcePileCount++;
                break;

            case OperationType.Remove:
                completedAcePileCount--;
                break;
        }

        if(completedAcePileCount >= 4)
        {
            EventsManager.Instance.OnGameWon.Invoke();
            AudioManager.Instance.Play("Victory");
        }
    }
    public void OnHomeSceneButton()
    {
        AudioManager.Instance.Play("Click_SFX");
        SceneManager.LoadScene(Scene.Home.ToString());
    }
    public void OnGameSceneButton()
    {
        AudioManager.Instance.Play("Click_SFX");
        SceneManager.LoadScene(Scene.Game.ToString());
    }
    public void UpdateGameState(GameState gameState)
    {
        EventsManager.Instance.OnGameStateChanged.Invoke(gameState);

        currentGameState = gameState;
    }

    #endregion

    #region Implementations
    private void InitSystems()
    {
        moveSystem = new MoveSystem();
        commandSystem = new CommandSystem();

    }
    private IEnumerator CheckForDeviceOrientationChange(float delay)
    {
        yield return new WaitForSeconds(delay);

        currentDeviceOrientation = Screen.orientation;

        while (keepChecking)
        {
            // Check for an Orientation Change
            if (currentDeviceOrientation != Screen.orientation)
            {
                currentDeviceOrientation = Screen.orientation;
                EventsManager.Instance.OnOrientationChanged.Invoke(currentDeviceOrientation);
                if (HomeManager.Instance != null)
                {
                    HomeManager.Instance.StartUI();
                }
            }

            yield return new WaitForSeconds(deviceOrientationCheckDelay);
        }
    }
    //editor logs
    private void HandleUnityLog()
    {
//#if UNITY_EDITOR
//        Debug.unityLogger.logEnabled = true;
//#else
//        Debug.unityLogger.logEnabled = false;
//#endif
    }
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode loadSceneMode)
    {
        StartCoroutine(CheckForDeviceOrientationChange(0.1f));
    }
    #endregion
}
