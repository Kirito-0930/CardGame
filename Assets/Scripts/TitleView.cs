using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleView : MonoBehaviour
{
    [SerializeField] List<GameObject> cardObjects;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void StartButton()
    {
        SceneManager.LoadScene(1);
    }
}
