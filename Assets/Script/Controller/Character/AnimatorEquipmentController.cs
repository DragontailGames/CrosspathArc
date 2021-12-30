using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEquipmentController : MonoBehaviour
{
    public RuntimeAnimatorController naked_hand;
    public RuntimeAnimatorController naked_hand_shield;
    public RuntimeAnimatorController naked_hand_shield_sword;
    public RuntimeAnimatorController naked_hand_sword;

    public RuntimeAnimatorController heavy_hand;
    public RuntimeAnimatorController heavy_hand_shield;
    public RuntimeAnimatorController heavy_hand_shield_sword;
    public RuntimeAnimatorController heavy_hand_sword;

    public RuntimeAnimatorController light_hand;
    public RuntimeAnimatorController light_hand_shield;
    public RuntimeAnimatorController light_hand_shield_sword;
    public RuntimeAnimatorController light_hand_sword;

    public RuntimeAnimatorController medium_hand;
    public RuntimeAnimatorController medium_hand_shield;
    public RuntimeAnimatorController medium_hand_shield_sword;
    public RuntimeAnimatorController medium_hand_sword;

    public Animator animator;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    public void SetController(EnumCustom.ArmorType armorType, bool shield, bool sword)
    {
        if (armorType == EnumCustom.ArmorType.None)
        {
            if(shield || sword)
            {
                if(shield && !sword)
                {
                    animator.runtimeAnimatorController = naked_hand_shield;
                }
                if(!shield && sword)
                {
                    animator.runtimeAnimatorController = naked_hand_sword;
                }
                if(shield && sword)
                {
                    animator.runtimeAnimatorController = naked_hand_shield_sword;
                }
            }
            else
            {
                animator.runtimeAnimatorController = naked_hand;
            }
        }
        if (armorType == EnumCustom.ArmorType.Heavy)
        {
            if (shield || sword)
            {
                if (shield && !sword)
                {
                    animator.runtimeAnimatorController = heavy_hand_shield;
                }
                if (!shield && sword)
                {
                    animator.runtimeAnimatorController = heavy_hand_sword;
                }
                if (shield && sword)
                {
                    animator.runtimeAnimatorController = heavy_hand_shield_sword;
                }
            }
            else
            {
                animator.runtimeAnimatorController = heavy_hand;
            }
        }
        if (armorType == EnumCustom.ArmorType.Light)
        {
            if (shield || sword)
            {
                if (shield && !sword)
                {
                    animator.runtimeAnimatorController = light_hand_shield;
                }
                if (!shield && sword)
                {
                    animator.runtimeAnimatorController = light_hand_sword;
                }
                if (shield && sword)
                {
                    animator.runtimeAnimatorController = light_hand_shield_sword;
                }
            }
            else
            {
                animator.runtimeAnimatorController = light_hand;
            }
        }
        if (armorType == EnumCustom.ArmorType.Medium)
        {
            if (shield || sword)
            {
                if (shield && !sword)
                {
                    animator.runtimeAnimatorController = medium_hand_shield;
                }
                if (!shield && sword)
                {
                    animator.runtimeAnimatorController = medium_hand_sword;
                }
                if (shield && sword)
                {
                    animator.runtimeAnimatorController = medium_hand_shield_sword;
                }
            }
            else
            {
                animator.runtimeAnimatorController = medium_hand;
            }
        }
    }
}
