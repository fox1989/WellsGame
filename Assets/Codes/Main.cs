using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Main : MonoLogicNode
{

    private void Awake()
    {
        EventCenter eventCenter = new EventCenter();
        Attach(eventCenter);


        MWellsGame mGame = gameObject.AddComponent<MWellsGame>();
        Attach(mGame);

        LWellsGame lGame = new LWellsGame();
        Attach(lGame);

    }

    private void Start()
    {
        //EventCenter.Instance.SendEvent();

    }


    private void Update()
    {
        UpdateLogic(Time.deltaTime);
    }


}

