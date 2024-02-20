using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource Audio;
    public AudioClip Attack, Hit, Clear, Button;

    public void PlaySound(string clip)
    {
        switch (clip) 
        {
            case "Attack":
                Audio.PlayOneShot(Attack);
                break;

            case "Hit":
                Audio.PlayOneShot(Hit);
                break;

            case "Clear":
                Audio.PlayOneShot(Clear);
                break;

            case "Button":
                Audio.PlayOneShot(Button);
                break;
        }
    }
}
