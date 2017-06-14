using HatchlingNet;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Header;

public class game_login_click : MonoBehaviour
{

    public InputField id;
    public InputField password;

    string id_s;
    string password_s;

    NetworkManager networkManager = null;

    void Awake()
    {
        networkManager = NetworkManager.GetInstance;
    }

    void Start()
    {
        password.contentType = InputField.ContentType.Password;
        
    }

    void Update()
    {

    }

    public void click()
    {
        print("login start");
        id_s = id.text;
        password_s = password.text;

        if (id_s == null || password_s == null)
        {
            //id나password를 입력하라는 메시지 출력.
            return;
        }

//        NetworkManager networkManager = NetworkManager.GetInstance.GetComponent<;
        networkManager = NetworkManager.GetInstance;

        if (networkManager != null)
        {
            Packet msg = PacketBufferManager.Instance.Pop((short)PROTOCOL.Login, (short)SEND_TYPE.Single);
            msg.Push(id_s);
            msg.Push(password_s);

            print("login Send");
            networkManager.Send(msg);

            networkManager.userID = id_s;
        }

        //SceneManager.LoadScene(3);
    }
    public void click_1()
    {
        SceneManager.LoadScene(2);
    }
}
