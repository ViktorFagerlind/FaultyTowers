using UnityEngine;
using System.Collections;

public class MoveObject : MonoBehaviour
{
  private Vector3 m_downOffset;
  private Rigidbody2D m_rigidBody;
  private bool m_ongoingTouch = false;

  // ------------------------------------------------------------------------------------------

  void Start ()
  {
    m_rigidBody = this.GetComponent<Rigidbody2D> ();
  }

  // ------------------------------------------------------------------------------------------

  bool GetTouch (out TouchPhase touch, out Vector2 touchPoint)
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

  void Update ()
  {
    TouchPhase  touch;
    Vector2     touchPoint;

    if (!GetTouch (out touch, out touchPoint))
      return;

    switch (touch)
    {
      case TouchPhase.Began:
        HandleGrab (touchPoint);
        break;

      case TouchPhase.Moved:
        HandleMove (touchPoint);
        break;

      case TouchPhase.Canceled:
      case TouchPhase.Ended:
        HandleDrop ();
        break;

    }
  }

  // ------------------------------------------------------------------------------------------

  void HandleGrab (Vector2 screenPosition)
  {
    // Check if this object was hit    
    Ray ray = Camera.main.ScreenPointToRay (screenPosition);
    RaycastHit2D hit = Physics2D.GetRayIntersection (ray);
    if (!hit || hit.transform != this.transform)
      return;

    m_downOffset = transform.position - Camera.main.ScreenToWorldPoint (new Vector3 (screenPosition.x, screenPosition.y, -Camera.main.transform.position.z));

    Cursor.visible = false;
    m_rigidBody.isKinematic = true;
    m_ongoingTouch = true;
  }

  // ------------------------------------------------------------------------------------------

  void HandleMove (Vector2 screenPosition)
  {
    if (!m_ongoingTouch)
      return;

    Vector3 currentPosition = Camera.main.ScreenToWorldPoint (new Vector3 (screenPosition.x, screenPosition.y, -Camera.main.transform.position.z));

    transform.position = currentPosition + m_downOffset;
  }

  // ------------------------------------------------------------------------------------------

  void HandleDrop ()
  {
    if (!m_ongoingTouch)
      return;

    Cursor.visible = true;
    m_rigidBody.isKinematic = false;
    m_ongoingTouch = false;
  }

}
