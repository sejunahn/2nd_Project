using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashManager : MonoBehaviour
{
    public void GoToGame()
    {
        StartCoroutine(LoadSceneAsync("1"));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        // 로딩 중에도 씬을 멈추지 않도록 설정
        asyncLoad.allowSceneActivation = true;

        // 로딩이 완료될 때까지 기다림
        while (!asyncLoad.isDone)
        {
            // 필요하다면 여기서 로딩 진행도 표시 가능 (asyncLoad.progress)
            yield return null;
        }
    }
}
