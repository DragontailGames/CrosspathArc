using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AnimationCustomController
{
    public string name;

    public List<AnimationCustom> animationCustoms;
}

[System.Serializable]
public class AnimationCustom
{
    public string name;

    public Sprite[] sprites;

    [Tooltip("Deixar vazio")]
    public List<AnimationFrames> animationFrames = new List<AnimationFrames>();

    public int spritesPerAnimation;
}

[System.Serializable]
public class AnimationFrames
{
    public string direction;

    public List<Sprite> sprites;
}
