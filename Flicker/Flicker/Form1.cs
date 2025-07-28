using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Flicker
{
    public partial class Form1 : Form
    {
        Bitmap? backgroundBitmap;
        List<FlickerPixel> flickerPixels = new();
        Random rand = new();
        bool isRunning = false;
        bool isPaused = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize form when it loads
            btnPauseResume.Visible = false;
            btnStart.Visible = true;
            btnBack.Visible = false;
            
            // Set default flicker rate (100ms is a good starting point)
            txtFlickerRate.Text = "100";
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtWord.Text))
                {
                    MessageBox.Show("Please enter a word to display.");
                    return;
                }

                if (!int.TryParse(txtFlickerRate.Text, out int baseRate) || baseRate < 10 || baseRate > 1000)
                {
                    MessageBox.Show("Please enter a valid flicker rate between 10 and 1000 milliseconds.");
                    return;
                }

                // Stop any existing flickering
                isRunning = false;
                System.Threading.Thread.Sleep(100); // Give time for tasks to stop

                GenerateBackground();
                GenerateText(txtWord.Text.Trim(), baseRate);
                pictureBoxDisplay.Image = backgroundBitmap;

                txtWord.Visible = txtFlickerRate.Visible = btnStart.Visible = false;
                btnPauseResume.Visible = btnBack.Visible = true;
                isRunning = true;
                StartFlickering();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting flicker: {ex.Message}\n\n{ex.StackTrace}");
            }
        }

        private void GenerateBackground()
        {
            backgroundBitmap = new Bitmap(pictureBoxDisplay.Width, pictureBoxDisplay.Height);
            // Create a random black and white pattern for the background
            for (int x = 0; x < backgroundBitmap.Width; x++)
            {
                for (int y = 0; y < backgroundBitmap.Height; y++)
                {
                    // Random black or white pixels
                    backgroundBitmap.SetPixel(x, y, rand.Next(2) == 0 ? Color.Black : Color.White);
                }
            }
        }

        private void GenerateText(string text, int baseRate)
        {
            flickerPixels.Clear();
            
            // Create a temporary bitmap to detect text pixels
            using Bitmap wordBmp = new Bitmap(pictureBoxDisplay.Width, pictureBoxDisplay.Height);
            using Graphics g = Graphics.FromImage(wordBmp);
            g.Clear(Color.Transparent);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            using Font font = new("Arial", 72, FontStyle.Bold);
            g.DrawString(text, font, Brushes.Black, new PointF(20, pictureBoxDisplay.Height / 2 - 60));

            // Collect all text pixels first
            List<Point> textPixels = new List<Point>();
            for (int x = 0; x < wordBmp.Width; x++)
            {
                for (int y = 0; y < wordBmp.Height; y++)
                {
                    Color px = wordBmp.GetPixel(x, y);
                    if (px.ToArgb() != Color.Transparent.ToArgb() && px.ToArgb() != Color.White.ToArgb())
                    {
                        textPixels.Add(new Point(x, y));
                    }
                }
            }

            // Create 4x4 pixel blocks instead of individual pixels
            const int blockSize = 4;
            Dictionary<Point, bool> textBlocks = new Dictionary<Point, bool>();
            
            // Group pixels into 32x32 blocks
            foreach (Point pixel in textPixels)
            {
                Point block = new Point(pixel.X / blockSize, pixel.Y / blockSize);
                textBlocks[block] = true;
            }

            // Create rate variations that are hard to detect at 30fps
            List<int> rateVariations = new List<int>();
            rateVariations.Add(baseRate);
            rateVariations.Add(baseRate + 17);
            rateVariations.Add(baseRate + 23);
            rateVariations.Add(baseRate + 29);
            rateVariations.Add(baseRate + 37);
            rateVariations.Add(baseRate + 41);
            rateVariations.Add(baseRate + 47);
            rateVariations.Add(baseRate + 53);

            // Create flicker blocks
            foreach (Point block in textBlocks.Keys)
            {
                int flickerRate = rateVariations[rand.Next(rateVariations.Count)];
                flickerRate = Math.Max(flickerRate, 10); // Ensure minimum rate
                
                flickerPixels.Add(new FlickerPixel { 
                    X = block.X * blockSize, 
                    Y = block.Y * blockSize, 
                    Interval = flickerRate, 
                    IsTextPixel = true,
                    BlockSize = blockSize 
                });
            }
            
            // Debug: Show how many blocks were created
            MessageBox.Show($"Found {textPixels.Count} text pixels, created {flickerPixels.Count} 4x4 blocks for flickering.");
        }

        private async void StartFlickering()
        {
            try
            {
                // Create a working bitmap for modifications and a display bitmap for the UI
                workingBitmap = (Bitmap)backgroundBitmap!.Clone();
                displayBitmap = (Bitmap)backgroundBitmap.Clone();
                
                // Overlay the word on both bitmaps
                using (Graphics g = Graphics.FromImage(workingBitmap))
                {
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                    using Font font = new("Arial", 72, FontStyle.Bold);
                    g.DrawString(txtWord.Text.Trim(), font, Brushes.Black, new PointF(20, pictureBoxDisplay.Height / 2 - 60));
                }
                
                using (Graphics g = Graphics.FromImage(displayBitmap))
                {
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                    using Font font = new("Arial", 72, FontStyle.Bold);
                    g.DrawString(txtWord.Text.Trim(), font, Brushes.Black, new PointF(20, pictureBoxDisplay.Height / 2 - 60));
                }
                
                pictureBoxDisplay.Image = displayBitmap;
                
                // Use a timer-based approach
                var flickerTimer = new System.Threading.Timer(FlickerCallback, null, 0, 16); // 60fps
                
                // Keep running until stopped
                while (isRunning)
                {
                    await Task.Delay(100);
                }
                
                // Clean up
                flickerTimer.Dispose();
                workingBitmap?.Dispose();
                displayBitmap?.Dispose();
                workingBitmap = null;
                displayBitmap = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in StartFlickering: {ex.Message}\n\n{ex.StackTrace}");
            }
        }
        
        private Bitmap? workingBitmap = null;
        private Bitmap? displayBitmap = null;
        private Dictionary<FlickerPixel, int> flickerStates = new Dictionary<FlickerPixel, int>();
        
        private void FlickerCallback(object? state)
        {
            try
            {
                if (!isRunning || isPaused || workingBitmap == null || displayBitmap == null) return;
                
                // Get current time for timing calculations
                long currentTime = Environment.TickCount64;
                
                // Only modify text blocks in the working bitmap based on their individual timing
                foreach (var px in flickerPixels)
                {
                    if (px.IsTextPixel)
                    {
                        // Check if it's time for this block to flicker based on its interval
                        if (!flickerStates.ContainsKey(px))
                        {
                            flickerStates[px] = 0;
                        }
                        
                        // Calculate if this block should flicker now
                        long timeSinceLastFlicker = currentTime - flickerStates[px];
                        if (timeSinceLastFlicker >= px.Interval)
                        {
                            // Toggle the flicker state for this block
                            flickerStates[px] = (int)currentTime;
                            
                            // Get the current color of this block and flip it
                            Color currentColor = workingBitmap.GetPixel(px.X, px.Y);
                            Color flickerColor = (currentColor == Color.Black) ? Color.White : Color.Black;
                            
                            // Fill the entire 4x4 block with the flicker color
                            for (int bx = 0; bx < px.BlockSize; bx++)
                            {
                                for (int by = 0; by < px.BlockSize; by++)
                                {
                                    int pixelX = px.X + bx;
                                    int pixelY = px.Y + by;
                                    
                                    // Make sure we don't go outside the bitmap bounds
                                    if (pixelX < workingBitmap.Width && pixelY < workingBitmap.Height)
                                    {
                                        workingBitmap.SetPixel(pixelX, pixelY, flickerColor);
                                    }
                                }
                            }
                        }
                    }
                }
                
                // Copy only the modified text blocks to display bitmap on UI thread
                if (pictureBoxDisplay.InvokeRequired)
                {
                    pictureBoxDisplay.BeginInvoke(() => {
                        try
                        {
                            // Copy only the text blocks that were modified
                            foreach (var px in flickerPixels)
                            {
                                if (px.IsTextPixel)
                                {
                                    // Copy the 4x4 block from working to display bitmap
                                    for (int bx = 0; bx < px.BlockSize; bx++)
                                    {
                                        for (int by = 0; by < px.BlockSize; by++)
                                        {
                                            int pixelX = px.X + bx;
                                            int pixelY = px.Y + by;
                                            
                                            if (pixelX < workingBitmap.Width && pixelY < workingBitmap.Height &&
                                                pixelX < displayBitmap.Width && pixelY < displayBitmap.Height)
                                            {
                                                displayBitmap.SetPixel(pixelX, pixelY, workingBitmap.GetPixel(pixelX, pixelY));
                                            }
                                        }
                                    }
                                }
                            }
                            pictureBoxDisplay.Invalidate();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"UI update error: {ex.Message}");
                        }
                    });
                }
                else
                {
                    // Copy only the text blocks that were modified
                    foreach (var px in flickerPixels)
                    {
                        if (px.IsTextPixel)
                        {
                            // Copy the 4x4 block from working to display bitmap
                            for (int bx = 0; bx < px.BlockSize; bx++)
                            {
                                for (int by = 0; by < px.BlockSize; by++)
                                {
                                    int pixelX = px.X + bx;
                                    int pixelY = px.Y + by;
                                    
                                    if (pixelX < workingBitmap.Width && pixelY < workingBitmap.Height &&
                                        pixelX < displayBitmap.Width && pixelY < displayBitmap.Height)
                                    {
                                        displayBitmap.SetPixel(pixelX, pixelY, workingBitmap.GetPixel(pixelX, pixelY));
                                    }
                                }
                            }
                        }
                    }
                    pictureBoxDisplay.Invalidate();
                }
            }
            catch (Exception ex)
            {
                // Log error but don't crash
                System.Diagnostics.Debug.WriteLine($"FlickerCallback error: {ex.Message}");
            }
        }



        private void btnPauseResume_Click(object sender, EventArgs e)
        {
            isPaused = !isPaused;
            btnPauseResume.Text = isPaused ? "Resume" : "Pause";
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            // Stop flickering
            isRunning = false;
            System.Threading.Thread.Sleep(100); // Give time for tasks to stop
            
            // Reset UI
            btnPauseResume.Visible = btnBack.Visible = false;
            txtWord.Visible = txtFlickerRate.Visible = btnStart.Visible = true;
            pictureBoxDisplay.Image = null;
            
            // Clear resources
            backgroundBitmap?.Dispose();
            backgroundBitmap = null;
            flickerPixels.Clear();
        }

        private class FlickerPixel
        {
            public int X, Y;
            public int Interval;
            public bool IsTextPixel; // Track if this is a text pixel
            public int BlockSize; // Size of the block (32x32)
        }
    }
}
