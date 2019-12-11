using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class ManagerMiniHeading : MonoBehaviour
{
    private GameObject chika;
    private GameObject kanan;
    private GameObject ball;
    private GameObject heighttext;
    private GameObject zujoimage;

    private double record;
    private bool headingflg;
    private bool finishflg;
    private float waitsecond;

    // Start is called before the first frame update
    void Start()
    {
        ball = GameObject.Find("Heading_Ball");
        chika = GameObject.Find("Heading_Chika");
        kanan = GameObject.Find("Heading_Kanan");
        heighttext = GameObject.Find("Heading_HeightText");
        zujoimage = GameObject.Find("zujoimage");

        record = 2.0;
        headingflg = false;
        waitsecond = 0.3f ;

        StartCoroutine("dropball");
    }


    IEnumerator dropball()
    {
        yield return new WaitForSeconds(0.3f);

        ball.GetComponent<BallMiniHeading>().ThrowingBall();
    }

    // Update is called once per frame
    void Update()
    {
        double height = ball.GetComponent<SpriteRenderer>().transform.position.y / 2 + 2.2;
        if (height < 0) height = 0;
        
        string txt = string.Format("<color=orange>Now</color> {0,5}m\n<color=orange>Record</color> {1,5}m"
            , (height).ToString("F1"), (record).ToString("F1"));
        heighttext.GetComponent<TextMeshProUGUI>().text = txt;

        kanan.GetComponent<Animator>().SetFloat("height", (float)height);


        if (height > 1.3f)
        {
            zujoimage.GetComponent<RectTransform>().localScale = new Vector3((float)(2 / height), (float)(2 / height), 1);
        }
        else
        {
            zujoimage.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 1);
        }

        if (height > record)
        {
            record = height;

            if (!GameObject.Find("HeadingEmotionalBaloon(Clone)"))
            {
                GameObject emotion = Instantiate(Resources.Load<GameObject>("Minigame/heading/HeadingEmotionalBaloon"));
                if (record > 6)
                {
                    emotion.GetComponent<Animator>().runtimeAnimatorController
                        = Resources.Load<RuntimeAnimatorController>("Emotion/fine");
                    waitsecond = 0.25f;
                }
                if (record > 50)
                {
                    emotion.GetComponent<Animator>().runtimeAnimatorController
                        = Resources.Load<RuntimeAnimatorController>("Emotion/surprised");
                    waitsecond = 0.2f;
                }
                if (record > 200)
                {
                    emotion.GetComponent<Animator>().runtimeAnimatorController
                        = Resources.Load<RuntimeAnimatorController>("Emotion/question");
                    waitsecond = 0.15f;
                }

                Destroy(emotion, 1f);
            }
        }
        else if(height < 0.3)
        {
            if (!finishflg)
            {
                finishflg = true;
                StartCoroutine( finishprocess());
            }
        }

    }

    IEnumerator finishprocess()
    {
        Instantiate(Resources.Load<GameObject>("Minigame/heading/Button_Restart"), GameObject.Find("Canvas").transform);
        GameObject.Find("Heading_You").GetComponent<Animator>().enabled = false;

        GameObject tweetbtn = Instantiate(Resources.Load<GameObject>("Minigame/heading/Button_Tweet"), GameObject.Find("Canvas").transform);

        yield return new WaitForSeconds(0.1f);
        tweetbtn.GetComponent<TweetButtonController>().InputTweetText("ヘディングで" + record.ToString("F1") + "m打ち上げた！ https://koke.link/wordpress/?p=17");
    }

    public void clickChika()
    {
        chika.GetComponent<Animator>().SetTrigger("trgHeading");
        StartCoroutine("heading", chika);
    }

    IEnumerator heading(GameObject chika)
    {
        switch (headingflg)
        {
            case false:
                headingflg = true;

                chika.GetComponent<BoxCollider2D>().isTrigger = false;
                chika.GetComponent<CapsuleCollider2D>().isTrigger = false;
                yield return new WaitForSeconds(waitsecond);
                chika.GetComponent<BoxCollider2D>().isTrigger = true;
                chika.GetComponent<CapsuleCollider2D>().isTrigger = true;

                yield return new WaitForSeconds(0.3f);

                headingflg = false;
                break;

        }
    }

}
