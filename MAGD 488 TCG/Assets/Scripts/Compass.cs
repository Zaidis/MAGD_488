using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{


    /// <summary>
    /// Called when a new turn begins!
    /// </summary>
    public void TurnCompass() {
        StartCoroutine(MoveUp(transform.localPosition, new Vector3(transform.localPosition.x, 0.006f, transform.localPosition.z)));
    }
    private IEnumerator MoveUp(Vector3 startPosition, Vector3 endPosition) {
        float speed = 7f;
        

        var i = 0f;
        //var rate = 1f / 2f;
        while (i < 1f) {
            i += Time.deltaTime * speed;
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, i);
            yield return null;
        }
        
        StartCoroutine(Spin(transform.localRotation, Quaternion.Euler(0, GameManager.Singleton.compassYRotation, 0)));
    }

    private IEnumerator Spin(Quaternion startRotation, Quaternion endRotation) {
        yield return new WaitForSeconds(0.3f);

        float speed = 7f;
        

        var i = 0f;
        //var rate = 1f / 2f;
        while (i < 1f) {
            i += Time.deltaTime * speed;
            transform.localRotation = Quaternion.Lerp(startRotation, endRotation, i);
            yield return null;
        }
        StartCoroutine(MoveDown(transform.localPosition, new Vector3(transform.localPosition.x, -0.008f, transform.localPosition.z)));
    }

    private IEnumerator MoveDown(Vector3 startPosition, Vector3 endPosition) {
        yield return new WaitForSeconds(0.3f);
        float speed = 7f;
        var i = 0f;
        //var rate = 1f / 2f;
        while (i < 1f) {
            i += Time.deltaTime * speed;
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, i);
            yield return null;
        }

    }

}
