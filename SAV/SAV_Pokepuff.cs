﻿using System;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_Pokepuff : Form
    {
        public SAV_Pokepuff()
        {
            InitializeComponent();
            Util.TranslateInterface(this, Main.curlanguage);
            sav = (byte[])Main.savefile.Clone();

            pfa = Main.puffs;
            pfa[0] = "---";
            Setup();

            new ToolTip().SetToolTip(B_Sort, "Hold CTRL to reverse sort.");
            new ToolTip().SetToolTip(B_All, "Hold CTRL to give Deluxe instead of Supreme.");
        }

        public byte[] sav;
        private string[] pfa =
        {
            "Empty",
            "Basic Sweet", "Basic Mint", "Basic Citrus", "Basic Mocha", "Basic Spice",
            "Frosted Sweet", "Frosted Mint", "Frosted Citrus", "Frosted Mocha", "Frosted Spice",
            "Fancy Sweet", "Fancy Mint", "Fancy Citrus", "Fancy Mocha", "Fancy Spice",
            "Deluxe Sweet", "Deluxe Mint", "Deluxe Citrus", "Deluxe Mocha", "Deluxe Spice",
            "Supreme Wish", "Supreme Honor", "Supreme Spring", "Supreme Summer", "Supreme Fall", "Supreme Winter",
        };
        private void Setup()
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();

            DataGridViewColumn dgvIndex = new DataGridViewTextBoxColumn();
            {
                dgvIndex.HeaderText = "Slot";
                dgvIndex.DisplayIndex = 0;
                dgvIndex.Width = 45;
                dgvIndex.ReadOnly = true;
                dgvIndex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            DataGridViewComboBoxColumn dgvPuff = new DataGridViewComboBoxColumn
            {
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            {
                foreach (string t in pfa)
                    dgvPuff.Items.Add(t);

                dgvPuff.DisplayIndex = 1;
                dgvPuff.Width = 135;
                dgvPuff.FlatStyle = FlatStyle.Flat;
            }
            dgv.Columns.Add(dgvIndex);
            dgv.Columns.Add(dgvPuff);

            dgv.Rows.Add(100);
            for (int i = 0; i < 100; i++)
            {
                dgv.Rows[i].Cells[0].Value = (i + 1).ToString();
                dgv.Rows[i].Cells[1].Value = pfa[sav[Main.SaveGame.Puff + i]];
            }
            MT_CNT.Text = BitConverter.ToUInt32(sav, Main.SaveGame.Puff + 100).ToString("0");
        }
        private void dropclick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 1) return;
            ((ComboBox)(sender as DataGridView).EditingControl).DroppedDown = true;
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_All_Click(object sender, EventArgs e)
        {
            int basepuff = 20;
            int basemod = 6;
            if (ModifierKeys == Keys.Control)
            {
                basepuff = 1;
                basemod = 0x19;
            }
            byte[] newpuffs = new byte[100];
            for (int i = 0; i < 100; i++)
                newpuffs[i] = (byte) (Util.rnd32()%basemod + basepuff);

            Array.Copy(newpuffs, 0, sav, Main.SaveGame.Puff, 100);
            Setup();
        }
        private void B_None_Click(object sender, EventArgs e)
        {
            byte[] newpuffs = new byte[100];
            newpuffs[0] = 1;
            newpuffs[1] = 2;
            newpuffs[2] = 3;
            newpuffs[3] = 4;
            newpuffs[4] = 5;
            Array.Copy(newpuffs, 0, sav, Main.SaveGame.Puff, 100);
            Setup();
        }
        private void B_Sort_Click(object sender, EventArgs e)
        {
            byte[] puffarray = new byte[100];
            if (ModifierKeys == Keys.Control)
            {
                for (int i = 0; i < 100; i++)
                {
                    string puff = dgv.Rows[i].Cells[1].Value.ToString();
                    puffarray[i] = (byte) Array.IndexOf(pfa, puff);
                }
                Array.Sort(puffarray);
                Array.Reverse(puffarray);
            }
            else
            {
                int count = 0;
                for (int i = 0; i < 100; i++)
                {
                    string puff = dgv.Rows[i].Cells[1].Value.ToString();
                    byte puffval = (byte) Array.IndexOf(pfa, puff);
                    if (puffval == 0) continue;
                    puffarray[count] = puffval;
                    count++;
                }
                Array.Resize(ref puffarray, count);
                Array.Sort(puffarray);
                Array.Resize(ref puffarray, 100);
            }
            Array.Copy(puffarray, 0, sav, Main.SaveGame.Puff, 100);
            Setup();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            byte[] puffarray = new byte[100];
            int emptyslots = 0;
            for (int i = 0; i < 100; i++)
            {
                string puff = dgv.Rows[i].Cells[1].Value.ToString();
                if (Array.IndexOf(pfa, puff) == 0)
                {
                    emptyslots++;
                    continue;
                }

                puffarray[i - emptyslots] = (byte) Array.IndexOf(pfa, puff);
            }
            Array.Copy(puffarray, 0, sav, Main.SaveGame.Puff, 100);
            Array.Copy(sav, Main.savefile, sav.Length);
            Main.savedited = true;
            Close();
        }
    }
}
