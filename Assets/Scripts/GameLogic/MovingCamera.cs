using UnityEngine;
using System.Collections;

public class MovingCamera : MonoBehaviour
{

  float m_unitsPerPixel;

  int m_scrollTouchID = -1;
  Vector2 m_scrollTouchOrigin;
  Vector2 m_cameraOrigin;

  void Start ()
  {
    UpdateUnitsPerPixel ();

    DeviceChange.OnOrientationChange += OrientationChange;
  }

  void OrientationChange (DeviceOrientation orientation) 
  {
    UpdateUnitsPerPixel ();
  }

  void UpdateUnitsPerPixel ()
  {
    m_unitsPerPixel = Vector3.Distance (Camera.main.ScreenToWorldPoint (Vector2.zero), Camera.main.ScreenToWorldPoint (Vector2.right));
  }

  void HandleTouches (TouchProxy[] touches)
  {
    if (touches.Length == 1)
      PerformPan (touches [0]);
    else
      PerformZoom (touches[0], touches[1]);
  }

  void PerformPan (TouchProxy touch)
  {
    //Note down the touch ID and position when the touch begins...
    if (touch.m_phase == TouchPhase.Began)
    {
      m_scrollTouchID = touch.m_fingerId;
      m_scrollTouchOrigin = touch.m_position;
      m_cameraOrigin = transform.position;

      return;
    } 
    else if (touch.m_phase == TouchPhase.Moved && touch.m_fingerId == m_scrollTouchID)
    {
      Vector3 CameraPos = transform.position;
      Vector2 distance = m_unitsPerPixel * (touch.m_position - m_scrollTouchOrigin);

      transform.position = new Vector3 (m_cameraOrigin.x - distance.x, m_cameraOrigin.y - distance.y, CameraPos.z);
    }
  }

  void PerformZoom (TouchProxy touchZero, TouchProxy touchOne)
  {
    /*
    // Find the position in the previous frame of each touch.
    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

    // Find the magnitude of the vector (the distance) between the touches in each frame.
    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

    // Find the difference in the distances between each frame.
    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

    // ... change the orthographic size based on the change in distance between the touches.
    camera.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

    // Make sure the orthographic size never drops below zero.
    camera.orthographicSize = Mathf.Max (camera.orthographicSize, 0.1f);
    */
  }
}
