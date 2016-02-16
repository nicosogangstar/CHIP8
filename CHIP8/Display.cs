﻿using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace CHIP8
{
    public partial class Display : Form
    {
        //Init chip
        Chip c = new Chip();

        Rectangle[,] pixels = new Rectangle[64, 32];

        public Display()
        {
            InitializeComponent();
            chipWorker.RunWorkerAsync(null);
            //load the program into the chip
            c.loadProgram(@"D:\Downloads\tetris.c8");

            //init for window
            displayGrid.Image = new Bitmap(displayGrid.Width, displayGrid.Height);

            for (int x = 0; x < 64; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    //set the positions for each rect
                    pixels[x, y] = new Rectangle(x * 10, y * 10, x * 10 + 10, y * 10 + 10);
                }
            }
        }

        public void display()
        {
            using (Graphics g = Graphics.FromImage(displayGrid.Image))
            {
                // draw black background
                g.Clear(Color.Black);

                byte[] disp = c.getDisplay();

                int count = 0;

                for (int x = 0; x < 64; x++)
                {
                    for (int y = 0; y < 32; y++)
                    {
                        if (disp[count] == 1)
                        {
                            g.FillRectangle(new SolidBrush(Color.White), pixels[x, y]);
                        }
                        else
                        {
                            g.FillRectangle(new SolidBrush(Color.Black), pixels[x, y]);
                        }
                        count++;
                    }
                }
            }
            displayGrid.Invalidate();
        }

        private void chipWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //infinite loop for the chip to run on
            for (;;)
            {
                c.run();
                if (c.needsRedraw())
                {
                    Debug.WriteLine("REDRAWING");
                    display();
                    c.removeDrawFlag();
                }
                Thread.Sleep(16); //60 hertz
            }
        }

        //keypress event
        private void Display_KeyPress(object sender, KeyPressEventArgs e)
        {
            c.setKey(e.KeyChar);
            //Debug.WriteLine("KEY PRESSED: " + (int)e.KeyChar);
        }
    }
}
