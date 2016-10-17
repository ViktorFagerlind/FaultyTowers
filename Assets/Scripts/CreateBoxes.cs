using UnityEngine;
using System.Collections;

public class CreateBoxes : MonoBehaviour
{
  public Transform m_createObj;
	
  public void CreateBox ()
  {
    Instantiate (m_createObj, new Vector3 (Random.Range (-2f, 2f), 5f, 0f), Quaternion.identity);
  }

}
