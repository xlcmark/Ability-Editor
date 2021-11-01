using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageNumFadeOut : MonoBehaviour
{
    public Text text;
    public float fadeOutSpeed = 5;
    Color color;

    public void SetNumber(string _num,Color _color)
    {
        text.text =_num;
        color = _color;
        text.color = color;
    }
    private void OnDisable()
    {
        gameObject.SetActive(false);
    }
    public void AnimEnd()
    {
        gameObject.SetActive(false);
    }
}
