using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject StartButton;
    [SerializeField] private GameObject OptionsButton;
    [SerializeField] private GameObject ExitButton;
    [SerializeField] private TransitionLayerController transitionLayerController;

    [SerializeField] private float introDuration = 0.6f;
    [SerializeField] private float staggerDelay = 0.15f;

    private void Start()
    {
        SetupButton(StartButton, OnStartButtonClicked);
        SetupButton(OptionsButton, OnOptionsButtonClicked);
        SetupButton(ExitButton, OnExitButtonClicked);

        PlayIntroAnimation();
    }

    private void SetupButton(GameObject buttonObj, UnityEngine.Events.UnityAction action)
    {
        Button button = buttonObj.GetComponent<Button>();
        button.onClick.AddListener(action);

        AddHoverAnimation(buttonObj);
    }

    #region Intro Animation

    private void PlayIntroAnimation()
    {
        AnimateButtonIntro(StartButton.transform, 0f);
        AnimateButtonIntro(OptionsButton.transform, staggerDelay);
        AnimateButtonIntro(ExitButton.transform, staggerDelay * 2f);
    }

    private void AnimateButtonIntro(Transform target, float delay)
    {
        target.localScale = Vector3.zero;

        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = target.gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;

        target.DOScale(1f, introDuration)
              .SetEase(Ease.OutBack)
              .SetDelay(delay);

        canvasGroup.DOFade(1f, introDuration)
                   .SetDelay(delay);
    }

    #endregion

    #region Hover Animation

    private void AddHoverAnimation(GameObject buttonObj)
    {
        EventTriggerListener listener = buttonObj.AddComponent<EventTriggerListener>();

        listener.onEnter += () =>
        {
            buttonObj.transform.DOScale(1.1f, 0.15f)
                .SetEase(Ease.OutQuad);
        };

        listener.onExit += () =>
        {
            buttonObj.transform.DOScale(1f, 0.15f)
                .SetEase(Ease.OutQuad);
        };

        listener.onClick += () =>
        {
            buttonObj.transform.DOPunchScale(
                new Vector3(0.1f, 0.1f, 0f),
                0.2f,
                8,
                0.8f
            );
        };
    }

    #endregion

    private void OnStartButtonClicked()
    {
        if (transitionLayerController != null)
        {
            transitionLayerController.OpenCurtain(() =>
            {
                SceneManager.LoadScene("LoadingScene");
            });
        }
    }

    private void OnOptionsButtonClicked()
    {
        Debug.Log("Options button clicked");
    }

    private void OnExitButtonClicked()
    {
        Application.Quit();
    }
}
