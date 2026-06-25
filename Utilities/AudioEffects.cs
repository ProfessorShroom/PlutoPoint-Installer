using System;
using System.IO;
using System.Media;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

// Copyright © Charlie Howard 2026 All rights reserved.

namespace PlutoPoint_Installer.Utilities
{
    public static class AudioEffects
    {
        public static void PlayHoverPop()
        {
            TriggerLayer(new[] { 300.0 }, 0, 0.2f, SignalGeneratorType.Sin);
            TriggerLayer(new[] { 400.0 }, 40, 0.2f, SignalGeneratorType.Sin);
        }
        public static void PlayClickChime()
        {
            TriggerLayer(new[] { 392.0 }, 0, 0.3f, SignalGeneratorType.Sin);
            TriggerLayer(new[] { 587.3 }, 40, 0.3f, SignalGeneratorType.Sin);
            TriggerLayer(new[] { 783.9 }, 80, 0.3f, SignalGeneratorType.Sin);
        }
        public static void PlayCompleteChime()
        {
            TriggerLayer(new[] { 261.63, 329.63 }, 0, 2.5f, SignalGeneratorType.Sin);
            TriggerLayer(new[] { 392.00, 523.25 }, 100, 1.8f, SignalGeneratorType.Sin);
            TriggerLayer(new[] { 783.99, 1046.5 }, 250, 1.0f, SignalGeneratorType.Sin);
        }
        public static void PlayCompleteChristmasChime()
        {
            double[] melody = { 659.3, 659.3, 659.3, 659.3, 659.3, 659.3, 659.3, 783.9, 523.3, 587.3, 659.3, 698.5, 698.5, 698.5, 698.5, 698.5, 659.3, 659.3, 659.3, 659.3, 587.3, 587.3, 659.3, 587.3, 783.9 };
            int[] timing = { 0, 250, 500, 1000, 1250, 1500, 2000, 2250, 2500, 2750, 3000, 3500, 3750, 4000, 4250, 4500, 4750, 5000, 5250, 5500, 5750, 6000, 6250, 6500, 7000 };
            for (int i = 0; i < melody.Length; i++)
            {
                double root = melody[i];
                double[] chord = { root, root * 2.0, root * 4.0 };
                TriggerLayer(chord, timing[i], 0.6f, SignalGeneratorType.Sin, 0.12);
                TriggerLayer(new[] { root * 4.0 }, timing[i] + 40, 0.2f, SignalGeneratorType.Triangle, 0.08);
            }
        }
        public static void PlayCompleteHalloweenChime()
        {
            TriggerLayer(new[] { 261.63, 277.18 }, 0, 2.5f, SignalGeneratorType.Sin, 0.08);
            TriggerLayer(new[] { 392.00, 415.30 }, 200, 2.0f, SignalGeneratorType.Sin, 0.08);
            TriggerLayer(new[] { 783.99, 740.00 }, 400, 1.5f, SignalGeneratorType.Sin, 0.08);
            TriggerLayer(new[] { 261.63 * 0.5 }, 1000, 3.5f, SignalGeneratorType.Sin, 0.05);
            TriggerLayer(new[] { 392.00 * 0.5 }, 1300, 3.0f, SignalGeneratorType.Sin, 0.05);
            TriggerLayer(new[] { 130.81, 130.81 * 1.5 }, 1800, 5.0f, SignalGeneratorType.Sin, 0.06);
        }
        public static void PlayCompleteBirthdayChime()
        {
            TriggerLayer(new[] { 392.00 }, 0, 0.4f, SignalGeneratorType.Sin, 0.1);
            TriggerLayer(new[] { 392.00 }, 250, 0.4f, SignalGeneratorType.Sin, 0.1);
            TriggerLayer(new[] { 440.00 }, 500, 0.4f, SignalGeneratorType.Sin, 0.1);
            TriggerLayer(new[] { 392.00 }, 750, 0.4f, SignalGeneratorType.Sin, 0.1);
            TriggerLayer(new[] { 523.25 }, 1000, 0.4f, SignalGeneratorType.Sin, 0.1);
            TriggerLayer(new[] { 493.88 }, 1250, 0.8f, SignalGeneratorType.Sin, 0.15);
            TriggerLayer(new[] { 392.00 }, 1800, 0.4f, SignalGeneratorType.Sin, 0.1);
            TriggerLayer(new[] { 392.00 }, 2050, 0.4f, SignalGeneratorType.Sin, 0.1);
            TriggerLayer(new[] { 440.00 }, 2300, 0.4f, SignalGeneratorType.Sin, 0.1);
            TriggerLayer(new[] { 392.00 }, 2550, 0.4f, SignalGeneratorType.Sin, 0.1);
            TriggerLayer(new[] { 587.33 }, 2800, 0.4f, SignalGeneratorType.Sin, 0.1);
            TriggerLayer(new[] { 523.25 }, 3050, 1.5f, SignalGeneratorType.Sin, 0.15);
        }
        public static void PlayCompleteNewYearsChime()
        {
            TriggerLayer(new[] { 130.81, 65.41 }, 0, 4.0f, SignalGeneratorType.Sin, 0.15);
            double[] sparkle = { 783.99, 1046.50, 1174.66, 1318.51 };
            for (int i = 0; i < sparkle.Length; i++)
            {
                TriggerLayer(new[] { sparkle[i] }, 800 + (i * 100), 1.0f, SignalGeneratorType.Sin, 0.08);
            }
            TriggerLayer(new[] { 523.25, 659.25, 783.99, 1046.50 }, 1500, 3.0f, SignalGeneratorType.Sin, 0.1);
        }
        public static void PlayCompleteValentinesChime()
        {
            TriggerLayer(new[] { 261.63 }, 0, 1.5f, SignalGeneratorType.Sin, 0.05);
            TriggerLayer(new[] { 329.63 }, 80, 1.5f, SignalGeneratorType.Sin, 0.05);
            TriggerLayer(new[] { 392.00 }, 160, 1.5f, SignalGeneratorType.Sin, 0.05);
            TriggerLayer(new[] { 523.25 }, 240, 1.5f, SignalGeneratorType.Sin, 0.05);
            TriggerLayer(new[] { 659.25 }, 400, 1.5f, SignalGeneratorType.Sin, 0.06);
            TriggerLayer(new[] { 523.25 }, 480, 1.5f, SignalGeneratorType.Sin, 0.05);
            TriggerLayer(new[] { 392.00 }, 560, 1.5f, SignalGeneratorType.Sin, 0.05);
            TriggerLayer(new[] { 261.63, 329.63, 392.00, 493.88 }, 700, 4.0f, SignalGeneratorType.Sin, 0.07);
        }
        private static void TriggerLayer(double[] freqs, int delayMs, float decay, SignalGeneratorType type, double gain = 0.05)
        {
            System.Threading.Tasks.Task.Delay(delayMs).ContinueWith(_ =>
            {
                var provider = new ChimeSource(freqs, 44100, decay, type, gain);
                var output = new WaveOutEvent();
                output.Init(provider);
                output.Play();
                System.Threading.Tasks.Task.Delay((int)(decay * 1000) + 500).ContinueWith(t => output.Dispose());
            });
        }
    }
    public class ChimeSource : ISampleProvider
    {
        private readonly SignalGenerator[] _oscillators;
        private readonly int _totalSamples;
        private int _samplesPlayed = 0;
        public ChimeSource(double[] freqs, int sampleRate, double duration, SignalGeneratorType type, double gain)
        {
            _totalSamples = (int)(duration * sampleRate);
            _oscillators = new SignalGenerator[freqs.Length];
            for (int i = 0; i < freqs.Length; i++)
            {
                _oscillators[i] = new SignalGenerator(sampleRate, 1) { Frequency = freqs[i], Type = type, Gain = gain };
            }
        }
        public WaveFormat WaveFormat => WaveFormat.CreateIeeeFloatWaveFormat(44100, 1);
        public int Read(float[] buffer, int offset, int count)
        {
            float[] mix = new float[count];
            int maxRead = 0;
            foreach (var osc in _oscillators)
            {
                float[] b = new float[count];
                int r = osc.Read(b, 0, count);
                maxRead = Math.Max(maxRead, r);
                for (int i = 0; i < r; i++) mix[i] += b[i];
            }
            for (int i = 0; i < maxRead; i++)
            {
                float progress = (float)_samplesPlayed / _totalSamples;
                float envelope = (float)Math.Exp(-progress * 5);
                buffer[offset + i] = (mix[i] / _oscillators.Length) * envelope;
                _samplesPlayed++;
            }
            return (_samplesPlayed >= _totalSamples) ? 0 : maxRead;
        }
    }
}