using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    // Start is called before the first frame update
    public int maxNumberOfShots = 3;
    public int usedNumberOfShots;

    [SerializeField] private int waitBeforEnd = 3;
    [SerializeField] private SlingShotHandler slingShot;
    [SerializeField] private GameObject restartScreen;
    [SerializeField] private Image nextLevelImage;

    private List<Piggie> piggies = new List<Piggie>();

    private IconHandler birdIcon;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        birdIcon = FindObjectOfType<IconHandler>();
        Piggie[] allPiggies = FindObjectsOfType<Piggie>();

        for (int i = 0; i < allPiggies.Length; i++)
        {
            piggies.Add(allPiggies[i]);
        }
        nextLevelImage.enabled = false;

    }
    public void UsedShot()
    {
        usedNumberOfShots++;
        birdIcon.UseShot(usedNumberOfShots);

        CheckLastShot();
    }
    public bool HasEnoughShots()
    {
        if(usedNumberOfShots < maxNumberOfShots)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void CheckLastShot()
    {
        if(usedNumberOfShots == maxNumberOfShots)
        {
            StartCoroutine(CheckAfterWaitTime());
        }
    }

    private IEnumerator CheckAfterWaitTime()
    {
        yield return new WaitForSeconds(waitBeforEnd);

        if( piggies.Count == 0)
        {
            WinGame();
        }
        else
        {
            RestartGame();
        }

    }

    private void CheckAllPiggieDead()
    {
        if(piggies.Count == 0)
        {
            WinGame();
        }
    }
    public void RemovePiggie(Piggie removePiggie)
    {
        piggies.Remove(removePiggie);
        CheckAllPiggieDead();
    }

    #region WinOrLose
    private void WinGame()
    {
        slingShot.enabled = false;
        restartScreen.SetActive(true);
        //Debug.Log("Win Game");

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int maxSceneIndex = SceneManager.sceneCountInBuildSettings;
        if(currentSceneIndex+1 < maxSceneIndex)
        {
            nextLevelImage.enabled = true;
        }
    }
    public void RestartGame()
    {
        DOTween.Clear(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    #endregion
}
