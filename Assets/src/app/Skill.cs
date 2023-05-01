using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    public GameObject go;
    public SkillPack parent;
    public int id;
    public int pos;
    public int idx;
    private UISprite sprite;
    public GameObject goChoose;
    public Skill(GameObject go, SkillPack parent)
    {
        this.go = go;
        this.parent = parent;
        id = -1;
        Init();
    }
    public void Init()
    {
        if (go == null) return;
        sprite = go.GetComponent<UISprite>();
        goChoose = go.Find("choose").gameObject;
        goChoose.SetActive(false);
        UIEventListener.Get(go).onClick = (x) => { OnClick(); };
        UIEventListener.Get(go).onHover = (x, getin) => { OnHover(getin); };
    }

    public void Refresh()
    {
        if (id == -1) sprite.spriteName = "ui_blank_white";
        else sprite.spriteName = sm.tables.skill.getSprite(id);
    }
    public void OnClick()
    {
        if (GameManager.get().IsGaming()) return;
        if(parent.nowChoose == null)
        {
            parent.nowChoose = this;
            goChoose.SetActive(true);
        }
        else
        {
            var first = parent.nowChoose;
            parent.nowChoose = null;
            first.goChoose.SetActive(false);
            var tmp = first.id;
            first.id = id;
            id = tmp;
            first.Refresh();
            Refresh();
        }
    }
    public void OnHover(bool getin)
    {
        if(getin) parent.ShowInfo(this);
        else parent.HideInfo();
    }
}
public class SkillPack
{
    Transform skillTrans, midTrans;
    public List<Skill> bag;
    public List<List<Skill>> sList;
    List<GameObject> goList;
    GameObject goItem;
    Transform infoTrans;
    UILabel infoLabel;
    UIGrid midGrid;

    public Skill nowChoose = null;

    public SkillPack(Transform skillTrans)
    {
        sList = new List<List<Skill>>(4);
        for(int i=0;i<4;i++)
        {
            sList.Add(new List<Skill>());
        }
        goList = new List<GameObject>();
        bag = new List<Skill>();
        this.skillTrans = skillTrans;
        Init();

        //≥ı º…Ë÷√
        sList[0][0].id = 1;
        sList[1][0].id = 6;
        sList[1][1].id = 2;
        RefreshAll();
    }
    private void Init()
    {
        goItem = skillTrans.Find("item").gameObject;
        goItem.SetActive(false);
        infoTrans = skillTrans.Find("bottom/info");
        infoLabel = infoTrans.GetComponent<UILabel>("label");
        infoTrans.SetActive(false);
        for (int i=0;i<4;i++)
        {
            var go = skillTrans.Find(string.Format("bottom/s{0}", i + 1)).gameObject;
            goList.Add(go);
            for(int j=0;j<i+1;j++)
            {
                var goSkill = NGUITools.AddChild(go.Find("grid").gameObject, goItem);
                goSkill.SetActive(true);
                Skill skill = new Skill(goSkill, this);
                skill.pos = i;
                skill.idx = j;
                sList[i].Add(skill);
            }
            go.GetComponent<UIGrid>("grid").Reposition();
        }

        midTrans = skillTrans.Find("mid");
        midGrid = midTrans.GetComponent<UIGrid>("grid");
        midTrans.SetActive(false);
    }
    public void ShowInfo(Skill skill)
    {
        int id = skill.id;
        if (id == -1) return;
        infoLabel.text = sm.__(sm.tables.skill.getDesc(id));
        infoTrans.SetActive(true);
    }
    public void HideInfo()
    {
        infoTrans.SetActive(false);
    }
    public void RefreshAll()
    {
        foreach(var list in sList)
        {
            foreach(var skill in list)
            {
                skill.Refresh();
            }
        }
    }
    public void ShowMid()
    {
        if (midGrid.transform.childCount > 0)
        {
            for (int i = 0; i < midGrid.transform.childCount; i++)
            {
                MonoBehaviour.Destroy(midGrid.transform.GetChild(i).gameObject);
            }
        }
        midTrans.SetActive(true);
        for(int i=0;i<bag.Count;i++)
        {
            var goSkill = NGUITools.AddChild(midGrid.gameObject, goItem);
            goSkill.SetActive(true);
            bag[i].go = goSkill;
            bag[i].Init();
            bag[i].Refresh();
            if (bag[i].id == -1) bag[i].go.SetActive(false);
        }
        midGrid.StartCoroutine(GridReposition());
        var btn = midTrans.Find("btn").gameObject;
        UIEventListener.Get(btn).onClick = (x) => {
            GameManager.get().FinishSetSkill();
            midTrans.SetActive(false);
        };
    }
    private IEnumerator GridReposition()
    {
        yield return new WaitForFixedUpdate();
        midGrid.Reposition();
    }
    public GameObject getItemPrefab()
    {
        return goItem;
    }
    public List<GameObject> getGoList()
    {
        return goList;
    }
}