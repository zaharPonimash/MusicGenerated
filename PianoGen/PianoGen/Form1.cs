using AI;
using AI.ComputerVision;
using AI.DSPCore;
using AI.Statistics;
using PianoGen.Data;
using PianoGen.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PianoGen
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            DatasetGenerate dataset = new DatasetGenerate("data");
            dataset.TSemple = 5; // 5 сек. отрывок
            dataset.Add("Forget Me Not - Patrick Patrikios.mp3", DatasetGenerate.SaveInfo.SaveAsMatrix);

         //   WavMp3 load = new WavMp3();
         //  // var vec = load.Load("2.wav");//.CutAndZero(10 * load.Fd); // Cut 10s
         //   var vec = load.LoadFromMp3("No Starlight Dey Beat - Nana Kwabena.mp3");//.CutAndZero(30 * load.Fd); 
         //   int fr = Functions.NextPow2(load.Fd/40);
         //   double k =2;
         //   double fdNew = load.Fd*k;



         //   var t = Vector.Time0(load.Fd, vec.Count / (double)load.Fd);

         //   chartVisual1.AddPlot(t,vec,"", width: 1, color: Color.Red); // Show plot
         //                                                               // Wav.Play(vec, load.Fd); // Play as sound
         //   heatMapControl1.CalculateHeatMap(FFT.TimeFrTransformHalf(vec,fr).TransformMatrix(x=>Math.Log10(x+1e-3)));

         //   Spectrum2WaveSyntez s = new Spectrum2WaveSyntez();

         //   vec = s.GetSoundSignalFromSpectrumTimeMatrix(FFT.TimeFrTransformHalf(vec, fr), (int)fdNew, fr*k);

         ////   vec = Filters.ExpAv(vec, 0.6);

         //   t = Vector.Time0(fdNew, vec.Count / fdNew);
         //   chartVisual1.AddPlot(t.CutAndZero(vec.Count), vec, "", width: 1, color: Color.Green);
         //   WavMp3.Play(vec, (int)fdNew);// load.Fd);
         //  // Wav.Save(vec, (int)fdNew, "morg.wav");
        }

    }
}
