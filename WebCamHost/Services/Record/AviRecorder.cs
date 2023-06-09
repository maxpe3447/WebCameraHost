using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Formats.Asn1;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpAvi.Output;
using SharpAvi;
using SharpAvi.Codecs;
using WebCamHost.Helpers;
using System.Windows.Media.Imaging;

namespace WebCamHost.Services.Record
{
    public class RecorderParams
    {
        public RecorderParams(string filename, int FrameRate, FourCC Encoder, int Quality, int height, int width)
        {
            FileName = filename;
            FramesPerSecond = FrameRate;
            Codec = Encoder;
            this.Quality = Quality;

            Height = height;
            Width = width;
        }

        string FileName;
        public int FramesPerSecond, Quality;
        FourCC Codec;

        public int Height { get; private set; }
        public int Width { get; private set; }

        public AviWriter CreateAviWriter()
        {
            return new AviWriter(FileName)
            {
                FramesPerSecond = FramesPerSecond,
                EmitIndex1 = true,
            };
        }

        public IAviVideoStream CreateVideoStream(AviWriter writer)
        {
            // Select encoder type based on FOURCC of codec
            if (Codec == KnownFourCCs.Codecs.Uncompressed)
                return writer.AddUncompressedVideoStream(Width, Height);
            else if (Codec == KnownFourCCs.Codecs.MotionJpeg)
                return writer.AddMotionJpegVideoStream(Width, Height, Quality);
            else
            {
                return writer.AddMpeg4VideoStream(Width, Height, (double)writer.FramesPerSecond,
                       quality: Quality,
                       codec: Codec,
                       forceSingleThreadedAccess: true);
            }
        }
    }

    public class AviRecorder : IDisposable
    {
        #region Fields
        AviWriter? writer;
        RecorderParams? Params;
        IAviVideoStream? videoStream;
        Thread? screenThread;
        ManualResetEvent? stopThread = new ManualResetEvent(false);
        public delegate Bitmap GetScreen();
        event GetScreen GetScreenHandler;
        GetScreen? _getScreen;
        #endregion

        public AviRecorder(RecorderParams Params, GetScreen getScreen)
        {
            _getScreen = getScreen;
            GetScreenHandler += getScreen;
            this.Params = Params;
            //_mainWindowViewModel = mainWindowViewModel;
            // Create AVI writer and specify FPS
            writer = Params.CreateAviWriter();

            // Create video stream
            videoStream = Params.CreateVideoStream(writer);
            // Set only name. Other properties were when creating stream, 
            // either explicitly by arguments or implicitly by the encoder used
            videoStream.Name = "Captura";

            screenThread = new Thread(RecordScreen)
            {
                Name = typeof(AviRecorder).Name + ".RecordScreen",
                IsBackground = true
            };

            screenThread.Start();
        }

        public void Dispose()
        {
            try
            {
                stopThread?.Set();
                screenThread?.Join();

                // Close writer: the remaining data is written to a file and file is closed
                writer?.Close();

                stopThread?.Dispose();

                GetScreenHandler -= _getScreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void RecordScreen()
        {
            try
            {
                var frameInterval = TimeSpan.FromSeconds(1 / (double)writer?.FramesPerSecond);
                var buffer = new byte[Params.Width * Params.Height * 4];
                Task videoWriteTask = null;
                var timeTillNextFrame = TimeSpan.Zero;

                while (!stopThread?.WaitOne(timeTillNextFrame) ?? false)
                {
                    lock (Constants.Locker)
                    {
                        var frame = GetScreenHandler?.Invoke();
                        if (frame == null)
                        {
                            continue;
                        }
                        BitmapConverter.BitmapToBytes(frame, buffer);

                        var timestamp = DateTime.Now;

                        // Wait for the previous frame is written
                        videoWriteTask?.Wait();

                        // Start asynchronous (encoding and) writing of the new frame
                        videoWriteTask = videoStream?.WriteFrameAsync(true, buffer, 0, buffer.Length);

                        timeTillNextFrame = timestamp + frameInterval - DateTime.Now;
                        if (timeTillNextFrame < TimeSpan.Zero)
                            timeTillNextFrame = TimeSpan.Zero;
                    }

                    // Wait for the last frame is written
                    videoWriteTask?.Wait();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Record process error");
            }
        }
    }

    //public void Screenshot(byte[] Buffer)
    //{
    //    using (var BMP = new Bitmap(Params.Width, Params.Height))
    //    {
    //        using (var g = Graphics.FromImage(BMP))
    //        {
    //            g.CopyFromScreen(Point.Empty, Point.Empty, new Size(Params.Width, Params.Height), CopyPixelOperation.SourceCopy);

    //            g.Flush();

    //            var bits = BMP.LockBits(new Rectangle(0, 0, Params.Width, Params.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
    //            Marshal.Copy(bits.Scan0, Buffer, 0, Buffer.Length);
    //            BMP.UnlockBits(bits);
    //        }
    //    }
    //}
}

