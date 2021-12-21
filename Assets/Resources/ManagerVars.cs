using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "CreateManagerVarsContainer")]
public class ManagerVars : ScriptableObject {

    public static ManagerVars GetManagerVars()
    {
        return Resources.Load<ManagerVars>("ManagerVars");
    }
    public Sprite whiteSprite;
    public Sprite blackSprite;

    public GameObject whiteGo;
    public GameObject blackGo;

    public List<Sprite> landlordSpriteList;
    public Sprite landlordBc;
    public Sprite farmer;
    public GameObject poker;
    public Sprite leftSpriteFarmer;
    public Sprite rightSpriteFarme;
    public Sprite leftSpriteLand;
    public Sprite rightSpriteLand;
    public List<Sprite> selfSpriteHead;
}
