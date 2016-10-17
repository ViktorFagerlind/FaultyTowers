using UnityEngine;
using System.Collections;

public class MoveObject : MonoBehaviour
{
  private Vector3     m_downOffset;
  private Rigidbody2D m_rigidBody;

  void Start ()
  {
    m_rigidBody = this.GetComponents<Rigidbody2D>()[0];
  }

  void OnMouseDown ()
  {
    m_downOffset = transform.position - Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

    Cursor.visible = false;
    m_rigidBody.isKinematic = true;
  }

  void OnMouseDrag ()
  { 
    Vector3 currentPosition = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

    transform.position = currentPosition + m_downOffset;
  }

  void OnMouseUp ()
  {
    Cursor.visible = true;
    m_rigidBody.isKinematic = false;
  }
}
