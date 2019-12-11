using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionController : MonoBehaviour
{

    List<GameObject> emotionalBaloons = new List<GameObject>();

    string getEmotion(int action)
    {
        switch (action)
        {
            case 2:
                return "drown";
            case 3:
                return "fine";
            case 4:
                return "surprised";
            case 5:
                return "question";
            default:
                return "";
        }
    }

    // Emotionを更新
    public void updateEmotion(int action, GameObject actionunit)
    {
        string emotion = getEmotion(action);


        // エモーショナルバルーンを追加
        if (emotion != "")
        {
            GameObject emotionalBaloon = Instantiate(Resources.Load<GameObject>("Prefab/EmotionalBaloon"), actionunit.transform);
            emotionalBaloons.Add(emotionalBaloon);
            emotionalBaloon.GetComponent<Animator>().runtimeAnimatorController
                = Resources.Load<RuntimeAnimatorController>("Emotion/" + emotion);
        }

        // Spriteの更新
        //actionunit.GetComponent<Animator>().SetInteger("storyState", action);
    }

    // 出現中のエモーショナルバルーンを削除
    public void clearEmotion()
    {
        while (emotionalBaloons.Count > 0)
        {
            GameObject tmpbaloon = emotionalBaloons[0];
            emotionalBaloons.RemoveAt(0);
            Destroy(tmpbaloon);
        }

        emotionalBaloons = new List<GameObject>();
    }

}
