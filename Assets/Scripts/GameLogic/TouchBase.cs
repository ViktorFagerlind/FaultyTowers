using UnityEngine;
using System.Collections;

public class TouchBase : MonoBehaviour
{

  // ------------------------------------------------------------------------------------------

  protected bool GetTouch (out TouchPhase touch, out Vector2 touchPoint)
  {
    bool anyTouch;

    touchPoint = new Vector2 ();
    
    if (Input.touchCount > 0) // Use real touch
    {
      touch = Input.GetTouch (0).phase;
      touchPoint = Input.GetTouch (0).position;
      return true;
    }
    else // Simulate touch with mouse
    {
      anyTouch = true;
      if (Input.GetMouseButtonDown (0))
        touch = TouchPhase.Began;
      else if (Input.GetMouseButton (0))
        touch = TouchPhase.Moved;
      else if (Input.GetMouseButtonUp (0))
        touch = TouchPhase.Ended;
      else
      {
        anyTouch = false;
        touch = TouchPhase.Stationary;
      }

      touchPoint = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
    }

    return anyTouch;
  }

  // ------------------------------------------------------------------------------------------

}
