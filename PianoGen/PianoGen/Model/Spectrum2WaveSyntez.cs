using AI;
using AI.Statistics;
using System;
using System.Collections.Generic;

namespace PianoGen.Model
{
    public class Spectrum2WaveSyntez
    {

        public Dictionary<int, double> old = new Dictionary<int, double>();

        /// <summary>
        /// Matrix to sound vector
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="fd"></param>
        /// <param name="semp"></param>
        /// <returns></returns>
        public Vector GetSoundSignalFromSpectrumTimeMatrix(Matrix matrix, int fd, double semp)
        {
            Vector[] spectrumOnetimeStep = Matrix.GetColumns(matrix);

            //for (int i = 1; i < spectrumOnetimeStep.Length; i++)
            //{
            //    spectrumOnetimeStep[i] = 0.3 * spectrumOnetimeStep[i - 1] + 0.7 * spectrumOnetimeStep[i];
            //}

            List<double> concatinate = new List<double>();

            for (int i = 0; i < spectrumOnetimeStep.Length; i++)
            {
                concatinate.AddRange(SpectrumMagnitud2Signal(spectrumOnetimeStep[i], semp / fd, fd).DataInVector);
            }

            Vector resultSignal = concatinate.ToArray();
            resultSignal /= resultSignal.MaxAbs();
            resultSignal += 0.003 * Statistic.rand(resultSignal.Count); // Noise masked
            resultSignal *= 0.98 + 0.02 * Statistic.rand(resultSignal.Count); // Noise decorelation

            return resultSignal;
        }

        /// <summary>
        /// Transform, spectrum 2 amplitudes
        /// </summary>
        public Vector SpectrumMagnitud2Signal(Vector x, double duration, int fd)
        {
            Dictionary<int, double> max = PsychoacousticDecoder(x);
            Vector result = new Vector((int)(duration * fd));
            Vector t = Vector.Time0(fd, duration).CutAndZero((int)(duration * fd));

            foreach (KeyValuePair<int, double> item in max)
            {
                int i = item.Key;
                Vector vec = t.TransformVector(r => item.Value * Sin(r * i / duration));
                result += vec;
            }

            double ofset = t[t.Count - 1];

            foreach (KeyValuePair<int, double> item in old)
            {
                double i = item.Key / duration;
                Vector vec = t.TransformVector(r => item.Value * Sinc(i*(r+ofset)));
                result += vec;
            }

            old = max;

            return result-result.Expend();
        }


        public static double Sin(double x)
        {
            double arg = 2.0 * Math.PI * x;
            return Math.Sin(arg);
        }

        public static double Sinc(double x)
        {
            double arg = 2.0 * Math.PI * x+0.0001;
            return Math.Sin(arg)/ (arg);
        }

        /// <summary>
        /// Psyho-acoustic dcoder
        /// </summary>
        /// <param name="data"></param>
        /// <param name="log"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static Dictionary<int, double> PsychoacousticDecoder(Vector data, double log = 1.02, int start = 3)
        {
            int index = 0;
            double dur = start;
            double[] semles = data;
            List<Vector> vects = new List<Vector>();
            Dictionary<int, double> max = new Dictionary<int, double>();

            while (index + dur < data.Count)
            {
                vects.Add(Vector.GetIntervalDouble(index, (int)(index + dur), semles));
                index = (int)(index + dur + 1);
                dur *= log;
            }

            dur = start;
            index = 0;

            for (int i = 0; i < vects.Count; i++, index = (int)(index + dur + 1), dur *= log)
            {
                int ind = vects[i].IndexMax();
                if (vects[i][ind] > 1e-3)
                {
                    max.Add(ind + index, vects[i][ind]);
                }
            }

            return max;
        }
    }
}
