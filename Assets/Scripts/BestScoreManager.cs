using Assets.Scripts.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class BestScoreManager : MonoBehaviour
{
    public static BestScoreManager Instance;

    [SerializeField]
    private Button _ratingButton;
    [SerializeField]
    private Text _bestScoreText;
    [SerializeField]
    private GameObject _ratingMenu;
    [SerializeField]
    private Text _ratingText;

    private User _user;
    private ScoreStorage _prevBestScore;
    private RatingStorage _prevRating;

    private string _path = @"D:\GIT\Unity\Unity Learn - Junior Programmer\Manage scene flow and data\Data-Persistence-Project";
    private string _jsonName = @"\highscore";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _user = User.GetInstance();
        LoadHighScore();
    }

    private void Update()
    {
        if (MainManager.IsGameActive)
        {
            _ratingButton.interactable = false;
            _ratingMenu.SetActive(false);
        }
        else
        {
            _ratingButton.interactable = true;
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

    [System.Serializable]
    class RatingStorage
    {
        public List<ScoreStorage> rating = new List<ScoreStorage>();
    }

    public void LoadHighScore()
    {
        string path = _path + _jsonName;
        //string path = Application.persistentDataPath + _jsonName + ".json";

        if (File.Exists(path) && File.ReadAllText(path).Length > 0)
        {
            string json = File.ReadAllText(path);
            _prevRating = JsonUtility.FromJson<RatingStorage>(json);
            _prevBestScore = _prevRating.rating[0];
            _bestScoreText.text = $"Best score: {_prevBestScore.userName} - {_prevBestScore.userScore}";
        }
        else
        {
            _prevRating = new RatingStorage();
            _bestScoreText.text = "Best score: can be yours!";
        }
    }

    //void UpdateRating()
    //{
    //    string path = _path + _jsonName;
    //    string json = File.ReadAllText(path);
    //    _prevRating = JsonUtility.FromJson<RatingStorage>(json);
    //}

    public void RecordHighScore()
    {
        ScoreStorage newScore = new ScoreStorage(_user.Name, _user.Score);

        if (_prevBestScore != null && (_prevBestScore.userScore < _user.Score))
        {
            RemoveDoubleRecord(_user.Name);
            _prevRating.rating.Insert(0, newScore);
        }
        else
        {
            RemoveDoubleRecord(newScore, out bool hasRecordCracked, out bool hasTween);

            if (hasRecordCracked || !hasTween)
            {
                _prevRating.rating.Add(newScore);
                _prevRating.rating.OrderByDescending(x => x.userScore);
            }
        }

        string json = JsonUtility.ToJson(_prevRating);
        File.WriteAllText(_path + _jsonName, json);
    }

    public void ClearRating()
    {
        string path = _path + _jsonName;
        File.WriteAllText(path, string.Empty);
        LoadHighScore();
        FillScores();
    }

    public void SwitchRating()
    {
        if (_ratingMenu.activeInHierarchy)
        {
            _ratingMenu.gameObject.SetActive(false);
        }
        else
        {
            _ratingMenu.gameObject.SetActive(true);
            FillScores();
        }
    }

    void FillScores()
    {
        List<ScoreStorage> rating = _prevRating.rating;
        StringBuilder userScores = new StringBuilder();
        int index = 1;

        foreach (var user in rating)
        {
            userScores.AppendLine($"{index}. {user.userName} - {user.userScore}");
            index++;
        }

        _ratingText.text = userScores.ToString();
    }

    void RemoveDoubleRecord(string name)
    {
        foreach (var score in _prevRating.rating)
        {
            if (score.userName == name)
            {
                _prevRating.rating.Remove(score);
                break;
            }
        }
    }

    void RemoveDoubleRecord(ScoreStorage user, out bool hasRecordCracked, out bool hasTwin)
    {
        hasRecordCracked = false;
        hasTwin = false;

        foreach (var score in _prevRating.rating)
        {
            if (score.userName == user.userName)
            {
                hasTwin = true;
                break;
            }

            if (score.userName == user.userName && score.userScore < user.userScore)
            {
                _prevRating.rating.Remove(score);
                hasRecordCracked = true;
                break;
            }
        }
    }
}
