using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    [SerializeField] private AudioClip initialClip, loopedClip;
    private AudioSource source;


    private float initialClipLength;

    public static MusicManager instance;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(this.gameObject);
        }
    }

    private void Start() {
        initialClipLength = initialClip.length;
        source = GetComponent<AudioSource>();

        source.clip = initialClip;
        source.Play();

        Invoke("LoopSong", initialClipLength);

        
    }

    private void LoopSong() {
        StartCoroutine(Play());
    }
    IEnumerator Play() {
        while (true) {
            source.clip = loopedClip;
            source.Play();
            yield return new WaitForSeconds(source.clip.length);
        }
    }

}
