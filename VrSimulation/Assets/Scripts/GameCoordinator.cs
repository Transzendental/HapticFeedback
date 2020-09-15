using System;
using UnityEngine;

public class GameCoordinator : MonoBehaviour
{
    public Bolt[] TrackedBolts = new Bolt[4];

    // goal configuarion - inital configuration
    private readonly float[][] Scenario1 = { new[] { 0.8f, 0.8f, 0.8f, 0.8f }, new[] { 0.3f, 0.3f, 0.3f, 0.3f } };
    private readonly float[][] Scenario2 = { new[] { 0.8f, 0.2f, 0.1f, 0.9f }, new[] { 0.5f, 0.6f, 0.8f, 0.5f } };

    private int choosenScenraio = 1;
    private int boltStateChangedCount = 0;
    private int evaluateCount = 0;

    public void StartScenarion(int choice)
    {
        choosenScenraio = choice;
        var i = 0;
        foreach (var bolt in TrackedBolts)
        {
            bolt.OnBoltStateChanged += () => { boltStateChangedCount++; };
            bolt.Tension = choosenScenraio == 1 ? Scenario1[1][i++] : Scenario1[2][i++];
        }
    }

    public void Evaluate()
    {
        var scenario = GetScenario();
        for (var i = 0; i < TrackedBolts.Length; i++)
        {
            var error = TrackedBolts[i].Tension - scenario[i];
            Debug.Log($"Delta tension on screw {i + 1} is {error}");
            TrackedBolts[i].Label.text = error.ToString();
        }
        evaluateCount++;
    }

    public void Finish()
    {
        var scenario = GetScenario();
        var nutScore = 0d;
        for (var i = 0; i < TrackedBolts.Length; i++)
        {
            nutScore += Math.Abs(TrackedBolts[i].Tension - scenario[i]) * 10;
        }

        var changedScore = boltStateChangedCount / 10f;
        var evalauteScore = evaluateCount / 5f;

        Debug.Log($"You finished this scenario with following stats: Nut: {nutScore} Changed: {changedScore} Evaluate: {evalauteScore} Resulting in a score of {nutScore + changedScore + evalauteScore} ");
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
}
