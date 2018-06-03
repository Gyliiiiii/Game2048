using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Num : MonoBehaviour {

    public int NumIndex=2;
    public int InitX,InitY;
    public Sprite[] ImageSouce;
    private Image NumImage;
    private Transform Mark;
    public bool isDestory=false;
    public float moveTime = 0.4f;

    void Awake() {
        NumImage = this.GetComponent<Image>();       
    }
    void Start() {
        Mark = this.transform.parent.Find("Mark").transform;
        CreateNum();
        InitPos();
    }
    void Update() {
        UpdateImage();
    }

    #region 生成动画-CreateNum
    private void CreateNum()
    {
        iTween.ScaleTo(this.gameObject, new Vector3(1, 1, 1), 0.2f);
    }
    #endregion

    #region 初始化位置-InitPos
    private void InitPos() {
        this.transform.localPosition = new Vector3(Mark.localPosition.x + 84 * InitX, Mark.localPosition.y - 84 * InitY, Mark.localPosition.z);
    }
    # endregion

    #region 移动位置-UpdatePos(int x,int y)
    public void UpdatePos(int x,int y)
    {
        //通过itween进行移动，但是由于itween只能通过position来移动，会出现适配问题，故尝试编写运动代码
        Hashtable args = new Hashtable();
        args.Add("time", moveTime);
        args.Add("islocal", true);
        args.Add("position", new Vector3(Mark.localPosition.x + 84 * x, Mark.localPosition.y - 84 * y, Mark.localPosition.z));
        args.Add("oncomplete", "SetUpXAndY");
        iTween.MoveTo(this.gameObject, args);

        ////Vector3 toLocalPos = new Vector3(Mark.localPosition.x + 84 * x, Mark.localPosition.y - 84 * y, Mark.position.z);
        //float timer = 0f;
        //float distance = 0f;

        InitX = x; InitY = y;
    }
    #endregion

    #region 移动完成后设置属性/删除-SetUpXAndY
    private void SetUpXAndY() {
        //print("运动完成");
        //InitPos();
        //如果要合并删除，就进行删除
        if (isDestory) {
            Destroy(this.gameObject);
        }
    }
    #endregion

    #region 根据NumIndex改变NumImage-UpdateImage
    private void UpdateImage()
    {
        NumImage.sprite = ImageSouce[CorrespondImageAndIndex(NumIndex)];
    }
    #endregion

    #region 将NumIndex与ImageSouce一一对应-CorrespondImageAndIndex(int NumIndex )
    private int CorrespondImageAndIndex(int NumIndex ){
        switch (NumIndex) { 
            case 2:
                return 0;
            case 4:
                return 1;
            case 8:
                return 2;
            case 16:
                return 3;
            case 32:
                return 4;
            case 64:
                return 5;
            case 128:
                return 6;
            case 256:
                return 7;
            case 512:
                return 8;
            case 1024:
                return 9;
            case 2048:
                return 10;
            default:
                return 0;
        }
    }
    #endregion
}
