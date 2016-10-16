using UnityEngine;
using System.Collections;

public class CreateBoxes : MonoBehaviour
{
  public Transform m_createObj;

  // Use this for initialization
  void Start ()
  {
	
  }
	

  void Update()
  {
    if (Input.GetButtonDown ("Fire1") || (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began))
    {
      Instantiate (m_createObj, new Vector3 (Random.Range (-2f, 2f), 5f, 0f), Quaternion.identity);
    }

  }
}
