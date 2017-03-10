using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepButton : MonoBehaviour 
{
    private Animator animator;

    public bool isOn = false;

    public void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (SettingManager.settings.isTutorialOn == false && collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            SettingManager.settings.isTutorialOn = true;
            animator.SetTrigger("stepOn");
        }
        else if (SettingManager.settings.isTutorialOn == true && collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            SettingManager.settings.isTutorialOn = false;
            animator.SetTrigger("stepOff");
        }
    }
}
