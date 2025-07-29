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
        private Bitmap? displayBitmap;
        private List<FlickerBlock> flickerBlocks = new();
        private Random rand = new();
        private bool isRunning = false;
        private bool isPaused = false;
        private int monitorRate = 60; // Default monitor refresh rate
        private readonly object bitmapLock = new object();
        private int flickerCount = 0; // Counter for flickering
        private const int BLOCK_SIZE = 3; // 3x3 pixel blocks

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize form when it loads
            btnPauseResume.Visible = false;
            btnCancel.Visible = false;
            btnStart.Visible = true;
            
            // Set default monitor rate
            txtMonitorRate.Text = "60";
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

                if (!int.TryParse(txtMonitorRate.Text, out monitorRate) || monitorRate < 30 || monitorRate > 240)
                {
                    MessageBox.Show("Please enter a valid monitor rate between 30 and 240 Hz.");
                    return;
                }

                // Stop any existing flickering
                StopFlickering();

                // Create display bitmap with 3x3 blocks
                lock (bitmapLock)
                {
                    displayBitmap = new Bitmap(pictureBoxDisplay.Width, pictureBoxDisplay.Height);
                    
                    // Step 1: Fill entire bitmap with random 3x3 blocks of black or white
                    FillBitmapWithRandomBlocks();
                    
                    // Step 2: Detect word block positions first
                    DetectWordBlockPositions(txtWord.Text.Trim());
                    
                    // Step 3: Fill word blocks with random black or white
                    FillWordBlocksWithRandomColors();
                    
                    // Step 4: Initialize flicker timing
                    foreach (var block in flickerBlocks)
                    {
                        if (block.IsTextBlock)
                        {
                            // Initialize the last flicker time with a random offset
                            block.LastFlicker = Environment.TickCount64 + rand.Next(0, block.Interval);
                        }
                    }
                }
                
                pictureBoxDisplay.Image = displayBitmap;

                txtWord.Visible = txtMonitorRate.Visible = btnStart.Visible = false;
                btnPauseResume.Visible = btnCancel.Visible = true;
                isRunning = true;
                StartFlickering();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting flicker: {ex.Message}");
            }
        }

        private void FillBitmapWithRandomBlocks()
        {
            // Calculate how many 3x3 blocks fit in the bitmap
            int blocksX = displayBitmap!.Width / BLOCK_SIZE;
            int blocksY = displayBitmap.Height / BLOCK_SIZE;
            
            for (int blockX = 0; blockX < blocksX; blockX++)
            {
                for (int blockY = 0; blockY < blocksY; blockY++)
                {
                    // Random black or white for each 3x3 block
                    Color randomColor = rand.Next(2) == 0 ? Color.White : Color.Black;
                    FillBlock(blockX * BLOCK_SIZE, blockY * BLOCK_SIZE, randomColor);
                }
            }
        }

        private void FillBlock(int startX, int startY, Color color)
        {
            // Fill a 3x3 block with the specified color
            for (int x = 0; x < BLOCK_SIZE; x++)
            {
                for (int y = 0; y < BLOCK_SIZE; y++)
                {
                    int pixelX = startX + x;
                    int pixelY = startY + y;
                    
                    if (pixelX < displayBitmap!.Width && pixelY < displayBitmap.Height)
                    {
                        displayBitmap.SetPixel(pixelX, pixelY, color);
                    }
                }
            }
        }

        private void DetectWordBlockPositions(string text)
        {
            flickerBlocks.Clear();
            
            // Create a temporary bitmap to detect where the word would be
            using (Bitmap tempBitmap = new Bitmap(displayBitmap!.Width, displayBitmap.Height))
            using (Graphics g = Graphics.FromImage(tempBitmap))
            {
                g.Clear(Color.Black); // Black background
                
                // Draw the word in white on the temporary bitmap
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                using Font font = new("Arial", 72, FontStyle.Bold);
                g.DrawString(text, font, Brushes.White, new PointF(20, tempBitmap.Height / 2 - 60));
                
                // Detect ALL white pixels (the word we drew)
                List<Point> wordPixels = new List<Point>();
                for (int x = 0; x < tempBitmap.Width; x++)
                {
                    for (int y = 0; y < tempBitmap.Height; y++)
                    {
                        Color px = tempBitmap.GetPixel(x, y);
                        // Look for white pixels (the word we drew) - more lenient detection
                        if (px.R > 50 || px.G > 50 || px.B > 50)
                        {
                            wordPixels.Add(new Point(x, y));
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Found {wordPixels.Count} word pixels");

                // Group pixels into 3x3 blocks - ensure we get ALL blocks that contain word pixels
                Dictionary<Point, bool> wordBlocks = new Dictionary<Point, bool>();
                
                foreach (Point pixel in wordPixels)
                {
                    Point block = new Point(pixel.X / BLOCK_SIZE, pixel.Y / BLOCK_SIZE);
                    wordBlocks[block] = true;
                }

                System.Diagnostics.Debug.WriteLine($"Created {wordBlocks.Count} word blocks");

                // Create flicker blocks for ALL word blocks
                foreach (Point block in wordBlocks.Keys)
                {
                    flickerBlocks.Add(new FlickerBlock
                    {
                        X = block.X * BLOCK_SIZE,
                        Y = block.Y * BLOCK_SIZE,
                        BlockSize = BLOCK_SIZE,
                        IsTextBlock = true,
                        Interval = rand.Next(50, 250) // Random interval for each block (2x faster)
                    });
                }
                
                // Debug: Show the range of blocks detected
                if (wordBlocks.Count > 0)
                {
                    var minX = wordBlocks.Keys.Min(p => p.X);
                    var maxX = wordBlocks.Keys.Max(p => p.X);
                    var minY = wordBlocks.Keys.Min(p => p.Y);
                    var maxY = wordBlocks.Keys.Max(p => p.Y);
                    System.Diagnostics.Debug.WriteLine($"Word block range: X({minX}-{maxX}), Y({minY}-{maxY})");
                }
                
                MessageBox.Show($"Found {wordPixels.Count} word pixels, created {flickerBlocks.Count} {BLOCK_SIZE}x{BLOCK_SIZE} blocks.\nTimer interval: {1000 / monitorRate}ms\nText: '{text}'");
            }
        }

        private void FillWordBlocksWithRandomColors()
        {
            // Fill each word block with random black or white
            foreach (var block in flickerBlocks)
            {
                if (block.IsTextBlock)
                {
                    // Random color for each text block
                    Color randomColor = rand.Next(2) == 0 ? Color.White : Color.Black;
                    FillBlock(block.X, block.Y, randomColor);
                    
                    // Debug: Log the initial color assignment
                    System.Diagnostics.Debug.WriteLine($"Word block at ({block.X}, {block.Y}) initialized with {(randomColor == Color.White ? "White" : "Black")}");
                }
            }
            
            System.Diagnostics.Debug.WriteLine($"Initialized {flickerBlocks.Count} word blocks with random colors");
        }

        private void StartFlickering()
        {
            try
            {
                // Use the designer-created timer1
                int timerInterval = 50; // Use 50ms for more responsive flickering
                timer1.Interval = timerInterval;
                timer1.Start();
                
                System.Diagnostics.Debug.WriteLine($"StartFlickering: Timer started with interval {timerInterval}ms, monitorRate: {monitorRate}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in StartFlickering: {ex.Message}");
            }
        }

        private void StopFlickering()
        {
            isRunning = false;
            timer1.Stop();
            
            // Clean up bitmaps
            lock (bitmapLock)
            {
                displayBitmap?.Dispose();
                displayBitmap = null;
            }
            

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"timer1_Tick called - isRunning: {isRunning}, isPaused: {isPaused}");
                
                if (!isRunning || isPaused) return;
                
                lock (bitmapLock)
                {
                    if (displayBitmap == null) 
                    {
                        System.Diagnostics.Debug.WriteLine("timer1_Tick: Bitmap is null");
                        return;
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"timer1_Tick: Processing {flickerBlocks.Count} blocks");
                    
                    // Use a counter to alternate colors
                    flickerCount++;
                    
                    // Only flicker the text blocks, leave the background blocks alone
                    long currentTime = Environment.TickCount64;
                    int blocksUpdated = 0;
                    
                    foreach (var block in flickerBlocks)
                    {
                        if (block.IsTextBlock)
                        {
                            // Check if it's time for this block to flicker
                            long timeSinceLastFlicker = currentTime - block.LastFlicker;
                            if (timeSinceLastFlicker >= block.Interval)
                            {
                                // Use global flicker state to determine color
                                Color newColor = rand.Next(2) == 0 ? Color.White : Color.Black;
                                
                                System.Diagnostics.Debug.WriteLine($"Block at ({block.X}, {block.Y}) flickering to {(newColor == Color.White ? "White" : "Black")}, timeSinceLast: {timeSinceLastFlicker}ms, interval: {block.Interval}ms");
                                
                                // Fill the entire 3x3 block with the new color
                                FillBlock(block.X, block.Y, newColor);
                                
                                // Update last flicker time
                                block.LastFlicker = currentTime;
                                blocksUpdated++;
                            }
                            else
                            {
                                // Debug: Show blocks that aren't ready to flicker yet
                                if (flickerCount % 10 == 0) // Only log every 10th tick to avoid spam
                                {
                                    System.Diagnostics.Debug.WriteLine($"Block at ({block.X}, {block.Y}) not ready: timeSinceLast={timeSinceLastFlicker}ms, interval={block.Interval}ms");
                                }
                            }
                        }
                    }
                    
                    // Debug output
                    System.Diagnostics.Debug.WriteLine($"timer1_Tick: Updated {blocksUpdated} blocks at count {flickerCount}");
                }
                
                // Update the display
                pictureBoxDisplay.Refresh(); // Use Refresh instead of Invalidate for immediate update
                System.Diagnostics.Debug.WriteLine("timer1_Tick: Refreshed pictureBox");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"timer1_Tick error: {ex.Message}");
            }
        }

        private void btnPauseResume_Click(object sender, EventArgs e)
        {
            isPaused = !isPaused;
            btnPauseResume.Text = isPaused ? "Resume" : "Pause";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Stop flickering
            StopFlickering();
            
            // Reset UI
            btnPauseResume.Visible = btnCancel.Visible = false;
            txtWord.Visible = txtMonitorRate.Visible = btnStart.Visible = true;
            pictureBoxDisplay.Image = null;
            
            // Clear resources
            lock (bitmapLock)
            {
                displayBitmap?.Dispose();
                displayBitmap = null;
            }
            flickerBlocks.Clear();
        }

        private class FlickerBlock
        {
            public int X, Y;
            public int BlockSize;
            public bool IsTextBlock; // True if this block represents text
            public int Interval; // Flicker interval in milliseconds
            public long LastFlicker = 0; // Last flicker time
        }
    }
}
