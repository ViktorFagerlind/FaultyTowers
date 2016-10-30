using UnityEngine;
using System.Collections;

public class ItemCreator : MonoBehaviour
{
  Transform m_objectToCreateOnUpdate = null;

  public void CreateItemAtRandomPosition (Transform objectToCreate)
  {
    Instantiate (objectToCreate, new Vector3 (Random.Range (-2f, 2f), 5f, 0f), Quaternion.identity);
  }

  public void CreateItemAtPointer (Transform objectToCreate)
  {
    m_objectToCreateOnUpdate = objectToCreate;
  }

  void Update ()
  {
    if (m_objectToCreateOnUpdate == null)
      return;

    TouchProxy[] touches = CaptureTouches.currentTouches;

    if (touches.Length > 0)
    {
      Vector3 position = Camera.main.ScreenToWorldPoint (new Vector3 (touches [0].m_position.x, touches [0].m_position.y, -Camera.main.transform.position.z));

      Transform newObject = Instantiate (m_objectToCreateOnUpdate, position, Quaternion.identity) as Transform;

      // TODO: Maybe not the most beautiful solution, should be handled in a better way...
      newObject.gameObject.GetComponent<MoveObject> ().HandleGrab (touches [0].m_position);

      m_objectToCreateOnUpdate = null;
    }

  }

}
