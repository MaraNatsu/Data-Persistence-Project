using Assets.Scripts.Helpers;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private InputField _userNameInput;

    private Color _inputPlaceholderColor;

    private void Awake()
    {
        _inputPlaceholderColor = _userNameInput.placeholder.color;
    }

    public void StartGame()
    {
        if (string.IsNullOrEmpty(_userNameInput.text) || string.IsNullOrWhiteSpace(_userNameInput.text))
        {
            _userNameInput.text = null;
            StartCoroutine(WaitWarningColor());
        }
        else
        {
            SceneManager.LoadScene(1);
            MainManager.IsGameActive = true;
        }
    }

    public void SaveName()
    {
        User user = User.GetInstance();
        user.Name = _userNameInput.text;
    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    IEnumerator WaitWarningColor()
    {
        SetPlaceholderColor(Color.red);
        yield return new WaitForSeconds(.5f);
        SetPlaceholderColor(_inputPlaceholderColor);
    }

    void SetPlaceholderColor(Color color)
    {
        _userNameInput.placeholder.color = color;
    }
}
