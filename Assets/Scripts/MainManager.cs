using Assets.Scripts.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text HighScoreText;
    public Text ScoreText;
    public GameObject GameOverText;

    private string _userName;
    private string _path = @"D:\GIT\Unity\Unity Learn - Junior Programmer\Manage scene flow and data\Data-Persistence-Project-";
    private string _jsonName = @"\highscore";

    private int _score;
    private ScoreStorage _prevRecord;
    private bool _isGameStarted = false;
    private bool _isGameOver = false;


    private void Awake()
    {
        _userName = User.Name;
        ScoreText.text = $"{_userName} score: " + _score;
        LoadHighScore();
    }

    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        if (!_isGameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _isGameStarted = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (_isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    [System.Serializable]
    class ScoreStorage
    {
        public string userName;
        public int userScore;

        public ScoreStorage(string name, int score)
        {
            userName = name;
            userScore = score;
        }
    }

    void LoadHighScore()
    {
        string path = _path + _jsonName;
        //string path = Application.persistentDataPath + _jsonName + ".json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            _prevRecord = JsonUtility.FromJson<ScoreStorage>(json);
            HighScoreText.text = $"Best score: {_prevRecord.userName} - {_prevRecord.userScore}";
        }
        else
        {
            HighScoreText.text = "Best score: Good luck!";
        }
    }

    void RecordHighScore()
    {
        if (_prevRecord != null && (_score < _prevRecord.userScore))
        {
            return;
        }

        ScoreStorage scoreRecording = new ScoreStorage(_userName, _score);
        string json = JsonUtility.ToJson(scoreRecording);
        File.WriteAllText(_path + _jsonName, json);
    }

    void AddPoint(int point)
    {
        _score += point;
        ScoreText.text = $"{_userName} score: {_score}";
    }

    public void GameOver()
    {
        _isGameOver = true;
        GameOverText.SetActive(true);
        RecordHighScore();
        LoadHighScore();
    }
}
