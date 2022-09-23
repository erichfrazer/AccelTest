using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform m_CanonicalRig;
    public Transform m_Imu;

    float LastTime;
    Matrix4x4 LastImuWorldSpace;
    Vector3 LastImuPos;

    float fDiameter = 5.0f;
    float fSpeed = .5f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = fDiameter * Mathf.Cos(fSpeed * Time.time);
        float y = fDiameter * Mathf.Sin(fSpeed * Time.time);

        transform.position = new Vector3(x, 0, y);

        float fNow = Time.time;
        if( LastTime == 0 )
        {
            LastTime = fNow;
            return;
        }

        float fDelta = fNow - LastTime;
        LastTime = fNow;

        Vector3 acc_WS = (m_Imu.position - LastImuPos) / ( fDelta * fDelta );
        LastImuPos = m_Imu.position;

        string sz;
        sz = string.Format("IMU's WS accel = {0},{1},{2}", acc_WS.x, acc_WS.y, acc_WS.z);
        Debug.Log(sz);
        
    }
}
