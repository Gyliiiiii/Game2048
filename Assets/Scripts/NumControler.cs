using UnityEngine;
using System.Collections;

public class NumControler : MonoBehaviour {

    public enum TouchDir { 
        Top,
        Bottom,
        Right,
        Left,
        None
    }
    public enum GameState { 
        Loading,
        Playing,
        End
    }

    #region 数据检测数组-NumIndexArray-NumAssemblyArray
    private int[][] NumIndexArray = new int[4][]{
        new int[4]{0,0,0,0},
        new int[4]{0,0,0,0},
        new int[4]{0,0,0,0},
        new int[4]{0,0,0,0}
    };
    private Num[][] NumAssemblyArray = new Num[4][]{
        new Num[4]{null,null,null,null},
        new Num[4]{null,null,null,null},
        new Num[4]{null,null,null,null},
        new Num[4]{null,null,null,null}
    };
    #endregion
    
    public GameObject NumPrefab;
    public GameState gameState;
    public Vector3 mouseDownPosition;
    public static NumControler _instance;

    public int newScore=0;
    public int historyScore = 0;

    void Awake() {
        _instance = this;
        gameState = GameState.Loading;
        GetHistoryScore();
    } 
    void Start() {
        CreateNum(TouchDir.None);
        CreateNum(TouchDir.None);
        StatisticsScore();
        gameState = GameState.Playing;
    }
    void Update() {
        #region 状态控制
        switch (gameState)
        {
            case GameState.Loading:
                break;
            case GameState.Playing:
                CheckAndMoveNum();
                break;
            case GameState.End:
                break;
        }
        #endregion 

        #region 按键输出表格
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OutputNumIndexArray();
        }
        #endregion   
    }

    #region 检测鼠标按下并通过鼠标抬起检测方法进行移动-CheckAndMoveNum
    private void CheckAndMoveNum() {
        if (Input.GetMouseButtonDown(0))
        {
            mouseDownPosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0) == false) return;

        //如果鼠标抬起则
        TouchDir touchDir=GetTouchDir();
        ToMove(touchDir);
    }
    #endregion

    #region 判断鼠标滑动方向- GetTouchDir
    private TouchDir GetTouchDir() {
        if (Input.GetMouseButtonUp(0)) {
            //print(0);
            Vector3 TouchOffset = Input.mousePosition - mouseDownPosition;
            if (Mathf.Abs(TouchOffset.x) >= Mathf.Abs(TouchOffset.y) && Mathf.Abs(TouchOffset.x) > 80)
            {
                if (TouchOffset.x > 0)
                {
                    return TouchDir.Right;
                }
                else if (TouchOffset.x < 0)
                {
                    return TouchDir.Left;
                }
            }
            if (Mathf.Abs(TouchOffset.x) < Mathf.Abs(TouchOffset.y) && Mathf.Abs(TouchOffset.y) > 80)
            {
                if (TouchOffset.y > 0)
                {
                    return TouchDir.Top;
                }
                else if (TouchOffset.y < 0)
                {
                    return TouchDir.Bottom;
                }
            } 
        }
        return TouchDir.None;
    }
    #endregion

    #region 生成Num-CreateNum(TouchDir touchDir)-(int x,int y,int Index)
    private void CreateNum(TouchDir touchDir) {
        Vector2 newPos = SelectEmptyPos(touchDir);
        if (newPos.x != 9 || newPos.y != 9)
        {
            GameObject item = Instantiate(NumPrefab) as GameObject;
            Num numItem = item.GetComponent<Num>();
            numItem.NumIndex = 2;
            numItem.InitX = (int)newPos.x;
            numItem.InitY = (int)newPos.y;
            item.transform.parent = this.transform;
            NumIndexArray[(int)newPos.x][(int)newPos.y] = 2;
            NumAssemblyArray[(int)newPos.x][(int)newPos.y] = numItem;
            StatisticsScore();
        }
        else { 
            //游戏失败//newX、newY均为9并且TouchDir不为none
            gameState = GameState.End;
            bool isEnd1 = true;
            bool isEnd2 = true;
            int index=0;

            #region 循环判断是否有相邻的相同数字
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (index == 0)
                    {
                        index = NumIndexArray[x][y];
                    }
                    else
                    {
                        if (index != NumIndexArray[x][y])
                        {
                            index = NumIndexArray[x][y];
                        }
                        else
                        {
                            isEnd1 = false;
                            break;
                        }
                    }
                }
            }
            index = 0;
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    if (index == 0)
                    {
                        index = NumIndexArray[x][y];
                    }
                    else
                    {
                        if (index != NumIndexArray[x][y])
                        {
                            index = NumIndexArray[x][y];
                        }
                        else
                        {
                            isEnd2 = false;
                            break;
                        }
                    }
                }
            }
            #endregion

            if (!isEnd1 || !isEnd2) {
                gameState = GameState.Playing;
            }
            StatisticsScore();
        } 
    }
    private void CreateNum(int x,int y,int Index)
    {
            GameObject item = Instantiate(NumPrefab) as GameObject;
            Num numItem = item.GetComponent<Num>();
            numItem.NumIndex = Index;
            numItem.InitX = x;
            numItem.InitY = y;
            item.transform.parent = this.transform;
            NumIndexArray[x][y] = Index;
            NumAssemblyArray[x][y] = numItem;
            StatisticsScore();
    }
    #endregion

    #region 根据不同的情况，进行移动-ToMove(TouchDir touchDir)
    private void ToMove(TouchDir touchDir) {
        switch (touchDir)
        {
            case TouchDir.Top:
                #region ToTop
                for (int x = 0; x < 4; x++)
                {
                    int emptyCount = 0;
                    Num preNum = null;
                    for (int y = 0; y < 4; y++)
                    {
                        if (NumIndexArray[x][y] == 0)
                        {
                            emptyCount++;
                        }
                        else
                        {
                            //初始Num
                            if (preNum == null) {
                                preNum = NumAssemblyArray[x][y];
                            }
                            //合并
                            else if (NumAssemblyArray[x][y].NumIndex == preNum.NumIndex)
                            {
                                emptyCount++;
                                //启动删除标记
                                NumAssemblyArray[x][y].isDestory = true;
                                preNum.isDestory = true;
                                //将响应表格中的引用清除

                                preNum = null;
                                //延时并创建一个num
                                //CreateNum(x + emptyCount, y, NumAssemblyArray[x][y].NumIndex * 2);
                                StartCoroutine(WaitAndCreateNum(x, y - emptyCount, NumAssemblyArray[x][y].NumIndex * 2, NumAssemblyArray[x][y].moveTime));
                            }
                            else 
                            {
                                preNum = NumAssemblyArray[x][y];    
                            }
                            
                            //移动
                            int newX = NumAssemblyArray[x][y].InitX;
                            int newY = NumAssemblyArray[x][y].InitY - emptyCount;
                            NumAssemblyArray[x][y].UpdatePos(newX,newY);
                            if (newX != x || newY != y)
                            {
                                NumAssemblyArray[newX][newY] = NumAssemblyArray[x][y];
                                NumIndexArray[newX][newY] = NumIndexArray[x][y];
                                NumAssemblyArray[x][y] = null;
                                NumIndexArray[x][y] = 0;
                            }
                            
                        }
                    }
                }
                CreateNum(TouchDir.Top);
                #endregion
                break;
            case TouchDir.Bottom:
                #region ToButtom
                for (int x = 0; x < 4; x++)
                {
                    int emptyCount = 0;
                    Num preNum = null;
                    for (int y = 3; y >= 0; y--)
                    {
                        if (NumIndexArray[x][y] == 0)
                        {
                            emptyCount++;
                        }
                        else
                        {
                            //初始Num
                            if (preNum == null) {
                                preNum = NumAssemblyArray[x][y];
                            }
                            //合并
                            else if (NumAssemblyArray[x][y].NumIndex == preNum.NumIndex)
                            {
                                emptyCount++;
                                //启动删除标记
                                NumAssemblyArray[x][y].isDestory = true;
                                preNum.isDestory = true;
                                //将响应表格中的引用清除

                                preNum = null;
                                //延时并创建一个num
                                //CreateNum(x + emptyCount, y, NumAssemblyArray[x][y].NumIndex * 2);
                                StartCoroutine(WaitAndCreateNum(x, y + emptyCount, NumAssemblyArray[x][y].NumIndex * 2, NumAssemblyArray[x][y].moveTime));
                            }
                            else 
                            {
                                preNum = NumAssemblyArray[x][y];    
                            }
                            
                            //移动
                            int newX = NumAssemblyArray[x][y].InitX;
                            int newY = NumAssemblyArray[x][y].InitY + emptyCount;
                            NumAssemblyArray[x][y].UpdatePos(newX,newY);
                            if (newX != x || newY != y)
                            {
                                NumAssemblyArray[newX][newY] = NumAssemblyArray[x][y];
                                NumIndexArray[newX][newY] = NumIndexArray[x][y];
                                NumAssemblyArray[x][y] = null;
                                NumIndexArray[x][y] = 0;
                            }
                            
                        }
                    }
                }
                CreateNum(TouchDir.Bottom);
                #endregion
                break;
            case TouchDir.Left:
                #region ToLeft
                for (int y = 0; y < 4; y++)
                {
                    int emptyCount = 0;
                    Num preNum = null;
                    for (int x = 0; x < 4; x++)
                    {
                        if (NumIndexArray[x][y] == 0)
                        {
                            emptyCount++;
                        }
                        else
                        {
                            //初始Num
                            if (preNum == null) {
                                preNum = NumAssemblyArray[x][y];
                            }
                            //合并
                            else if (NumAssemblyArray[x][y].NumIndex == preNum.NumIndex)
                            {
                                emptyCount++;
                                //启动删除标记
                                NumAssemblyArray[x][y].isDestory = true;
                                preNum.isDestory = true;
                                //将响应表格中的引用清除

                                preNum = null;
                                //延时并创建一个num
                                //CreateNum(x + emptyCount, y, NumAssemblyArray[x][y].NumIndex * 2);
                                StartCoroutine(WaitAndCreateNum(x - emptyCount, y, NumAssemblyArray[x][y].NumIndex * 2, NumAssemblyArray[x][y].moveTime));
                            }
                            else 
                            {
                                preNum = NumAssemblyArray[x][y];    
                            }
                            
                            //移动
                            int newX = NumAssemblyArray[x][y].InitX - emptyCount;
                            int newY = NumAssemblyArray[x][y].InitY;
                            NumAssemblyArray[x][y].UpdatePos(newX,newY);
                            if (newX != x || newY != y)
                            {
                                NumAssemblyArray[newX][newY] = NumAssemblyArray[x][y];
                                NumIndexArray[newX][newY] = NumIndexArray[x][y];
                                NumAssemblyArray[x][y] = null;
                                NumIndexArray[x][y] = 0;
                            }
                            
                        }
                    }
                }
                CreateNum(TouchDir.Left);
                #endregion
                break;
            case TouchDir.Right:
                #region ToRight
                for (int y = 0; y < 4; y++)
                {
                    int emptyCount = 0;
                    Num preNum = null;
                    for (int x = 3; x >= 0; x--)
                    {
                        if (NumIndexArray[x][y] == 0)
                        {
                            emptyCount++;
                        }
                        else
                        {
                            //初始Num
                            if (preNum == null) {
                                preNum = NumAssemblyArray[x][y];
                            }
                            //合并
                            else if (NumAssemblyArray[x][y].NumIndex == preNum.NumIndex)
                            {
                                emptyCount++;
                                //启动删除标记
                                NumAssemblyArray[x][y].isDestory = true;
                                preNum.isDestory = true;
                                //将响应表格中的引用清除

                                preNum = null;
                                //延时并创建一个num
                                //CreateNum(x + emptyCount, y, NumAssemblyArray[x][y].NumIndex * 2);
                                StartCoroutine(WaitAndCreateNum(x + emptyCount, y, NumAssemblyArray[x][y].NumIndex * 2, NumAssemblyArray[x][y].moveTime));
                            }
                            else 
                            {
                                preNum = NumAssemblyArray[x][y];    
                            }
                            
                            //移动
                            int newX = NumAssemblyArray[x][y].InitX + emptyCount;
                            int newY = NumAssemblyArray[x][y].InitY;
                            NumAssemblyArray[x][y].UpdatePos(newX,newY);
                            if (newX != x || newY != y)
                            {
                                NumAssemblyArray[newX][newY] = NumAssemblyArray[x][y];
                                NumIndexArray[newX][newY] = NumIndexArray[x][y];
                                NumAssemblyArray[x][y] = null;
                                NumIndexArray[x][y] = 0;
                            }
                            
                        }
                    }
                }
                CreateNum(TouchDir.Right);
                #endregion
                break;
        }
    }
    #endregion

    #region 根据不同的情况，选择一个空的位置-SelectEmptyPos(TouchDir touchDir)
    //private Vector2 SelectEmptyPos() { 
    //    int emptyNumCount = 0;
    //    int newX=9, newY=9;//若输出9，9；则已经没有空位，游戏结束
    //    for (int x = 0; x < 4; x++)
    //    {
    //        for (int y = 0; y < 4; y++)
    //        {
    //            if (NumIndexArray[x][y] == 0) { 
    //                emptyNumCount++;
    //            }
    //        }
    //    }
    //    if (emptyNumCount != 0)
    //    {
    //        int randomIndex = Random.Range(1, emptyNumCount + 1);
    //        int Count = 0;
    //        for (int x = 0; x < 4; x++)
    //        {
    //            for (int y = 0; y < 4; y++)
    //            {
    //                if (NumIndexArray[x][y] == 0)
    //                {
    //                    Count++;
    //                    if (Count == randomIndex)
    //                    {
    //                        newX = x;
    //                        newY = y;
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    return new Vector2(newX, newY);
    //}

    //返回值还未设置
    private Vector2 SelectEmptyPos(TouchDir touchDir)
    {
        int newX = 9, newY = 9;
        if (touchDir == TouchDir.Top)
        {
            #region moveToTop
            int emptyNumCount = 0;
            int y = 3;
            for (int x = 0; x < 4; x++)
            {
                if (NumIndexArray[x][y] == 0)
                {
                    emptyNumCount++;
                }
            }
            if (emptyNumCount != 0)
            {
                int randomIndex = Random.Range(1, emptyNumCount + 1);
                int count = 0;
                for (int x = 0; x < 4; x++)
                {
                    if (NumIndexArray[x][y] == 0)
                    {
                        count++;
                        if (count == randomIndex)
                        {
                            newX = x;
                            newY = y;
                        }
                    }
                }
            }
            #endregion

        }
        else if (touchDir == TouchDir.Bottom)
        {
            #region moveToBottom
            int emptyNumCount = 0;
            int y = 0;
            for (int x = 0; x < 4; x++)
            {
                if (NumIndexArray[x][y] == 0)
                {
                    emptyNumCount++;
                }
            }
            if (emptyNumCount != 0)
            {
                int randomIndex = Random.Range(1, emptyNumCount + 1);
                int count = 0;
                for (int x = 0; x < 4; x++)
                {
                    if (NumIndexArray[x][y] == 0)
                    {
                        count++;
                        if (count == randomIndex)
                        {
                            newX = x;
                            newY = y;
                        }
                    }
                }
            }
            #endregion
        }
        else if (touchDir == TouchDir.Right)
        {
            #region moveToRight
            int emptyNumCount = 0;
            int x = 0;
            for (int y = 0; y < 4; y++)
            {
                if (NumIndexArray[x][y] == 0)
                {
                    emptyNumCount++;
                }
            }
            if (emptyNumCount != 0)
            {
                int randomIndex = Random.Range(1, emptyNumCount + 1);
                int count = 0;
                for (int y = 0; y < 4; y++)
                {
                    if (NumIndexArray[x][y] == 0)
                    {
                        count++;
                        if (count == randomIndex)
                        {
                            newX = x;
                            newY = y;
                        }
                    }
                }
            }
            #endregion
        }
        else if (touchDir == TouchDir.Left)
        {
            #region moveToLeft
            int emptyNumCount = 0;
            int x = 3;
            for (int y = 0; y < 4; y++)
            {
                if (NumIndexArray[x][y] == 0)
                {
                    emptyNumCount++;
                }
            }
            if (emptyNumCount != 0)
            {
                int randomIndex = Random.Range(1, emptyNumCount + 1);
                int count = 0;
                for (int y = 0; y < 4; y++)
                {
                    if (NumIndexArray[x][y] == 0)
                    {
                        count++;
                        if (count == randomIndex)
                        {
                            newX = x;
                            newY = y;
                        }
                    }
                }
            }
            #endregion
        }
        else if (touchDir == TouchDir.None)
        {
            #region AddOne
            int emptyNumCount = 0;
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (NumIndexArray[x][y] == 0)
                    {
                        emptyNumCount++;
                    }
                }
            }
            if (emptyNumCount != 0)
            {
                int randomIndex = Random.Range(1, emptyNumCount + 1);
                int Count = 0;
                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        if (NumIndexArray[x][y] == 0)
                        {
                            Count++;
                            if (Count == randomIndex)
                            {
                                newX = x;
                                newY = y;
                            }
                        }
                    }
                }
            }
            #endregion
            
        }
        return new Vector2(newX, newY);
    }
    #endregion

    #region 输出数组表格-OutputNumIndexArray
    private void OutputNumIndexArray() {
        string s = "";
        for (int y = 0; y < 4; y++) {
            for (int x = 0; x < 4; x++) {
                s=s+NumIndexArray[x][y]+"  ";
            }
            s=s+"\r\n";
        }
        print(s);
    }
    #endregion

    #region 等待并且在合并时生成新的Num-WaitAndCreateNum(int x,int y,int index,float WaitTime)
    IEnumerator WaitAndCreateNum(int x,int y,int index,float WaitTime) {
        WaitTime -= 0.2f;
        yield return new WaitForSeconds(WaitTime);
        CreateNum(x, y, index);
    }
    #endregion

    #region 统计分数-StatisticsScore
    private void StatisticsScore() {
            int score = 0;
            for (int x = 0; x < 4; x++) {
                for (int y = 0; y < 4; y++) {
                    score += NumIndexArray[x][y];
                }
            }
            newScore = score;
            SetHistoryScore();
    }
    #endregion

    #region 获取历史最高纪录-GetHistoryScore
    private void GetHistoryScore() {
        int HScore = PlayerPrefs.GetInt("HistoryScore", 0);
        historyScore = HScore;
    }
    #endregion

    #region 设置历史记录-SetHistoryScore
    private void SetHistoryScore() {
        if (newScore > historyScore) {
            PlayerPrefs.SetInt("HistoryScore", newScore);
        }
    }
    #endregion

}
