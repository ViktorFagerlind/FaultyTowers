using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DebugTextHandle
{
  public string text = "";
}

public class DebugText : MonoBehaviour
{
  // Singleton
  private static DebugText instance;
  public static DebugText Instance
  {
    get 
    {
      return instance; 
    }
  }

  private Text m_debugText;
  private List<DebugTextHandle> m_textHandles;


  public DebugTextHandle getDebugTextHandle ()
  {
    DebugTextHandle debugTextHandle = new DebugTextHandle ();
    m_textHandles.Add (debugTextHandle);
    return debugTextHandle;
  }

  // Use this for initialization
  void Awake ()
  {
    instance = this;

    m_debugText = GameObject.Find("DebugText").GetComponent<Text>();

    m_textHandles = new List<DebugTextHandle> ();
  }

  void Update ()
  {
    string debugString = "Debug:\n";

    foreach (DebugTextHandle dth in m_textHandles)
    {
      debugString += dth.text + "\n";
    }

    m_debugText.text = debugString;
  }

}
