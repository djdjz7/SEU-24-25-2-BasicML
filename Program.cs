﻿using System;

namespace MLBasics;

class Program
{
    const int POP_SIZE = 10000;
    const int SUB_POP_COUNT = 10000;
    const double MAX_ERROR = 0.00001;
    const int ITERATIONS = 1000;

    public static void Main()
    {
        var inputFile = "sampledata.txt";
        var lines = File.ReadAllLines(inputFile);
        var data = lines
            .Select(x =>
                x.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            )
            .Where(x => x.Length == 2)
            .Select(x => new DataModel(double.Parse(x[0]), double.Parse(x[1])));
        var model = new PredictionModel(POP_SIZE, SUB_POP_COUNT);
        model.TrainAsync(data, ITERATIONS, MAX_ERROR);
        var coefficients = model.BestCoefficients;
        if (coefficients is null)
        {
            Console.WriteLine("Training failed.");
            return;
        }
        Console.WriteLine($"Training Finished @ {model.Iterations}. Best coefficients:");
        Console.WriteLine($"A: {coefficients.A}");
        Console.WriteLine($"B: {coefficients.B}");
        Console.WriteLine($"C: {coefficients.C}");
        Console.WriteLine($"Training Time: {model.TrainingTime} ");
        Console.WriteLine("Enter input values to predict: (or 'r' to continue training)");
        while (true)
        {
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                break;
            }
            if (input == "r")
            {
                model.TrainAsync(data, ITERATIONS, MAX_ERROR);
                Console.WriteLine($"Training Finished @ {model.Iterations}. Best coefficients:");
                Console.WriteLine($"A: {coefficients.A}");
                Console.WriteLine($"B: {coefficients.B}");
                Console.WriteLine($"C: {coefficients.C}");
                Console.WriteLine($"Training Time: {model.TrainingTime} ");
                Console.WriteLine("Enter input values to predict: (or 'r' to continue training)");
                continue;
            }
            var value = double.Parse(input);
            Console.WriteLine($"Predicted value: {model.Predict(value)}");
        }
    }
}
