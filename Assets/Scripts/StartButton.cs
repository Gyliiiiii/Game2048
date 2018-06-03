using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StartButton : MonoBehaviour,IPointerDownHandler,IPointerUpHandler{

    private Transform ButtonStart;
    private Image ButtonImage;

    private bool isPointerDown;
    private Vector3 InitMousePos;


    void Awake() {
        ButtonStart = this.transform.Find("Start").transform;
        ButtonImage=ButtonStart.GetComponent<Image>();
    }
    void Start() {
        ButtonStart.localPosition = new Vector3(ButtonStart.localPosition.x, 60f, ButtonStart.transform.localPosition.z);
        InitMousePos = Vector3.zero;
    }
    void Update() {
        UpdateButton();
    }

    //根据Y值来改变游戏状态
    private void UpdateButton()
    {
        if (isPointerDown)
        {
            if (ButtonStart.localPosition.y > 60f || ButtonStart.localPosition.y < -60f)
            {
                float newY = (Mathf.Abs(ButtonStart.localPosition.y) / ButtonStart.localPosition.y) * 60f;
                if (newY <= 0) {
                    ButtonImage.color = new Color(104, 255, 0, 255);
                }
                ButtonStart.localPosition = new Vector3(ButtonStart.localPosition.x, newY, ButtonStart.transform.localPosition.z);
            }
        }
        else {
            float y = ButtonStart.localPosition.y;
            if (y <= 0)
            {
                ButtonStart.localPosition = new Vector3(ButtonStart.localPosition.x, -60f, ButtonStart.transform.localPosition.z);
                ButtonImage.color = new Color(104, 255, 0, 255);
            }
            else
            {
                ButtonStart.localPosition = new Vector3(ButtonStart.localPosition.x, 60f, ButtonStart.transform.localPosition.z);
            }
            if (ButtonStart.localPosition.y == -60f) {
                this.GetComponent<ScrollRect>().enabled = false;
                StartCoroutine(WaitAndSkip());
            }
            
        }
    }
    
    //控制场景等待、跳转
    private IEnumerator WaitAndSkip() {
        yield return new WaitForSeconds(0.5f);
        Application.LoadLevel(1);
    }


    //检测鼠标按下与抬起
    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
    }
    public void OnPointerUp(PointerEventData eventData) {
        isPointerDown = false;
    }
}

