using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
//using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;

namespace Challenge1
{
    public partial class Form1 : Form
    {
        private Mat queryFrame = new Mat();
        private VideoCapture videoCapture;
        private Size formSize;
        private int width, height;

        // github上のopencv/opencvを実行可能ファイルと同じディレクトリでクローン
        // Haar-likeカスケード型検出器の読み込み
        private CascadeClassifier cascade = new CascadeClassifier(
            "./opencv/data/haarcascades/haarcascade_frontalface_default.xml");

        public Form1()
        {
            InitializeComponent();

            // カメラの起動
            videoCapture = new VideoCapture(0);
            if (!videoCapture.IsOpened())
            {
                MessageBox.Show("Camera was not found", "Face Capture", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }

            // ビデオキャプチャするカメラのサイズ
            width = videoCapture.FrameWidth;
            height = videoCapture.FrameHeight;

            // デフォルトのカメラサイズ
            formSize = new Size(pictureBox1.Width, pictureBox1.Height);

            //textBox1.Text = videoCapture.Fps.ToString();
            //timer1.Interval = (int)(1000 / videoCapture.Fps);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // スタート・再開時の処理
            timer1.Start();
            button1.Enabled = false;
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 一時停止時の処理
            timer1.Stop();
            button1.Enabled = true;
            button2.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                // 画像取得
                videoCapture.Retrieve(queryFrame, CameraChannels.OpenNI_BGRImage);

                // 320 x 240へリサイズ
                Cv2.Resize(queryFrame, queryFrame, formSize);

                // キャプチャ画像表示
                pictureBox1.Image = queryFrame.ToBitmap();

                // 顔認識
                pictureBox2.Image = faceDetect(ref queryFrame).ToBitmap();
            }
            catch (OpenCVException)
            {
                // アプリの終了
                timer1.Stop();
                MessageBox.Show("Camera was not found", "Face Capture", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private Mat faceDetect(ref Mat image)
        {
            // 顔と認識した箇所をRect配列に格納
            Rect[] rects = cascade.DetectMultiScale(
                image, 1.1, 3, HaarDetectionType.Zero, 
                new OpenCvSharp.CPlusPlus.Size(30, 30));

            // Rectを描画
            foreach (Rect rect in rects)
            {
                Cv2.Rectangle(image, rect, new Scalar(0, 0, 255), 2);
            }

            // 検出数を表示
            int detects = rects.Length;
            textBox1.Text = (detects > 0 ? detects.ToString() : "No") + " face" + (detects > 1 ? "s" : "") + " detected";

            return image;
        }
    }
}
