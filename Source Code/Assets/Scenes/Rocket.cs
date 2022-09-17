
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{

    Rigidbody rigidBody;
    AudioSource audioSource;
    private enum State { Alive, Dying, Transcending }
    private State state = State.Alive;
    bool collisionEnabled = true;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem levelCompleteParticles;

    [SerializeField] float rcsThrust = 300f;
    [SerializeField] float mainThrust = 1200f;
    [SerializeField] float levelLoadDelay = 2f;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
            if (Debug.isDebugBuild)
            {
                ChangeLevelInput();
                CollisionToggle();
            }
        }

        if(Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void CollisionToggle()
    {
        if(Input.GetKey(KeyCode.C))
        {
            collisionEnabled = !collisionEnabled;
        }
    }

    private void ChangeLevelInput()
    {
        if(Input.GetKey(KeyCode.L))
        {
            LoadNextLevel();
        }
    }

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true; //pause physics

        ApplyRotation();

        rigidBody.freezeRotation = false; //resume physics

    }

    private void ApplyRotation()
    {
        float rotationThisFrame = rcsThrust * Time.deltaTime; // for different frame durations

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
    }

    private void RespondToThrustInput()
    {

        if (Input.GetKey(KeyCode.Space)) //can thrust while rotating
        {
            ApplyThrust();
        }
        else
        {
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        float thrustThisFrame = mainThrust * Time.deltaTime;

        rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
        
        if(!mainEngineParticles.isPlaying)
        {
            mainEngineParticles.Play();
        }
       
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || !collisionEnabled)
        {
            return;
        }
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                //do nothing
                break;
            case "Finish":
                SuccessSequence();
                break;
            default:
                DeathSequence();
                break;
        }
    
    }

    private void DeathSequence()
    {
        state = State.Dying;
       
        if(!deathParticles.isPlaying)
        {
            deathParticles.Play();
        }
        Invoke(nameof(LoadCurrentLevel), levelLoadDelay);      //reload start level
    }

    private void SuccessSequence()
    {
        state = State.Transcending;
        
        if(!levelCompleteParticles.isPlaying)
        {
            levelCompleteParticles.Play();
        }
        Invoke(nameof(LoadNextLevel), levelLoadDelay); //load scene with delay to allow time for particles
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadCurrentLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex != SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
        else
        {
            audioSource.Stop();
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            GameObject.FindGameObjectWithTag("Success").GetComponent<TextMesh>().text="Success!";
            Invoke(nameof(LoadFirstLevel), 8f);
        }
    }
}

