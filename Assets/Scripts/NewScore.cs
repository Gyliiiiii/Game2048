using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NewScore : MonoBehaviour {

    private Text NewScoreText;
    int Score;

    void Awake() {
        NewScoreText = this.transform.Find("ScoreText").GetComponent<Text>();
    }
    void Update() {
        if (NumControler._instance.gameState == NumControler.GameState.Playing) {
            Score = NumControler._instance.newScore;
            NewScoreText.text = Score + "";
        }
        
    }

}
