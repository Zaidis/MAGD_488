using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tile : MonoBehaviour {
    public GameObject token;
    public void SetToken(GameObject token) {
        this.token = token;
        token.transform.position = transform.position;
        token.transform.parent = transform;
    }
}
