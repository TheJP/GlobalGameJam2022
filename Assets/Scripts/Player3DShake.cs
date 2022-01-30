using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player3D))]
public class Player3DShake : MonoBehaviour
{
    [SerializeField] private Transform busModel;
    [SerializeField] private float shakeStriveSize = 10f;
    [SerializeField] private float shakeTime = 0.05f;

    private void Start() => GetComponent<Player3D>().TookDamage += Shake;
    private void Shake() => StartCoroutine(nameof(ShakeBus));

    private IEnumerator LinkEnumerators(params IEnumerator[] enumerators)
    {
        foreach (var enumerator in enumerators)
        {
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }
    }

    private IEnumerator ShakeBus()
    {
        return LinkEnumerators(
            ShakeBusBy(-shakeStriveSize),
            ShakeBusBy(2f * shakeStriveSize),
            ShakeBusBy(-2f * shakeStriveSize),
            ShakeBusBy(2f * shakeStriveSize),
            ShakeBusBy(-2f * shakeStriveSize),
            ShakeBusBy(shakeStriveSize)
        );
    }

    private IEnumerator ShakeBusBy(float move)
    {
        yield return null;
        var startTime = Time.time;
        var startPosition = busModel.position.x;
        while (Time.time - startTime < shakeTime)
        {
            var x = Mathf.Lerp(startPosition, startPosition + move, (Time.time - startTime) / shakeTime);
            busModel.position = new Vector3(x, busModel.position.y, busModel.position.z);
            yield return null;
        }
    }
}
