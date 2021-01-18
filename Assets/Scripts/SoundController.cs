using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This component loads audio files in from game resources and has public method for playing those sounds when needed
public class SoundController : MonoBehaviour
{
    public static SoundController soundController;

    private AudioSource audioSource;

    private AudioClip movePieceSound;
    private AudioClip dirtSound;
    private AudioClip stoneSound;
    private AudioClip woodSound;

    // Start is called before the first frame update
    void Start()
    {
        soundController = this;
        audioSource = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();

        movePieceSound = Resources.Load<AudioClip>("Audio/wood-small");
        dirtSound = Resources.Load<AudioClip>("Audio/interface2");
        stoneSound = Resources.Load<AudioClip>("Audio/chainmail2");
        woodSound = Resources.Load<AudioClip>("Audio/cloth-heavy");

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayMovePieceSound()
    {
        audioSource.PlayOneShot(movePieceSound);
    }  
    
    public void PlayDirtSound()
    {
        audioSource.PlayOneShot(dirtSound);
    } 
    
    public void PlayStoneSound()
    {
        audioSource.PlayOneShot(stoneSound);
    } 
    
    public void PlayWoodSound()
    {
        audioSource.PlayOneShot(woodSound);
    }

    public void PlayResourceSound(Resource resource)
    {
        switch (resource)
        {
            case Resource.CLAY:
                PlayDirtSound();
                break;
            case Resource.STONE:
                PlayStoneSound();
                break;
            case Resource.WOOD:
                PlayWoodSound();
                break;
        }
    }
}
