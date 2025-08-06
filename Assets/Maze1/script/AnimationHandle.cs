using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class AnimationHandle : MonoBehaviour
{
    
    
 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AttackEndFun()
    {
        PlayerMovements.Instance.isAttack = false;
        PlayerMovements.Instance.wasWalking = false;
        PlayerMovements.Instance.wasGun = !PlayerMovements.Instance.isGun;
        PlayerMovements.Instance.HandleMovementInput(); // Resume walk/idle/gunwalk based on current input
    }
       


}
