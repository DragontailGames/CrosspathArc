using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class AnimatorEquipmentController : MonoBehaviour
{
    public List<AnimationCustomController> animationsCustom = new List<AnimationCustomController>();

    public AnimationCustomController currentAnimationCustom;

    public Sprite[] constantAnimation = new Sprite[] { };
    public Sprite[] framesAnimation = new Sprite[] { };

    public bool isWalking = false;

    public float delayBetweenFrames = 0.1f;

    private bool isDead = false;

    private string lastdirection = "S";
    private string lastAnimation = "";
    private int currentFrame;

    private Dictionary<int, string> fromToDirection = new Dictionary<int, string>()
    {
        {0, "S"},
        {1, "W"},
        {2, "E"},
        {3, "N"},
        {4, "SW"},
        {5, "NW"},
        {6, "SE"},
        {7, "NE"},
    };

    public void Start()
    {
        currentAnimationCustom = animationsCustom[0];
        SetupFrames();
    }

    public void SetupFrames()
    {
        foreach(var aux in currentAnimationCustom.animationCustoms)
        {
            aux.sprites = aux.sprites.OrderBy(n => n.name).ToArray();
            var auxFrames = aux.sprites;
            var spritePerAnimation = aux.spritesPerAnimation;
            aux.animationFrames = new List<AnimationFrames>();

            for (int i = 0; i < 8; i++)
            {
                aux.animationFrames.Add(new AnimationFrames()
                {
                    sprites = new ArraySegment<Sprite>(auxFrames, auxFrames.Length / 8 * i, spritePerAnimation).ToList(),
                    direction = fromToDirection[i]
                });
            }
        }
        PlayAnimation("Idle", "S", false, false, true);
    }

    IEnumerator animationCoroutine;

    public void PlayAnimation(string name, string direction, bool onlyOneCast, bool isDie = false, bool forceUpdate = false)
    {
        if(lastdirection == direction && lastAnimation == name && !forceUpdate)
        {
            StopCoroutine(animationCoroutine);
            if (constantAnimation.Length - 1 > currentFrame)
            {
                animationCoroutine = Play(currentFrame + 1);
            }
            else
            {
                animationCoroutine = Play(0);
            }
            StartCoroutine(animationCoroutine);
            return;
        }

        lastAnimation = name;
        lastdirection = direction;

        if(animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        if(isDead)
        {
            return;
        }
        var frames = currentAnimationCustom.animationCustoms.Find(n => n.name == name).animationFrames.Find(n=>n.direction == direction).sprites;
        if(onlyOneCast)
        {
            framesAnimation = frames.ToArray();
            constantAnimation = currentAnimationCustom.animationCustoms.Find(n => n.name == "Idle").animationFrames.Find(n=>n.direction == direction).sprites.ToArray();
        }
        else
        {
            framesAnimation = frames.ToArray();
            constantAnimation = frames.ToArray();
        }
        if(isDie == true)
        {
            framesAnimation = frames.ToArray();
            constantAnimation = frames.ToArray();
            isDead = true;
            StopCoroutine(animationCoroutine);
        }
        animationCoroutine = Play(0);
        StartCoroutine(animationCoroutine);
    }

    private IEnumerator Play(int index)
    {
        currentFrame = index;
        if(framesAnimation.Length <= 0)
        {
            framesAnimation = constantAnimation;
        }

        Sprite[] frames = framesAnimation;
        this.transform.GetComponent<SpriteRenderer>().sprite = frames[index];
        yield return new WaitForSeconds(delayBetweenFrames);
        if(frames.Length-1>index)
        {
            animationCoroutine = Play(index + 1);
            StartCoroutine(animationCoroutine);
        }
        else
        {
            framesAnimation = new Sprite[] { };
            animationCoroutine = Play(0);
            StartCoroutine(animationCoroutine);
        }
    }

    public void SetController(EnumCustom.ArmorType armorType, bool shield, bool sword)
    {
        string finalString = "";
        string armorName = "Naked";
        string equipment = "";

        if (shield && !sword)
        {
            equipment = "Shield";
        }
        if (!shield && sword)
        {
            equipment = "Sword";
        }
        if (shield && sword)
        {
            equipment = "Shield_Sword";
        }

        if (armorType != EnumCustom.ArmorType.None)
        {
            armorName = Enum.GetName(typeof(EnumCustom.ArmorType), armorType);
        }

        finalString = armorName + (equipment != "" ? ("_" + equipment) : "");

        currentAnimationCustom = animationsCustom.Find(n => n.name.EndsWith(finalString));
        SetupFrames();
    }
}
