using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordOrderCLickLetter : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource deal_card;
    bool chosen;
    public bool disable;
    public bool move_flag = false;
    public Vector3 original_pos;
    public Vector3 new_pos;
    public int cur_chosen_pos;
    public int correct_pos_i = -1;
    public int num_chars;
    public string word;

    void Start()
    {
        disable = false;
        chosen = false;
    }

    public void set_correct_pos(int i)
    {
        correct_pos_i = i;
    }

    public void set_new_pos(Vector3 pos)
    {
        new_pos = pos;
    }

    public void set_orig_pos(Vector3 pos)
    {
        original_pos = pos;
    }

    public void set_move_flag(bool move)
    {
        move_flag = move;
    }

    public void OnMouseDown()
    {
        if (!disable && !move_flag)
        {
            deal_card.Play();
            if (chosen)
            {
                /*
                GameControl.remove_pos(this.gameObject, num_chars);
                new_pos = original_pos;
                move_flag = true;
                chosen = false;
                */
            }
            else
            {
                new_pos = WordOrder.available_pos(this.gameObject, num_chars);
                move_flag = true;
                chosen = true;
            }
        }
    }

    public void set_disable(bool val)
    {
        disable = val;
        chosen = false; //lmao kinda lazy
    }

    public Vector3 get_pos()
    {
        return original_pos;
    }

    public int get_correct_pos_i()
    {
        return correct_pos_i;
    }

    public bool get_disabled()
    {
        return disable;
    }

    public void return_home()
    {
        new_pos = original_pos;
        move_flag = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (move_flag)
        {
            move();
        }

    }

    void move()
    {
        transform.localPosition = Vector2.MoveTowards(transform.localPosition, new_pos, 2000f * Time.deltaTime);
        if (Vector2.Distance(transform.localPosition, new_pos) < 0.1f)
        {
            move_flag = false;
        }

    }
}
