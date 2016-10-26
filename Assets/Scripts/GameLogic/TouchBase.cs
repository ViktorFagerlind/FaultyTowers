using UnityEngine;
using System.Collections;

public class TouchProxy
{
  public Vector2      m_position;
  public TouchPhase   m_phase;
  public int          m_fingerId;

  public TouchProxy (Vector2 position, TouchPhase phase, int fingerId)
  {
    m_position  = position;
    m_phase     = phase;
    m_fingerId  = fingerId;
  }
}

public class TouchBase : MonoBehaviour
{

  // ------------------------------------------------------------------------------------------

  private TouchProxy m_savedTouch;

  // ------------------------------------------------------------------------------------------

  protected bool GetTouches (out TouchProxy[] touches)
  {
    touches = null;

    if (Input.touchCount > 0) // Use real touch
    {
      touches = new TouchProxy[Input.touchCount];
      
      for (int i = 0; i < Input.touchCount; i++)
      {
        Touch t = Input.touches [i];
        touches[i] = new TouchProxy (t.position, t.phase, t.fingerId);
      }

      return true;
    }


    // Simulate touch with mouse
    TouchPhase tf;

    if (Input.GetMouseButtonDown (0))
      tf = TouchPhase.Began;
    else if (Input.GetMouseButton (0))
      tf = TouchPhase.Moved;
    else if (Input.GetMouseButtonUp (0))
      tf = TouchPhase.Ended;
    else
    {
      touches = new TouchProxy[0];
      return false;
    }

    Vector2 mousePosition = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);

    if (Input.GetKey (KeyCode.LeftControl))
    {
      touches = new TouchProxy[2];

      if (tf == TouchPhase.Began)
      {
        touches [1] = new TouchProxy (mousePosition, tf, 1);
        m_savedTouch = touches [1];
      } 
      else
        touches [1] = m_savedTouch;

    }
    else
      touches = new TouchProxy[1];
    
    touches[0] = new TouchProxy (mousePosition, tf, 0);  

    return true;
  }

  // ------------------------------------------------------------------------------------------

}
