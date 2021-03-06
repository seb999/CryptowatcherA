using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CryptowatcherA.Misc;
using CryptowatcherA.ClassTransfer;
using System.IO;
using Microsoft.ML;
using static CryptowatcherA.Misc.Prediction;

namespace cryptowatcherR.Controllers
{

    [Route("api/[controller]")]
    public class AIController : Controller
    {
        public List<PredictionTransfer> GetPrediction(string symbol, SymbolTransfer coinTicker)
        {
            List<PredictionTransfer> predictionList = new List<PredictionTransfer>();

            //1 - List models available for symbol
            var symbolPair = Helper.GetSymbolPair(symbol);
            var rootFolder = Environment.CurrentDirectory + "/aiModel/" + symbolPair + "/";
            var modelPathList = Directory.GetFiles(rootFolder, symbol + "*", SearchOption.AllDirectories);

            if (modelPathList.Length == 0)
                return predictionList;

            //3 - Iterate throw model and fire prediction
            foreach (var modelPath in modelPathList)
            {
                PredictionTransfer prediction = new PredictionTransfer();

                var fromIndex = Path.GetFileName(modelPath).IndexOf("-") + 1;
                var toIndex = Path.GetFileName(modelPath).Length - fromIndex - 4;
                prediction.ModelName = Path.GetFileName(modelPath).Substring(fromIndex, toIndex);

                prediction.FuturePrice = Math.Round(CalculatePrediction(coinTicker, modelPath).FuturePrice, 2);
                predictionList.Add(prediction);
            }

            return predictionList;
        }

        private CoinPrediction CalculatePrediction(SymbolTransfer coin, string modelPath)
        {
            //Load model
            ITransformer loadedModel = LoadModel(modelPath);

            //Predict future price
            return PredictFuturePrice(coin, loadedModel);
        }

        /// <summary>
        ///  Calculate prediction with default Model (Fast tree for home page)
        /// </summary>
        /// <param name="coinList">The list of coinTicketTransfer</param>
        /// <returns>void</returns>
        internal static void CalculatePredictionDefaultModel(string symbol, ref QuotationTransfer coin)
        {
            if (CheckModelExist(symbol) == true)
            {
                string modelPath = Environment.CurrentDirectory + "/aiModel/" + symbol + "-Fast Tree.zip";

                //Load model
                ITransformer loadedModel = LoadModel(modelPath);

                //Predict future price
                MLContext mlContext = new MLContext();
                var predictionFunction = mlContext.Model.CreatePredictionEngine<CoinData, CoinPrediction>(loadedModel);
                CoinPrediction prediction = predictionFunction.Predict(new CoinData
                {
                    Volume = (float)coin.Volume,
                    Open = (float)coin.Open,
                    Rsi = (float)coin.Rsi,
                    MacdHist = (float)coin.MacdHist,
                });

                coin.FuturePrice = prediction.FuturePrice;
            }
        }

        private static bool CheckModelExist(string symbol)
        {
            var symbolPair = Helper.GetSymbolPair(symbol);
            var rootFolder = Environment.CurrentDirectory + "/aiModel/" + symbolPair + "/";
            return Directory.GetFiles(rootFolder, symbol + "*", SearchOption.AllDirectories).Length > 0 ? true : false;
        }

        private static ITransformer LoadModel(string modelPath)
        {
            // MLContext mlContext = new MLContext();

            // ITransformer loadedModel;
            // using (var stream = new FileStream(modelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            // {
            //      mlContext.Model.LoadWithDataLoader(stream, out loadedModel);
            // }
            // return loadedModel;
            return null;
        }

        private static CoinPrediction PredictFuturePrice(SymbolTransfer coin, ITransformer model)
        {
            MLContext mlContext = new MLContext();
            var predictionFunction = mlContext.Model.CreatePredictionEngine<CoinData, CoinPrediction>(model);
            CoinPrediction prediction = predictionFunction.Predict(new CoinData
            {
                Volume = (float)coin.Volume,
                Open = (float)coin.OpenPrice,
                Rsi = (float)coin.Rsi,
                MacdHist = (float)coin.MacdHist,
            });

            return prediction;
        }
    }
}