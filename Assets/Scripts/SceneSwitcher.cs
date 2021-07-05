using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // 新しいシーンを追加したらここに増やせばいけるかも......
    // タイプミスに要注意!!!
    public enum Scene
    {
        GameScene,
        ResultScene,
        SampleScene,
        SelectScene,
        TestScene2,
        TitleScene,
        Unknown
    }
    [SerializeField]
    Scene prevScene = Scene.Unknown;
    [SerializeField]
    Scene nextScene = Scene.Unknown;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (nextScene != Scene.Unknown) SceneManager.LoadScene(nextScene.ToString());
        }

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (prevScene != Scene.Unknown) SceneManager.LoadScene(prevScene.ToString());
        }
    }
}
