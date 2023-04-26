using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;
using UnityEngine.UI;

public class ClassificationDisplay : MonoBehaviour
{
    /// <summary>
    /// Classifier used to make predictions
    /// </summary>
    private Classifier classifier;
    /// <summary>
    /// Texture with the digit, must be 28x28
    /// </summary>
    private Texture2D texture;
    /// <summary>
    /// Probabilities display
    /// </summary>
    [SerializeField] private Text textProbabilities;
    /// <summary>
    /// Prediction display
    /// </summary>
    [SerializeField] private Text textPrediction;

    // Start is called before the first frame update
    void Start()
    {
        texture = FindObjectOfType<DrawingCanvas>().GetTexture();
        classifier = FindObjectOfType<Classifier>();
    }

    // Update is called once per frame
    void Update()
    {
        float[] preds = classifier.Predict(texture);
         
        float max = -1;
        int best = -1;
        for (int i = 0; i < 10; i++)
        {
            if (max < preds[i])
            {
                max = preds[i];
                best = i;
            }
        }
        textPrediction.text = best.ToString();

        string proba = "";
        for (int i = 0; i < 10; i++)
        {
            proba += $"{i}: " + (preds[i]  * 100).ToString("0.00") + "%\n";
        }
        textProbabilities.text = proba;
    }
}
