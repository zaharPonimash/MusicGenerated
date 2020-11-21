using AI;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;



namespace PianoGen.Data
{
    public class WavMp3
    {

        public int Fd { get; set; } = -1; // Semple rate

        /// <summary>
        /// Load file
        /// </summary>
        public Vector Load(string path)
        {
            List<double> semplList = new List<double>();

            using (WaveFileReader reader = new WaveFileReader(path))
            {

                var outFormat = Resampler(reader);// to 8kHz
                var semples = outFormat.ToSampleProvider().ToMono();
                Fd = semples.WaveFormat.SampleRate;

                float[] fl = new float[Setting.BufferSize];
                int m;
                do
                {
                    m = semples.Read(fl, 0, Setting.BufferSize);

                    for (int i = 0; i < Setting.BufferSize; i++)
                        semplList.Add(fl[i]);
                }
                while (m == Setting.BufferSize);
            }

            return semplList.ToArray();
        }



        public Vector LoadFromMp3(string path)
        {
            List<double> semplList = new List<double>();

            using (Mp3FileReader reader = new Mp3FileReader(path))
            {
                var outFormat = Resampler(reader);// to 8kHz
                var semples = outFormat.ToSampleProvider().ToMono();
                Fd = semples.WaveFormat.SampleRate;

                float[] fl = new float[Setting.BufferSize];
                int m;
                do
                {
                    m = semples.Read(fl, 0, Setting.BufferSize);

                    for (int i = 0; i < Setting.BufferSize; i++)
                        semplList.Add(fl[i]);
                }
                while (m == Setting.BufferSize);
            }

            return semplList.ToArray();
        }

        // Play vector
        public static void Play(Vector vector, int fd)
        {
            CastomWavFormat format = new CastomWavFormat(vector, fd);
            WaveOut waveOut = new WaveOut();
            waveOut.Init(format);
            waveOut.Volume = 1;
            waveOut.Play();
        }


        public static void Save(Vector vector, int fd, string path)
        {
            using (WaveFileWriter wr = new WaveFileWriter(path, new WaveFormat(fd, 32, 1)))
            {

                for (int i = 0; i < vector.Count; i++)
                {
                    int datS = (int)(vector[i] * int.MaxValue);
                    byte[] bts2 = BitConverter.GetBytes(datS);
                    wr.Write(bts2, 0, 4);
                }

            }
        }



        MediaFoundationResampler Resampler(IWaveProvider reader, int newFreq = 8000)
        {
            var outFormat = new WaveFormat(newFreq, reader.WaveFormat.Channels);
            var resampler = new MediaFoundationResampler(reader, outFormat);
            return resampler;
        }








        // Castom
        public class CastomWavFormat : IWaveProvider, IDisposable
        {
            public WaveFormat WaveFormat { get; private set; }
            MemoryStream stream;

            public CastomWavFormat(Vector vector, int fd)
            {
                WaveFormat = new WaveFormat(fd, 32, 1);
                var data = new byte[vector.Count * 4];


                for (int i = 0, k = 0; i < vector.Count; i++)
                {
                    int datS = (int)(vector[i] * int.MaxValue);
                    byte[] bts2 = BitConverter.GetBytes(datS);
                    data[k++] = bts2[0];
                    data[k++] = bts2[1];
                    data[k++] = bts2[2];
                    data[k++] = bts2[3];
                }

                stream = new MemoryStream(data);

            }

            public int Read(byte[] buffer, int offset, int count)
            {
                return stream.Read(buffer, offset, count);
            }

            public void Dispose()
            {
                stream.Dispose();
            }


          
        }
    }
}
