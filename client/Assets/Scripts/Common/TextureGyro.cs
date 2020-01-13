using UnityEngine;

/// <summary>
/// 仿农药的Loading图，根据陀螺仪而偏移图片
/// </summary>
public class TextureGyro : MonoBehaviour
{
    //陀螺仪是否存在
    private bool gyroBool;
    //陀螺仪
    private Gyroscope gyro;
    //X轴方向移动的速度参数
    private float xSpeed = 200;
    //移动方向的三维向量
    private Vector3 directionV3;
    //陀螺仪x轴的取值
    private float gyrosParameter;

    private void Awake()
    {
        //判断是否支持陀螺仪
        gyroBool = SystemInfo.supportsGyroscope;
        if (gyroBool)
        {
            //给陀螺仪复制
            gyro = Input.gyro;
            gyro.enabled = true;
            if (Screen.orientation == ScreenOrientation.LandscapeLeft)
            {
                directionV3 = new Vector3(1, 0, 0);
            }

            if (Screen.orientation == ScreenOrientation.LandscapeRight)
            {
                directionV3 = new Vector3(-1, 0, 0);
            }

            //设置屏幕长亮
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //位置随着陀螺仪重力感应的X轴变化而变化
        if (gyroBool)
        {
            gyrosParameter = gyro.gravity.x;
            transform.localPosition = gyrosParameter * directionV3 * xSpeed;
        }
    }
}