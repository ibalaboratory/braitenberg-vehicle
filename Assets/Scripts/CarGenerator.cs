using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] carPrefabs = new GameObject[6];//6種類のビークルを格納する
    [SerializeField] private Text crashText;
    private float[] xlim = new float[] { -150, 150 };//フィールドの領域の範囲
    private float[] zlim = new float[] { -100, 100 };//フィールドの領域の範囲
    private static int N = 90;//ビークルの数
    private GameObject[] cars = new GameObject[N];
    private WheelCollider[][] wheels = new WheelCollider[N][];
    private int[] counts = new int[N];//各ビークルが横転していると判定された回数を格納
    private int crashCount;//全てのビークルの横転した回数の総和
    private float LIntensity;

    private List<GameObject> carList1 = new List<GameObject>();//種類1のビークルが格納されている配列
    private List<GameObject> carList2 = new List<GameObject>();//種類2のビークルが格納されている配列
    private List<GameObject> carList3 = new List<GameObject>();//種類3のビークルが格納されている配列
    private List<GameObject> carList4 = new List<GameObject>();//種類4のビークルが格納されている配列
    private List<GameObject> carList5 = new List<GameObject>();//種類5のビークルが格納されている配列
    private List<GameObject> carList6 = new List<GameObject>();//種類6のビークルが格納されている配列

    // ランダムな種類のビークルをランダムな位置に召喚
    private void Awake()
    {
        Random.InitState(177);
        for (int i = 0; i < N; i++)
        {
            InstantiateCar(i);
        }
    }

    private void Start()
    {
        crashCount = 0;
        crashText.text = "Crash: " + crashCount.ToString();
        // ビークルが横転しているかどうかを2秒ごとにCheckCars関数を呼び出すことで確認する
        InvokeRepeating("CheckCars", 2.0f, 2.0f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            // F1キーを押すと種類1(臆病者)の車体の表示/非表示を切り替える。
            foreach (GameObject car in carList1)
            {
                SwitchRenderer(car);
            }
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            // F2キーを押すと種類2(攻撃者)の車体の表示/非表示を切り替える。
            foreach (GameObject car in carList2)
            {
                SwitchRenderer(car);
            }
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            // F3キーを押すと種類3(求愛者)の車体の表示/非表示を切り替える。
            foreach (GameObject car in carList3)
            {
                SwitchRenderer(car);
            }
        }
        else if (Input.GetKeyDown(KeyCode.F4))
        {
            // F4キーを押すと種類4(探索者)の車体の表示/非表示を切り替える。
            foreach (GameObject car in carList4)
            {
                SwitchRenderer(car);
            }
        }
        else if (Input.GetKeyDown(KeyCode.F5))
        {
            // F5キーを押すと種類5の車体の表示/非表示を切り替える。
            foreach (GameObject car in carList5)
            {
                SwitchRenderer(car);
            }
        }
        else if (Input.GetKeyDown(KeyCode.F6))
        {
            // F6キーを押すと種類6の車体の表示/非表示を切り替える。
            foreach (GameObject car in carList6)
            {
                SwitchRenderer(car);
            }
        }
    }

    // ランダムな種類のビークルをランダムな位置に召喚
    private void InstantiateCar(int i)
    {
        // 0以上1未満のランダムな小数を生成して、ランダムな位置を決める
        float x = xlim[0] + (xlim[1] - xlim[0]) * Random.value;// x座標
        float y = 1.0f;// y座標（高さを1.0に固定）
        float z = zlim[0] + (zlim[1] - zlim[0]) * Random.value;// z座標

        int rnd = Random.Range(0, 6);//0から6未満のランダムな整数を生成して、生成する車体の種類を決める
        // cars[i]にランダムな車体をインスタンス化（召喚）し、ランダムな位置と向きを設定
        cars[i] = Instantiate(carPrefabs[rnd], new Vector3(x, y, z), Quaternion.identity);//車体の位置をランダムにする
        cars[i].transform.rotation = Quaternion.Euler(0, Random.Range(-180f, 180f), 0);//車体の向きをランダムにする

        // 車体内のWheelColliderを取得し、wheels[i]に格納
        wheels[i] = cars[i].GetComponentsInChildren<WheelCollider>();
        // 車体内のTextMeshProコンポーネントを取得し、そのテキストにビークルのインデックス番号iを表示
        TextMeshPro indexText = cars[i].GetComponentInChildren<TextMeshPro>();
        // indexTextがnullでないことを確認（Debug用）
        Debug.Assert(indexText is object);
        // テキストにビークルのインデックス番号iを表示
        indexText.text = i.ToString();
        // 車体内のLightコンポーネントからLightのIntensityを取得し、LIntensityに格納。LIntensityによってビークルの表示/非表示を切り替えている
        LIntensity = cars[i].GetComponentInChildren<Light>().intensity;

        switch (rnd)
        {
            case 0:
                carList1.Add(cars[i]);
                break;
            case 1:
                carList2.Add(cars[i]);
                break;
            case 2:
                carList3.Add(cars[i]);
                break;
            case 3:
                carList4.Add(cars[i]);
                break;
            case 4:
                carList5.Add(cars[i]);
                break;
            case 5:
                carList6.Add(cars[i]);
                break;
            default:
                break;
        }
    }

    // ビークルが横転しているかどうかを確認し横転していたら向きを直して復活させる
    private void CheckCars()
    {
        for (int i = 0; i < N; i++)
        {
            WheelHit[] hits = new WheelHit[2];
            // 両車輪が接地しているか確認
            if (wheels[i][0].GetGroundHit(out hits[0]) && wheels[i][1].GetGroundHit(out hits[1]))
            {
                // 両車輪が"Floor"タグのオブジェクトに接地している場合
                if (hits[0].collider.gameObject.tag == "Floor" && hits[1].collider.gameObject.tag == "Floor")
                {
                    // 横転していないと判断し、カウントをリセットして次の車両をチェック
                    counts[i] = 0;
                    continue;
                }
            }
            // 両車輪のどちらかが接地していない場合、横転していると判断
            counts[i]++;
            // カウントが一定数以上になった場合、ビークルを復活させる
            if (counts[i] >= 3)
            {
                counts[i] = 0;// カウントをリセット
                // クラッシュ回数を増やし、テキストに表示
                crashCount++;
                crashText.text = "Crash: " + crashCount.ToString();
                // ビークルのTransformを取得
                Transform carTransform = cars[i].transform;
                // ビークルのy軸回転角度を元に戻す
                float y = carTransform.eulerAngles.y;
                carTransform.rotation = Quaternion.Euler(0, y, 0);
                // ビークルを一定の位置に移動させる（復活させる）
                carTransform.Translate(0.0f, 1.0f, -10.0f);
                // 壁の外に出るビークルが稀にいるので対策
                // ビークルが特定の領域内にいる場合は処理を中断して再配置しない
                if (xlim[0] < carTransform.position.x && carTransform.position.x < xlim[1] &&
                    zlim[0] < carTransform.position.z && carTransform.position.z < zlim[1])
                {
                    return;
                }
                // ビークルが特定の領域外にいる場合、位置をリセットして復活させる
                carTransform.position = Vector3.up;
            }
        }
    }

    // 指定したビークルの見た目を透明にして見えなくする
    private void SwitchRenderer(GameObject car)
    {
        Renderer[] renderers = car.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = !renderer.enabled;
        }
        Light light = car.GetComponentInChildren<Light>();
        light.intensity = (light.intensity > 0) ? 0 : LIntensity;
    }
}
