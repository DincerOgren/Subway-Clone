using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPatternChooser : MonoBehaviour
{
    public GameObject[] patterns;
    private void OnEnable()
    {
        SetPattern();
        
    }
    void SetPattern()
    {
        ActiveSelectedPattern(GetPatternNum());
    }

    void ActiveSelectedPattern(int patternNum)
    {
        for (int i = 0; i < patterns.Length; i++)
        {
            if (i == patternNum)
            {
                patterns[patternNum].SetActive(true);
            }
            else
                patterns[patternNum].SetActive(false);

        }
    }
    int GetPatternNum()
    {
        return Random.Range(0, 3);
    }
}
