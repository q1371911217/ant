using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config
{

    private static Config _instance;
    public static Config Instance
    {
        get
        {
            if (_instance == null)
                _instance = new Config();
            return _instance;
        }
    }

    public List<int[,]> levelData;

    public List<float> startXList;

    private int clear = -1;
    private int wall = 0;
    private int stone = 1;
    private int ant = 2;
    private int space = 3;
    private int pool = 4;


    //墙0 石头1 蚂蚁2 空地3 水池4 空-1
    public Config()
    {
        startXList = new List<float>() { -246, -246, -291, -320, -171, -291 };

        levelData = new List<int[,]>();

        int[,] level1 = new int[,]
        {
            {clear, clear,wall,wall, wall,clear,clear, clear },
            {clear, clear,wall,space, wall,clear,clear, clear },
            {clear, clear,wall,space, wall,wall,wall, wall },
            {wall, wall,wall,space, space,space,space, wall },
            {wall, pool,space,stone, space,wall,wall, wall },
            {wall, wall,wall,wall, space,wall,clear, clear },
            {clear, clear,clear,wall, ant,wall,clear, clear },
            {clear, clear,clear,wall, wall,wall,clear, clear },
        };

        int[,] level1_1 = new int[,]
        {
            {clear,clear, wall,wall,wall, clear,clear,clear },
            {clear,clear, wall,pool,wall, clear,clear,clear },
            {clear,clear, wall, space, wall,wall,wall,wall },
            {wall,wall,wall, stone, space,stone,pool,wall },
            {wall,pool,space,stone,ant,wall,wall,wall },
            {wall,wall,wall,wall,stone,wall,clear,clear },
            {clear,clear,clear,wall,pool,wall,clear,clear },
            {clear,clear,clear,wall,wall,wall,clear,clear },
        };

        int[,] level2 = new int[,]
        {
            {wall, wall, wall, wall, wall, clear,clear,clear,clear },
            {wall, ant,space,space,wall, clear,clear,clear,clear },
            {wall, space,stone,stone,wall, clear,wall,wall,wall },
            {wall, space,stone,space,wall, clear,wall,pool,wall },
            {wall,wall,space,space,wall,wall,wall,pool,wall },
            {clear,wall,space,space,space,space,space,pool,wall },
            {clear, wall, space,space,space,wall,space,space,wall },
            {clear,wall, space,space,space,wall,wall,wall,wall },
            {clear,wall,wall,wall,wall,wall,clear,clear,clear },
        };

        int[,] level3 = new int[,]
        {
            {clear, wall, wall, wall, wall,wall, wall,wall, clear,clear },
            {clear, wall, space, space, space,space, space,wall, wall,wall },
            {wall, wall, stone, wall,wall,wall,space,space,space,wall },
            {wall, space,ant,space,stone,space,space,stone,space,wall },
            {wall, space,pool,pool,wall, space,stone,space,wall,wall },
            {wall,wall,pool,pool,wall,space,space,space,wall,clear },
            {clear,wall, wall,wall,wall,wall,wall,wall,wall,clear},
        };

        int[,] level4 = new int[,]
        {
            {clear,wall,wall,wall,wall,clear },
            {wall,wall,space,space,wall,clear },
            {wall,ant,stone,space,wall,clear },
            {wall,wall,stone,space,wall,wall },
            {wall,wall,space,stone,space,wall },
            {wall,pool,stone,space,space,wall },
            {wall,pool,pool,space,pool,wall },
            {wall,wall,wall,wall,wall,wall },
        };

        int[,] level5 = new int[,]
        {
            {wall,wall,wall,wall,wall,wall,clear,clear,clear },
            {wall,space,space,space,space,wall,clear,clear,clear },
            {wall,space,stone,stone,stone,wall,wall,clear,clear },
            {wall,space,space,wall,pool,pool,wall,clear,clear },
            {wall,wall,space,space,pool,pool,wall,wall,wall },
            {clear,wall,space,space,space,space,stone,space,wall },
            {clear,wall,space,ant,space,space,space,space,wall },
            {clear,wall,wall,wall,wall,wall,wall,wall,wall },
        };
        
        levelData.Add(level1);
        levelData.Add(level1_1);
        levelData.Add(level2);
        levelData.Add(level3);
        levelData.Add(level4);
        levelData.Add(level5);
    }

    public int[,] getDataByLevel(int level)
    {
        if (level <= levelData.Count)
            return copy(levelData[level - 1]);
        return null;
    }

    public float getStartXByLevel(int level)
    {
        if (level <= startXList.Count)
            return startXList[level - 1];
        return -318.5f;
    }

    public int[,] copy(int[,] arr)
    {
        int[,] copyArr = new int[arr.GetLength(0), arr.GetLength(1)];
        for(int i = 0; i < arr.GetLength(0); i++)
        {
            for(int j = 0; j < arr.GetLength(1); j++)
            {
                copyArr[i, j] = arr[i, j];
            }
        }

        return copyArr;
    }
}
