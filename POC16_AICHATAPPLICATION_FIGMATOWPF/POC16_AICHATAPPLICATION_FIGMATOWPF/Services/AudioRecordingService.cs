using NAudio.Wave;
using System;
using System.IO;
using System.Threading.Tasks;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Services
{
    public class AudioRecordingService : IDisposable
    {
        private WaveInEvent _waveIn;
        private MemoryStream _audioStream;
        private WaveFileWriter _waveWriter;

        private TaskCompletionSource<bool> _recordingStoppedTcs;

        public bool IsRecording { get; private set; }

        public void StartRecording()
        {
            if (IsRecording)
                return;

            _audioStream = new MemoryStream();

            _waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(
                    16000,
                    16,
                    1)
            };

            _waveWriter = new WaveFileWriter(
                _audioStream,
                _waveIn.WaveFormat);

            _waveIn.DataAvailable += OnDataAvailable;
            _waveIn.RecordingStopped += OnRecordingStopped;

            _recordingStoppedTcs =
                new TaskCompletionSource<bool>(
                    TaskCreationOptions.RunContinuationsAsynchronously);

            _waveIn.StartRecording();

            IsRecording = true;
        }

        public async Task<byte[]> StopRecordingAsync()
        {
            if (!IsRecording || _waveIn == null)
                return null;

            try
            {
                _waveIn.StopRecording();

                // IMPORTANT:
                // Wait until NAudio completely stops recording.
                if (_recordingStoppedTcs != null)
                {
                    await _recordingStoppedTcs.Task;
                }

                IsRecording = false;

                if (_waveWriter != null)
                {
                    _waveWriter.Flush();
                    _waveWriter.Dispose();
                    _waveWriter = null;
                }

                byte[] audioData =
                    _audioStream?.ToArray();

                _audioStream?.Dispose();
                _audioStream = null;

                return audioData;
            }
            catch
            {
                IsRecording = false;

                Cleanup();

                throw;
            }
        }

        private void OnDataAvailable(
            object sender,
            WaveInEventArgs e)
        {
            if (_waveWriter == null)
                return;

            _waveWriter.Write(
                e.Buffer,
                0,
                e.BytesRecorded);

            _waveWriter.Flush();
        }

        private void OnRecordingStopped(
            object sender,
            StoppedEventArgs e)
        {
            _recordingStoppedTcs?.TrySetResult(true);
        }

        private void Cleanup()
        {
            try
            {
                _waveWriter?.Dispose();
                _waveIn?.Dispose();
                _audioStream?.Dispose();
            }
            catch
            {
            }

            _waveWriter = null;
            _waveIn = null;
            _audioStream = null;

            IsRecording = false;
        }

        public void Dispose()
        {
            try
            {
                if (IsRecording &&
                    _waveIn != null)
                {
                    _waveIn.StopRecording();
                }
            }
            catch
            {
            }

            Cleanup();
        }
    }
}