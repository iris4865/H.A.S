using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class game_join_click : MonoBehaviour {

    public InputField id;
    public InputField password;
    public InputField password_submit;
    public InputField last_name;
    public InputField first_name;
    public InputField mail;
    public InputField brithday;

    string id_s;
    string password_s;
    string password_submit_s;
    string last_name_s;
    string first_name_s;
    string mail_s;
    string brithday_s;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void click()
    {
        id_s = id.text;
        password_s = password.text;
        password_submit_s = password_submit.text;
        last_name_s = last_name.text;
        first_name_s = first_name.text;
        mail_s = mail.text;
        brithday_s = brithday.text;

        if(password_s == password_submit_s)
        {
            //패스워드가 같지 않다고 메시지 출력.
            return;
        }

        if (id_s == null || password_s == null || last_name_s == null || first_name_s == null || mail_s == null || brithday_s == null)
        {
            //입력하지 않은 내용 있다고 메시지 출력.
            return;
        }
        SceneManager.LoadScene(1);
    }
}
