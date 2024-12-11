using System.Dynamic;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;

namespace MLBasics;

public class Coefficients
{
    public double A;
    public double B;
    public double C;
    public double E = double.NaN;

    public Coefficients(double A, double B, double C)
    {
        this.A = A;
        this.B = B;
        this.C = C;
    }
}

public class PredictionModel
{
    private int popSize;
    private List<Coefficients> coefficients;
    private int subPopCount;
    public Coefficients? BestCoefficients
    {
        get => coefficients.Count > 0 ? coefficients[0] : null;
    }

    public TimeSpan TrainingTime { get; private set; }
    public int Iterations { get; private set; }

    // PopSize is not always strictly obeyed.
    public PredictionModel(int popSize, int subPopCount)
    {
        this.popSize = popSize / subPopCount * subPopCount;
        this.subPopCount = subPopCount;
        coefficients = new(this.popSize);
    }

    public void GenerateInitalCoefficients()
    {
        lock (coefficients)
        {
            for (int i = 0; i < popSize; i++)
            {
                var a = Random.Shared.NextDouble() * (Random.Shared.Next(200) - 100);
                var b = Random.Shared.NextDouble() * (Random.Shared.Next(200) - 100);
                var c = Random.Shared.NextDouble() * (Random.Shared.Next(200) - 100);
                coefficients.Add(new Coefficients(a, b, c));
            }
        }
        Console.WriteLine("Initial coefficients generated.");
    }

    public void DivergeCoefficients(int maxIterations, int currentIteration)
    {
        var srcSize = popSize / subPopCount;
        var ratio = 1 - Math.Pow((double)currentIteration / maxIterations, 0.5);
        lock (coefficients)
        {
            Parallel.For(
                0,
                popSize / subPopCount,
                i =>
                {
                    var srcA = coefficients[i].A;
                    var srcB = coefficients[i].B;
                    var srcC = coefficients[i].C;
                    var srcE = coefficients[i].E;
                    for (int j = 1; j < subPopCount; j++)
                    {
                        var dstA =
                            srcA
                            + (Random.Shared.Next(200) - 100)
                                * Math.Min(0.2, ratio)
                                * Math.Min(1, srcE);
                        var dstB =
                            srcB
                            + (Random.Shared.Next(200) - 100)
                                * Math.Min(0.2, ratio)
                                * Math.Min(1, srcE);
                        var dstC =
                            srcC
                            + (Random.Shared.Next(200) - 100)
                                * Math.Min(0.2, ratio)
                                * Math.Min(1, srcE);
                        coefficients[srcSize * j + i] = new Coefficients(dstA, dstB, dstC);
                    }
                }
            );
        }
    }

    public void TrainAsync(IEnumerable<DataModel> dataModels, int iterations, double maxError)
    {
        var startTime = DateTime.Now;
        if (coefficients.Count == 0)
            GenerateInitalCoefficients();
        for (int i = 0; i < iterations; i++)
        {
            lock (coefficients)
            {
                Parallel.For(
                    0,
                    coefficients.Count,
                    i =>
                    {
                        var x = coefficients[i];
                        x.E = 0;
                        foreach (var y in dataModels)
                        {
                            var predicted = Function(x, y.Input);
                            var error = y.Observed - predicted;
                            // Console.WriteLine($"Iteration {i + 1}: Predicted = {predicted}, Error = {error}");
                            x.E += error * error;
                            // Console.WriteLine(x.E);
                        }
                        x.E /= dataModels.Count();
                    }
                );
                coefficients.Sort(
                    (a, b) =>
                    {
                        if (double.IsNaN(a.E))
                        {
                            if (double.IsNaN(b.E))
                                return 0;
                            else
                                return -1;
                        }
                        else if (double.IsNaN(b.E))
                            return 1;
                        else
                            return a.E.CompareTo(b.E);
                    }
                );
                Console.WriteLine($"Iteration {Iterations + 1}: Best E = {coefficients[0].E}");
                if (coefficients[0].E < maxError)
                {
                    Iterations++;
                    break;
                }
            }
            DivergeCoefficients(Iterations + iterations, Iterations + i);
            Iterations++;
        }
        var endTime = DateTime.Now;
        TrainingTime += endTime - startTime;
    }

    private double Function(Coefficients coefficients, double input)
    {
        return coefficients.A * input * input + coefficients.B * input + coefficients.C;
    }

    public double Predict(double input)
    {
        if (coefficients.Count == 0)
            throw new InvalidOperationException("Model has not been trained yet.");
        var bestCoefficients = BestCoefficients!;
        return Function(bestCoefficients, input);
    }
}
