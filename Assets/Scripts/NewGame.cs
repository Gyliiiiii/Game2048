using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NewGame : MonoBehaviour{

    void Start() {
        var btn = transform.gameObject.GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }
    private void OnClick()
    {
        Application.LoadLevel(1);
    } 
}
