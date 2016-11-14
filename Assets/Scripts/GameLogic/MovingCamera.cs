using UnityEngine;
using System;
using System.Collections;

public class MovingCamera : MonoBehaviour
{
  public enum State
  {
    Panning,
    Zooming,
    None
  }

  public float    m_minSize = 3f;
  public float    m_maxSize = 10f;
  public float    m_zoomSpeed = 0.03f;
  public float    m_damping = 0.1f;
  public Vector2  m_lowerLeft  = new Vector2 (-10,-10);
  public Vector2  m_upperRight = new Vector2 (10,10);

  Vector3         m_panStartPosition;

  Vector3         m_speed = new Vector3 (0f,0f,0f);

  int             m_scrollTouchID = -1;
  Camera          m_camera;

  State           m_state = State.None;
  bool            m_panPerformed=false;
  bool            m_zoomPerformed=false;

  FixedSizedVector2Queue m_speedAverageQueue = new FixedSizedVector2Queue (6);

  void Start ()
  {
    m_camera = GetComponent<Camera> ();
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
    Vector3 worldTouchPosition = m_camera.ScreenToWorldPoint (new Vector3 (touch.m_position.x, touch.m_position.y, -m_camera.transform.position.z));

    //Note down the touch ID and position when the touch begins...
    if (touch.m_phase == TouchPhase.Began)
    {
      m_scrollTouchID = touch.m_fingerId;

      m_panStartPosition = worldTouchPosition;
    }
    else if (touch.m_fingerId == m_scrollTouchID)
    {
      m_speed = -(worldTouchPosition - m_panStartPosition);

      m_speedAverageQueue.Enqueue (m_speed);
    }

    m_panPerformed=true;
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

    m_zoomPerformed=true;
  }

  void HandleState ()
  {
    State newState = State.None;
    if (m_panPerformed)
      newState = State.Panning;
    else if (m_zoomPerformed)
      newState = State.Zooming;

    if (m_state == State.Panning)
      if (newState != State.Panning)
        OnExitPanning ();

    m_state = newState;

    m_panPerformed=false;
    m_zoomPerformed=false;
  }

  void OnExitPanning ()
  {
    m_speed = m_speedAverageQueue.Average ();
    m_speedAverageQueue.Clear ();

    m_scrollTouchID = -1; // Reset scroll id to make sure that one of the zoom touches are not mistaken for an ongoing pan
  }

  // The only reason it works with setting the speed in PerformPan (comes from CaptureTouches' Update) and then using it in LateUpdate
  // here is that CaptureTouches' Update is run before LateUpdate. This may not be super intuitive and nice...
  void LateUpdate ()
  {
    HandleState ();

    if (transform.position.x > m_upperRight.x && m_speed.x > 0 ||
      transform.position.x < m_lowerLeft.x && m_speed.x < 0)
      m_speed.x = 0f;
    
    if (transform.position.y > m_upperRight.y && m_speed.y > 0 ||
      transform.position.y < m_lowerLeft.y && m_speed.y < 0)
      m_speed.y = 0f;

    transform.position += m_speed;
    m_speed -= m_damping * m_speed;
  }
}
