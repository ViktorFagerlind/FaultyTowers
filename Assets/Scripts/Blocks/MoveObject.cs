using UnityEngine;
using System.Collections;

public class MoveObject : MonoBehaviour
{
  private Vector3     m_downOffset;
  private Rigidbody2D m_rigidBody;
  private bool        m_ongoingTouch = false;

  // ------------------------------------------------------------------------------------------

  void Start ()
  {
    m_rigidBody = this.GetComponents<Rigidbody2D> () [0];
  }

  // ------------------------------------------------------------------------------------------

  void Update ()
  {
    if (Input.touchCount > 0)
    {
      switch (Input.GetTouch (0).phase)
      {
        case TouchPhase.Began:
          Ray ray = Camera.main.ScreenPointToRay (Input.GetTouch (0).position);
          if (Physics.Raycast (ray))
          {
            HandleDown (Input.GetTouch (0).position);
          }
          break;

        case TouchPhase.Moved:
          HandleMove (Input.GetTouch (0).position);
          break;

        case TouchPhase.Canceled:
        case TouchPhase.Ended:
          HandleUp ();
          break;

      }
    }
  }

  // ------------------------------------------------------------------------------------------

  void HandleDown (Vector2 screenPosition)
  {
    m_downOffset = transform.position - Camera.main.ScreenToWorldPoint (new Vector3 (screenPosition.x, screenPosition.y, -Camera.main.transform.position.z));

    Cursor.visible            = false;
    m_rigidBody.isKinematic   = true;
    m_ongoingTouch            = true;
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

  void HandleUp ()
  {
    if (!m_ongoingTouch)
      return;

    Cursor.visible            = true;
    m_rigidBody.isKinematic   = false;
    m_ongoingTouch            = false;
  }

  // ------------------------------------------------------------------------------------------

  void OnMouseDown ()
  {
    HandleDown (new Vector2 (Input.mousePosition.x, Input.mousePosition.y));
  }

  // ------------------------------------------------------------------------------------------

  void OnMouseDrag ()
  { 
    HandleMove (new Vector2 (Input.mousePosition.x, Input.mousePosition.y));
  }

  // ------------------------------------------------------------------------------------------

  void OnMouseUp ()
  {
    HandleUp ();
  }
}
