using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;
using MathNet.Numerics.Statistics;
using SmartPlayer.Core.DTOs;

namespace SmartPlayer.Core.SongAnalyzer
{
    public class Analyzer
    {
        private static bool _initialized = false;

        public static List<double> GetCorreleationCoefficientsFor(string filename, List<AnalyzableSong> allCurrentSongs)
        {
            if(!_initialized)
            {
                _initialized = Bass.BASS_Init(0, 1000, BASSInit.BASS_DEVICE_NOSPEAKER, IntPtr.Zero);
                if (!_initialized && Bass.BASS_ErrorGetCode().ToString() != "BASS_ERROR_ALREADY")
                    throw new Exception(Bass.BASS_ErrorGetCode().ToString());
            }

            float[] mainPcm = ReadMonoFromFile(filename, 1000, 30000, 30000);
            string mainMarkovChain = ConvertToMarkovChain(mainPcm);
            var currentSongMarkovChains = new List<string>();
            foreach(var song in allCurrentSongs)
            {
                var currentSongPcm = ReadMonoFromFile(song.PhysicalFileName, 1000, 30000, 30000);
                currentSongMarkovChains.Add(ConvertToMarkovChain(currentSongPcm));
                // convert pcms to markov chains
            }

            var correlationCoefficients = new List<double>();
            foreach (var otherChain in currentSongMarkovChains)
            {
                var comparisonResults = LongestCommonSubstring(mainMarkovChain, otherChain);
                correlationCoefficients.Add(comparisonResults);
            }
            return correlationCoefficients;
        }

        public static string ConvertToMarkovChain(float[] pcm)
        {
            StringBuilder builder = new StringBuilder(pcm.Length / 2 - 1);
            char value = '=';
            for(int i = 2; i < pcm.Length; i+=2)
            {
                value = '=';
                if(pcm[i] != pcm[i - 1])
                {
                    value = pcm[i] > pcm[i - 1] ? '>' : '<';
                }
                builder.Append(value);
            }
            return builder.ToString();
        }

        public static int LongestCommonSubstring(string str1, string str2)
        {
            int[][] num = new int[str1.Length][];
            for(int i = 0; i < num.Length; i++)
            {
                num[i] = new int[str2.Length];
            }
            int maxlen = 0;
            List<string> result = new List<string>();
            StringBuilder sequenceBuilder = new StringBuilder();

            for (int i = 0; i < str1.Length; i++)
            {
                for (int j = 0; j < str2.Length; j++)
                {
                    if (str1[i] != str2[j])
                        num[i][j] = 0;
                    else
                    {
                        if ((i == 0) || (j == 0))
                            num[i][j] = 1;
                        else
                            num[i][j] = 1 + num[i - 1][j - 1];

                        if (num[i][j] > maxlen)
                        {
                            maxlen = num[i][j];
                        }
                    }
                }
            }

            var matchCount = num.Aggregate(0, (x, y) => x + y.Count(z => z > 13));

            return matchCount;
        }

        /// <summary>
        /// This method was copied from a project for 
        /// </summary>
        /// <param name="filename">An absolute path to the file</param>
        /// <param name="samplerate">Samplerate (E.G.: 41000) in Hz</param>
        /// <param name="milliseconds">The wavelength in miliseconds (0 for whole song)</param>
        /// <param name="startmillisecond">The first milisecond of the wavelength</param>
        /// <returns></returns>
        private static float[] ReadMonoFromFile(string filename, int samplerate = 44100, int milliseconds = 0, int startmillisecond = 0)
        {
            int totalmilliseconds = milliseconds <= 0 ? Int32.MaxValue : milliseconds + startmillisecond;
            float[] data = null;
            //create streams for re-sampling
            int stream = Bass.BASS_StreamCreateFile(filename, 0, 0,
                BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_MONO |
                BASSFlag.BASS_SAMPLE_FLOAT); //Decode the stream
            if (stream == 0)
                throw new Exception(Bass.BASS_ErrorGetCode().ToString());
            int mixerStream = BassMix.BASS_Mixer_StreamCreate(samplerate, 2, BASSFlag.BASS_SAMPLE_FLOAT |
                BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_MONO);
            if (mixerStream == 0)
                throw new Exception(Bass.BASS_ErrorGetCode().ToString());
            if (BassMix.BASS_Mixer_StreamAddChannel(mixerStream, stream, BASSFlag.BASS_MIXER_DOWNMIX))
            {
                int bufferSize = samplerate * 10 * 4; /*read 10 seconds at each iteration*/
                float[] buffer = new float[bufferSize];
                List<float[]> chunks = new List<float[]>();
                int size = 0;
                while ((float)(size) / samplerate * 1000 < totalmilliseconds)
                {
                    //get re-sampled/mono data
                    int bytesRead = Bass.BASS_ChannelGetData(mixerStream, buffer, bufferSize);
                    if (bytesRead == 0)
                        break;
                    float[] chunk = new float[bytesRead / 4]; //each float contains 4 bytes
                    Array.Copy(buffer, chunk, bytesRead / 4);
                    chunks.Add(chunk);
                    size += bytesRead / 4; //size of the data
                }

                if ((float)(size) / samplerate * 1000 < (milliseconds + startmillisecond))
                    return null; /*not enough samples to return the requested data*/
                int start = (int)((float)startmillisecond * samplerate / 1000);
                int end = (milliseconds <= 0) ? size :
                   (int)((float)(startmillisecond + milliseconds) * samplerate / 1000);
                data = new float[size];
                int index = 0;
                /*Concatenate*/
                foreach (float[] chunk in chunks)
                {
                    Array.Copy(chunk, 0, data, index, chunk.Length);
                    index += chunk.Length;
                }
                /*Select specific part of the song*/
                if (start != 0 || end != size)
                {
                    float[] temp = new float[end - start];
                    Array.Copy(data, start, temp, 0, end - start);
                    data = temp;
                }
            }
            else
                throw new Exception(Bass.BASS_ErrorGetCode().ToString());
            return data;
        }
    }
}
