﻿using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private SceneBounds _sceneBounds;

    public float enemiesVelocity = 1.0f;
    private float _enemiesVelocityMultiplier = 1;
    public float enemiesVelocityMultiplier => _enemiesVelocityMultiplier;

    public ScoreManager scoreManager;
    public long currentScore => scoreManager.currentScore;

    public LevelManager levelManager;
    public int currentLevel => levelManager.currentLevel;

    public EnemyPooler enemyPooler;

    public EnemySpawner enemySpawner;

    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            this._sceneBounds = new SceneBounds()
            {
                bottomRightCorner = this.SceneBottomRightCorner(),
                topLeftCorner = this.SceneTopLeftCorner()
            };
            this.StartGame();//TODO: this will be moved to an other place
            Instance = this;
        }
    }

    public void StartGame()
    {
        levelManager.Init();
        levelManager.LevelChanged += LevelManager_LevelChanged;
        scoreManager.init();
        enemySpawner.Init(currentLevel);
    }

    public void EndGame()
    {
        enemySpawner.Stop();
        scoreManager.stop();
        levelManager.LevelChanged -= LevelManager_LevelChanged;
        levelManager.Stop();
    }

    private void LevelManager_LevelChanged(object sender, int level)
    {
        enemySpawner.LevelChanged(level);
        enemyPooler.LevelChanged(level);
    }

    //horizontalMovement is a float between -1,1
    // Returns a value between -1 and 1
    public float ChangePlayerVelocity(float horizontalMovement)
    {
        _enemiesVelocityMultiplier = Mathf.Clamp(_enemiesVelocityMultiplier + horizontalMovement, 0.5f, 1.5f);
        return _enemiesVelocityMultiplier * 2 - 2;
    }

    public bool IsLocatedAtTheLeftOfTheScene(Vector3 transform, Vector3 localScale)
    {
        return transform.x + localScale.x / 2 < GameManager.Instance._sceneBounds.topLeftCorner.x;
    }

    public float ClampXInSceneBounds(float newX, float width)
    {
        return Math.Clamp(
                    newX,
                    GameManager.Instance._sceneBounds.left + width / 2,
                    GameManager.Instance._sceneBounds.right - width / 2
                );
    }

    public float ClampYInSceneBounds(float newY, float height)
    {
        return Math.Clamp(
            newY,
            GameManager.Instance._sceneBounds.bottom + height / 2,
            GameManager.Instance._sceneBounds.top - height / 2
        );
    }

    public float GetRandomYInSceneBounds(Vector3 scale)
    {
        var maxY = GameManager.Instance._sceneBounds.top - scale.y / 2;
        var minY = GameManager.Instance._sceneBounds.bottom + scale.y / 2;
        return UnityEngine.Random.Range(minY, maxY);
    }

    public float GetSceneMaxX()
    {
        return this._sceneBounds.right;
    }

    #region Bounds Auxiliar Methods

    private Vector3 SceneTopLeftCorner()
    {
        var topLeftCornerRef = GameObject.FindGameObjectWithTag("topLeftCorner");
        return topLeftCornerRef.transform.position;
    }

    private Vector3 SceneBottomRightCorner()
    {
        var bottomRightCornerRef = GameObject.FindGameObjectWithTag("bottomRightCorner");
        return bottomRightCornerRef.transform.position;
    }

    #endregion
}

public struct SceneBounds
{
    public float top
    {
        get
        {
            return topLeftCorner.y;
        }
    }

    public float bottom
    {
        get
        {
            return bottomRightCorner.y;
        }
    }

    public float left
    {
        get
        {
            return topLeftCorner.x;
        }
    }

    public float right
    {
        get
        {
            return bottomRightCorner.x;
        }
    }

    public Vector3 topLeftCorner;
    public Vector3 bottomRightCorner;
}