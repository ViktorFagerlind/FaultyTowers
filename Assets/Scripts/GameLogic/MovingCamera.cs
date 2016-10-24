using UnityEngine;
using System.Collections;

public class MovingCamera : TouchBase
{

  // The ID of the touch that began the scroll.
  int ScrollTouchID = -1;
  // The position of that initial touch
  Vector2 ScrollTouchOrigin;

  void Update()
  {
    foreach (Touch T in Input.touches)
    {
      //Note down the touch ID and position when the touch begins...
      if (T.phase == TouchPhase.Began)
      {
        if (ScrollTouchID == -1)
        {
          ScrollTouchID = T.fingerId;
          ScrollTouchOrigin = T.position;
        }
      }
      //Forget it when the touch ends
      if ((T.phase == TouchPhase.Ended) || (T.phase == TouchPhase.Canceled))
      {
        ScrollTouchID = -1;
      }
      if (T.phase == TouchPhase.Moved)
      {
        //If the finger has moved and it's the finger that started the touch, move the camera along the Y axis.
        if (T.fingerId == ScrollTouchID)
        {
          Vector3 CameraPos = transform.position;
          transform.position = new Vector3 (CameraPos.x, CameraPos.y + T.deltaPosition.y, CameraPos.z);
        }
      }
    }
  }
}
