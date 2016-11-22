using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody2D))]

public class MoveableObject : MonoBehaviour
{
  private Vector3 m_downOffset;
  private Camera m_camera;
  protected Rigidbody2D m_rigidBody;

  protected bool m_ongoingMove = false;

  protected bool MoveAllowed {set; get;}


  // ------------------------------------------------------------------------------------------

  protected virtual void OnGrabbed () {}

  protected virtual void OnMoved () {}

  protected virtual void OnDropped () {}

  // ------------------------------------------------------------------------------------------

  protected virtual void Awake ()
  {
    MoveAllowed = true;

    m_rigidBody = this.GetComponent<Rigidbody2D> ();

    CaptureTouches.m_touchDelegates += HandleTouches;

    m_camera = Camera.main;
  }

  // ------------------------------------------------------------------------------------------

  protected virtual void OnDestroy ()
  {
    CaptureTouches.m_touchDelegates -= HandleTouches;
  }

  // ------------------------------------------------------------------------------------------

  bool HandleTouches (TouchProxy[] touches)
  {
    if (!MoveAllowed)
      return false;

    if (touches.Length != 1)
      return false;

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
    m_ongoingMove = true;

    OnGrabbed ();

    return true;
  }

  // ------------------------------------------------------------------------------------------

  bool HandleMove (Vector2 screenPosition)
  {
    if (!m_ongoingMove)
      return false;

    Vector3 currentPosition = m_camera.ScreenToWorldPoint (new Vector3 (screenPosition.x, screenPosition.y, -m_camera.transform.position.z));

    transform.position = currentPosition + m_downOffset;

    OnMoved ();

    return true;
  }

  // ------------------------------------------------------------------------------------------

  bool HandleDrop ()
  {
    if (!m_ongoingMove)
      return false;

    Cursor.visible = true;
    m_rigidBody.isKinematic = false;
    m_ongoingMove = false;

    OnDropped ();

    return true;
  }

}
