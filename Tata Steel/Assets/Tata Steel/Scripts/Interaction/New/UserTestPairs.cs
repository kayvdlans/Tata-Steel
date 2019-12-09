using System;
using System.Collections.Generic;
using UnityEngine;

public class UserTestPairs : MonoBehaviour
{
    [Serializable]
    private struct Pair
    {
        [SerializeField] private ObjectDroppingArea area;
        [SerializeField] private UserTestThrowable testObject;

        public ObjectDroppingArea Area => area;
        public UserTestThrowable TestObject => testObject;
    }

    [SerializeField]
    private List<Pair> pairs;

    private void OnFinishTest()
    {
        //check which ones are right. visualize it someway. also push it to database for analysis.
        //db results => time, sequences (1; 5; 2-3-4; etc.), right ones (1, 4, 9, etc.), amount right, amount wrong, something else?
    }
}
