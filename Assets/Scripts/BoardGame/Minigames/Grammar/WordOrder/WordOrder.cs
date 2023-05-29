using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class WordOrder : Minigame
{

    private AudioSource deal_card, failure_sfx, success_sfx, buttonclick_sfx, kids_cheer;

    public GameObject roundStartContainer;
    public TextMeshProUGUI randomCategory;

    public Image seacreature_img;
    public GameObject next;
    public GameObject correct_img, incorrect_img;
    public GameObject correct_correct_img, failure_img;
    public GameObject congrats_con;
    public TextMeshProUGUI round_counter;
    public TextMeshProUGUI timer_text;
    public TextMeshProUGUI category;
    public TextMeshProUGUI engl_text;
    public TextMeshProUGUI kata_text;
    public Button giveup_button;
    public Button redo_button;    
    public AudioSource bgm;

    public GameObject choose_container;
    public Transform[] waypoints;
    static public Transform[] static_wps;
    static public bool[] wp_bitmap;

    const int INTER_WIDTH = 25;

    static GameObject[] bitter_map;
    static List<GameObject> interactables;
    static int count_selected;
    int rounds_completed = 0;
    int max_rounds = 2;
    int giveups;
    float timeLimit = 60f;
    float secs_passed = 0f;
    bool start = false;
    List<string> cur_line;
    string cur_engl;
    static List<int> cur_locked;

    static bool check_flag;

    private List<List<string>> japanese_words;
    private List<string> english_words;
    private List<List<int>> locked_words; 
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
       
        rounds_completed = 0;
        giveups = 0;
        secs_passed = timeLimit;
        timer_text.text = secs_passed.ToString("0.00");
        static_wps = waypoints;
        next.GetComponent<Button>().onClick.AddListener(next_round);
        redo_button.onClick.AddListener(redo);
        giveup_button.onClick.AddListener(onclick_giveup);
        giveup_button.interactable = false;

        StartCoroutine("StartGame");
        StartCoroutine("LoadResources");
    }

    private IEnumerator LoadResources()
    {
        inter_sprites = Resources.LoadAll<Sprite>("Minigames/Grammar/WordOrder/interactables/");
        seacreatures = Resources.LoadAll<Sprite>("Minigames/Grammar/WordOrder/seacreatures/");
        seacreature_img.sprite = seacreatures[Random.Range(0, seacreatures.Length)];
        TMP_FontAsset[] fonts = Resources.LoadAll<TMP_FontAsset>("SU3DJPFont/TextMeshProFont/Selected/");
        font = fonts[0];
        yield break;
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(0.1f);
        randomCategory.transform.gameObject.SetActive(true);
        TextAsset[] texts = Resources.LoadAll<TextAsset>("Minigames/Grammar/WordOrder/Texts/");
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
        locked_words = new List<List<int>>();
        japanese_words = new List<List<string>>();
        english_words = new List<string>();
        string[] splitted = txtfile.text.Split("\n"[0]);
        for (int i = 1; i < splitted.Length; i++) {
            string[] line = splitted[i].Split("_"[0]);
            //numbers
            string[] locked_nums = line[line.Length - 1].Split(","[0]);
            List<int> int_list = new List<int>();
            foreach (string str in locked_nums) {
                int_list.Add(int.Parse(str));
            }
            locked_words.Add(int_list);

            //english
            english_words.Add(line[line.Length - 2]);

            //Japanese
            List<string> str_list = new List<string>();
            for (int j = 0; j < line.Length - 2; j++) {
                str_list.Add(line[j]);
            }
            japanese_words.Add(str_list);

        }
    }

    private IEnumerator setup_game() {
        //randomly choose a word
        bool found_new_sentence = false;
        int random = -1;
        while (!found_new_sentence) {
            random = Random.Range(0, japanese_words.Count);
            if (!words_selected.Contains(random)) {
                found_new_sentence = true;
                words_selected.Add(random);
            }
        }
        cur_line = japanese_words[random];
        cur_engl = english_words[random];
        cur_locked = locked_words[random];
        //create interactables
        interactables = create_interactables(cur_line[0]);
        yield return new WaitForSeconds(0.5f);
        //set wps
        int from_back = interactables.Count - 1;
        bitter_map = new GameObject[interactables.Count];
        wp_bitmap = new bool[static_wps.Length];
        string[] words = cur_line[0].Split("　"[0]);

        for (int i = 0; i < interactables.Count; i++) {
           
            for (int j = 0; j < cur_locked.Count; j++) {
                int locked_num = cur_locked[j] - 1;
                if (interactables[i].GetComponent<WordOrderCLickLetter>().get_correct_pos_i() == locked_num) {
                    WordOrderCLickLetter cl = interactables[i].GetComponent<WordOrderCLickLetter>();
                    cl.set_new_pos(static_wps[next_pos(locked_pos(locked_num, words), cl.word.Length)].localPosition);
                    cl.set_move_flag(true);
                    cl.set_disable(true);
                    Color tmp = interactables[i].GetComponent<Image>().color;
                    tmp.a = 0.5f;
                    interactables[i].GetComponent<Image>().color = tmp;
                    bitter_map[cur_locked[j] - 1] = interactables[i];
                }
            }

            interactables[from_back].GetComponent<WordOrderCLickLetter>().set_move_flag(true);
            from_back--;
            if (from_back != -1) {
                interactables[from_back].transform.GetChild(0).gameObject.SetActive(true);
            }
            deal_card.Play();
            yield return new WaitForSeconds(0.3f);
        }

        //round setup
        secs_passed = timeLimit;
        timer_text.text = secs_passed.ToString("0.00");
        round_counter.text = (rounds_completed + 1).ToString() + "/" + max_rounds.ToString();

        count_selected = 0;
        check_flag = false;
        redo_button.interactable = true;
        giveup_button.interactable = true;
        start = true;
    }

    public int locked_pos(int index, string[] words)
    {
        int waypoint = 0;
        for (int i = 0; i < words.Length; i++)
        {
            if (i == index)
            {
                return waypoint;
            }
            waypoint += words[i].Length;
        }
        return waypoint;
    }

    public static int next_pos(int start, int length)
    {
        for (int i = start; i < static_wps.Length; i++)
        {
            if (!wp_bitmap[i])
            {
                if (wp_bitmap[i + length - 1])
                {
                    i = i + length - 1; 
                    continue;
                } else
                {
                    for (int j = i; j < i + length; j++)
                    {
                        wp_bitmap[j] = true;
                    }
                    return i;
                }
            }
        }
        return 0;
    }


    //tbh this is all really jank, like i would really want to revamp this but i am quite tired of this project
    //and i want to work on something else so we're working with jank
    public static Vector3 available_pos(GameObject that, int length) {
        for (int i = 0; i < interactables.Count; i++) {
            if (bitter_map[i] == null) {
                bitter_map[i] = that;
                count_selected++;
                if (count_selected >= (interactables.Count - cur_locked.Count)) {
                    check_flag = true;
                }
                int new_pos = next_pos(0, length);
                that.GetComponent<WordOrderCLickLetter>().cur_chosen_pos = new_pos;
                return static_wps[new_pos].localPosition;
            }
        }
        //shouldn't get here
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
                secs_passed = 0f;
                timer_text.text = secs_passed.ToString("0.00");
                start = false;
                StartCoroutine("Failure");
            }
            else
            {
                timer_text.text = secs_passed.ToString("0.00");
            }
        }
        
    }

    private IEnumerator Failure()
    {
        start = false;
        failure_sfx.Play();
        giveups++;
        foreach (GameObject letter in interactables)
        {
            letter.GetComponent<WordOrderCLickLetter>().set_disable(true);
        }
        engl_text.text = cur_engl;
        kata_text.text = cur_line[0].Replace("　", string.Empty);
        correct_img.SetActive(true);
        correct_correct_img.SetActive(false);
        failure_img.SetActive(true);
        redo_button.interactable = false;
        giveup_button.interactable = false;
        yield return new WaitForSeconds(2f);
        EndGame(false);
    }

    private IEnumerator check_corr() {
        List<int> incorrect = new List<int>();
        int correct_index = -1;
         for (int i = cur_line.Count - 1; i >= 0; i--) {
            bool incorr_bool = false;
            string[] cur_correct = cur_line[i].Split("　"[0]);
            for (int j = 0; j < interactables.Count; j++) {
                if (cur_locked.Contains(j + 1)) {
                    continue;
                }
                WordOrderCLickLetter letter = bitter_map[j].GetComponent<WordOrderCLickLetter>();
                letter.set_disable(true);
                Color tmp = bitter_map[j].GetComponent<Image>().color;
                tmp.a = 0.5f;
                bitter_map[j].GetComponent<Image>().color = tmp;
                //TODO check this
                string cur_string = bitter_map[j].GetComponent<WordOrderCLickLetter>().word;
                if (!cur_correct[j].Equals(cur_string)) {
                    incorr_bool = true;
                    if (i == 0) {
                        incorrect.Add(j);
                    }
                }
            }
            if (!incorr_bool) {
                correct_index = i;
                break;
            }

        }
        yield return new WaitForSeconds(1f);
        if (incorrect.Count == 0) {
            start = false;
            engl_text.text = cur_engl;
            kata_text.text = cur_line[correct_index].Replace("　", string.Empty);
            correct_img.SetActive(true);
            correct_correct_img.SetActive(true);
            failure_img.SetActive(false);
            redo_button.interactable = false;
            giveup_button.interactable = false;
            success_sfx.Play();
            if (rounds_completed < japanese_words.Count) {
                rounds_completed++;
                if (rounds_completed >= max_rounds) {
                    yield return new WaitForSeconds(2f);
                    display_finish();
                } else {
                    next.SetActive(true);
                }
            } 
        } else {
            failure_sfx.Play();
            incorrect_img.SetActive(true);
            yield return new WaitForSeconds(3f);
            incorrect_img.SetActive(false);
            redo();
        }
    }
    private void redo()
    {
        for (int i = 0; i < bitter_map.Length; i++)
        {
            if (!cur_locked.Contains(i + 1) && bitter_map[i] != null)
            {
                Color tmp = bitter_map[i].GetComponent<Image>().color;
                tmp.a = 1f;
                bitter_map[i].GetComponent<Image>().color = tmp;
                WordOrderCLickLetter letter = bitter_map[i].GetComponent<WordOrderCLickLetter>();
                letter.set_disable(false);
                letter.return_home();
                for (int j = letter.cur_chosen_pos; j < letter.cur_chosen_pos + letter.num_chars; j++) { 
                    wp_bitmap[j] = false;
                }
                bitter_map[i] = null;
                count_selected--;
            }
        }
        deal_card.Play();
    }

    private void next_round() {
        buttonclick_sfx.Play();
        if (rounds_completed != japanese_words.Count) {
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
        congrats_con.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "You did it! ";
        congrats_con.SetActive(true);
    }
    
    private List<GameObject> create_interactables(string whole_word) {
        TMP_DefaultControls.Resources resources = new TMP_DefaultControls.Resources();
        List<GameObject> ret = new List<GameObject>();
        string[] word = whole_word.Split("　"[0]);  //poorly named but I don't feel like refactoring all of the word variables here.
        List<int> rand_letter = new List<int>();
        Debug.Log(whole_word);
        int y_multiplier = 0;
        float start_x_pos = -800;
        for (int i = 0; i < word.Length; i++) {

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
            trans.transform.SetParent(choose_container.transform);
            trans.localScale = new Vector3(INTER_WIDTH * 3f, INTER_WIDTH * 3f, 1);
            trans.sizeDelta = new Vector3(1, 1, 1);

            Image sprite = inter.GetComponent<Image>();
            sprite.sprite = null;
            trans.localPosition = new Vector3(-200, 500, -1);


            Destroy(inter.transform.GetChild(0).gameObject);
            for (int j = 0; j < word[word_index].Length; j++) {
                GameObject img_obj = new GameObject();
                RectTransform img_trans = img_obj.AddComponent<RectTransform>();
                img_trans.SetParent(inter.transform);
                img_trans.localScale = new Vector3(1, 1, 1);
                img_trans.sizeDelta = new Vector3(1, 1, 1);
                img_trans.localPosition = new Vector3(1 * j, 0, -1);
                Image img = img_obj.AddComponent<Image>();
                img.sprite = inter_sprites[1];

                
                GameObject letter = new GameObject();
                RectTransform letter_trans = letter.AddComponent<RectTransform>();
                letter_trans.SetParent(img_obj.transform);
                letter_trans.localScale = new Vector3(1, 1, 1);
                letter_trans.sizeDelta = new Vector3(1, 1, 1);
                letter_trans.localPosition = new Vector3(0, 0, 0);
                TextMeshProUGUI text = letter.AddComponent<TextMeshProUGUI>();
                text.text = word[word_index][j] + "";
                text.fontSize = 1f;
                text.font = font;
                text.color = Color.black;
                text.alignment = TextAlignmentOptions.Center;    
            }
            WordOrderCLickLetter click = inter.AddComponent<WordOrderCLickLetter>();
            float end_x_pos = start_x_pos + (INTER_WIDTH * 3f) * (word[word_index].Length + 0.5f);
            if (end_x_pos >= 600)
            {
                start_x_pos = -800;
                end_x_pos = start_x_pos + (INTER_WIDTH * 3f) * (word[word_index].Length + 0.5f);
                y_multiplier++;
            }
            Vector3 vec_pos = new Vector3(start_x_pos, 200 - y_multiplier * (INTER_WIDTH * 4), -1);
            
            start_x_pos = end_x_pos;
            click.set_new_pos(vec_pos);
            click.set_orig_pos(vec_pos);
            click.set_correct_pos(word_index);
            click.deal_card = deal_card;
            click.num_chars = word[word_index].Length;
            click.word = word[word_index];
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

    void onclick_giveup() {
        StartCoroutine("Failure");
    }
    
}
