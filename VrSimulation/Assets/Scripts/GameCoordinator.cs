using System;
using UnityEngine;

public class GameCoordinator : MonoBehaviour
{
    public Bolt[] TrackedBolts = new Bolt[4];
    // Time which is considered good
    public int ReferenceTime = 300;

    // goal configuarion - inital configuration
    private readonly float[][] Scenario1 = { new[] { 0.8f, 0.8f, 0.8f, 0.8f }, new[] { 0.3f, 0.3f, 0.3f, 0.3f } };
    private readonly float[][] Scenario2 = { new[] { 0.2f, 0.2f, 0.1f, 0.9f }, new[] { 0.5f, 0.6f, 0.8f, 0.5f } };

    private int choosenScenraio;
    private float elapsedTime;
    
    public void StartScenarion(int choice)
    {
        choosenScenraio = choice;
        elapsedTime = 0;
    }

    public void Evaluate()
    {
        var scenario = GetScenario();
        for (var i = 0; i < TrackedBolts.Length; i++)
        {
            Debug.Log($"Delta tension on screw {i + 1} is {TrackedBolts[i].Tension - scenario[i]}");
        }
    }

    public void Finish()
    {
        var scenario = GetScenario();
        var nutScore = 0d;
        for (var i = 0; i < TrackedBolts.Length; i++)
        {
            nutScore += ScoreTension(TrackedBolts[i].Tension, scenario[i]);
        }

        var score = nutScore + elapsedTime / ReferenceTime;
        Debug.Log("You finished this scenario with an score of: " + score);
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
    }

    private float[] GetScenario()
    {
        switch (choosenScenraio)
        {
            case 1:
                return Scenario1[0];
            case 2:
                return Scenario2[0];
            default:
                throw new ArgumentException();
        }

    }

    /// <summary>
    /// Evaluation function: |goal - is|³. Allowing small  deviations and punishing larger. Is between 0 and 1.
    /// </summary>
    /// <param name="actual"></param>
    /// <param name="goal"></param>
    /// <returns></returns>
    private double ScoreTension(float actual, float goal) => Math.Pow(Math.Abs(actual - goal), 3);
}
