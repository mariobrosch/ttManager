﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Scoreboard.Data.data.requests;
using Scoreboard.Data.models;

namespace Scoreboard.forms
{
    public partial class FrmPlayers : Form
    {
        private List<Player> players;
        private Player currentPlayer;

        public FrmPlayers()
        {
            InitializeComponent();

            LoadPlayers();
        }

        private void LoadPlayers()
        {
            players = PlayerData.Get(chkDisplayRemoved.Checked)?.OrderBy(p => p.Name).ToList();

            lbPlayers.DataSource = players;
            lbPlayers.DisplayMember = "Name";

            if (players.Count == 0)
            {
                txtId.Text = FormsHelper.GetResourceText("new");
            }
        }

        private void LbPlayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((Player)lbPlayers.SelectedItem != null)
            {
                LoadPlayer(((Player)lbPlayers.SelectedItem));
            }
        }

        private void LoadPlayer(Player player)
        {
            txtId.Text = player.Id.ToString();
            txtName.Text = player.Name;
            txtPhotoUrl.Text = player.PhotoUrl;
            chkEnabled.Checked = player.IsEnabled;
            chkRemoved.Checked = player.IsRemoved;

            btnDelete.Visible = true;

            var singlePlayerMatches = SinglePlayerMatchData.GetForPlayer(player.Id);
            if (singlePlayerMatches.Count > 0)
            {
                var bestMatch = singlePlayerMatches.OrderBy(x => x.HighScore).Last();
                lblSinglePlayerMatches.Text = singlePlayerMatches.Count + " " + FormsHelper.GetResourceText("singlePlayerStat") + " " + bestMatch.MatchDateParsed.ToString("yyyy-MM-dd") + " met als score " + bestMatch.HighScore.ToString();
            }
            else
            {
                lblSinglePlayerMatches.Text = "-";
            }

            var matchesForPlayer = MatchData.GetForPlayer(player.Id);
            var matchesWon = matchesForPlayer.Where(m => m.WinnerId == player.Id).ToList().Count;
            lblMatchResults.Text = matchesForPlayer.Count > 0 ? matchesForPlayer.Count.ToString() + " " + FormsHelper.GetResourceText("played") + " " + FormsHelper.GetResourceText("and") + matchesWon.ToString() + " " + FormsHelper.GetResourceText("won") : "0";
            currentPlayer = player;
            
            LoadPhoto();
        }

        private void TxtPhotoUrl_TextChanged(object sender, EventArgs e)
        {
            currentPlayer.PhotoUrl = txtPhotoUrl.Text;
            LoadPhoto();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var matchesForPlayer = MatchData.GetForPlayer(Int32.Parse(txtId.Text));

            if (matchesForPlayer.Count == 0)
            {
                if (MessageBox.Show(FormsHelper.GetResourceText("RemovePlayerCompletely"), FormsHelper.GetResourceText("RemovePlayerCompletelyTitle"), MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    PlayerData.Delete(txtId.Text);
                    LoadPlayers();
                }
            }
            else if (MessageBox.Show(FormsHelper.GetResourceText("removePlayer"), FormsHelper.GetResourceText("remove"), MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var player = (Player)lbPlayers.SelectedItem;
                player.IsRemoved = true;
                PlayerData.Update(player);
                LoadPlayers();
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (txtId.Text != FormsHelper.GetResourceText("new"))
            {
                var player = (Player)lbPlayers.SelectedItem;
                if (player == null)
                {
                    return;
                }
                player.IsEnabled = chkEnabled.Checked;
                player.Name = txtName.Text;
                player.PhotoUrl = txtPhotoUrl.Text;

                PlayerData.Update(player);
            }
            else
            {
                Player newPlayer = new Player
                {
                    IsEnabled = chkEnabled.Checked,
                    Name = txtName.Text,
                    PhotoUrl = txtPhotoUrl.Text,
                    IsRemoved = chkRemoved.Checked
                };

                PlayerData.Create(newPlayer);
            }
            LoadPlayers();
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            lbPlayers.ClearSelected();
            txtName.Text = "";
            txtId.Text = FormsHelper.GetResourceText("new");
            txtPhotoUrl.Text = "";
            pbPhoto.Image = null;
            chkEnabled.Checked = true;
            btnDelete.Visible = false;
            lblSinglePlayerMatches.Text = "";
            lblPlayedMatches.Text = "";
        }

        private void ChkDisplayRemoved_CheckedChanged(object sender, EventArgs e)
        {
            LoadPlayers();
        }

        private void BtnUploadFile_Click(object sender, EventArgs e)
        {
            ofdPhoto.Filter = "Jpg|*.jpg";
            ofdPhoto.Title = FormsHelper.GetResourceText("OpenPhoto");

            if (ofdPhoto.ShowDialog() == DialogResult.OK)
            {
                byte[] imageArray = System.IO.File.ReadAllBytes(ofdPhoto.FileName);
                currentPlayer.Photo = Convert.ToBase64String(imageArray);
                PlayerData.Update(currentPlayer);
                LoadPhoto();
            }
        }

        private void BtnRemovePhoto_Click(object sender, EventArgs e)
        {
            currentPlayer.Photo = "";
            PlayerData.Update(currentPlayer);
            LoadPhoto();
        }

        private void LoadPhoto()
        {
            btnRemovePhoto.Visible = false;
            if (!string.IsNullOrEmpty(currentPlayer.Photo))
            {
                pbPhoto.Image = Image.FromStream(new MemoryStream(Convert.FromBase64String(currentPlayer.Photo)));
                btnRemovePhoto.Visible = true;
            }
            else if (!string.IsNullOrEmpty(currentPlayer.PhotoUrl))
            {
                pbPhoto.Load(currentPlayer.PhotoUrl);
            }
            else
            {
                pbPhoto.Image = null;
            }
        }
    }
}