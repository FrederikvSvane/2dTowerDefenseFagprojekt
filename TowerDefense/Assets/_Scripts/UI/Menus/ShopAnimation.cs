using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopAnimation : MonoBehaviour
{
   [Header("References")]
   [SerializeField] Animator anim;

   private bool isMenuOpen = true;

   public void ToggleMenu(){
    isMenuOpen = !isMenuOpen;
    anim.SetBool("MenuOpen", isMenuOpen);
   }
}
