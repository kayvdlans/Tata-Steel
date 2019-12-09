using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class ObjectInteractionState : MonoBehaviour
{
    public enum State
    {
        Scatter,
        Grabbable
    }

    //Dont really know if this is gonna be used but jsut in case.
    public State CurrentState { get; private set; }

    public UnityAction<State> OnUpdateState { get; set; }

    private void Update()
    {
        //Just for debugging purposes, remove later.
        if (Input.GetKeyDown(KeyCode.F))
            UpdateState(State.Scatter);
    }

    public void UpdateState(State state)
    {
        OnUpdateState?.Invoke(CurrentState = state);
    }
}
