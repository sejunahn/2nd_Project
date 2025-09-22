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

        // �ε� �߿��� ���� ������ �ʵ��� ����
        asyncLoad.allowSceneActivation = true;

        // �ε��� �Ϸ�� ������ ��ٸ�
        while (!asyncLoad.isDone)
        {
            // �ʿ��ϴٸ� ���⼭ �ε� ���൵ ǥ�� ���� (asyncLoad.progress)
            yield return null;
        }
    }
}
