using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using SimpleJSON;

public class UpdateUser: MonoBehaviour 
{
    [SerializeField] private string update_url = "http://16.170.112.13:8000/updateuser";

    public void UpdateUserInfo(){
        StartCoroutine(UpdateUserInServer());
    }

    IEnumerator UpdateUserInServer(){
         string username = GameManager.Instance.username;
         string password = GameManager.Instance.password;
         int accquiredXP = GameManager.Instance.accquiredXP;
         int level = GameManager.Instance.level;
        
         WWWForm form = new WWWForm();
         form.AddField("username", username);
        form.AddField("password", password);
        form.AddField("accquiredXP", accquiredXP);
        form.AddField("level", level);
         UnityWebRequest request = UnityWebRequest.Post(update_url, form);
         var handler = request.SendWebRequest();
         float startTime = 0.0f;
         while (!handler.isDone)
         {
             startTime += Time.deltaTime;
 
             if (startTime > 10.0f)
             {
                 break;
             }
 
             yield return null;
         }
         if (request.result == UnityWebRequest.Result.Success)
         {
            Debug.Log(request.downloadHandler.text);
         }
      
    }
}
