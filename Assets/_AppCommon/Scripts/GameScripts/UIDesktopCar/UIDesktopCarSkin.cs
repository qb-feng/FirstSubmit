using UnityEngine;
using System.Collections;

public class UIDesktopCarSkin : UIDesktopCarPre
{
    public override void CarDirectionCompute()
    {
        Vector3 carScreenPosition = UIManager.Instance.WorldCamera.WorldToScreenPoint(m_currentCar.transform.position);
        carScreenPosition.z = 0;
        Vector3 screenDirection = Input.mousePosition - carScreenPosition;
        if (screenDirection.x > 0)
        {
            Car.transform.localEulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            Car.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }

    public override void SetCurrentCarImg()
    {
        Car.transform.position = ImageCarPosition.position;
    }
}
