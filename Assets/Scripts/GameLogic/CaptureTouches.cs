using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchProxy
{
  public Vector2 m_position;
  public TouchPhase m_phase;
  public int m_fingerId;
  public Vector2 m_deltaPosition;

  public TouchProxy (Vector2 position, TouchPhase phase, int fingerId, Vector2 deltaPosition)
  {
    m_position = position;
    m_phase = phase;
    m_fingerId = fingerId;
    m_deltaPosition = deltaPosition;
  }

  public TouchProxy (Vector2 position, TouchPhase phase, int fingerId) : this (position, phase, fingerId, new Vector2 ())
  {
  }
}

public delegate bool TouchDelegate (TouchProxy[] touches);

public class CaptureTouches : MonoBehaviour
{
  [HideInInspector]
  public static TouchDelegate m_touchDelegates = null;

  DebugTextHandle m_fpsTouchHandle;


  // ------------------------------------------------------------------------------------------

  public static TouchProxy[] currentTouches {
    get { return m_currentTouches; }
  }
  // ------------------------------------------------------------------------------------------

  private Vector2 m_fakeSecondPosition;

  Dictionary<int, FixedSizedVector2Queue> id2positionAverage = new Dictionary<int, FixedSizedVector2Queue> ();
  Dictionary<int, Vector2> id2previousPosition = new Dictionary<int, Vector2> ();

  private static TouchProxy[] m_currentTouches = new TouchProxy[0];
  // ------------------------------------------------------------------------------------------

  void Start ()
  {
    m_fpsTouchHandle = DebugText.Instance.getDebugTextHandle ();
  }

  protected bool GetRealTouches (out TouchProxy[] touches)
  {
    touches = null;

    if (Input.touchCount == 0)
      return false;
    
    touches = new TouchProxy[Input.touchCount];

    for (int i = 0; i < Input.touchCount; i++)
    {
      Touch t = Input.touches [i];
      touches [i] = new TouchProxy (t.position, t.phase, t.fingerId);

      if (i == 0)
        m_fpsTouchHandle.text = "Touch FPS: " + (1f / t.deltaTime).ToString ("N0");
    }

    return true;
  }

  protected bool GetFakeTouches (out TouchProxy[] touches)
  {
    touches = null;

    // Simulate touch with mouse (if any)
    TouchPhase tf = TouchPhase.Stationary;

    if (Input.GetMouseButtonDown (0))
      tf = TouchPhase.Began;
    else if (Input.GetMouseButton (0))
      tf = TouchPhase.Moved;
    else if (Input.GetMouseButtonUp (0))
      tf = TouchPhase.Ended;
    else
      return false;

    Vector2 mousePosition = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);

    if (Input.GetKey (KeyCode.LeftControl))
    {
      touches = new TouchProxy[2];

      if (tf == TouchPhase.Began)
        m_fakeSecondPosition = new Vector2 (mousePosition.x - 200, mousePosition.y);

      touches [1] = new TouchProxy (m_fakeSecondPosition, tf, 1);

    }
    else
      touches = new TouchProxy[1];

    touches [0] = new TouchProxy (mousePosition, tf, 0);

    return true;
  }

  // Use real touches if available, otherwise simulate touches with mouse.
  // Holding left control is used to simulate double finger touch
  protected bool GetTouches (out TouchProxy[] touches)
  {
    bool AnyTouch = GetRealTouches (out touches);

    if (!AnyTouch)
      AnyTouch = GetFakeTouches (out touches);

    if (AnyTouch)
    {
      // Average the positions (due to unity bug making touches update at another rate than frames)
      // Also calculate the delta position that correspond to this.
      for (int i = 0; i < touches.Length; i++)
      {
        TouchProxy t = touches [i];

        if (t.m_phase == TouchPhase.Began)
        {
          id2positionAverage [t.m_fingerId] = new FixedSizedVector2Queue (3);
          id2positionAverage [t.m_fingerId].Enqueue (t.m_position);

          id2previousPosition [t.m_fingerId] = t.m_position;
          t.m_deltaPosition = new Vector2 (0, 0);
        }
        else
        {
          id2positionAverage [t.m_fingerId].Enqueue (t.m_position);
          t.m_position = id2positionAverage [t.m_fingerId].Average ();

          t.m_deltaPosition = t.m_position - id2previousPosition [t.m_fingerId];
          id2previousPosition [t.m_fingerId] = t.m_position;
        }
      }
    }

    return AnyTouch;
  }

  // ------------------------------------------------------------------------------------------

  void Update ()
  {
    if (!GetTouches (out m_currentTouches))
      return;

    if (m_touchDelegates != null)
    {
      foreach (TouchDelegate d in m_touchDelegates.GetInvocationList ())
      {
        // Once the touch is handled, it should not be used by anyone else
        if (d (m_currentTouches))
          return;
      }
    }

    // If no one else wants the touch, send it to the camera
    Camera.main.gameObject.SendMessage ("HandleTouches", m_currentTouches);
  }


  // ------------------------------------------------------------------------------------------

}
