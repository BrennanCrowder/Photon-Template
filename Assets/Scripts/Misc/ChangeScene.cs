using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
   public string nextSceneName;
   public void ChangeScenes()
    {
        StartCoroutine(WaitSceneTime(nextSceneName));
    }

    IEnumerator WaitSceneTime(string sceneName)
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(sceneName);
    }
}
