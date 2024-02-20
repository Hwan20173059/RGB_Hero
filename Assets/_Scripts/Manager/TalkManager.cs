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
        talkData.Add(100, new string[] { "�������̶�� ������ �Ա��ݾ�?:1", "�귿�� �������� ��������:0" });
        talkData.Add(200, new string[] { "�̷� ü���� �� ���� �پ����°�?:0", "�̷��� �˰� ���濡 ������ �� ��������:1", "���� ��ư�� ���� ������ �Ծ� ü���� ȸ���� �� �־�:0" });
        talkData.Add(300, new string[] { "���� �ذ� ���� �����ϴ±�:0", "���͵鵵 ���� �� �������°� ����:1", "�����ؼ� ��� �����ڰ�:2" });
        talkData.Add(400, new string[] { "���Ⱑ �ʿ��� �������ΰ�..:0", "���.. ������..?:1", "�����ض�..:2" });

        portraitData.Add(100 + 0, portraitArr[0]);
        portraitData.Add(100 + 1, portraitArr[1]);

        portraitData.Add(200 + 0, portraitArr[2]);
        portraitData.Add(200 + 1, portraitArr[3]);
        portraitData.Add(200 + 2, portraitArr[2]);

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
