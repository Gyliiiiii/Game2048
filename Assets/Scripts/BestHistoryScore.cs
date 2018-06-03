using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BestHistoryScore : MonoBehaviour {

    int HistoryScore = 0;
    private Text BestText;

    void Awake() {
        BestText = this.transform.Find("BestText").GetComponent<Text>();
    }
    void Start() {
        HistoryScore = NumControler._instance.historyScore;
        BestText.text = HistoryScore+"";
    }
}
