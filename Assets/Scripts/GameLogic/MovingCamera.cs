using UnityEngine;
using System.Collections;

public class MovingCamera : MonoBehaviour
{

  float m_unitsPerPixel;

  //float m_pixelsPerUnit;

  int m_scrollTouchID = -1;
  Vector2 m_scrollTouchOrigin;
  Vector2 m_cameraOrigin;

  void Start ()
  {
    m_unitsPerPixel = Vector3.Distance (Camera.main.ScreenToWorldPoint(Vector2.zero), Camera.main.ScreenToWorldPoint(Vector2.right));
    //m_pixelsPerUnit = 1f / m_unitsPerPixel;
  }

  void HandleTouches (TouchProxy[] touches)
  {
    if (touches.Length != 1)
      return;

    TouchProxy t = touches [0];

    //Note down the touch ID and position when the touch begins...
    if (t.m_phase == TouchPhase.Began)
    {
      m_scrollTouchID     = t.m_fingerId;
      m_scrollTouchOrigin = t.m_position;
      m_cameraOrigin      = transform.position;

      return;
    }
    else if (t.m_phase == TouchPhase.Moved && t.m_fingerId == m_scrollTouchID)
    {
      Vector3 CameraPos = transform.position;
      Vector2 distance = m_unitsPerPixel * (t.m_position - m_scrollTouchOrigin);

      transform.position = new Vector3 (m_cameraOrigin.x - distance.x, m_cameraOrigin.y - distance.y, CameraPos.z);
    }
  }
}
