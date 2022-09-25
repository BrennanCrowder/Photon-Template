using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameObject manager;

    void Awake()
    {
        if (manager != null && manager != this)
        {
            Destroy(this.gameObject);
        }
        manager = this.gameObject;
        DontDestroyOnLoad(this.gameObject);
    }

    public void Reset(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
