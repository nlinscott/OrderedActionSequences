using NUnit.Framework;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestSceneLoader 
{
    private GameObject[] _rootGameObjects;

    public static class RootObjects
    {
        public const string SequenceDataExtractorTest = "SequenceDataExtractorTest";
    }

    private const string TestScene = "TestScene";

    public IEnumerator LoadTestScene()
    {
        Scene testScene = SceneManager.GetSceneByName(TestScene);

        if (!testScene.IsValid())
        {
            Debug.Log("loading scene");

            AsyncOperation op = SceneManager.LoadSceneAsync(TestScene);

            yield return new WaitUntil(() =>
            {
                return op.isDone;
            });

            testScene = SceneManager.GetSceneByName(TestScene);
        }

        Assert.That(testScene.IsValid());
        Assert.That(testScene.isLoaded);

        _rootGameObjects = testScene.GetRootGameObjects();
    }

    public GameObject GetTestContextByName(string name)
    {
        GameObject go = _rootGameObjects.FirstOrDefault(g => string.Equals(g.name, name, System.StringComparison.InvariantCultureIgnoreCase));

        Assert.That(go, Is.Not.Null);

        return go;
    }
}
