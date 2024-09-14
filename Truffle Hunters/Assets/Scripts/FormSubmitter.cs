using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using SimpleJSON;

public class FormSubmitter : MonoBehaviour
{
    [SerializeField] private TMP_InputField username_input;
    [SerializeField] private TMP_InputField password_input;
    [SerializeField] private TextMeshProUGUI alertText;
    [SerializeField] private string login_url = "http://16.170.112.13:8000/login";
    [SerializeField] private string register_url = "http://16.170.112.13:8000/register";

    public void OnLoginClick(){
        Debug.Log("trying login");
        StartCoroutine(Login());
    }
 
    IEnumerator Login (){
         string username = username_input.text;
         string password = password_input.text;
         WWWForm form = new WWWForm();
         form.AddField("username", username);
         form.AddField("password", password);
         UnityWebRequest request = UnityWebRequest.Post(login_url, form);
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
             ProcessLoginResponse(request.downloadHandler.text);
         }
         else
         {
             alertText.text = "Error connecting to the server...";
         }
 
         yield return null;
    }

    void LoadMapScene(){
        SceneManager.LoadScene("Map");
    }

    void ProcessLoginResponse(string response)
    {
        JSONNode node = JSON.Parse(response);
        string code = node["code"];
        Debug.Log(response);
        switch (code)
        {
            case "5":
                alertText.text = "Successfully login to your account";
                //save user data
                GameManager.Instance.username = username_input.text;
                GameManager.Instance.password = password_input.text;
                GameManager.Instance.level = node["level"];
                GameManager.Instance.accquiredXP = node["accquiredXP"];
                LoadMapScene();
                break;
            case "1":
                alertText.text = "this username hasn't been registered plz register first";
                break;
            case "3":
                alertText.text = "wrong password";
                break;
            default:
                alertText.text = "Corruption detected";
                break;
        }
        
    }
 
    public void OnRegisterClick(){
        Debug.Log("trying register");
        //disable the login button //erease the text 
        StartCoroutine(Register());
    }

    IEnumerator Register (){
        string username = username_input.text;
        string password = password_input.text;
        Debug.Log(username);
        Debug.Log(password);
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        UnityWebRequest request = UnityWebRequest.Post(register_url, form);
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
            ProcessRegisterResponse(request.downloadHandler.text);
        }
        else
        {
            Debug.Log(request);
            alertText.text = "Error connecting to the server...";
        }


        yield return null;
    }

    void ProcessRegisterResponse(string response){
        JSONNode node = JSON.Parse(response);
        string code = node["code"];
        switch (code)
        {
            case "0":
            alertText.text = "Successfully created your account. Please login";
             GameManager.Instance.username = username_input.text;
             GameManager.Instance.password = password_input.text;
            GameManager.Instance.level = node["level"];
                GameManager.Instance.accquiredXP = node["accquiredXP"];
            LoadMapScene();
            break;
            case "2":
                alertText.text = "Username is already taken";
                break;
            case "3":
                alertText.text = "Password is unsafe";
                break;
            default:
                alertText.text = "Corruption detected";
                break;
        }
    }
}

