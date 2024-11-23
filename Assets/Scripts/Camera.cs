using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Camera : MonoBehaviour
{
    [SerializeField] private GameObject sunLight;
    [SerializeField] private Text indexText;
    [SerializeField] private InputField inputField;
    private GameObject[] cars;
    private Transform myTransform;
    private int index;
    private bool b_left, b_right, b_up, b_down;

    //初期設定では、視点は車体0に視点となる。
    private void Start()
    {
        if (cars == null)
            cars = GameObject.FindGameObjectsWithTag("Car");
        myTransform = this.transform;
        index = 0;
        ChangeCamera();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //左の矢印ボタンが押されてかつ、現在カメラが車体目線の時は、車体番号の一つ小さい車体に視点を切り替える。カメラが車体目線でないときは、車体目線だった時のカメラに戻す
            if (myTransform.parent is object)
            {
                if (index > 0)
                {
                    index--;
                }
                else//これ以上車体番号の小さい車体が存在しないときは、最も車体番号が大きい車体に視点を切り替える
                {
                    index = cars.Length - 1;
                }
            }
            ChangeCamera();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //右の矢印ボタンが押されてかつ、現在カメラが車体目線の時は、車体番号の一つ大きい車体に視点を切り替える。カメラが車体目線でないときは、車体目線だった時のカメラに戻す
            if (myTransform.parent is object)
            {
                if (index < cars.Length - 1)
                {
                    index++;
                }
                else//これ以上車体番号の大きい車体が存在しないときは、最も車体番号が小さい車体に視点を切り替える
                {
                    index = 0;
                }
            }
            ChangeCamera();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //上の矢印ボタンが押されたときは、フィールド全体を上から見るカメラに切り替える
            ResetCamera();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //下のボタンが押されたときは、フィールド全体のライティングを切り替える
            sunLight.transform.Rotate(180f, 0, 0);
        }
    }

    /***
    Input Fieldに値が入力された際に、カメラを別の車体目線に切り替える。
    ***/
    private void ChangeCamera()
    {
        // インデックス番号を表示するテキストを更新
        indexText.text = "Index: " + index.ToString();
        // 選択された車体のTransformを取得
        Transform carTransform = cars[index].transform;
        // カメラのTransformを選択された車体に設定し、車体の位置と回転をカメラに反映
        myTransform.SetParent(carTransform);// カメラを選択された車体の子オブジェクトに設定
        myTransform.position = carTransform.position;// カメラの位置を車体の位置に設定
        myTransform.rotation = carTransform.rotation;// カメラの回転を車体の回転に設定
        // カメラの相対位置を設定（車体上後方に配置）
        myTransform.localPosition = new Vector3(0, 5f, -10f);
        // カメラを若干前方に傾ける（上方から見下ろすような角度）
        myTransform.Rotate(15f, 0, 0);
    }

    /***
    フィールド全体を上から見るカメラに切り替える
    ***/
    private void ResetCamera()
    {
        myTransform.SetParent(null);// カメラの親オブジェクトを解除（視点をフィールド全体に設定）
        myTransform.position = new Vector3(0, 200f, 0);// カメラの位置をフィールド全体の上空（y軸上）に設定
        myTransform.eulerAngles = new Vector3(90f, 0, 0);// カメラの回転角度を水平から垂直（上からの視点）に設定
    }
}
