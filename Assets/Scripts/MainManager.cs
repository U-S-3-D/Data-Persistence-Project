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
    public int scorePoints;

    public Text ScoreText;
    public Text BestScoreText; //For best score
    public GameObject GameOverText;
    
    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;

    
    // Start is called before the first frame update
    void Start()
    {
        LoadData();

        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                if(SceneManager.GetActiveScene().buildIndex == 1)
                {
                    var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                    brick.PointValue = pointCountArray[i];
                    brick.onDestroyed.AddListener(AddPoint);
                }
                
            }
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        scorePoints = m_Points;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        
        //SaveData();
        LoadData();
    }

    public void SaveData()
    {
        BestScoreSaveData bestScoreData = new BestScoreSaveData();
        bestScoreData.bestScoreName = MenuManager.Name;
        bestScoreData.bestScore = scorePoints;
        string json = JsonUtility.ToJson(bestScoreData);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json); //Saved as JSON
        DisplayBestScore(bestScoreData);
    }

    public void LoadData()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if(!File.Exists(path)) //if file doesn't exist, then there's no save, call SaveData()
        {
            SaveData();
        }
        else
        {
            //A save file exists, compare current score with best score, if current score > best score, update best score, else don't update
            string json = File.ReadAllText(path);
            BestScoreSaveData loadedData = JsonUtility.FromJson<BestScoreSaveData>(json);
            if(scorePoints > loadedData.bestScore)
            {
                SaveData();
            }
            else
            {
                DisplayBestScore(loadedData);
            }
        }
    }

    public void DisplayBestScore(BestScoreSaveData bestScoreData)
    {
        BestScoreText.text = "Best Score : " + bestScoreData.bestScoreName + " : " + bestScoreData.bestScore; //Update best score on screen
        MenuManager.BestScoreText = BestScoreText;
    }

    [System.Serializable]
    public class BestScoreSaveData
    {
        public string bestScoreName;
        public int bestScore;
    }
}
