// This file was auto-generated by ML.NET Model Builder. 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks.Sources;
using FacialExpressionRecognitionML.Model;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Vision;

namespace FacialExpressionRecognitionML.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            MLContext mlContext = new MLContext();
          

            var filePaths = @"c:\Users\matit\Desktop\testImages\";

            IEnumerable<ModelInput> images = LoadImagesFromDirectory(folder: filePaths, useFolderNameAsLabel: true);
            IDataView imageData = mlContext.Data.LoadFromEnumerable(images);

               DataViewSchema modelSchema;
            ITransformer trainedModel = mlContext.Model.Load(@"c:\Users\matit\Desktop\aplikacjaV2\FacialExpressionRecognition\MLModel.zip", out modelSchema);

            IDataView shuffledData = mlContext.Data.ShuffleRows(imageData);

            var preprocessingPipeline = mlContext.Transforms.Conversion.MapValueToKey(
         inputColumnName: "Label",
         outputColumnName: "LabelAsKey")
     .Append(mlContext.Transforms.LoadRawImageBytes(
         outputColumnName: "Image",
         imageFolder: filePaths,
         inputColumnName: "ImageSource"));

            IDataView preProcessedData = preprocessingPipeline
                    .Fit(shuffledData)
                    .Transform(shuffledData);

            var evalPredictions = trainedModel.Transform(shuffledData);

            var metrics = mlContext.MulticlassClassification.Evaluate(evalPredictions);

            Console.WriteLine($"************************************************************");
            Console.WriteLine($"*    Metrics for multi-class classification model   ");
            Console.WriteLine($"*-----------------------------------------------------------");
            Console.WriteLine($"    MacroAccuracy = {metrics.MacroAccuracy:0.####}, a value between 0 and 1, the closer to 1, the better");
            Console.WriteLine($"    MicroAccuracy = {metrics.MicroAccuracy:0.####}, a value between 0 and 1, the closer to 1, the better");
            Console.WriteLine($"    LogLoss = {metrics.LogLoss:0.####}, the closer to 0, the better");
            Console.WriteLine($"    Confusion MATRIX = {metrics.ConfusionMatrix.GetFormattedConfusionTable()}");
            Console.WriteLine($"    TopKAccuracy = {metrics.TopKAccuracy:0.####}, the closer to 0, the better");
            Console.WriteLine($"    TopKPredictionCount = {metrics.TopKPredictionCount:0.####}, the closer to 0, the better");
            Console.WriteLine($"PerClassLogLoss is: {String.Join(" , ", metrics.PerClassLogLoss.Select(c => c.ToString()))}");



            //var imageDataView = context.Data.LoadFromEnumerable(data);
            //imageDataView = context.Data.ShuffleRows(imageDataView);

            //var dataProcessPipeline = context.Transforms.Conversion.MapValueToKey("Label", "Label")
            //               .Append(context.Transforms.LoadRawImageBytes("ImageSource_featurized", null, "ImageSource"))
            //               .Append(context.Transforms.CopyColumns("Features", "ImageSource_featurized"));
            //var trainer = context.MulticlassClassification.Trainers.ImageClassification(new ImageClassificationTrainer.Options() { LabelColumnName = "Label", FeatureColumnName = "Features" })
            //                         .Append(context.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel")).Fit(imageDataView);


            //var trainingPipeline = dataProcessPipeline.Append(trainer);
            //var predictions = trainedModel.Transform(imageDataView);
            //var metrics = context.MulticlassClassification.Evaluate(predictions);
            //var list = images.ToList();

            //int a = 0, b = 0, c = 0, d = 0;

            //for(int i=0; i < list.Count(); i++)
            //{
            //    var predictionResult = ConsumeModel.Predict(list[i]);
                
            ////    Console.WriteLine($"ImageSource: {list[i].ImageSource}");
            //  //  Console.WriteLine($"ImageSource: {list[i].Label}");
            //    if (predictionResult.Prediction == "smutek")
            //    {
            //        a++;
            //    }
            //    if (predictionResult.Prediction == "usmiech")
            //    {
            //        b++;
            //    }
            //    if (predictionResult.Prediction == "zdegustowanie")
            //    {
            //        c++;
            //    }
            //    if (predictionResult.Prediction == "zdziwienie")
            //    {
            //        d++;
            //    }
            //    Console.WriteLine($"\n\nPredicted Label value {predictionResult.Prediction} \nPredicted Label scores: [{String.Join(",", predictionResult.Score)}]\n\n");

           // }

            //Console.WriteLine("smutek" + " " + a);
            //Console.WriteLine("usmiech" + " " + b);
            //Console.WriteLine("zdegustowanie" + " " + c);
            //Console.WriteLine("zdziwienie" + " " + d);
            //Console.ReadKey();

        }

       
        public static IEnumerable<ModelInput> LoadImagesFromDirectory(string folder, bool useFolderNameAsLabel = true)
        {
            var files = Directory.GetFiles(folder, "*",
                searchOption: SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if ((Path.GetExtension(file) != ".jpg") && (Path.GetExtension(file) != ".png"))
                    continue;

                var label = Path.GetFileName(file);

                if (useFolderNameAsLabel)
                    label = Directory.GetParent(file).Name;
                else
                {
                    for (int index = 0; index < label.Length; index++)
                    {
                        if (!char.IsLetter(label[index]))
                        {
                            label = label.Substring(0, index);
                            break;
                        }
                    }
                }

                yield return new ModelInput()
                {
                    ImageSource = file,
                    Label = label
                };
            }
        }
    }

   
}
