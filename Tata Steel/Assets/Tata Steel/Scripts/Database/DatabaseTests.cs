using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseTests : MonoBehaviour
{
    [SerializeField] private UserData user;
    [SerializeField] private uint id;

    private void Start()
    {
        user.InitializeUser(id);    
    }
}
