using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Blackboard : MonoBehaviour
{
    public float timeOfDay;
    public Text clock;
    static Blackboard _instance;
    public static Blackboard Instance
    {
        get
        {
            if (!_instance)
            {
                Blackboard[] blackboards = GameObject.FindObjectsByType<Blackboard>(FindObjectsSortMode.None);
                if (blackboards != null)
                {
                    if(blackboards.Length == 1)
                    {
                        _instance = blackboards[0];
                        return _instance;
                    }
                }
                GameObject go = new GameObject("Blackboard", typeof(Blackboard));
                _instance = go.GetComponent<Blackboard>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
        set
        {
            _instance = value as Blackboard;
        }
    }

    public void Start()
    {
        StartCoroutine("UpdateClock");
    }

    IEnumerator UpdateClock()
    {
        while (true)
        {
            timeOfDay += 1;
            if (timeOfDay >23)
            {
                timeOfDay = 0;
            }
            clock.text = timeOfDay + ":00";
            yield return new WaitForSeconds(1);
        }
    }
}
