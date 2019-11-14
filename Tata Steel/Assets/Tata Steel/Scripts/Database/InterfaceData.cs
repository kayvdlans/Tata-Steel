﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceData : MonoBehaviour
{
    public enum Type
    {
        Time,
        Points,
        Mistakes,
        Attempts
    }

    [SerializeField] private Type dataType;

    public Type DataType { get => dataType; }
}
