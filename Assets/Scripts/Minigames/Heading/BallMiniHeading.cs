using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMiniHeading : MonoBehaviour
{
    bool flgFirst = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //オブジェクトが衝突したとき
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name != "jimendayo")
        {
            gameObject.GetComponent<Animator>().SetTrigger("trgHeading");
            gameObject.GetComponent<Rigidbody2D>().freezeRotation = false;

        }

        if (flgFirst)
        {
            gameObject.GetComponent<Rigidbody2D>().freezeRotation = true;
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(0, gameObject.GetComponent<Rigidbody2D>().velocity.y,0);
            flgFirst = false;
            //
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 2.5f;
        }
    }


    /// <summary>
    /// ボールを射出する
    /// </summary>
    public void ThrowingBall()
    {
        GameObject target = GameObject.Find("Heading_Chika");
        GameObject throwfrom = GameObject.Find("Heading_You");
        float angle = 80;

        //
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 2;

        // 標的の座標
        Vector3 targetPosition = new Vector3(target.transform.position.x, target.transform.position.y + target.transform.localScale.y, 0);
        
        // 射出速度を算出
        Vector3 velocity = CalculateVelocity(throwfrom.transform.position, targetPosition, angle) *1.4f;

        // 射出
        Rigidbody2D rid = gameObject.GetComponent<Rigidbody2D>();
        rid.AddForce(velocity * rid.mass, ForceMode2D.Impulse);


        //
        throwfrom.GetComponent<Animator>().SetTrigger("trgThrow");
    }

    /// <summary>
    /// 標的に命中する射出速度の計算
    /// </summary>
    /// <param name="pointA">射出開始座標</param>
    /// <param name="pointB">標的の座標</param>
    /// <returns>射出速度</returns>
    private Vector3 CalculateVelocity(Vector3 pointA, Vector3 pointB, float angle)
    {
        // 射出角をラジアンに変換
        float rad = angle * Mathf.PI / 180;

        // 水平方向の距離x
        float x = Vector2.Distance(new Vector2(pointA.x, pointA.z), new Vector2(pointB.x, pointB.z));

        // 垂直方向の距離y
        float y = pointA.y - pointB.y;

        // 斜方投射の公式を初速度について解く
        float speed = Mathf.Sqrt(-Physics.gravity.y * Mathf.Pow(x, 2) / (2 * Mathf.Pow(Mathf.Cos(rad), 2) * (x * Mathf.Tan(rad) + y)));

        if (float.IsNaN(speed))
        {
            // 条件を満たす初速を算出できなければVector3.zeroを返す
            return Vector3.zero;
        }
        else
        {
            return (new Vector3(pointB.x - pointA.x, x * Mathf.Tan(rad), pointB.z - pointA.z).normalized * speed);
        }
    }
}
