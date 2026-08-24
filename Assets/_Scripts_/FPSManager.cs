using System;
using TMPro;
using UnityEngine;

public class FPSManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text fpsText;

    [Header("Settings")]
    [SerializeField, Min(0.1f)] private float updateInterval = 0.25f;
    [SerializeField] private int gameTargetFPS = 90;

    [Header("Colors")]
    [SerializeField] private Color goodColor = Color.green;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color badColor = Color.red;

    private float _elapsedTime;
    private int _frameCount;

    private float _currentFPS;
    private float _averageFPS; 
    private float _minFPS = float.MaxValue;
    private float _maxFPS;
    
    private void Awake()
    {
        Application.targetFrameRate = gameTargetFPS;
    }

    private void Update()
    {
        float deltaTime = Time.unscaledDeltaTime;

        if (deltaTime <= 0f)
        {
            return;
        }

        _frameCount++;
        _elapsedTime += deltaTime;

        _currentFPS = 1f / deltaTime;

        _minFPS = Mathf.Min(_minFPS, _currentFPS);
        _maxFPS = Mathf.Max(_maxFPS, _currentFPS);

        if (_elapsedTime >= updateInterval)
        {
            _averageFPS = _frameCount / _elapsedTime;

            UpdateDisplay();

            _elapsedTime = 0f;
            _frameCount = 0;
            _minFPS = float.MaxValue;
            _maxFPS = 0f;
        }
    }

    private void UpdateDisplay()
    {
        if (fpsText == null)
        {
            return;
        }

        fpsText.text = $"FPS: {Mathf.RoundToInt(_averageFPS)}\n" + $"{1000f / _averageFPS:F1} ms";

        fpsText.color = GetFPSColor(_averageFPS);
    }

    private Color GetFPSColor(float fps)
    {
        if (fps >= gameTargetFPS / 1.05f)
        {
            return goodColor;
        }

        if (fps >= gameTargetFPS / 1.3f)
        {
            return warningColor;
        }
        return badColor;
    }
}