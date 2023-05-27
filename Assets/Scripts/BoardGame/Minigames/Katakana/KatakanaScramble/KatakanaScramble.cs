using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class KatakanaScramble : Minigame
{

    public AudioSource deal_card, failure_sfx, success_sfx, buttonclick_sfx, kids_cheer;

    public GameObject roundStartContainer;
    public TextMeshProUGUI randomCategory;
    public Image seacreature_img;
    public GameObject next;
    public GameObject correct_img, incorrect_img;
    public GameObject correct_correct_img, failure_img;
    public static GameObject on_hover;
    public GameObject congrats_con;
    public TextMeshProUGUI round_counter;
    public TextMeshProUGUI timer_text;
    public TextMeshProUGUI category;
    public TextMeshProUGUI engl_text;
    public TextMeshProUGUI kata_text;
    public TextMeshProUGUI hint_text;
    public Button hint_button;
    public AudioSource bgm;

    public GameObject interactableContainer;

    const int MAX_WORDS_ROW = 8;
    const int INTER_WIDTH = 25;

    static Vector3[] wp;
    static GameObject[] bitter_map;
    List<GameObject> interactables;
    static int count_selected;
    int rounds_completed = 0;
    int max_rounds = 2;
    float timeLimit = 5f;
    int num_hints = 100;
    static bool disable_hints;
    float secs_passed = 0f;
    bool start = false;
    string cur_word;
    string cur_engl;

    static bool check_flag;

    private List<string> words;
    private List<string> english_words;
    private List<int> words_selected;
    private TextAsset txtfile;
    private string category_name;

    private Sprite[] inter_sprites;
    private Sprite[] seacreatures;
    private TMP_FontAsset font;
    // Start is called before the first frame update
    public override void Start() {
        base.Start();
        setup_music();
        inter_sprites = Resources.LoadAll<Sprite>("Minigames/Katakana/KatakanaScramble/interactables/");
        seacreatures = Resources.LoadAll<Sprite>("Minigames/Katakana/KatakanaScramble/seacreatures/");
        seacreature_img.sprite = seacreatures[Random.Range(0, seacreatures.Length)];
        TMP_FontAsset[] fonts = Resources.LoadAll<TMP_FontAsset>("SU3DJPFont/TextMeshProFont/Selected/");
        font = fonts[0];
        rounds_completed = 0;
        disable_hints = false;
        hint_text.text = "HINTS LEFT x" + num_hints;
        timer_text.text = secs_passed.ToString("0.00");

        next.GetComponent<Button>().onClick.AddListener(next_round);

        on_hover = GameObject.Find("on_hover");
        on_hover.SetActive(false);

        StartCoroutine("StartGame");
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(0.1f);
        randomCategory.transform.gameObject.SetActive(true);
        TextAsset[] texts = Resources.LoadAll<TextAsset>("Minigames/Katakana/KatakanaScramble/Texts/");
        txtfile = texts[Random.Range(0, texts.Length)];
        category_name = txtfile.text.Split("\n"[0])[0];
        randomCategory.text = category_name;
        yield return new WaitForSeconds(2f);
        randomCategory.transform.gameObject.SetActive(true);

        get_words();

        yield return new WaitForSeconds(2f);
        category.text = category_name;
        roundStartContainer.SetActive(false);
        words_selected = new List<int>();
        StartCoroutine("setup_game");
    }
    private void get_words() { //could be a one liner but late night duct tape 
        words = new List<string>();
        english_words = new List<string>();
        string[] splitted = txtfile.text.Split("\n"[0]);
        for (int i = 1; i < splitted.Length; i++) {
            english_words.Add(splitted[i].Split("_"[0])[1]);
            words.Add(splitted[i].Split("_"[0])[0]);
        }
    }

    private IEnumerator setup_game() {
        //randomly choose a word
        bool found_new_word = false;
        int random = -1;
        while (!found_new_word) {
            random = Random.Range(0, words.Count);
            if (!words_selected.Contains(random)) {
                found_new_word = true;
                words_selected.Add(random);
            }
        }
        cur_word = words[random];
        cur_engl = english_words[random];
        if (string.IsNullOrWhiteSpace(cur_word[cur_word.Length - 1] + "")) {
            cur_word = cur_word.Substring(0, cur_word.Length - 1);
        }
        //create interactables
        interactables = create_interactables(cur_word);
        yield return new WaitForSeconds(0.5f);
        //set wps
        wp = new Vector3[cur_word.Length];
        int from_back = cur_word.Length - 1;
        int y_multiplier = 0;
        int x_multiplier = 0;
        int first_card_i = -1;
        for (int i = 0; i < cur_word.Length; i++) {
            if (i % MAX_WORDS_ROW == 0) {
                y_multiplier++;
                x_multiplier = 0;
            }
            wp[i] = new Vector3(-800 + (INTER_WIDTH * 6 * x_multiplier), 800 - y_multiplier * (INTER_WIDTH * 6), 1);
            if (interactables[i].GetComponent<ClickLetter>().get_correct_pos_i() == 0) {
                interactables[i].GetComponent<ClickLetter>().set_new_pos(wp[0]);
                interactables[i].GetComponent<ClickLetter>().set_move_flag(true);
                interactables[i].GetComponent<ClickLetter>().set_disable(true);
                first_card_i = i;
            }

            interactables[from_back].GetComponent<ClickLetter>().set_move_flag(true);
            from_back--;
            if (from_back != -1) {
                interactables[from_back].transform.GetChild(0).gameObject.SetActive(true);
            }
            deal_card.Play();
            yield return new WaitForSeconds(0.3f);
            x_multiplier++;
        }

        secs_passed = timeLimit;
        timer_text.text = secs_passed.ToString("0.00");
        round_counter.text = (rounds_completed + 1).ToString() + "/" + words.Count.ToString();
        count_selected = 0;
        check_flag = false;
        bitter_map = new GameObject[wp.Length];
        bitter_map[0] = interactables[first_card_i];
        disable_hints = false;
        hint_button.interactable = true;
        start = true;
    }

    public static Vector3 available_pos(GameObject that) {
        for (int i = 0; i < wp.Length; i++) {
            if (bitter_map[i] == null) {
                bitter_map[i] = that;
                count_selected++;
                if (count_selected >= wp.Length - 1) {
                    disable_hints = true;
                    check_flag = true;
                }
                return wp[i];
            }
        }
        //shouldn't get here
        return new Vector3();
    }

   
    public static void remove_pos(GameObject that) {
        for (int i = 0; i < wp.Length; i++) {
            if (GameObject.ReferenceEquals(bitter_map[i], that)) {
                bitter_map[i] = null;
                count_selected--;
                return;
            }
        }
    }

    public static Vector3 hover_avail_pos() {
        for (int i = 0; i < wp.Length; i++) {
            if (bitter_map[i] == null) {
                return wp[i];
            }
        }
        return new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
        if (check_flag) {
            check_flag = false;
            StartCoroutine("check_corr");
        }

        if (start) {
            secs_passed -= Time.deltaTime;
            if (secs_passed <= 0f)
            {
                start = false;
                StartCoroutine("Failure");
            } else
            {
                timer_text.text = secs_passed.ToString("0.00");
            }
        }
        
    }

    private IEnumerator Failure()
    {
        hint_button.interactable = false;
        failure_sfx.Play();
        foreach (GameObject letter in interactables)
        {
            letter.GetComponent<ClickLetter>().set_disable(true);
        }
        engl_text.text = cur_engl;
        kata_text.text = cur_word;
        correct_img.SetActive(true);
        correct_correct_img.SetActive(false);
        failure_img.SetActive(true);
        hint_button.interactable = false;
        yield return new WaitForSeconds(2f);
        EndGame();
    }
 
    private IEnumerator check_corr() {
        List<int> incorrect = new List<int>();
        for (int i = 0; i < wp.Length; i++) {
            ClickLetter letter = bitter_map[i].GetComponent<ClickLetter>();
            letter.set_disable(true);
            Color tmp = bitter_map[i].GetComponent<Image>().color;
            tmp.a = 0.5f;
            bitter_map[i].GetComponent<Image>().color = tmp;
            string cur_letter = bitter_map[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text;
            if (cur_letter != cur_word[i] + "") {
                incorrect.Add(i);
            }
        }
        yield return new WaitForSeconds(1f);
        if (incorrect.Count == 0) {
            // Debug.Log("Correct!");
            start = false;
            foreach(GameObject letter in bitter_map) {
                letter.GetComponent<ClickLetter>().allow_hover = false;
            }
            engl_text.text = cur_engl;
            kata_text.text = cur_word;
            correct_img.SetActive(true);
            correct_correct_img.SetActive(true);
            failure_img.SetActive(false);
            hint_button.interactable = false;
            success_sfx.Play();
            if (rounds_completed < words.Count) {
                rounds_completed++;
                if (rounds_completed >= max_rounds) {
                    yield return new WaitForSeconds(2f);
                    display_finish();
                } else {
                    next.SetActive(true);
                }
            } 
        } else {
            // Debug.Log("incorrect");
            failure_sfx.Play();
            incorrect_img.SetActive(true);
            yield return new WaitForSeconds(3f);
            incorrect_img.SetActive(false);

            foreach (int i in incorrect) {
                Color tmp = bitter_map[i].GetComponent<Image>().color;
                tmp.a = 1f;
                bitter_map[i].GetComponent<Image>().color = tmp;
                ClickLetter letter = bitter_map[i].GetComponent<ClickLetter>();
                letter.set_disable(false);
                letter.return_home();
                bitter_map[i] = null;
                count_selected--;
            }
            disable_hints = true;
            deal_card.Play();
        }
    }

    private void next_round() {
        buttonclick_sfx.Play();
        if (rounds_completed != words.Count) {
            foreach (GameObject inter in interactables) {
                Destroy(inter);
        }
            correct_img.SetActive(false);
            next.SetActive(false);
            
            StartCoroutine("setup_game");
        }
    }

    private void display_finish() {
        kids_cheer.Play();
        
        foreach (GameObject inter in interactables) {
                Destroy(inter);
        }
        correct_img.SetActive(false);
        failure_img.SetActive(false);
        GameObject.Find("RoundContainer").SetActive(false);
        congrats_con.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>()
            .text = "You finished " + (rounds_completed) + " out of " + rounds_completed +" rounds in "+ secs_passed.ToString("0.00") + " seconds!";
        congrats_con.SetActive(true);
    }
    
    private List<GameObject> create_interactables(string word) {
        TMP_DefaultControls.Resources resources = new TMP_DefaultControls.Resources();
        List<GameObject> ret = new List<GameObject>();
        List<int> pos = new List<int>();
        List<int> rand_letter = new List<int>();
        int y_multiplier = 0;
        Debug.Log("Word:" + word);
        for (int i = 0; i < word.Length; i++) {
            if (i % MAX_WORDS_ROW == 0) {
                y_multiplier++;
                pos.Clear();
            }

            int word_index = -1;
            while (word_index == -1) {
                int random = Random.Range(0, word.Length);
                if (!rand_letter.Contains(random)) {
                    rand_letter.Add(random);
                    word_index = random;
                }
            }

            GameObject inter = TMP_DefaultControls.CreateButton(resources);
            RectTransform trans = inter.GetComponent<RectTransform>();
            trans.transform.SetParent(interactableContainer.transform);
            trans.localScale = new Vector3(INTER_WIDTH * 5, INTER_WIDTH * 5, 1);
            trans.sizeDelta = new Vector3(1, 1, 1);
            int rand_cap = -1;
            if (word.Length <= MAX_WORDS_ROW) {
                rand_cap = word.Length;
            } else {
                rand_cap = i >= word.Length - (word.Length % MAX_WORDS_ROW) ? (word.Length % MAX_WORDS_ROW) : MAX_WORDS_ROW;
                if (rand_cap == 0) {
                    rand_cap = MAX_WORDS_ROW;
                }
            }
            

            int x_multiplier = -1;
            while (x_multiplier == -1) {
                int random = Random.Range(0, rand_cap);
                if (!pos.Contains(random)) {
                    pos.Add(random);
                    x_multiplier = random;
                }
            }

            Image sprite = inter.GetComponent<Image>();
            sprite.sprite = inter_sprites[0];
            trans.localPosition = new Vector3(-200, 500, -1);


            // GameObject letter = new GameObject("letter");
            GameObject letter = inter.transform.GetChild(0).gameObject;
            if (i < word.Length - 1) {
                letter.SetActive(false);
            }
            RectTransform letter_trans = letter.GetComponent<RectTransform>();
            letter_trans.transform.SetParent(inter.transform);
            letter_trans.localPosition = new Vector3(0, 0, -1);  //????????????? WHY -1????
            letter_trans.localScale = Vector3.one;
            letter_trans.sizeDelta = new Vector3(1, 1, 1);
            TextMeshProUGUI text = letter.GetComponent<TextMeshProUGUI>();
            

            text.text = word[word_index] + "";
            text.fontSize = 0.8f;
            text.font = font;
            text.color = Color.black;
            text.alignment = TextAlignmentOptions.Center;    
            
            ClickLetter click = inter.AddComponent<ClickLetter>();
            Vector3 vec_pos = new Vector3(-800 + (INTER_WIDTH * (6 * x_multiplier)), 300 - y_multiplier * (INTER_WIDTH * 6), -1);
            click.set_new_pos(vec_pos);
            click.set_orig_pos(vec_pos);
            click.set_correct_pos(word_index);
            click.deal_card = deal_card;
            inter.AddComponent<BoxCollider>();
            inter.GetComponent<Button>().onClick.AddListener(click.OnMouseDown);            

            ret.Add(inter);
        }
        return ret;
    }

    void setup_music() {
        deal_card = audio_src_comp("DealCard");
        success_sfx = audio_src_comp("Nice");
        failure_sfx = audio_src_comp("Failure");
        buttonclick_sfx = audio_src_comp("ButtonClick");
        kids_cheer = audio_src_comp("KidsCheer");
    }    
    
    AudioSource audio_src_comp(string gameobj_name) {
        return GameObject.Find(gameobj_name).GetComponent<AudioSource>();
    }

    //returns true if provided gameobject has not been selected and already in it's correct position.
    bool auto_correct(GameObject inter) {
        ClickLetter cl = inter.GetComponent<ClickLetter>();
        bool prev_disabled = cl.get_disabled();
        if (!prev_disabled) {
            int cl_index = cl.get_correct_pos_i();
            inter.GetComponent<ClickLetter>().set_new_pos(wp[cl_index]);
            inter.GetComponent<ClickLetter>().set_move_flag(true);
            inter.GetComponent<ClickLetter>().set_disable(true);
            bitter_map[cl_index] = inter;
        }
        return !prev_disabled; 
    }

    public void hint() {
        Debug.Log("Hint?");
        if (num_hints <= 0 || !start || disable_hints) {
            return;
        }
        count_selected++;
        if (count_selected >= wp.Length - 1) {
            disable_hints = true;
        }
        num_hints--;
        hint_text.text = "HINTS LEFT x" + num_hints;
        bool found = false;
        while (!found && !disable_hints) {
            int rand = Random.Range(0, interactables.Count);
            found = auto_correct(interactables[rand]);
        }
        if (count_selected >= wp.Length - 1) {
            check_flag = true;
        }
    }

    void onclick_giveup() {
        StartCoroutine("giveup");
    }

    IEnumerator giveup() {
        start = false;
        failure_sfx.Play();
        foreach(GameObject letter in interactables) {
            letter.GetComponent<ClickLetter>().set_disable(true);
        }
        engl_text.text = cur_engl;
        kata_text.text = cur_word;
        correct_img.SetActive(true);
        correct_correct_img.SetActive(false);
        failure_img.SetActive(true);
        hint_button.interactable = false;
        if (rounds_completed < words.Count) {
            rounds_completed++;
            if (rounds_completed >= words.Count) {
                yield return new WaitForSeconds(2f);
                display_finish();
            } else {
                next.SetActive(true);
            }
        } 
    }
    
}
