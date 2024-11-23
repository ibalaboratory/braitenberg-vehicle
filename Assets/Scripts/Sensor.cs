using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SensorInfo
{
    public List<WheelCollider> wheels;// タイヤ
    public GameObject sensor;//センサ。ビークルの種類ごとにセンサを付け替える。
}

public class Sensor : MonoBehaviour
{
    public List<SensorInfo> sensorInfos;
    public float maxMotorTorque;
    public float sensitivity = 1f; // 光に対するセンサーの感度
    public bool invert; // インバーターの有無
    public float maxSpeed = 5f; // Replace with your max speed
    public bool gaussian; // for Test5
    public float mean; // for Test5
    public float stdev; // for Test5
    public bool discontinuous; // for Test6
    public float threshold; // for Test6
    private List<GameObject> lights;
    private Rigidbody rb;

    private void Start()
    {
        //ライトオブジェクトが空だった時、フィールド内のライトオブジェクトをリストで取得する
        if (lights == null)
            lights = new List<GameObject>(GameObject.FindGameObjectsWithTag("Light"));
        int a = lights.RemoveAll(light => light.transform.IsChildOf(this.transform));
        Debug.Assert(a == 1);
        rb = this.gameObject.GetComponent<Rigidbody>();//物理演算を行うためのオブジェクト
    }

    private void FixedUpdate()
    {
        foreach (SensorInfo sensorInfo in sensorInfos)
        {
            Vector3 position = sensorInfo.sensor.transform.position;
            float x = calculateLightIntensity(position);//光の強さを計算
            float motor = 0; // モータのトルク
            if (gaussian)//Test4,5
            {
                motor = maxMotorTorque * Mathf.Exp(-(x - mean) * (x - mean) / (2 * stdev * stdev));//ガウシアン関数に基づくトルク
            }
            else if (discontinuous)
            {
                motor = (x < threshold) ? 0 : maxMotorTorque;
            }
            else
            {
                motor = maxMotorTorque * x; // 光強度に対して線形にトルクを大きくする
                motor = Mathf.Min(motor, maxMotorTorque);//トルクの最大値を制限
            }
            // invertがtrueならばトルクを反転させる
            if (invert)
            {
                motor = maxMotorTorque - motor;
            }
            //光強度から両輪のトルクを実際に変更する
            foreach (WheelCollider wheel in sensorInfo.wheels)
            {
                if (motor > 0)
                {
                    wheel.motorTorque = motor;
                    wheel.brakeTorque = 0;
                }
                else
                {
                    wheel.motorTorque = 0;
                }
            }
        }
        //スピードの最大値を制限する
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    /***
    ビークルがフィールドを囲っているフェンスに衝突したときは、
    ビークルの向きや速度を維持したまま、元の位置とx軸またはz軸に関して対称な位置にビークルを移動させる.
    これによりビークルがフィールド端のフェンスにぶつかって移動できなくなることを防いでいる.
    ***/
    private void OnTriggerEnter(Collider other)
    {
        Vector3 pos = this.transform.position;
        if (other.gameObject.name == "Left" || other.gameObject.name == "Right")
        {
            pos.x *= -0.99f;
        }
        if (other.gameObject.name == "Up" || other.gameObject.name == "Down")
        {
            pos.z *= -0.99f;
        }
        this.transform.position = pos;
    }

    //ライトとセンサとの距離からライトの光強度の総和を計算する関数
    private float calculateLightIntensity(Vector3 position)
    {
        float intensity = 0;
        foreach (GameObject light in lights)
        {
            Vector3 diff = light.transform.position - position;//ライトとセンサの距離を計算
            float sqrDistance = diff.sqrMagnitude;//距離の二乗を計算
            intensity += 1 / sqrDistance; // 距離の二乗に反比例
        }
        return intensity * sensitivity;//センサの感度で補正をかける
    }
}

