using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    public Transform startPosition, endPosition;

    private void Start() {
        StartCoroutine(MoveToTarget());
    }

    IEnumerator MoveToTarget() {
        float speed = 2f;

        var i = 0f;

        while(i < 1f) {
            i += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(startPosition.position, endPosition.position, i);
            yield return null;
        }

        StartCoroutine(MoveToStart());
    }

    IEnumerator MoveToStart() {
        float speed = 2f;

        var i = 0f;

        while (i < 1f) {
            i += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(endPosition.position, startPosition.position, i);
            yield return null;
        }

        StartCoroutine(MoveToTarget());
    }


}
