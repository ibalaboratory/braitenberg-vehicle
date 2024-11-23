using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleController : MonoBehaviour
{
    public void OnTimeScaleChanged(float timeScale) {
        Time.timeScale = timeScale;
    }
}
