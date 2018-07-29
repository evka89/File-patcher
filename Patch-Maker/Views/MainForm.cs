using Patch_Maker.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Patch_Maker
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        // Open Dir
        private void btnOpen_Click(object sender, EventArgs e)
        {
            fldDlg.ShowDialog();
            txtPath.Text = fldDlg.SelectedPath;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
        // Get Button
        private void btnGen_Click(object sender, EventArgs e)
        {
            if (txtPath.Text != "" && Directory.Exists(txtPath.Text)){
                PatchMaker patchMaker = new PatchMaker(txtPath.Text);
                patchMaker.patchgeneratingprogress += PatchMaker_patchgeneratingprogress;
                patchMaker.GeneratePatch();
            }else{
                MessageBox.Show($"Directory is not available: {txtPath.Text}");
            }
        }

        private void PatchMaker_patchgeneratingprogress(object sender, Data.PatchMakerGeneratingEventArgs e)
        {
            prgProgMake.Maximum = e.countFiles;
            prgProgMake.Value = e.countFiles;
        }
    }
}
