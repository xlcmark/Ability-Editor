
using UnityEngine;

//計時器//放一個在場景中
public class Thinker : MonoBehaviour
{
    public static Thinker instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

}
