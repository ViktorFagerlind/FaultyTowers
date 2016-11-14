using UnityEngine;
using System.Collections;

public class MoveObject : MonoBehaviour
{
  private Vector3 m_downOffset;
  private Rigidbody2D m_rigidBody;
  private bool m_ongoingTouch = false;

  private Camera m_camera;

  // ------------------------------------------------------------------------------------------

  void Awake ()
  {
    m_rigidBody = this.GetComponent<Rigidbody2D> ();

    CaptureTouches.m_touchDelegates += HandleTouches;

    m_camera = Camera.main;
  }

  // ------------------------------------------------------------------------------------------

  bool HandleTouches (TouchProxy[] touches)
  {
    switch (touches[0].m_phase)
    {
      case TouchPhase.Began:
        return HandleGrab (touches[0].m_position);

      case TouchPhase.Moved:
      case TouchPhase.Stationary:
        return HandleMove (touches[0].m_position);

      default: // TouchPhase.Ended or TouchPhase.Canceled
        return HandleDrop ();
    }
  }

  // ------------------------------------------------------------------------------------------

  public bool HandleGrab (Vector2 screenPosition)
  {
    // Check if this object was hit    
    Ray ray = Camera.main.ScreenPointToRay (screenPosition);
    RaycastHit2D hit = Physics2D.GetRayIntersection (ray);
    if (!hit || hit.transform != this.transform)
      return false;

    m_downOffset = transform.position - m_camera.ScreenToWorldPoint (new Vector3 (screenPosition.x, screenPosition.y, -m_camera.transform.position.z));

    Cursor.visible = false;
    m_rigidBody.isKinematic = true;
    m_ongoingTouch = true;

    return true;
  }

  // ------------------------------------------------------------------------------------------

  bool HandleMove (Vector2 screenPosition)
  {
    if (!m_ongoingTouch)
      return false;

    Vector3 currentPosition = m_camera.ScreenToWorldPoint (new Vector3 (screenPosition.x, screenPosition.y, -m_camera.transform.position.z));

    transform.position = currentPosition + m_downOffset;

    return true;
  }

  // ------------------------------------------------------------------------------------------

  bool HandleDrop ()
  {
    if (!m_ongoingTouch)
      return false;

    Cursor.visible = true;
    m_rigidBody.isKinematic = false;
    m_ongoingTouch = false;

    return true;
  }

}
