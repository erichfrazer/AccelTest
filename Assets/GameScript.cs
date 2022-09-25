using System.Collections; 
using System.Collections.Generic;
using UnityEngine;

class CachedFrameInfo
{
    public Matrix4x4 localToWorldMatrix;
}

public class GameScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform m_Canon;
    public Transform m_Imu;

    List<CachedFrameInfo> m_CachedFrames = new List<CachedFrameInfo>();

    void Start()
    {
        Time.captureFramerate = 25;

        Vector3 pos = transform.position;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(-pos * Random.Range(1.0f, 10.0f));
        rb.AddTorque(Random.insideUnitSphere);
    }

    void _CalculateAccel()
    {
        CachedFrameInfo os = new CachedFrameInfo();
        os.localToWorldMatrix = m_Imu.localToWorldMatrix;
        m_CachedFrames.Add(os);

        if (m_CachedFrames.Count < 3) return;

        float fTimePerFrame = Time.deltaTime;

        string szDebugString;

        Matrix4x4 delta_A = m_CachedFrames[0].localToWorldMatrix.inverse * m_CachedFrames[1].localToWorldMatrix;
        Vector3 linVel_A = new Vector3(delta_A[0, 3], delta_A[1, 3], delta_A[2, 3]) / fTimePerFrame;

        // multiplies ( local / world ) * world to get local...
        Matrix4x4 delta_B = m_CachedFrames[1].localToWorldMatrix.inverse * m_CachedFrames[2].localToWorldMatrix;
        Vector3 linVel_B = new Vector3(delta_B[0, 3], delta_B[1, 3], delta_B[2, 3]) / fTimePerFrame;

        // 2nd differentiation gets us acceleration. Not sure at what exact point the acceleration is
        // calculated...
        Vector3 linAcc = (linVel_B - linVel_A) / fTimePerFrame;

        // add in gravity
        Vector3 gravVector_World = new Vector3(0, -9.81f, 0);
        Vector3 gravVector_Imu = m_Imu.localToWorldMatrix.inverse * gravVector_World;
        linAcc += gravVector_Imu;

        m_CachedFrames.RemoveAt(0);

        szDebugString = string.Format("IMU's lin vel2 = {0},{1},{2}", linVel_A.x, linVel_A.y, linVel_A.z);
//        Debug.Log(sz);

        szDebugString = string.Format("IMU's lin acce2 = {0},{1},{2}", linAcc.x, linAcc.y, linAcc.z);
//        Debug.Log(sz);

        // why do we do this? this is weird.
        //
        Vector3 velocity_axis_A;
        Vector3 velocity_axis_B;
        float angle_A;
        float angle_B;
        delta_A.rotation.ToAngleAxis(out angle_A, out velocity_axis_A);
        delta_B.rotation.ToAngleAxis(out angle_B, out velocity_axis_B);

        // 2nd differentiation gets us acceleration. Angular velocity is still acceleration.
        Vector3 angular_velocity = ( velocity_axis_B - velocity_axis_A ) * ( angle_B - angle_A ) / fTimePerFrame;

        szDebugString = string.Format("IMU's angular acce2 = {0},{1},{2}", angular_velocity.x, angular_velocity.y, angular_velocity.z);
//        Debug.Log(sz);
    }

    int thrust_burn_counter;
    Vector3 random_burn_angle;
    float thurst_burn_power = 4.0f;

    // Update is called once per frame
    void Update()
    {
#if false
        Vector3 pos = transform.position;
        
        Rigidbody rb = GetComponent<Rigidbody>();
        int rr = Random.Range(0, 1000);
        if (rr == 999)
        {
            Vector3 RandomForce = Random.insideUnitSphere;
            RandomForce.y = 0;
            rb.AddForce(RandomForce * Random.Range(-10.0f, 10.0f));
        }
        else if( rr == 998 )
        {
            thrust_burn_counter = Random.Range(50, 100);
            random_burn_angle = Random.insideUnitSphere;
        }
        else if( rr > 990 )
        {
            rb.AddForce(-pos * Random.Range(1.0f, 10.0f));
        }

        if( thrust_burn_counter > 0 )
        {
            thrust_burn_counter--;
            rb.AddForce((transform.forward + random_burn_angle) * thurst_burn_power);
        }
#endif
        _CalculateAccel();
    }
}
