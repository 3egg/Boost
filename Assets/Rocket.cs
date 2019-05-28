using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] private float rccThrust = 300f;
    [SerializeField] private float mainThrust = 2000f;
    [SerializeField] private float levelLoadDelay = 2f;
    new Rigidbody rigidbody;

    [SerializeField] private ParticleSystem successParticle;
    [SerializeField] private ParticleSystem deathParticle;
    [SerializeField] private ParticleSystem thrustParticle;

    [SerializeField] private AudioClip successClip;

    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip thrustClip;

    private State state = State.Alive;

    enum State
    {
        Alive,
        Dead,
        Transcend
    }

    private bool immortal;
    AudioSource audioSource;

    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        //冻结除了移动方向外的收引力作用的移动
        /*rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;

        rigidbody.constraints = RigidbodyConstraints.FreezeRotationX;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotationY;*/
    }

    // Update is called once per frame
    void Update()
    {
        /*rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotationX;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotationY;*/
        if (state == State.Alive)
        {
            Rotate();
            Thrust();
        }

        MoveForward();

        if (Debug.isDebugBuild)
        {
            PressLKey();
            PressCKeyForImmortal();
        }
    }

    private void MoveForward()
    {
    }

    private void OnCollisionEnter(Collision other)
    {
        if (immortal)
        {
            return;
        }

        if (state == State.Dead)
            return;

        //transform.Translate(new Vector3(0,0,0));
        switch (other.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                StartSuccess();

                break;
            default:
                StartDeath();
                break;
        }
    }

    private void StartDeath()
    {
        if (state == State.Transcend)
        {
            return;
        }

        state = State.Dead;
        deathParticle.Play();
        audioSource.Stop();
        audioSource.PlayOneShot(deathClip);
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void StartSuccess()
    {
        if (state == State.Alive)
        {
            state = State.Transcend;
            audioSource.Stop();
            audioSource.PlayOneShot(successClip);
            successParticle.Play();
            Invoke("LoadNextLevel", levelLoadDelay);
        }
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;//获取到所有scene.是数量,不是index
        if (currentScene == SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(0);//这里是加载索引.
        }
        else
        {
            SceneManager.LoadScene(currentScene + 1);
        }
    }

    private void Rotate()
    {
        //每次移动要冻结偏移
        rigidbody.freezeRotation = true;
        float rotateThisFrame = rccThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            //根据火箭前进的方向来rotate偏移火箭
            transform.Rotate(Vector3.forward * rotateThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotateThisFrame);
        }

        rigidbody.freezeRotation = false;
    }

    private void Thrust()
    {
        {
            if (Input.GetKey(KeyCode.Space))
            {
                //vector就是对象的transform里面的position
                rigidbody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
                if (!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(thrustClip);
                    thrustParticle.Play();
                }
            }
            else
            {
                audioSource.Stop();
                thrustParticle.Stop();
            }
        }
    }

    private void PressLKey()
    {
        if (Input.GetKey(KeyCode.L))
        {
            LoadNextLevel();
        }
    }

    private void PressCKeyForImmortal()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (immortal)
            {
                immortal = false;
            }
            else
            {
                immortal = true;
            }
        }
    }
}