using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacialExpressionRecognition.Models
{
    public class ExpressionResult
    {
        public List<float> Score = new List<float>();


        public  Dictionary<string, float> dict = new Dictionary<string, float>();
        public static void GetExpressionName(ExpressionResult ex)
        {
            
            string[] FacialExpression = { "Gniew", "Smutek", "Uśmiech", "Zaskoczenie", "Zdegustowanie" };
            for(int i=0; i < ex.Score.Count; i++)
            {
                float value = float.Parse( string.Format("{0:F2}", ex.Score[i] * 100));
                ex.dict.Add(FacialExpression[i], value);
            }

            ex.dict = ex.dict.OrderBy(x => -x.Value).ToDictionary(x => x.Key, x => x.Value);
        }


    }
}
