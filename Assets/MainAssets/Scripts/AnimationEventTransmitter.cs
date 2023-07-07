using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventTransmitter : MonoBehaviour
{
    public event Action OnStandUpEnd;

    public void ProcessOnGotUpevent() => OnStandUpEnd?.Invoke();
}
