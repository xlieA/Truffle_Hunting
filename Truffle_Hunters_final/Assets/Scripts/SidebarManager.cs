using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SidebarManager : MonoBehaviour
{
    [SerializeField] public GameObject sidebarPanel;
    [SerializeField] public GameObject avatarButton;
    [SerializeField] public GameObject exitButton;
    [SerializeField] public int currentXP;
    [SerializeField] public int maxXP;
    [SerializeField] public Image mask;
    [SerializeField] private TextMeshProUGUI username1;
    [SerializeField] private TextMeshProUGUI username2;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private TextMeshProUGUI xp;
    public void UpdateUsername(){
        username1.text = GameManager.Instance.username;
        username2.text = GameManager.Instance.username;
    }
    public void Awake(){
        UpdateUsername();
    }

    public void ActivateSidebar()
    {
        UpdateProgressBar();
        username1.enabled = false;
        avatarButton.SetActive(false);
        sidebarPanel.SetActive(true);
       
    }
    
    public void DeactivateSidebar()
    {
        username1.enabled = true;
        avatarButton.SetActive(true);
        sidebarPanel.SetActive(false);
    }
    
    public void UpdateProgressBar(){
        level.text = "Next level: " + (GameManager.Instance.level+1).ToString();
        xp.text = GameManager.Instance.accquiredXP.ToString() + "/" + (GameManager.Instance.level*100).ToString();
        float fillAmount = (float) GameManager.Instance.accquiredXP / (float)(GameManager.Instance.level*100);
        mask.fillAmount = fillAmount;
    }
  
    void Update()
    {
        
    }
}
