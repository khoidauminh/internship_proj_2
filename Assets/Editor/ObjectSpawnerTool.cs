using UnityEngine;
using UnityEditor;

public class ObjectSpawnerTool : EditorWindow
{
    private GameObject objectToSpawn;
    private BaseUnitConfig config;

    [MenuItem("Tools/Object Spawner")]
    public static void ShowWindow()
    {
        GetWindow<ObjectSpawnerTool>("Object Spawner");
    }

    // This handles the visual presentation and interaction inside the window
    private void OnGUI()
    {
        GUILayout.Label("Spawn Settings", EditorStyles.boldLabel);

        objectToSpawn = (GameObject)EditorGUILayout.ObjectField("Prefab to Spawn", objectToSpawn, typeof(GameObject), false);

        config = (BaseUnitConfig)EditorGUILayout.ObjectField("Config to use", config, typeof(BaseUnitConfig), false);

        GUILayout.Space(10);

        if (GUILayout.Button("Spawn Objects"))
        {
            SpawnObjects();
        }
    }

    private void SpawnObjects()
    {
        if (objectToSpawn == null)
        {
            Debug.LogError("Please assign a Prefab to spawn!");
            return;
        }

        Vector3 randomPosition = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 2f), Random.Range(-1f, 1f));
        GameObject spawnedObj = Instantiate(objectToSpawn, randomPosition, Quaternion.identity);

        BaseUnitController controller = spawnedObj.GetComponent<BaseUnitController>();
        controller.Initialize(config.Get());

        EnemyManager e = FindAnyObjectByType<EnemyManager>();
        if (e == null)
        {
            Debug.Log("You need to be in the game scene where EnemyManager is present to use this tool.");
            return;
        }

        e.Adopt(controller);

        Undo.RegisterCreatedObjectUndo(spawnedObj, "Spawned Object");
    }
}
