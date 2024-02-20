using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    Dictionary<int, string[]> talkData;
    Dictionary<int, Sprite> portraitData;

    public Sprite[] portraitArr;

    void Awake()
    {
        talkData = new Dictionary<int, string[]>();
        portraitData = new Dictionary<int, Sprite>();
        GenerateData();
    }

    void GenerateData()
    {
        talkData.Add(100, new string[] { "슬라임이라니 식은죽 먹기잖아?:1", "룰렛을 가동시켜 공격하자:0" });
        talkData.Add(200, new string[] { "역시 식은 죽 먹기라니까:0", "이대로 계속 가보자고:1" });
        talkData.Add(300, new string[] { "초원을 계속 걸으면 기분이 산뜻해져:0", "이대로 계속 걸어가고 싶군:1", "계속 진행해보자고:2" });
        talkData.Add(400, new string[] { "여기가 초원의 막바지인가..:0", "어엇.. 저것은..?:1", "절망해라..:2" });

        portraitData.Add(100 + 0, portraitArr[0]);
        portraitData.Add(100 + 1, portraitArr[1]);
        portraitData.Add(200 + 0, portraitArr[2]);
        portraitData.Add(200 + 1, portraitArr[3]);
        portraitData.Add(300 + 0, portraitArr[4]);
        portraitData.Add(300 + 1, portraitArr[5]);
        portraitData.Add(300 + 2, portraitArr[6]);
        portraitData.Add(400 + 0, portraitArr[7]);
        portraitData.Add(400 + 1, portraitArr[8]);
        portraitData.Add(400 + 2, portraitArr[9]);
    }

    public string GetTalk(int id, int talkIndex)
    {
        if(talkIndex == talkData[id].Length)
            return null;
        else
            return talkData[id][talkIndex];
    }

    public Sprite GetPortrait(int id, int portraitIndex)
    {
        return portraitData[id + portraitIndex];
    }
}
