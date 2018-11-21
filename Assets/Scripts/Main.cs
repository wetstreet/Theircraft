using UnityEngine;

public class Main : MonoBehaviour {

	// Use this for initialization
	void Start () {
        InputManager.Init();

        if (NetworkManager.Connect())
        {
            LoginPanel.ShowLoginPanel();
        }
        else
        {
            DisconnectedUI.Show();
        }
    }
}
