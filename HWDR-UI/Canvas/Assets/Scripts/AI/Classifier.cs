using System;
using Unity.Barracuda;
using UnityEngine;

public class Classifier : MonoBehaviour
{

    /// <summary>
    /// Model used to make predictions
    /// </summary>
    [SerializeField] private NNModel classifier;
    /// <summary>
    /// Worker used to run the model
    /// </summary>
    private IWorker worker;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    private void Start()
    {
        Model model = ModelLoader.Load(classifier);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
    }

    public float[] Predict(Texture2D texture)
    {
        Color[] bytes = texture.GetPixels();
        int size = (int)Mathf.Sqrt(bytes.Length);
        float[] data = new float[bytes.Length];
        float[] rotatedData = new float[bytes.Length];
        for (int i = 0; i < bytes.Length; i++)
        {
            int row = i / size;
            int col = i - row * size;
            rotatedData[col + (27 - row) * 28] = -(bytes[i].grayscale - 1);
        }
        data = rotatedData;

        Debug.Assert(data.Length == 28 * 28, $"{data.Length} != {28 * 28}");
        Tensor inputs = new Tensor(1, 28, 28, 1, data);
        worker.Execute(inputs);
        Tensor output = worker.PeekOutput();
        //Debug.Log(string.Join(" ", output.ToReadOnlyArray()));
        //Debug.Log(string.Join(" ", Softmax(output.ToReadOnlyArray())));

        return Softmax(output.ToReadOnlyArray());
    }

    private static float[] Softmax(float[] oNodes)
    {
        // softmax(x) = e ^ x / (e ^ x + e ^ y + e ^ z)
        // softmax(y) = e ^ y / (e ^ x + e ^ y + e ^ z)
        // softmax(z) = e ^ z / (e ^ x + e ^ y + e ^ z)

        // Find the max value for the softmax max trick
        float max = 0;
        foreach (float value in oNodes)
        {
            if (value > max) max = value;
        }

        // Calculates denominator
        float denominator = 0;
        foreach (float value in oNodes)
        {
            denominator += (float)Math.Exp(value - max);
        }

        // Calculates the resulting values
        float[] product = new float[oNodes.Length];
        for (int i = 0; i < product.Length; i++)
        {
            product[i] = (float)Math.Exp(oNodes[i] - max) / denominator;
        }

        // Returns the product
        return product;
    }
}
