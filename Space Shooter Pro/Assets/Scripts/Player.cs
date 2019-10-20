using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // public or private refrence
    //data type (int , float , bool,  string)
    //every variable has a name
    //optional value assigned
    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private float _speedMultiplier = 2;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;
    [SerializeField]
    private bool _tripleShotActive = false;
    [SerializeField]
    private bool _isSpeedBoostActive = false;
    [SerializeField]
    private bool _isShieldsActive = false;
    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private int _score;
    [SerializeField]
    private UIManager _uiManager;
    // Start is called before the first frame update
    void Start()
    {
        //take the current position = new position (0,0,0)
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is Null");
        }
        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is Null");
        }

    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }
    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        if (_isSpeedBoostActive == false)
        {
            transform.Translate(direction * _speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(direction * _speed * _speedMultiplier * Time.deltaTime);
        }
        

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x< -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    
    }

    void FireLaser() 
    {
        // if i hit space key spawn game object
        _canFire = Time.time + _fireRate;

        if(_tripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }

        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.0f, 0), Quaternion.identity);
        }

    }

    public void Damage()
    {
        if(_isShieldsActive == true)
        {
            _isShieldsActive = false;
            _shieldVisualizer.SetActive(false);
            return;
        }

        _lives--;

        _uiManager.UpdateLives(_lives);

        if (_lives < 1 )
        {
            //communicate with spawn manager
            _spawnManager.OnPlayerDeath();

            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive()
    {
        _tripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _tripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }
    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedBoostActive = false;
    }
    public void ShieldsActive()
    {
        _isShieldsActive = true;
        _shieldVisualizer.SetActive(true);
        StartCoroutine(ShieldsPowerDownRoutine());
    }
    IEnumerator ShieldsPowerDownRoutine()
    {
        yield return new WaitForSeconds(10.0f);
        _shieldVisualizer.SetActive(false);
        _isShieldsActive = false;
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

}
