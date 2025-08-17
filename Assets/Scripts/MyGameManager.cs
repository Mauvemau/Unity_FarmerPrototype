using System;
using UnityEngine;

public class MyGameManager : MonoBehaviour {
    [ContextMenu("Force Close Game")]
    private void ExitApplication() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ExitApplication();
        }
    }
}
