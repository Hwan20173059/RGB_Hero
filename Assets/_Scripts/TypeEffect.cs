using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeEffect : MonoBehaviour
{
    public GameManager battlesystem;
    public string targetMsg;
    public int CharPerSeconds;
    float interval;

    public AudioSource audioSource;

    public GameObject EndCursor;

    Text msgText;
    int index;
    public bool isAnim;

    private void Awake()
    {
        msgText = GetComponent<Text>();
        audioSource = GetComponent<AudioSource>();
    }

    public void SetMsg(string msg)
    {
        if (isAnim)
        {
            msgText.text = targetMsg;
            CancelInvoke();
            EffectEnd();
        }
        else
        {
            targetMsg = msg;
            EffectStart();
        }
    }

    void EffectStart()
    {
        msgText.text = "";
        index = 0;

        EndCursor.SetActive(false);

        isAnim = true;

        interval = 1.0f / CharPerSeconds;
        Invoke("Effecting", interval);
    }

    void Effecting()
    {
        if (msgText.text == targetMsg) 
        {
            EffectEnd();
            return;
        }
        msgText.text += targetMsg[index];

        if (targetMsg[index] != ' ' || targetMsg[index] != '.')
        { 
            audioSource.Play(); 
        }

        index++;
        Invoke("Effecting", interval);
    }
    
    void EffectEnd()
    {
        isAnim = false;
        battlesystem.talkIndex++;
        EndCursor.SetActive(true);
    }

}
