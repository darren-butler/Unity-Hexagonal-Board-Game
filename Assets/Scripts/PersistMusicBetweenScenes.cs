using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Component attatched to Background Music game object
// This allows the background music to persist between scenes uninterrupted
// https://www.youtube.com/watch?v=Xtfe5S9n4SI - this tutorial was used when implementing this feature
public class PersistMusicBetweenScenes : MonoBehaviour
{
    private void Awake()
    {
        GameObject[] musicObjects = GameObject.FindGameObjectsWithTag("Music");

        if(musicObjects.Length > 1)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
    }
}
