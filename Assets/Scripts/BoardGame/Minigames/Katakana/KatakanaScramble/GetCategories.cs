using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GetCategories : MonoBehaviour
{

    public GameObject parent;
    public GameObject loadingPanel;
    public GameObject loadingText;

    private Sprite[] inter_sprites;

    private GameObject[] buttons;
    private TMP_FontAsset font;

    string[] files;
    // Start is called before the first frame update
    void Start()
    {
        inter_sprites = Resources.LoadAll<Sprite>("interactables/");
        TMP_FontAsset[] fonts = Resources.LoadAll<TMP_FontAsset>("SU3DJPFont/TextMeshProFont/Selected/");
        font = fonts[0];
        TextAsset[] texts = Resources.LoadAll<TextAsset>("Texts/");
        buttons = new GameObject[texts.Length];
        
        //heavy assumption that meta files are going to right after the regular files.
        for (int i = 0; i < texts.Length; i++) {
            string category_name = texts[i].text.Split("\n"[0])[0];
            buttons[i] = create_button(texts[i], category_name, new Vector3(0, -100 - (250 * i)));
        }
    }

    private GameObject create_button(TextAsset txtfile ,string category_name, Vector3 pos) {
        //perhaps in the future add an image slideshow here showcasing the category?
        TMP_DefaultControls.Resources resources = new TMP_DefaultControls.Resources();
        GameObject button = TMP_DefaultControls.CreateButton(resources);
        button.GetComponent<Button>().onClick.AddListener(delegate{choose_category(txtfile, category_name);});
        RectTransform trans = button.GetComponent<RectTransform>();
        trans.transform.SetParent(parent.transform);
        trans.localScale = Vector3.one;
        trans.sizeDelta = new Vector3(1000, 200, 1);
        trans.localPosition = pos;

        Image sprite = button.GetComponent<Image>();
        sprite.sprite = inter_sprites[4];

       GameObject text_box = button.transform.GetChild(0).gameObject;
        RectTransform text_trans = text_box.GetComponent<RectTransform>();
        text_trans.localPosition = new Vector3(1, 1, 0);
        text_trans.localScale = Vector3.one;
        text_trans.sizeDelta = new Vector3(1000, 200, 1);
        TextMeshProUGUI text = text_box.GetComponent<TextMeshProUGUI>();
        text.text = category_name;
        // text.font = font;
        text.fontSize = 80;
        text.color = Color.black;
        text.alignment = TextAlignmentOptions.Center;

        return button;   
    }

    void choose_category(TextAsset txtfile, string category_name) {
        KatakanaScramble.txtfile = txtfile;
        KatakanaScramble.category_name = category_name;
        loadingPanel.GetComponent<FadeOutPanel>().sceneIndex = 2;
        loadingPanel.SetActive(true);
    }

}
