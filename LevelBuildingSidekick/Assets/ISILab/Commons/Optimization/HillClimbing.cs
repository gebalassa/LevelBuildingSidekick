using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LBS.Representation.TileMap;

namespace Utility
{
    public class HillClimbing
    {
        static float prevScore = 0;
        public static float PrevScore
        {
            get
            {
                return prevScore;
            }
        }

        static float score = 0;
        public static float Score
        {
            get
            {
                return score;
            }
        }

        static int nonSignificantEpochs = 0;
        public static int NonSignificantEpochs
        {
            get
            {
                return nonSignificantEpochs;
            }
        }

        public static T Run<T>(T root, System.Func<bool> endCondition, System.Func<T, List<T>> GetNeighbors, System.Func<T, float> Evaluate)
        {
            var r = new System.Random();
            score = 0;
            nonSignificantEpochs = 0;
            prevScore = score;
            T best = root;
            while (endCondition?.Invoke() == false)
            {
                prevScore = score;
                score = Evaluate(best);
                List<T> candidates = GetNeighbors(best);
                List<T> betters = new List<T>();
                Debug.Log("Before: " + candidates.Count);
                float higherScore = score;
                for(int i = 0; i < candidates.Count; i++)
                {
                    float newScore = Evaluate(candidates[i]);
                    if (newScore > higherScore)
                    {
                        betters.Clear();
                        betters.Add(candidates[i]);
                    }
                    else if(newScore == higherScore)
                    {
                        betters.Add(candidates[i]);
                    }
                    higherScore = higherScore < newScore ? newScore : higherScore;
                    //Debug.Log("New Score: " + newScore);

                }
                if(higherScore <= score)
                {
                    nonSignificantEpochs++;
                }
                else
                {
                    nonSignificantEpochs = 0;
                }
                if(betters.Count == 0)
                {
                    return best;
                }
                Debug.Log("After: " + betters.Count);
                best = betters[r.Next(0, betters.Count - 1)];
                Debug.Log("Score: " + score);
            }
            return best;
        }

        public static T Run<T, U>(T root, U heuristic, System.Func<bool> endCondition, System.Func<T, List<T>> GetNeighbors, System.Func<T, U, float> Evaluate,int? seed = null, bool debug = false)
        {
            int iterations = 0;

            System.Random random = (seed != null)? new System.Random((int)seed) : new System.Random();

            score = prevScore = 0;
            nonSignificantEpochs = 0;

            T best = root;
            while (endCondition?.Invoke() == false) //(!endCondition?.Invoke())
            {
                iterations++;

                prevScore = score;
                score = Evaluate(best, heuristic);  
                List<T> candidates = GetNeighbors(best);
                List<T> betters = new List<T>();
                
                float higherScore = score;
                for (int i = 0; i < candidates.Count; i++)
                {
                    float newScore = Evaluate(candidates[i], heuristic);
                    if (newScore > higherScore)
                    {
                        betters.Clear();
                        betters.Add(candidates[i]);
                    }
                    else if (newScore == higherScore)
                    {
                        betters.Add(candidates[i]);
                    }
                    higherScore = higherScore < newScore ? newScore : higherScore;

                }
                if (higherScore <= score)
                {
                    nonSignificantEpochs++;
                }
                else
                {
                    nonSignificantEpochs = 0;
                }

                if (betters.Count == 0)
                {
                    return best;
                }

                best = betters[random.Next(0, betters.Count - 1)];

                if (best is LBSTileMapData)
                {
                    (best as LBSTileMapData).Print(); // debug temporal quitar luego
                    Debug.Log(higherScore);
                }

                if (debug)
                {
                    var msg = "";
                    msg = "<b> Iteration '" + iterations + "'</b>\n";
                    msg = "candidates: " + candidates.Count;
                    msg = "betters: " + betters.Count;
                    msg = "better score: " + score;
                    Debug.Log(msg);
                }
            }

            return best;
        }

    }
}

