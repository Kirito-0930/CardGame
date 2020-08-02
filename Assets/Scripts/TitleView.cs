using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleView : MonoBehaviour
{
    [SerializeField] List<GameObject> cardObjects;

    float offSet_X = 0.36f, offSet_Z = 1.05f;

    void Start()
    {
        /*for (int i = 0; i < 53; i++) {
            cardObjects[i].transform.position = new Vector3(-2.15f + offSet_X * i, 0, 1.05f + offSet_Z * i);
        }*/
    }

    void Update()
    {
        
    }

    public void StartButton()
    {
        SceneManager.LoadScene(1);
    }
}
