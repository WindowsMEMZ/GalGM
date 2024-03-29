﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GalGM
{
    public partial class EnterFileName : Form
    {
        private string n;

        public EnterFileName(string defaultName = "Resource1")
        {
            n = defaultName;
            InitializeComponent();
        }

        public string GetName
        {
            get { return textBox1.Text; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult= DialogResult.Cancel;
        }

        private void EnterFileName_Load(object sender, EventArgs e)
        {
            textBox1.Text = n;
        }
    }
}
