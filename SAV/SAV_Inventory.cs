﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_Inventory : Form
    {
        public SAV_Inventory()
        {
            InitializeComponent();
            Util.TranslateInterface(this, Main.curlanguage);
            sav = (byte[])Main.savefile.Clone();

            getListItems();
            getListKeyItems();
            getListTMHM();
            getListMedicine();
            getListBerries();

            B_DisplayItems.ForeColor = Color.Red;

            popItems();

            B_DisplayItems.Text = Main.itempouch[0];
            B_DisplayMedicine.Text = Main.itempouch[1];
            B_DisplayTMHM.Text = Main.itempouch[2];
            B_DisplayBerries.Text = Main.itempouch[3];
            B_DisplayKeyItems.Text = Main.itempouch[4];
        }
        public byte[] sav;

        public string[] item_val;
        public string[] keyitem_val;
        public string[] tmhm_val;
        public string[] medicine_val;
        public string[] berries_val;


        // Initialize String Tables
        private void getListItems()
        {
            ushort[] itemlist = (Main.SaveGame.ORAS) ? Legal.Pouch_Items_ORAS : Legal.Pouch_Items_XY;
            item_val = new string[itemlist.Length];
            for (int i = 0; i < itemlist.Length; i++)
                item_val[i] = Main.itemlist[itemlist[i]];
            Array.Sort(item_val);
        }
        private void getListKeyItems()
        {
            ushort[] itemlist = (Main.SaveGame.ORAS) ? Legal.Pouch_Key_ORAS : Legal.Pouch_Key_XY;
            keyitem_val = new string[itemlist.Length];
            for (int i = 0; i < itemlist.Length; i++)
                keyitem_val[i] = Main.itemlist[itemlist[i]];
            Array.Sort(keyitem_val);
        }
        private void getListTMHM()
        {
            ushort[] itemlist = (Main.SaveGame.ORAS) ? Legal.Pouch_TMHM_ORAS : Legal.Pouch_TMHM_XY;
            tmhm_val = new string[itemlist.Length];
            for (int i = 0; i < itemlist.Length; i++)
                tmhm_val[i] = Main.itemlist[itemlist[i]];
            // Array.Sort(tmhm_val); Already sorted, keep HMs last.
        }
        private void getListMedicine()
        {
            ushort[] itemlist = (Main.SaveGame.ORAS) ? Legal.Pouch_Medicine_ORAS : Legal.Pouch_Medicine_XY;
            medicine_val = new string[itemlist.Length];
            for (int i = 0; i < itemlist.Length; i++)
                medicine_val[i] = Main.itemlist[itemlist[i]];
            Array.Sort(medicine_val); 
        }
        private void getListBerries()
        {
            ushort[] itemlist = Legal.Pouch_Berry_XY;
            berries_val = new string[itemlist.Length];
            for (int i = 0; i < itemlist.Length; i++)
                berries_val[i] = Main.itemlist[itemlist[i]];
            Array.Sort(berries_val); 
        }

        // Populate DataGrid
        private void popItems()
        {
            int offset = Main.SaveGame.Items.HeldItem;
            populateList(item_val, offset, item_val.Length - 1); // max 400
        }
        private void popKeyItems()
        {
            int offset = Main.SaveGame.Items.KeyItem;
            populateList(keyitem_val, offset, keyitem_val.Length - 1); // max 96
        }
        private void popTMHM()
        {
            int offset = Main.SaveGame.Items.TMHM;
            populateList(tmhm_val, offset, tmhm_val.Length - 1);
        }
        private void popMedicine()
        {
            int offset = Main.SaveGame.Items.Medicine;
            populateList(medicine_val, offset, medicine_val.Length - 1); // 64 total slots
        }
        private void popBerries()
        {
            int offset = Main.SaveGame.Items.Berry;
            populateList(berries_val, offset, berries_val.Length - 1); // 102 slots
        }

        private void populateList(string[] itemarr, int offset, int itemcount)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            DataGridViewColumn dgvIndex = new DataGridViewTextBoxColumn();
            {
                dgvIndex.HeaderText = "CNT";
                dgvIndex.DisplayIndex = 1;
                dgvIndex.Width = 45;
                dgvIndex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            DataGridViewComboBoxColumn dgvItemVal = new DataGridViewComboBoxColumn
            {
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            {
                foreach (string t in itemarr)
                    dgvItemVal.Items.Add(t); // add only the Item Names

                dgvItemVal.DisplayIndex = 0;
                dgvItemVal.Width = 135;
                dgvItemVal.FlatStyle = FlatStyle.Flat;
            }
            dataGridView1.Columns.Add(dgvItemVal);
            dataGridView1.Columns.Add(dgvIndex);

            dataGridView1.Rows.Add(itemcount);
            dataGridView1.CancelEdit();

            string itemname = "";
            for (int i = 0; i < itemcount; i++)
            {
                int itemvalue = BitConverter.ToUInt16(sav, offset + i*4);
                try { itemname = Main.itemlist[itemvalue]; }
                catch
                {
                    Util.Error("Unknown item detected.", "Item ID: " + itemvalue, "Item is after: " + itemname);
                    continue;
                }
                int itemarrayval = Array.IndexOf(itemarr,itemname);
                if (itemarrayval == -1)
                {
                    dataGridView1.Rows[i].Cells[0].Value = itemarr[0];
                    dataGridView1.Rows[i].Cells[1].Value = 0;
                    Util.Alert(itemname + " removed from item pouch.", "If you exit the Item Editor by saving changes, the item will no longer be in the pouch.");
                }
                else
                {
                    dataGridView1.Rows[i].Cells[0].Value = itemarr[itemarrayval];
                    dataGridView1.Rows[i].Cells[1].Value = BitConverter.ToUInt16(sav, offset + i * 4 + 2);
                }
            }
        }
        private void dropclick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0) return;

            ComboBox comboBox = (ComboBox)dataGridView1.EditingControl;
            comboBox.DroppedDown = true;
        }
        private void saveBag(object sender)
        {
            int offset = 0;
            if (B_DisplayItems.ForeColor == Color.Red)
                offset = Main.SaveGame.Items.HeldItem;
            else if (B_DisplayKeyItems.ForeColor == Color.Red)
                offset = Main.SaveGame.Items.KeyItem;
            else if (B_DisplayTMHM.ForeColor == Color.Red)
                offset = Main.SaveGame.Items.TMHM;
            else if (B_DisplayMedicine.ForeColor == Color.Red)
                offset = Main.SaveGame.Items.Medicine;
            else if (B_DisplayBerries.ForeColor == Color.Red)
                offset = Main.SaveGame.Items.Berry;

            // Fetch Data
            int itemcount = dataGridView1.Rows.Count;
            int emptyslots = 0;
            for (int i = 0; i < itemcount; i++)
            {
                string item = dataGridView1.Rows[i].Cells[0].Value.ToString();
                int itemindex = Array.IndexOf(Main.itemlist, item);
                int itemcnt;
                try 
                { itemcnt = Convert.ToUInt16(dataGridView1.Rows[i].Cells[1].Value.ToString()); }
                catch { itemcnt = 0; }

                if (itemindex == 0) // Compression of Empty Slots
                {
                    emptyslots++;
                    continue;
                }
                if (itemcnt == 0)
                    itemcnt++; // No 0 count of items
                else if (itemcnt > 995)
                    itemcnt = 995; // cap out

                // Write Data into Save File
                Array.Copy(BitConverter.GetBytes((ushort)itemindex), 0, sav, offset + 4 * (i - emptyslots), 2); // item #
                Array.Copy(BitConverter.GetBytes((ushort)itemcnt), 0, sav, offset + 4 * (i - emptyslots) + 2, 2); // count
            }

            // Delete Empty Trash
            for (int i = itemcount - emptyslots; i < itemcount; i++)
            {
                Array.Copy(BitConverter.GetBytes((ushort)0), 0, sav, offset + 4 * i + 0, 2); // item #
                Array.Copy(BitConverter.GetBytes((ushort)0), 0, sav, offset + 4 * i + 2, 2); // count
            }

            // Load New Button Color, after finished we'll load the new data.
            B_DisplayItems.ForeColor =
            B_DisplayKeyItems.ForeColor =
            B_DisplayTMHM.ForeColor =
            B_DisplayMedicine.ForeColor =
            B_DisplayBerries.ForeColor = Main.defaultControlText;

            (sender as Button).ForeColor = Color.Red;
        }
        private void giveAll(string[] inarray, int count)
        {
            for (int i = 0; i < inarray.Length - 1; i++)
            {
                string itemname = inarray[i+1];
                int itemarrayval = Array.IndexOf(inarray, itemname);
                dataGridView1.Rows[i].Cells[0].Value = inarray[itemarrayval];
                dataGridView1.Rows[i].Cells[1].Value = count;
            }
        }
        private void B_DisplayItems_Click(object sender, EventArgs e)
        {
            // Store Current Items back to the save file
            saveBag(sender);
            popItems();
            if (ModifierKeys == Keys.Alt)
                giveAll(item_val, 995);
        }
        private void B_DisplayKeyItems_Click(object sender, EventArgs e)
        {
            // Store Current Items back to the save file
            saveBag(sender);
            popKeyItems();
            if (ModifierKeys == Keys.Alt && Util.Prompt(MessageBoxButtons.YesNo, String.Format("Warning: Adding all {0} is dangerous.", B_DisplayKeyItems.Text), "Continue?") == DialogResult.Yes)
                giveAll(keyitem_val, 1);
        }
        private void B_DisplayTMHM_Click(object sender, EventArgs e)
        {
            // Store Current Items back to the save file
            saveBag(sender);
            popTMHM();
            if (ModifierKeys == Keys.Alt && Util.Prompt(MessageBoxButtons.YesNo, String.Format("Warning: Adding all {0} is dangerous.", B_DisplayTMHM.Text), "Continue?") == DialogResult.Yes)
                giveAll(tmhm_val, 1);
        }
        private void B_DisplayMedicine_Click(object sender, EventArgs e)
        {
            // Store Current Items back to the save file
            saveBag(sender);
            popMedicine();
            if (ModifierKeys == Keys.Alt)
                giveAll(medicine_val, 995);
        }
        private void B_DisplayBerries_Click(object sender, EventArgs e)
        {
            // Store Current Items back to the save file
            saveBag(sender);
            popBerries();
            if (ModifierKeys == Keys.Alt)
                giveAll(berries_val, 995);
        }
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            saveBag(sender);
            Array.Copy(sav, Main.savefile, Main.savefile.Length);
            Main.savedited = true;
            Close();
        }
    }
}
