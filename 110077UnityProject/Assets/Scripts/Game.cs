using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//墙0 石头1 蚂蚁2 空地3 水池4 空-1
public enum ObjType
{
    CLEAR = -1,
    WALL = 0,
    STONE = 1,
    ANT = 2,
    SPACE = 3,
    POOL = 4,
}

public enum NearType
{
    NOT = -1,
    UP = 1,
    DOWN = 2,
    Left = 3,
    Right = 4,
}

public enum Direction
{
    up,
    down,
    left,
    right,
}


public class Game : MonoBehaviour
{
    public AudioClip audioClip;
    public AudioClip winClip;
    public AudioClip loseClip;

    public Transform menuLayer;
    public Transform menuCellRoot;

    public GameObject resultLayer;
    public Transform resultWinLayer;
    public Transform resultLoseLayer;

    public GameObject firstLevelTip;
    public GameObject btnVolume;
    public Transform objRoot;
    public Text lblLevel, lblStep;

    public GameObject wallGo, stoneGo, antGo, spaceGo, poolGo;

    AudioSource audioSource;

    private Cell AntCell;
    private List<Cell> stoneCellList = new List<Cell>();
    private List<Cell> poolCellList = new List<Cell>();

    //private const float startX = -318.5f;
    private const float startY = 319f;
    private const float offsetX = 71;
    private const float offsetY = -71;

    private Dictionary<int, GameObject> objDict;

    private Config config;

    private int curLevel;
    private int maxLevel;
    private const int totalLevel = 6;

    public static bool volumOpen = true;

    private int curStep;

    void Awake()
    {
        objDict = new Dictionary<int, GameObject>();
        
    }

    void Start()
    {

        audioSource = GameObject.Find("GameManager").GetComponent<AudioSource>();
        audioSource.volume = Game.volumOpen ? 1 : 0;
        btnVolume.transform.Find("spDisable").gameObject.SetActive(!Game.volumOpen);

        config = Config.Instance;

        curLevel = 1;
        maxLevel = PlayerPrefs.GetInt("maxLevel", 1);

        objDict[0] = wallGo;
        objDict[1] = stoneGo;
        objDict[2] = antGo;
        objDict[3] = spaceGo;
        objDict[4] = poolGo;

        menuLayer.gameObject.SetActive(true);
        menuLayer.transform.localScale = Vector3.zero;
        menuLayer.transform.DOScale(Vector3.one, 0.3f);
        updateMenu();
    }

    void clear()
    {
        AntCell = null;
        stoneCellList.Clear();
        poolCellList.Clear();

        curStep = 0;

        if (menuLayer.gameObject.activeSelf)
        {
            menuLayer.DOScale(Vector3.zero, 0.3f).OnComplete(() =>
            {
                menuLayer.gameObject.SetActive(false);
            });
        }

        for(int i = objRoot.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(objRoot.GetChild(i).gameObject);
        }

        StopAllCoroutines();
        audioSource.Play();
    }

    int[,] levelData;
    float startX;

    RoutingObject[,] routingMapArr;

    void gameStart()
    {
        clear();

        curStep = 0;

        firstLevelTip.gameObject.SetActive(curLevel == 1);

        lblLevel.text = string.Format("Level:{0}", curLevel);
        lblStep.text = string.Format("Step:{0}", curStep);

        levelData = config.getDataByLevel(curLevel);
        
        if(levelData != null)
        {
            startX = config.getStartXByLevel(curLevel);
            int row = levelData.GetLength(0);
            int col = levelData.GetLength(1);

            Routing.Instance.row = row;
            Routing.Instance.col = col;

            routingMapArr = new RoutingObject[row, col];

            for (int x = 0; x < row; x++)
            {
                for(int y = 0; y < col; y++)
                {
                    int type = levelData[x, y];
                    RoutingObject obj = new RoutingObject();
                    obj.x = x;
                    obj.y = y;
                    if (type != -1)
                    {
                        if (type != 0) // 创建空地
                        {
                            Transform objTrans = generateObj(objDict[3]);
                            objTrans.localPosition = new Vector3(startX + y * offsetX, startY + x * offsetY, 0);
                            Cell cell = objTrans.GetComponent<Cell>();
                            cell.x = x;
                            cell.y = y;
                            cell.type = (int)ObjType.SPACE;
                            objTrans.GetComponent<Button>().onClick.AddListener(() =>
                            {
                                onClickSpace(cell);
                            });                            
                        }
                        if(type != 3)
                        {
                            Transform objTrans = generateObj(objDict[type]);
                            objTrans.localPosition = new Vector3(startX + y * offsetX, startY + x * offsetY, 0);
                            Cell cell = objTrans.GetComponent<Cell>();
                            cell.x = x;
                            cell.y = y;
                            cell.type = type;
                            if (type == (int)ObjType.ANT)
                                AntCell = cell;
                            else if (type == (int)ObjType.STONE)
                            {
                                objTrans.GetComponent<Button>().onClick.AddListener(() =>
                                {
                                    onStoneClick(cell);
                                });
                                stoneCellList.Add(cell);
                            }
                            else if (type == (int)ObjType.POOL)
                                poolCellList.Add(cell);
                            
                        }

                        obj.isCanMove = (type != 0 && type != 1);
                        routingMapArr[x, y] = obj;

                    }
                    else
                    {
                        obj.isCanMove = false;
                        routingMapArr[x, y] = obj;
                    }
                }
            }

            AntCell.transform.SetAsLastSibling();
            for(int i = 0; i < stoneCellList.Count; i++)
            {
                stoneCellList[i].transform.SetAsLastSibling();
            }
        }
    }

    Transform generateObj(GameObject go)
    {
        GameObject obj = GameObject.Instantiate(go);
        obj.SetActive(true);
        Transform objTrans = obj.transform;
        objTrans.SetParent(objRoot);
        objTrans.localRotation = Quaternion.identity;
        objTrans.localScale = Vector3.one;

        return objTrans;
    }

    void onStoneClick(Cell cell)
    {
        //TODO //判断蚂蚁是否在旁边 蚂蚁对向是否是空地水池墙

        //TODO 判断游戏结束

        //Debug.LogError("Click stone");
        //Debug.LogError(cell.x + "   " + cell.y);
        //Debug.LogError(AntCell.x + "   " + AntCell.y);
        NearType type = Path.isNear(cell, AntCell);
        //Debug.LogError(type);
        if(type == NearType.UP)
        {
            int targetX = cell.x - 1;
            if (targetX < 0)
                return;
            int upObjType = levelData[targetX, cell.y];
            if(upObjType > (int)ObjType.STONE)
            {
                levelData[cell.x, cell.y] = (int)ObjType.SPACE;
                routingMapArr[cell.x, cell.y].isCanMove = true;
                levelData[targetX, cell.y] = (int)ObjType.STONE;
                routingMapArr[targetX, cell.y].isCanMove = false;
                cell.x = targetX;
                Vector3 stoneTargetPos = new Vector3(startX + cell.y * offsetX, startY + cell.x * offsetY, 0);
                cell.move(stoneTargetPos, () => { checkOver(); });

                AntCell.x = targetX + 1;
                Vector3 antTargetPos = new Vector3(startX + AntCell.y * offsetX, startY + AntCell.x * offsetY, 0);
                AntCell.move(antTargetPos);
                
            }
        }else if(type == NearType.DOWN)
        {
            int targetX = cell.x + 1;
            if (targetX >= levelData.GetLength(0))
                return;
            int upObjType = levelData[targetX, cell.y];
            if (upObjType > (int)ObjType.STONE)
            {
                levelData[cell.x, cell.y] = (int)ObjType.SPACE;
                routingMapArr[cell.x, cell.y].isCanMove = true;
                levelData[targetX, cell.y] = (int)ObjType.STONE;
                routingMapArr[targetX, cell.y].isCanMove = false;
                cell.x = targetX;
                Vector3 stoneTargetPos = new Vector3(startX + cell.y * offsetX, startY + cell.x * offsetY, 0);
                cell.move(stoneTargetPos, () => { checkOver(); });

                AntCell.x = targetX - 1;
                Vector3 antTargetPos = new Vector3(startX + AntCell.y * offsetX, startY + AntCell.x * offsetY, 0);
                AntCell.move(antTargetPos);
            }
        }
        else if(type == NearType.Left)
        {
            int targetY = cell.y - 1;
            if (targetY < 0)
                return;
            int upObjType = levelData[cell.x, targetY];
            if (upObjType > (int)ObjType.STONE)
            {
                levelData[cell.x, cell.y] = (int)ObjType.SPACE;
                routingMapArr[cell.x, cell.y].isCanMove = true;
                levelData[cell.x, targetY] = (int)ObjType.STONE;
                routingMapArr[cell.x, targetY].isCanMove = false;
                cell.y = targetY;
                Vector3 stoneTargetPos = new Vector3(startX + cell.y * offsetX, startY + cell.x * offsetY, 0);
                cell.move(stoneTargetPos, () => { checkOver(); });

                AntCell.y = targetY + 1;
                Vector3 antTargetPos = new Vector3(startX + AntCell.y * offsetX, startY + AntCell.x * offsetY, 0);
                AntCell.move(antTargetPos);
            }
        }
        else if(type == NearType.Right)
        {
            int targetY = cell.y + 1;
            if (targetY >= levelData.GetLength(1))
                return;
            int upObjType = levelData[cell.x, targetY];
            if (upObjType > (int)ObjType.STONE)
            {
                levelData[cell.x, cell.y] = (int)ObjType.SPACE;
                routingMapArr[cell.x, cell.y].isCanMove = true;
                levelData[cell.x, targetY] = (int)ObjType.STONE;
                routingMapArr[cell.x, targetY].isCanMove = false;
                cell.y = targetY;
                Vector3 stoneTargetPos = new Vector3(startX + cell.y * offsetX, startY + cell.x * offsetY, 0);
                cell.move(stoneTargetPos, () => { checkOver(); });

                AntCell.y = targetY - 1;
                Vector3 antTargetPos = new Vector3(startX + AntCell.y * offsetX, startY + AntCell.x * offsetY, 0);
                AntCell.move(antTargetPos);
            }
        }
    }

    void checkOver()
    {
        curStep += 1;
        lblStep.text = string.Format("Step:{0}", curStep);
        int sameCount = 0;
        for (int j = 0; j < poolCellList.Count; j++)
        {
            poolCellList[j].transform.GetComponent<Image>().enabled = true;
            for (int i = 0; i < stoneCellList.Count; i++)
            {
                if (stoneCellList[i].x == poolCellList[j].x && stoneCellList[i].y == poolCellList[j].y)
                {
                    poolCellList[j].transform.GetComponent<Image>().enabled = false;
                    sameCount += 1;
                    break;
                }

            }
        }
        if(sameCount == poolCellList.Count)
        {
            //Debug.LogError("win");
            StartCoroutine(gameOver(true));
        }
        else
        {
            
        }
    }

    IEnumerator gameOver(bool isWin)
    {
        //menuLayer.gameObject.SetActive(false);
        if (isWin)
        {
            if (curLevel + 1 > maxLevel)
            {
                maxLevel = curLevel + 1;
                if (maxLevel > totalLevel)
                    maxLevel = totalLevel;
                PlayerPrefs.SetInt("maxLevel", maxLevel);
            }
        }
        yield return new WaitForSeconds(0.5f);
        audioSource.Stop();
        
        resultLayer.gameObject.SetActive(true);
        resultLayer.transform.localScale = Vector3.zero;
        displayResultLayer(true);
        resultWinLayer.gameObject.SetActive(isWin);
        resultLoseLayer.gameObject.SetActive(!isWin);
        if (isWin)
        {
            audioSource.PlayOneShot(winClip);
        }
        else
        {
            audioSource.PlayOneShot(loseClip);
        }
        yield return new WaitForSeconds(2.5f);
        audioSource.Play();
    }

    //List<XY> pathList = new List<XY>();
    List<RoutingObject> routePath = new List<RoutingObject>();


    void onClickSpace(Cell cell)
    {
        //Debug.LogError("Click space");
        Debug.LogError(routingMapArr.Length);
        routePath.Clear();

        bool havePath = Routing.Instance.IsRouting(routingMapArr[AntCell.x, AntCell.y], routingMapArr[cell.x, cell.y], routingMapArr);
        if (havePath)
        {
           routePath = Routing.Instance.GetRoute();
            //for (int i = 0; i < routePath.Count; i++)
            //{
            //    Debug.LogError(routePath[i].x + "," + routePath[i].y);
            //}
                anteMove();
        }
        //int[] array02 = null;
        //bool havePath = Path.Find(AntCell, cell, levelData, out array02);
        //pathList.Clear();
        //if (havePath)
        //{
        //    for(int i = 0; i < array02.Length; i += 2)
        //    {
        //        if (array02[i] != -1 && array02[i + 1] != -1)
        //        {
        //            pathList.Add(new XY(array02[i], array02[i + 1]));
        //        }
        //        else
        //            break;
        //    }
        //    pathList.RemoveAt(0);
        //    anteMove();
        //}
    }

    void anteMove()
    {
        if(routePath.Count > 0)
        {
            RoutingObject curXY = routePath[0];
            routePath.RemoveAt(0);
            AntCell.x = curXY.x;
            AntCell.y = curXY.y;
            Vector3 targetPos = new Vector3( startX + curXY.y * offsetX, startY + curXY.x * offsetY, 0);
            AntCell.move(targetPos, () => {
                curStep += 1;
                lblStep.text = string.Format("Step:{0}", curStep);
                anteMove(); 
            });
        }
       
    }

    void updateMenu()
    {
        for (int i = 1; i <= maxLevel; i++)
        {
            Transform cell = menuCellRoot.GetChild(i - 1);
            cell.Find("spCur").gameObject.SetActive(i == curLevel);
            cell.Find("lblLevel").GetComponent<Text>().text = i.ToString();
            cell.Find("spLock").gameObject.SetActive(false);
           
            Button btn = cell.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            int tmpLevel = i;
            btn.onClick.AddListener(() =>
            {
                audioSource.PlayOneShot(audioClip);
                curLevel = tmpLevel;
                gameStart();
            });
        }
    }


    public void onBtnClick(string name)
    {
        audioSource.PlayOneShot(audioClip);
        if (name == "btnMenu")
        {
            displayResultLayer(false);
            menuLayer.gameObject.SetActive(true);
            menuLayer.transform.localScale = Vector3.zero;
            menuLayer.transform.DOScale(Vector3.one, 0.3f);
            updateMenu();
        }
        else if(name == "btnVolume")
        {
            Game.volumOpen = !Game.volumOpen;
            audioSource.volume = Game.volumOpen ? 1 : 0;
            btnVolume.transform.Find("spDisable").gameObject.SetActive(!Game.volumOpen);
        }
        else if(name == "btnRestart")
        {
            displayResultLayer(false);
            gameStart();
        }
        else if(name == "btnNext")
        {
            displayResultLayer(false);
            curLevel += 1;
            if (curLevel > totalLevel)
                curLevel = 1;
            gameStart();
        }
        else if(name == "btnHome")
        {
            SceneManager.LoadSceneAsync("LoginScene");
        }
    }

    void displayResultLayer(bool isShow)
    {
        Vector3 targetScale = isShow ? Vector3.one : Vector3.zero;
        if (!resultLayer.activeSelf) return;
        resultLayer.transform.DOScale(targetScale, 0.3f).OnComplete(() =>
        {
            if (!isShow)
            {
                resultWinLayer.gameObject.SetActive(false);
                resultLoseLayer.gameObject.SetActive(false);
                resultLayer.gameObject.SetActive(false);
            }
        });
    }
}



public struct XY
{
    public int x;
    public int y;
    public XY(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class RoutingObject
{
    /// <summary>
    /// x坐标
    /// </summary>
    public int x;

    /// <summary>
    /// y坐标
    /// </summary>
    public int y;

    /// <summary>
    /// 目标距离
    /// </summary>
    public int targetDistance;

    /// <summary>
    /// 移动距离
    /// </summary>
    public int moveDistance;

    /// <summary>
    /// A*和值(目标距离+移动距离)
    /// </summary>
    public int moveSum;

    /// <summary>
    /// 是否可以移动
    /// </summary>
    public bool isCanMove;

    /// <summary>
    /// 移动方向
    /// </summary>
    public Direction direction;
}