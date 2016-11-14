using UnityEngine;
using System.Collections;

public class SetFps : MonoBehaviour
{

  public int m_frameRate = 60;

  DebugTextHandle m_fpsHandle;

  FixedSizedFloatQueue m_fpsAverage = new FixedSizedFloatQueue (10);


  // Use this for initialization
  void Start()
  {
    Application.targetFrameRate = m_frameRate;

    m_fpsHandle = DebugText.Instance.getDebugTextHandle ();
  }

  // Update is called once per frame
  void Update()
  {
    m_fpsAverage.Enqueue (1f / Time.deltaTime);

    m_fpsHandle.text = "FPS: " + m_fpsAverage.Average ().ToString ("N0");
  }
}
