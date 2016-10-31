using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour
{
  [System.Serializable]
  public class Layer 
  {
    public float     m_scale;             // The proportion of the camera's movement to move the backgrounds by.
    public Transform m_object;
  }

  public Layer[] m_layers;                   // Array of all the layers to be parallaxed.

  //public float smoothing;                 // How smooth the parallax effect should be.
  
  private Transform m_cam;               // Shorter reference to the main camera's transform.
  private Vector3   m_camPreviousPosition;          // The postion of the camera in the previous frame.
  
  
  void Awake ()
  {
    // Setting up the reference shortcut.
    m_cam = Camera.main.transform;
  }
  
  
  void Start ()
  {
    // The 'previous frame' had the current frame's camera position.
    m_camPreviousPosition = m_cam.position;
  }

  void Update ()
  {
    Vector2 distanceMoved = m_camPreviousPosition - m_cam.position;
    
    // For each successive background...
    foreach (Layer l in m_layers)
    {
      // The camera has already moved all objects "distanceMoved" in the eyes of the beholder...
      Vector2 layerDistanceToMove = -distanceMoved + distanceMoved * l.m_scale;

      Vector2 backgroundTargetPos = new Vector2 (l.m_object.position.x, l.m_object.position.y) + layerDistanceToMove;

      // Create a target position which is the background's current position but with it's target x position.
      Vector3 backgroundTargetPos3 = new Vector3 (backgroundTargetPos.x, backgroundTargetPos.y, l.m_object.position.z);

      l.m_object.position = backgroundTargetPos3;
      
      // Lerp the background's position between itself and it's target position.
      //backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPos, smoothing * Time.deltaTime);
    }
    
    // Set the previousCamPos to the camera's position at the end of this frame.
    m_camPreviousPosition = m_cam.position;
  }
}