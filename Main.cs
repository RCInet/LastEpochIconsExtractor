﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace IconsExtractor
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }
        private void Main_Shown(object sender, EventArgs e)
        {
            textBox_Css.Text = "";
            Icons_Count_In_Css.Text = "";
            textBox_Picture.Text = "";
            textBox_Output_Path.Text = System.IO.Directory.GetCurrentDirectory() + @"\Output";
            ShowHideExtractBtn();
        }
        private int StringToNumber(string s)
        {
            int result = 0;
            int bad_char = 0;
            if (s.Contains("px}")) { bad_char = 3; }
            else if (s.Contains("px")) { bad_char = 2; }
            try { result = Convert.ToInt32(s.Substring(0, s.Length - bad_char)); }
            catch { }

            return result;
        }
        private void LoadCss()
        {
            textBox_Css.Text = "";
            Icons_Count_In_Css.Text = "";
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = System.IO.Directory.GetCurrentDirectory(),
                Title = "CSS",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "txt",
                Filter = "css files (*.css)|*.css",
                RestoreDirectory = true,
                ReadOnlyChecked = true,
                ShowReadOnly = true
            };
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox_Css.Text = openFileDialog1.FileName;
                string[] css_text_array = File.ReadAllText(openFileDialog1.FileName).Split('.');
                Icons.List = new List<Icons.Icon_Structure>();
                foreach (string str in css_text_array)
                {
                    string name = str.Split('{')[0];
                    if (name.Contains("itemdb-"))
                    {
                        try
                        {
                            Icons.List.Add(new Icons.Icon_Structure
                            {
                                name = name,
                                background_position_x = StringToNumber(str.Split('{')[1].Split(';')[0].Split(':')[1].Split(' ')[0]),
                                background_position_y = StringToNumber(str.Split('{')[1].Split(';')[0].Split(':')[1].Split(' ')[1]),
                                size_x = StringToNumber(str.Split('{')[1].Split(';')[1].Split(':')[1]),
                                size_y = StringToNumber(str.Split('{')[1].Split(';')[2].Split(':')[1])
                            });
                        }
                        catch { }
                    }
                }
                Icons_Count_In_Css.Text = Icons.List.Count + " Icons Found";                
            }
            ShowHideExtractBtn();
        }
        private void LoadPicture()
        {
            textBox_Picture.Text = "";
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = System.IO.Directory.GetCurrentDirectory(),
                Title = "PNG Sprites",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "png",
                Filter = "png files|*.png",
                RestoreDirectory = true,
                ReadOnlyChecked = true,
                ShowReadOnly = true
            };
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox_Picture.Text = openFileDialog1.FileName;
                Icons.Bitmap = new Bitmap(openFileDialog1.FileName);
            }
            ShowHideExtractBtn();
        }
        private void SetPath()
        {
            textBox_Output_Path.Text = "";
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog
            {
                Description = "Output",
                SelectedPath = System.IO.Directory.GetCurrentDirectory()
            };
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox_Output_Path.Text = folderBrowserDialog1.SelectedPath;
            }
            ShowHideExtractBtn();
        }
        private bool AllLoad()
        {
            bool result = false;
            if ((textBox_Css.Text != "") && (textBox_Picture.Text != "") && (textBox_Output_Path.Text != ""))
            { result = true; }

            return result;
        }
        private void ShowHideExtractBtn()
        {
            button_Extract_Icons.Visible = AllLoad();
        }
        private void ExtractIcons()
        {
            if (AllLoad())
            {
                foreach (Icons.Icon_Structure icon in Icons.List)
                {
                    Bitmap bitmap = new Bitmap(icon.size_x, icon.size_y);
                    Graphics graphics = Graphics.FromImage(bitmap);
                    graphics.DrawImage(Icons.Bitmap, icon.background_position_x, icon.background_position_y);
                    if (!Directory.Exists(textBox_Output_Path.Text + @"\")) { Directory.CreateDirectory(textBox_Output_Path.Text + @"\"); }
                    bitmap.Save(textBox_Output_Path.Text + @"\" + icon.name + ".png", ImageFormat.Png);
                }
            }
        }

        //Events
        private void button_Select_Css_Click(object sender, EventArgs e)
        {
            LoadCss();
        }
        private void button_Select_Picture_Click(object sender, EventArgs e)
        {
            LoadPicture();
        }
        private void button_Path_Click(object sender, EventArgs e)
        {
            SetPath();
        }
        private void button_Extract_Icons_Click(object sender, EventArgs e)
        {
            ExtractIcons();
        }        
    }
}
