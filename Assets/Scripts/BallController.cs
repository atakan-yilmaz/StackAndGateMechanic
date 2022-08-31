using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BallController : MonoBehaviour
{
    [SerializeField] private GameObject _ballPrefab;
    [SerializeField] private TMP_Text _ballCountText = null; //ballun uzerindeki degisken sayiyi tanimlamak icin
    [SerializeField] private List<GameObject> _balls = new List<GameObject>();
    [SerializeField] private float _horizontalSpeed;
    [SerializeField] private float _horizontalLimit;
    [SerializeField] private float _moveSpeed;


    private float _horizontal;
    private int _gateNumber;
    private int _targetCount;

    private void Start()
    {
        
    }

    private void Update()
    {
        HorizontalBallMove();
        ForwardBallMove();
        UpdateBallCountText();
    }

    private void HorizontalBallMove()
    {
        float _newX;

        if (Input.GetMouseButton(0))
        {
            _horizontal = Input.GetAxisRaw("Mouse X");
        }
        else
        {
            _horizontal = 0;
        }

        _newX = transform.position.x + _horizontal * _horizontalSpeed * Time.deltaTime;
        _newX = Mathf.Clamp(_newX, -_horizontalLimit, _horizontalLimit); ////x pozisyonu yeniden ayarlandi ve ground icerisine sabitlendi

        transform.position = new Vector3(_newX, transform.position.y, transform.position.z); //topun kendisine dahil etmek icin
    }

    private void ForwardBallMove()
    {
        transform.Translate(Vector3.forward * _moveSpeed * Time.deltaTime);
    }

    private void UpdateBallCountText() //ballun uzerindeki degisken sayiyi tanimlamak icin 
    {
        _ballCountText.text = _balls.Count.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("StackBall"))
        {
            other.gameObject.transform.SetParent(transform);
            other.gameObject.GetComponent<SphereCollider>().enabled = false; //spherelerin icerisindeki collideri kapattim cunku stackler arkaya gelecek
            other.gameObject.transform.localPosition = new Vector3(0f, 0f, _balls[_balls.Count - 1].transform.localPosition.z - 1f);//stacklenen toplarin duzgun hizada arka arkaya gelmesini saglayan kod.
            _balls.Add(other.gameObject);
        }

        if (other.gameObject.CompareTag("Gate"))
        {
            _gateNumber = other.gameObject.GetComponent<GateController>().GetGateNumber();
            _targetCount = _balls.Count + _gateNumber;

            if (_gateNumber > 0)
            {
                IncreaseBallCount();
            }
            else if (_gateNumber < 0)
            {
                DecreaseBallCount();
            }
        }
    }

    private void IncreaseBallCount()
    {
        for (int i = 0; i < _gateNumber; i++)
        {
            GameObject _newBall = Instantiate(_ballPrefab);
            _newBall.transform.SetParent(transform);
            _newBall.GetComponent<SphereCollider>().enabled = false;
            _newBall.transform.localPosition = new Vector3(0f, 0f, _balls[_balls.Count - 1].transform.localPosition.z - 1f);
            _balls.Add(_newBall);
        }
    }

    private void DecreaseBallCount()
    {
        for (int i = _balls.Count -1; i >= _targetCount; i--)
        {
            _balls[i].SetActive(false);
            _balls.RemoveAt(i);
        }
    }
}