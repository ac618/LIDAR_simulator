using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class GetRequest : MonoBehaviour
{
    [SerializeField] private string url = "http://192.168.2.191";
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine(get(url));
        }
    }


    IEnumerator get(string url)
    {
        Debug.Log("sending get request to " + url);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
            }
        }
    }
}
