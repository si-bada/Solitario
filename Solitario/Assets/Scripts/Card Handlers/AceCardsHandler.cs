using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AceCardsHandler : MonoBehaviour
{
    [SerializeField]
    private Transform[] _acePilesTransform = null;

    [SerializeField]
    private Transform _landscapeParent = null;
    [SerializeField]
    private Transform _portraitParent = null;

    private void Start()
    {
        InitEvents();
    }

    #region Events Handlers
    private void InitEvents()
    {
        EventsManager.Instance.OnOrientationChanged.AddListener(HandleEventDeviceOrientationChange);
    }

    private void HandleEventDeviceOrientationChange(DeviceOrientation deviceOrientation)
    {
        switch (deviceOrientation)
        {
            case DeviceOrientation.Portrait:
                for (int i = 0; i < _acePilesTransform.Length; i++)
                {
                    Transform tablePileTransform = _acePilesTransform[i];
                    tablePileTransform.SetParent(_portraitParent);
                }
                break;

            case DeviceOrientation.PortraitUpsideDown:
                for (int i = 0; i < _acePilesTransform.Length; i++)
                {
                    Transform tablePileTransform = _acePilesTransform[i];
                    tablePileTransform.SetParent(_portraitParent);
                }
                break;

            case DeviceOrientation.LandscapeLeft:
                for (int i = 0; i < _acePilesTransform.Length; i++)
                {
                    Transform tablePileTransform = _acePilesTransform[i];
                    tablePileTransform.SetParent(_landscapeParent);
                }
                break;

            case DeviceOrientation.LandscapeRight:
                for (int i = 0; i < _acePilesTransform.Length; i++)
                {
                    Transform tablePileTransform = _acePilesTransform[i];
                    tablePileTransform.SetParent(_landscapeParent);
                }
                break;
        }
    }
    #endregion
}
