using UnityEngine;
using System.Collections;

public class MovingCamera : MonoBehaviour
{

  public float    m_minSize = 3f;
  public float    m_maxSize = 10f;
  public float    m_zoomSpeed = 0.03f;
  public float    m_damping = 0.1f;
  public Vector2  m_lowerLeft  = new Vector2 (-10,-10);
  public Vector2  m_upperRight = new Vector2 (10,10);

  Vector3      m_speed = new Vector3 (0f,0f,0f);

  float m_unitsPerPixel;

  int m_scrollTouchID = -1;
  Camera m_camera;

  void Start ()
  {
    UpdateUnitsPerPixel ();

    DeviceChange.OnOrientationChange += OrientationChange;

    m_camera = GetComponent<Camera> ();

    GetComponent<Rigidbody2D> ().gravityScale = 0f;

    UpdateUnitsPerPixel ();
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
      PerformZoom (touches [0], touches [1]);
  }

  void PerformPan (TouchProxy touch)
  {
    //Note down the touch ID and position when the touch begins...
    if (touch.m_phase == TouchPhase.Began)
      m_scrollTouchID = touch.m_fingerId;
    else if (touch.m_phase == TouchPhase.Moved && touch.m_fingerId == m_scrollTouchID)
    {
      Vector2 distance = m_unitsPerPixel * touch.m_deltaPosition;

      m_speed = -new Vector3 (distance.x, distance.y, 0f);
    }
  }

  void Update ()
  {
    if (transform.position.x > m_upperRight.x && m_speed.x > 0 ||
      transform.position.x < m_lowerLeft.x && m_speed.x < 0)
      m_speed.x = 0f;
    
    if (transform.position.y > m_upperRight.y && m_speed.y > 0 ||
      transform.position.y < m_lowerLeft.y && m_speed.y < 0)
      m_speed.y = 0f;

    transform.position += m_speed;

    m_speed -= m_damping * m_speed;
  }

  void PerformZoom (TouchProxy touchZero, TouchProxy touchOne)
  {
    // Find the position in the previous frame of each touch.
    Vector2 touchZeroPrevPos = touchZero.m_position - touchZero.m_deltaPosition;
    Vector2 touchOnePrevPos = touchOne.m_position - touchOne.m_deltaPosition;

    // Find the magnitude of the vector (the distance) between the touches in each frame.
    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
    float touchDeltaMag = (touchZero.m_position - touchOne.m_position).magnitude;

    // Find the difference in the distances between each frame.
    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

    // ... change the orthographic size based on the change in distance between the touches.
    m_camera.orthographicSize += deltaMagnitudeDiff * m_zoomSpeed;

    // Make sure the orthographic size never drops below zero.
    m_camera.orthographicSize = Mathf.Max (m_camera.orthographicSize, m_minSize);
    m_camera.orthographicSize = Mathf.Min (m_camera.orthographicSize, m_maxSize);

    UpdateUnitsPerPixel ();
  }
}
