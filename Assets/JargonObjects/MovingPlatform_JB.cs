using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform_JB : JargonBlock
{
    [SerializeField] private Transform _t;
    [SerializeField] private Transform _tInit;
    [SerializeField] private Transform _tFinal;
    [SerializeField] private float moveDuration;
    private bool readyToMove = true;
    private bool flipSwitch = true;

    private void Awake()
    {
        isCollidable = true;
    }
    
    void Update()
    {
        if (readyToMove && flipSwitch)
        {
            StartCoroutine(SlowMovePosition(_tInit, _tFinal, moveDuration));
        }
        else if (readyToMove && !flipSwitch)
        {
            StartCoroutine(SlowMovePosition(_tFinal, _tInit, moveDuration));
        }
    }

    private IEnumerator SlowMovePosition(Transform initialPos, Transform finalPos, float duration)
    {
        readyToMove = false;
        flipSwitch = !flipSwitch;
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            t = t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
            _t.position = Vector3.Lerp(initialPos.position, finalPos.position, t);

            yield return null;
        }
        _t.position = finalPos.position;
        readyToMove = true;
    }
}
