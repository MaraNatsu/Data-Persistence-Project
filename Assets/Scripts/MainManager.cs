using Assets.Scripts.Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public static bool IsGameActive { get; set; } = false;

    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public GameObject GameOverText;

    private User _user;

    //private bool _isGameOver = false;
    private BestScoreManager _scoreManager;


    private void Awake()
    {
        _user = User.GetInstance();
        _scoreManager = BestScoreManager.Instance;
        ScoreText.text = $"{_user.Name} score: " + _user.Score;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject parent = new GameObject();
        parent.name = "Bricks";
        Instantiate(parent, Vector3.zero, Quaternion.identity);

        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity, parent.transform);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }

        parent.transform.localScale = new Vector3(.9f, .82f, 1);
    }

    private void Update()
    {
        Ball.transform.localScale += Vector3.one;

        if (IsGameActive)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                IsGameActive = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                _user.ResetScore();
                IsGameActive = true;
            }
        }
    }

    void AddPoint(int point)
    {
        _user.IncreaseScore(point);
        ScoreText.text = $"{_user.Name} score: {_user.Score}";
    }

    public void GameOver()
    {
        IsGameActive = false;
        GameOverText.SetActive(true);
        _scoreManager.RecordHighScore();
        _scoreManager.LoadHighScore();
    }
}
