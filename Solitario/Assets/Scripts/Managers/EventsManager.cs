using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventsManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance reference
    /// </summary>
    public static EventsManager Instance;

    public Events.EventGameStateChanged OnGameStateChanged;
    public Events.EventOrientationChanged OnOrientationChanged;
    public Events.EventShuffleEnded OnShuffleEnded;
    public Events.EventCardsDealed OnCardsDealed;
    public Events.EventCardDropped OnCardDropped;
    public Events.EventCardPointerEnter OnCardPointerEnter;
    public Events.EventCardPointerExit OnCardPointerExit;
    public Events.EventPilePointerEnter OnPilePointerEnter;
    public Events.EventPilePointerExit OnPilePointerExit;
    public Events.EventCardMove OnCardMove;
    public Events.EventUndoCardMove OnUndoCardMove;
    public Events.EventCardDragging OnCardDragging;
    public Events.EventCardFailMove OnCardFailMove;
    public Events.EventPick OnPick;
    public Events.EventUndoPick OnUndoPick;
    public Events.EventUndoDraw OnUndoDraw;
    public Events.EventDeckEmpty OnDeckEmpty;
    public Events.EventReset OnReset;
    public Events.EventUndoReset OnUndoReset;
    public Events.EventScore OnScore;
    public Events.EventUndoScore OnUndoScore;
    public Events.EventCommand OnCommand;
    public Events.EventGameWon OnGameWon;

    private void Awake()
    {
        if (Instance == null)
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
        DontDestroyOnLoad(this);
    }
}

public class Events
{
    [System.Serializable] public class EventGameStateChanged : UnityEvent<GameState> { };
    [System.Serializable] public class EventOrientationChanged : UnityEvent<DeviceOrientation> { };
    [System.Serializable] public class EventShuffleEnded : UnityEvent<List<CardData>> { };
    [System.Serializable] public class EventCardsDealed : UnityEvent<List<CardData>> { };
    [System.Serializable] public class EventCardDragging : UnityEvent<CardUI> { };
    [System.Serializable] public class EventCardDropped : UnityEvent { };
    [System.Serializable] public class EventCardPointerEnter : UnityEvent<CardUI> { };
    [System.Serializable] public class EventCardPointerExit : UnityEvent { };
    [System.Serializable] public class EventPilePointerEnter : UnityEvent<PileHandler> { };
    [System.Serializable] public class EventPilePointerExit : UnityEvent { };
    [System.Serializable] public class EventCardMove : UnityEvent<CardUI, Transform> { };
    [System.Serializable] public class EventUndoCardMove : UnityEvent<CardUI, Transform> { };
    [System.Serializable] public class EventCardFailMove : UnityEvent<CardUI> { };
    [System.Serializable] public class EventPick : UnityEvent<CardUI> { }
    [System.Serializable] public class EventUndoPick : UnityEvent<CardUI, int> { }
    [System.Serializable] public class EventUndoDraw : UnityEvent { }
    [System.Serializable] public class EventDeckEmpty : UnityEvent { }
    [System.Serializable] public class EventReset : UnityEvent { }
    [System.Serializable] public class EventUndoReset : UnityEvent { }
    [System.Serializable] public class EventScore : UnityEvent<int> { }
    [System.Serializable] public class EventUndoScore : UnityEvent<int> { }
    [System.Serializable] public class EventCommand : UnityEvent { };
    [System.Serializable] public class EventGameWon : UnityEvent { };
}

