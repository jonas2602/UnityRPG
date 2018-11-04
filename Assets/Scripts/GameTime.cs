using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameTime : MonoBehaviour
{
    [System.Serializable]
    public struct Clock
    {
        public int second;
        public int minute;
        public int houre;
        public int day;

        public Clock(int second, int minute, int houre, int day)
        {
            this.second = second;
            this.minute = minute;
            this.houre = houre;
            this.day = day;
        }

        public string GetTime
        {
            get
            {
                return houre + ":" + minute + ":" + second;
            }
        }
    }

    [System.Serializable]
    public struct Action
    {
        public GameObject actor;
        public string action;
        public int message;

        public Action(GameObject actor, string action)
        {
            this.actor = actor;
            this.action = action;
            this.message = -1;
        }


        public Action(GameObject actor, string action, int message)
        {
            this.actor = actor;
            this.action = action;
            this.message = message;
        }

    }

    public float timer;
    public float timeScale = 48;
    public Clock gameTime;
    public Transform sun;
    public List<Action>[] dailys = new List<Action>[24];
    public List<Action>[] weeklys = new List<Action>[7];

    // Use this for initialization
    void Awake()
    {
        gameTime = new Clock(0, 0, 0, 0);
        for (int i = 0; i < dailys.Length; i++)
        {
            dailys[i] = new List<Action>();
        }

        for (int i = 0; i < weeklys.Length; i++)
        {
            weeklys[i] = new List<Action>();
        }

        // setup dailys
        Schedule[] schedules = Resources.FindObjectsOfTypeAll<Schedule>();

        for(int i = 0; i < schedules.Length; i++)
        {
            schedules[i].SetupAlerts(GetComponent<GameTime>());
        }

        // dailys[1].Add(new Action(GameObject.FindWithTag("Player"),"testen"));
    }

    void FixedUpdate()
    {
        timer += Time.deltaTime * timeScale;
        gameTime.second = (int)timer % 60;
        gameTime.minute = (int)timer / 60 % 60;
        
        int nextHoure = (int)timer / 3600 % 24;
        if (nextHoure != gameTime.houre)
        {
            gameTime.houre = nextHoure;
            ActivateDailys(nextHoure);
        }

        int nextDay = (int)timer / 86400 % 7;
        if (nextDay != gameTime.day)
        {
            gameTime.day = nextDay;
            ActivateWeeklys(nextDay);
        }

        sun.rotation = Quaternion.Euler(360 * ((timer % 86400) / 86400), 0, 0);
    }
    
    // Update is called once per frame
    void Update()
    {

    }

    void ActivateDailys(int houre)
    {
        Debug.Log("Activate Dailys");
        for (int i = 0; i < dailys[houre].Count; i++)
        {
            Action curAction = dailys[houre][i];
            Debug.Log(gameTime.GetTime + ", " + curAction.actor.name + ", " + curAction.action);
            if (curAction.message != -1)
            {
                curAction.actor.SendMessage(curAction.action, curAction.message);
            }
            else
            {
                curAction.actor.SendMessage(curAction.action);
            }
        }
    }

    void ActivateWeeklys(int day)
    {
        Debug.Log("Activate Weeklys");
        for (int i = 0; i < weeklys[day].Count; i++)
        {
            Action curAction = weeklys[day][i];
            curAction.actor.SendMessage(curAction.action);
            if (curAction.message != -1)
            {
                curAction.actor.SendMessage(curAction.action, curAction.message);
            }
            else
            {
                curAction.actor.SendMessage(curAction.action);
            }
        }
    }
}
