using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using AI;
using System.Drawing;
using AI.ComputerVision;

namespace PianoGen.Data
{
    public class DatasetGenerate
    {
        string outDir;
        const int fr =  256;
        long time;


        /// <summary>
        /// Time segment size(s)
        /// </summary>
        public int TSemple { get; set; } = 5;


        /// <summary>
        /// Dataset generator
        /// </summary>
        /// <param name="outpDirectory">Output folder</param>
        public DatasetGenerate(string outpDirectory) 
        {
            outDir = outpDirectory;

            if (!Directory.Exists(outDir)) 
            {
                Directory.CreateDirectory(outDir);
            }
        }

        /// <summary>
        /// Add music file to dataset
        /// </summary>
        /// <param name="pathToFile">path to music file (only mp3 or wav)</param>
        public void Add(string pathToFile, SaveInfo saveInfo = SaveInfo.SaveAsMatrix) 
        {
            FileInfo fileInfo = new FileInfo(pathToFile);
            Vector sound;
            WavMp3 loader = new WavMp3();
            int len;


            if (!fileInfo.Exists)
                throw new Exception("The specified file does not exist!");

            

            if (fileInfo.Extension == ".mp3")
            {
                sound = loader.LoadFromMp3(pathToFile);
            }
            else if (fileInfo.Extension == ".wav")
            {
                sound = loader.Load(pathToFile);
            }
            else
                throw new Exception("unknow format");


            time = DateTime.Now.Millisecond;
            len = TSemple * loader.Fd;
            Vector[] semples = Vector.GetWindows(sound, len, len); // Получение непересекающихся отрезков


            for (int i = 0; i < semples.Length; i++)
            {
                Matrix spectrogram = FFT.TimeFrTransformHalf(sound, fr);
                spectrogram = spectrogram.TransformMatrix(x => Math.Log(x + Setting.Eps));// Логарифм

                if (saveInfo == SaveInfo.SaveAsMatrix)
                {
                    MatrixSaver.Save($"{outDir}\\{NameGen(i)}.matr", spectrogram);
                }
                else 
                {
                    double min = spectrogram.Min();
                    double max = spectrogram.Max();
                    Bitmap bitmap = ImgConverter.ToBitmap((spectrogram-min)/(max-min));
                    bitmap.Save($"{outDir}\\{NameGen(i)}.jpg");
                }

            }
        }

        string NameGen(int i) 
        {
            return $"{i}_{time}";
        }


        public enum SaveInfo 
        {
            SaveAsImg,
            SaveAsMatrix
        }
    }
}
