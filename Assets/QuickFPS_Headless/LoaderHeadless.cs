using System.Collections;
using UnityEngine;
using System.Collections.Generic;
public class LoaderHeadless : MonoBehaviour
{
    private float totalTime = 0;
    int counter = 0;
    float tempSum = 0;
    private bool done = false;
    private string finalOutput = "ZZRES>> ";

    class SceneResult
    {
        public string sceneName;
        public float avg;
        public float min;
        public float max;
    }

    [SerializeField]
    [Tooltip("How much time (in seconds) to wait before recording frames")]
    float preTestTime = 5;
    [SerializeField]
    [Tooltip("How much time (in seconds) the test should take")]
    float testDuration = 15;

    Coroutine test;
    List<SceneResult> results = new List<SceneResult>();

    // Use this for initialization
    void Start()
    {
        Application.targetFrameRate = 1000;                 // For wherever it's relevant, set a very large target to strive for
        Debug.Log("[AutoRunner] Graphics API = " + SystemInfo.graphicsDeviceType);
        Time.maximumDeltaTime = 0.5f;                       // Setting a higher value than default of 0.1 to increase fidelity of results
        DontDestroyOnLoad(gameObject);
        test = StartCoroutine(RunTest());
        Debug.Log("[AutoRunner] APP_STARTED");
    }

    void Update()
    {
        totalTime += Time.deltaTime;
    }

    public void FinishTest(float avgF, float minF, float maxF)
    {
        results.Add(new SceneResult { sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, avg = avgF, min = minF, max = maxF });
        if (NextScene())
        {
            Debug.Log("[AutoRunner] Starting next scene");
        } 
        else
        {
            finalOutput += "{ \"Results\": {" +
                                  "\"Scenes\": {";
            foreach (var result in results)
            {
                finalOutput += string.Format(" \"{0}\": {{ " +
                                                 "\"AVERAGE\": {1}," +
                                                 "\"MIN\": {2}," +
                                                 "\"MAX\": {3} " +           
                                                 "}},",     //Add a comma for repeating scenes the last of which we will remove after the loop
                                                 result.sceneName, result.avg, result.min, result.max); 
            }
            // Take off the last character from the list of scenes that was a hanging comma
            finalOutput = finalOutput.Remove(finalOutput.Length - 1);
            // Close up the JSON
            finalOutput += " } } }";
            Debug.Log(finalOutput);
            Debug.Log("[AutoRunner] Finish test");
            Application.Quit();
            StopAllCoroutines(); //In the case when running on editor
        }

    }

    bool NextScene()
    {
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        int currScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        if (currScene < sceneCount - 1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(currScene + 1,UnityEngine.SceneManagement.LoadSceneMode.Single);
            ResetTest();
            test = StartCoroutine(RunTest());
            return true;
        }
        return false;
    }

    void ResetTest()
    {
        tempSum = 0;
        counter = 0;
        totalTime = 0;
        done = false;
    }

    public IEnumerator RunTest()
    {
        yield return new WaitForSeconds(preTestTime);
        float frameTime = Time.deltaTime;
        float min = frameTime;
        float max = frameTime;
        while (true)
        {
            frameTime = Time.deltaTime;
            if (totalTime < preTestTime + testDuration)
            {
                if (frameTime < min) min = frameTime;
                if (frameTime > max) max = frameTime;
                tempSum += frameTime;
                counter++;
            }
            else
            {
                done = true;
            }

            if (done)
            {
                float avg = (tempSum / counter) * 1000;
                min *= 1000;
                max *= 1000;
                StopCoroutine(test);
                FinishTest(avg, min, max);
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
