﻿using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script is used to monitor cards in preset container. 
/// <summary>
public class CardMonitor : MonoBehaviour
{
    [SerializeField]
    private Button presetBtn;
    [SerializeField]
    private List<GameObject> layerList;
    private GameObject startBtn;
    private GameObject randBtn;
    private GameObject prepBtn;
    private GameObject editBtn;
    private GameObject backBtn;
    private List<GameObject> btnList;
    private GameObject cardContainer;
    private GameObject cardPreview;
    [SerializeField]
    private List<GameObject> barList;
    [SerializeField]
    private List<GameObject> cardList;
    [SerializeField]
    private List<GameObject> presetList;
    private Dictionary<Button, CardPreset> btnToCp;

    const float HIGHLIGHT_SIZE = 24;
    const float NORMAL_SIZE = 18;
    private void Awake()
    {
        layerList = new List<GameObject>();
        btnList = new List<GameObject>();
        barList = new List<GameObject>();
        presetList = new List<GameObject>();
        cardList = new List<GameObject>();
        btnToCp = new Dictionary<Button, CardPreset>();
        cardPreview = GameObject.Find("Canvas/Background/CardContainer/CardPreview");
        presetBtn = GameObject.Find("Canvas/Background/BottomMask/BottomPanel/ButtonPrep").GetComponent<Button>();
    }

    private void Start()
    {
        ExtractBottomBtns();
        randBtn.SetActive(false);
        editBtn.SetActive(false);
        ExtractPresets();
        ExtractLayers();        
        cardContainer = layerList[4];
        ExtractBars();
        GeneratePresetCard();
        ExtractCards();
        cardContainer.SetActive(false);
        BindingEvent();
        SetPresets(false);
        InitMap();
    }

    private void InitMap()
    {
        CardPreset[] cps = { Utilities.cp1, Utilities.cp2, Utilities.cp3, Utilities.cp4 };
        for (int i = 0; i < presetList.Count(); i++)
        {
            btnToCp.Add(presetList[i].GetComponent<Button>(), cps[i]);
        }
        
    }
    private void SetPresets(bool activeSelf)
    {
  
        foreach(GameObject preset in presetList)
        {
            preset.SetActive(activeSelf);
        }
    }

    private void ExtractBottomBtns()
    {
        string btnPath = "Canvas/Background/BottomMask/BottomPanel/";
        startBtn = GameObject.Find(btnPath + "ButtonStart");
        randBtn = GameObject.Find(btnPath + "ButtonRand");
        prepBtn = GameObject.Find(btnPath + "ButtonPrep");
        editBtn = GameObject.Find(btnPath + "ButtonEdit");
        backBtn = GameObject.Find(btnPath + "ButtonQuit");
        btnList.Add(startBtn);
        btnList.Add(randBtn);
        btnList.Add(prepBtn);
        btnList.Add(editBtn);
    }

    private void ExtractPresets()
    {
        string presetPath = "Canvas/Background/TopMask/";
        string[] presetSet = { "PresetOne", "PresetTwo", "PresetThree", "PresetFour" };
        for(int i = 0; i < 4; i++)
        {
            presetList.Add(GameObject.Find(presetPath+ presetSet[i]));
        }
    }

    private void ExtractLayers()
    {
        GameObject[] layerArr = GameObject.FindGameObjectsWithTag("Configuration");
        foreach (GameObject layer in layerArr)
        {
            layerList.Add(layer);
        }
    }

    private void ExtractBars()
    {
        GameObject cc = GameObject.Find("Canvas/Background/CardContainer/CountingChamber");
        Transform[] bars = cc.GetComponentsInRealChildren<Transform>();
        int count = 0;
        foreach(Transform tf in bars)
        {
            if (count % 5 == 0)
            {
                barList.Add(tf.gameObject);
            }
            count++;
        }
    }
    private void InitInterface()
    {
        SetPresets(true);
        Utilities.stageID = Utilities.StageOptions.PRESET_STAGE;
        layerList[0].SetActive(true);
        layerList[2].SetActive(true);
        layerList[0].GetComponent<Image>().sprite = Resources.Load(Utilities.res_folder_path_mask+"yspz", typeof(Sprite)) as Sprite;
        DisplayPresetCard();
        cardPreview.SetActive(false);
    }

    private void DisplayPresetCard()
    {
        //GameObject mask = layerList[1];
        //mask.SetActive(true);
        foreach(GameObject btn in btnList)
        {
            btn.SetActive(!btn.activeSelf);
        }
        cardContainer.SetActive(true);

    }

    private void GeneratePresetCard()
    {

        Transform cardPool = cardContainer.transform.GetChild(1);

        //Vector3 bounds = cardContainer.GetComponent<MeshFilter>().mesh.bounds.size;
        //print(bounds.x* cardContainer.transform.localScale.x);
        //print(bounds.y* cardContainer.transform.localScale.y);

        GridLayoutGroup glg = cardPool.GetComponent<GridLayoutGroup>();
        int k = 20;
        glg.padding.left = 2;
        glg.padding.top = 2;
        glg.cellSize = new Vector2(3 * k, 4 * k);
        glg.spacing = new Vector2(5, 5);

        for (int i = 0; i < 32; i++)
        {
            GameObject go = (GameObject)Resources.Load(Utilities.res_folder_path_prefabs + "card");
            go.name = "card " + i;
            go = Instantiate(go);
            go.transform.SetParent(cardPool);

            //go.AddComponent<Image>();
            //go.GetComponent<Image>().sprite = Resources.Load(Utilities.res_folder_path_cards+ "blank", typeof(Sprite)) as Sprite;
            //go.AddComponent<Button>();
        }

    }

    private void HideAllLayers()
    {
        foreach(GameObject layer in layerList)
        {
            layer.SetActive(false);
        }
    }

    private void DestroyCards()
    {
        GameObject cardPool = cardContainer.transform.Find("CardPool").gameObject;
        //print(cardPool.name);
        Transform[] cards = cardPool.GetComponentsInChildren<Transform>();
        for(int i = 1; i < cards.Length; i ++)
        {
            //print(card.name);
            Destroy(cards[i].gameObject); 
        }
        
    }

    private void ExtractCards()
    {
        GameObject cardPool = GameObject.Find("Canvas/Background/CardContainer/CardPool");
        Transform[] cards = cardPool.GetComponentsInRealChildren<Transform>();
        foreach(Transform tf in cards)
        {
            cardList.Add(tf.gameObject);
        }
    }

    private void ChangeSpriteState(Button btn,string pathPrefix,bool isBlank = false)
    {
        if (!isBlank)
        {
            SpriteState ss = new SpriteState();
            ss.highlightedSprite = Resources.Load(pathPrefix + "_h", typeof(Sprite)) as Sprite; ;
            ss.pressedSprite = Resources.Load(pathPrefix + "_p", typeof(Sprite)) as Sprite; ;
            btn.spriteState = ss;
        }
        else
        {
            SpriteState ss = new SpriteState();
            ss.highlightedSprite = Resources.Load(pathPrefix + "", typeof(Sprite)) as Sprite; ;
            ss.pressedSprite = Resources.Load(pathPrefix + "", typeof(Sprite)) as Sprite; ;
            btn.spriteState = ss;
        }   
    }

    private void DisplayMessageWhenMouseEnter(Button btn, Card card, bool isBlank = false)
    {
        if (!isBlank)
        {
            btn.onClick.AddListener(() =>
            {
                cardPreview.SetActive(true);
                Transform[] tfs = cardPreview.GetComponentsInRealChildren<Transform>();
                tfs[0].gameObject.GetComponent<Image>().sprite = Resources.Load(Utilities.res_folder_path_cards + card.Name + "_n", typeof(Sprite)) as Sprite;
                tfs[1].gameObject.GetComponent<TextMeshProUGUI>().text = card.Name;
                tfs[3].gameObject.GetComponent<TextMeshProUGUI>().text = MyTool.GetEnumDescription(card.TypeName);
                tfs[4].gameObject.GetComponent<TextMeshProUGUI>().text = card.Description;
            });
        }
        else
        {
            btn.onClick.AddListener(() =>
            {
                cardPreview.SetActive(false);
            });
        }
    }

    private void ReplaceAllCardsWithBlank(CardPreset cp)
    {
        barList[0].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = cp.MarionetteCardNumber.ToString();
        barList[1].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = cp.DrawingCardNumber.ToString();
        barList[2].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = cp.GoodCardNumber.ToString();
        barList[3].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = cp.TotalNumber.ToString();

        for (int k = 0; k < 32; k++)
        {
            string pathPrefix = Utilities.res_folder_path_cards + "blank";
            ChangeSpriteState(cardList[k].GetComponent<Button>(), pathPrefix, true);
            Button btn = cardList[k].GetComponent<Button>();
            DisplayMessageWhenMouseEnter(btn, null, true);
            cardList[k].GetComponent<Image>().sprite = Resources.Load(pathPrefix, typeof(Sprite)) as Sprite;
        }
    }

    private void AttachCard(CardPreset cp, bool isBlank = false)
    {
        if (isBlank)
        {
            ReplaceAllCardsWithBlank(cp);
            return;
        }

        barList[0].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = cp.MarionetteCardNumber.ToString();
        barList[1].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = cp.DrawingCardNumber.ToString();
        barList[2].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = cp.GoodCardNumber.ToString();
        barList[3].transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = cp.TotalNumber.ToString();
        int j = 0;
        for(int i = 0; i < cp.MarionetteCardNumber;i++)
        {
            string pathPrefix = Utilities.res_folder_path_cards + cp.MarionetteCardList[i].Name;
            Button btn = cardList[j].GetComponent<Button>();
            ChangeSpriteState(btn, pathPrefix);
            DisplayMessageWhenMouseEnter(btn, cp.MarionetteCardList[i]);
            cardList[j++].GetComponent<Image>().sprite = Resources.Load(pathPrefix + "_n", typeof(Sprite)) as Sprite;
        }
        for (int i = 0; i < cp.DrawingCardNumber; i++)
        {
            string pathPrefix = Utilities.res_folder_path_cards + cp.DrawingCardList[i].Name;
            ChangeSpriteState(cardList[j].GetComponent<Button>(), pathPrefix);
            Button btn = cardList[j].GetComponent<Button>();
            DisplayMessageWhenMouseEnter(btn, cp.DrawingCardList[i]);
            cardList[j++].GetComponent<Image>().sprite = Resources.Load(pathPrefix + "_n", typeof(Sprite)) as Sprite;
        }
        for (int i = 0; i < cp.GoodCardNumber; i++)
        {
            string pathPrefix = Utilities.res_folder_path_cards + cp.GoodCardList[i].Name;
            ChangeSpriteState(cardList[j].GetComponent<Button>(), pathPrefix);
            Button btn = cardList[j].GetComponent<Button>();
            DisplayMessageWhenMouseEnter(btn, cp.GoodCardList[i]);
            cardList[j++].GetComponent<Image>().sprite = Resources.Load(pathPrefix + "_n", typeof(Sprite)) as Sprite;
        }
        for(int k = j; k < /*cp.MAX_TOTAL_NUM*/32; k++)
        {
            string pathPrefix = Utilities.res_folder_path_cards + "blank";
            ChangeSpriteState(cardList[k].GetComponent<Button>(), pathPrefix,true);
            Button btn = cardList[k].GetComponent<Button>();
            DisplayMessageWhenMouseEnter(btn, null, true);
            cardList[k].GetComponent<Image>().sprite = Resources.Load(pathPrefix, typeof(Sprite)) as Sprite;
        }
    }

    private void ChangeFontSizeAndColor(Button presetBtn)
    {


        foreach (GameObject preset in presetList)
        {
            var btn = preset.GetComponent<Button>();
            TextMeshProUGUI _tmp = btn.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            if(_tmp.fontSize == HIGHLIGHT_SIZE)
            {
                _tmp.fontSize = NORMAL_SIZE;
                TMP_ColorGradient _colorGradient = Resources.Load<TMP_ColorGradient>(Utilities.res_folder_path_tmp + "ColorGradient/Gray - Single");
                _tmp.colorGradientPreset = _colorGradient;
                break;
            }
            
        }

        TextMeshProUGUI tmp = presetBtn.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        tmp.fontSize = 24;
        if (!tmp.enableVertexGradient)
        {
            tmp.enableVertexGradient = true;
        }
        TMP_ColorGradient colorGradient = Resources.Load<TMP_ColorGradient>(Utilities.res_folder_path_tmp+ "ColorGradient/Yellow to Orange - Vertical");
        tmp.colorGradientPreset = colorGradient;
        //tmp.colorGradientPreset = ScriptableObject.CreateInstance<TMP_ColorGradient>();
        //tmp.colorGradientPreset.colorMode = ColorMode.FourCornersGradient;
    }
    private void BindingEvent()
    {
        foreach(GameObject preset in presetList)
        {
            var btn = preset.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                ChangeFontSizeAndColor(btn);
                AttachCard(btnToCp[btn], !btnToCp[btn].RandomTag);
            });
        }

        presetBtn.onClick.AddListener(() =>
        {
            HideAllLayers();
            InitInterface();
            AttachCard(Utilities.cp1);
        });;

        backBtn.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (Utilities.stageID == Utilities.StageOptions.PRESET_STAGE)
            {
                layerList[1].SetActive(true);
                layerList[2].SetActive(true);
                layerList[3].SetActive(true);
                layerList[4].SetActive(false);
                foreach (GameObject btn in btnList)
                {
                    btn.SetActive(!btn.activeSelf);
                }
                SetPresets(false);
                //DestroyCards();
            }
        });

        randBtn.GetComponent<Button>().onClick.AddListener(() =>
        {
            CardPreset curCp = CheckCurrentCp();
            Utilities.DisableAllCards();
            curCp.ClearAll();
            curCp.GenerateCardPresetRandomly();
            curCp.PrintAll();
            AttachCard(curCp);
        });
    }

    private CardPreset CheckCurrentCp()
    {
        foreach (GameObject preset in presetList)
        {
            var btn = preset.GetComponent<Button>();
            TextMeshProUGUI _tmp = btn.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            if (_tmp.fontSize == HIGHLIGHT_SIZE)
            {
                return btnToCp[btn];
            }

        }
        return null;
    }
}
