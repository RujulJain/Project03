using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkScript : MonoBehaviour
{
    // Use this for initialization
    CharacterController cc;
    new AudioSource audio;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cc.isGrounded == true && cc.velocity.magnitude > 2f && audio.isPlaying == false)
        {
            audio.volume = Random.Range(0.8f, 1);
            audio.pitch = Random.Range(0.8f, 1);
            audio.Play();
        }
    }
}
