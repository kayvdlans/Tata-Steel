using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBehavior : MonoBehaviour
{
    Animator m_Animator;
    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();

        m_Animator.SetBool("SlowCycle", false);
        m_Animator.SetBool("FastCycle", false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StopFan()
    {
        m_Animator.SetBool("SlowCycle", false);
        m_Animator.SetBool("FastCycle", false);

    }

    public void Click1Speed()
    {
        m_Animator.SetBool("SlowCycle", true);
        m_Animator.SetBool("FastCycle", false);
    }
    public void Click2Speed()
    {
        m_Animator.SetBool("FastCycle", true); 

    }
}
