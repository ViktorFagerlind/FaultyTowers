using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SystemCommands : MonoBehaviour 
{
	// Update is called once per frame
	void Update () 
  {
    if (Input.GetKey("escape"))
        Application.Quit();
    
    if (Input.GetKey("p"))
      Application.CaptureScreenshot("Screenshot.png");
    
    if (Input.GetKey (KeyCode.Space))
      ResetScene ();
	}

  public void ResetScene ()
  {
    string currentSceneName = SceneManager.GetActiveScene ().name;
    //SceneManager.UnloadScene (currentSceneName);
    SceneManager.LoadScene (currentSceneName);
  }
}
