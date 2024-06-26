﻿using MySqlConnector;
using System.IO;
using System;
using System.IO;
using System.Linq;
using System.Configuration;


namespace wave
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            var cs = ConnString.connString;

            using (var con = new MySqlConnection(cs))
            {
                con.Open();

                string sql = "SELECT user_surname FROM users;";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                var userSurname = cmd.ExecuteScalar()?.ToString();
                HelloText.Text = "User Surname: " + userSurname;
            }
        }
    }

}
