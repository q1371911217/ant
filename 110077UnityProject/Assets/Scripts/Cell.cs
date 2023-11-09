using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Cell : MonoBehaviour
{
    public int x { get; set; }
    public int y { get; set; }

    public int type { get; set; }

    GameObject lastGo;

    public void move(Vector3 target, Action callback = null)
    {
        this.transform.DOKill();
        if(type == (int)ObjType.ANT)
        {
            if(lastGo == null)
                transform.Find("left").gameObject.SetActive(false);
            if (lastGo != null)
                lastGo.gameObject.SetActive(false);
            if (target.x - this.transform.localPosition.x > 1)
            {
                lastGo = transform.Find("right").gameObject;
                transform.Find("right").gameObject.SetActive(true);
            }
            else if(target.x - this.transform.localPosition.x < -1)
            {
                lastGo = transform.Find("left").gameObject;
                transform.Find("left").gameObject.SetActive(true);
            }
            else if(target.y - this.transform.localPosition.y > 1)
            {
                lastGo = transform.Find("up").gameObject;
                transform.Find("up").gameObject.SetActive(true);
               
            }
            else if (target.y - this.transform.localPosition.y < -1)
            {
                lastGo = transform.Find("down").gameObject;
                transform.Find("down").gameObject.SetActive(true);
                
            }
        }
        this.transform.DOLocalMove(target, 0.3f).OnComplete(() =>
        {
            if (callback != null)
                callback();
        });
    }
}
