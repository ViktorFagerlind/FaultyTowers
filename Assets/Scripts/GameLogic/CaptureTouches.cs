using UnityEngine;
using System.Collections;

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
}

public delegate bool TouchDelegate (TouchProxy[] touches);

public class CaptureTouches : MonoBehaviour
{
  [HideInInspector]
  public static TouchDelegate m_touchDelegates = null;


  // ------------------------------------------------------------------------------------------

  public static TouchProxy[] currentTouches
  {
    get { return m_currentTouches; }
  }
  // ------------------------------------------------------------------------------------------

  private Vector2 m_fakeSecondPosition;
  private Vector2 m_previousMousePosition = new Vector2 (0, 0);

  private static TouchProxy[] m_currentTouches = new TouchProxy[0];
  // ------------------------------------------------------------------------------------------

  // Use real touches if available, otherwise simulate touches with mouse.
  // Holding left control is used to simulate double finger touch
  protected bool GetTouches (out TouchProxy[] touches)
  {
    touches = new TouchProxy[0];

    if (Input.touchCount > 0) // Use real touch
    {
      touches = new TouchProxy[Input.touchCount];
      
      for (int i = 0; i < Input.touchCount; i++)
      {
        Touch t = Input.touches [i];
        touches [i] = new TouchProxy (t.position, t.phase, t.fingerId, t.deltaPosition);
      }

      return true;
    }


    bool anySimulatedTouch = true;

    // Simulate touch with mouse
    TouchPhase tf = TouchPhase.Stationary;

    if (Input.GetMouseButtonDown (0))
      tf = TouchPhase.Began;
    else if (Input.GetMouseButton (0))
      tf = TouchPhase.Moved;
    else if (Input.GetMouseButtonUp (0))
      tf = TouchPhase.Ended;
    else
      anySimulatedTouch = false;

    Vector2 mousePosition = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);

    if (anySimulatedTouch)
    {
      if (Input.GetKey (KeyCode.LeftControl))
      {
        touches = new TouchProxy[2];

        if (tf == TouchPhase.Began)
          m_fakeSecondPosition = new Vector2 (mousePosition.x - 200, mousePosition.y);

        touches [1] = new TouchProxy (m_fakeSecondPosition, tf, 1, new Vector2 (0, 0));

      }
      else
        touches = new TouchProxy[1];
      
      touches [0] = new TouchProxy (mousePosition, tf, 0, mousePosition - m_previousMousePosition);
    }

    m_previousMousePosition = mousePosition;

    return anySimulatedTouch;
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
