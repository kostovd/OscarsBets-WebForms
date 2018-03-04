﻿using MovieScrapper.Business;
using MovieScrapper.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MovieScrapper.Admin
{
    public partial class Calendar : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var gamePropertyService = GetBuisnessService<IGamePropertyService>();
            DateTime startGameDate = gamePropertyService.GetGameStartDate();
            DateTime endGameDate = gamePropertyService.GetGameStopDate();

            lblServerDate.Text = string.Format("Server date and time: {0}", DateTime.Now);
            lblStartGameDate.Text = string.Format("Start game date and time: {0}", startGameDate);
            lblEndGameDate.Text = string.Format("End game date and time: {0}", endGameDate);

            if (!Page.IsPostBack)
            {
                StartGameCalendar.SelectedDate = startGameDate.Date;
                StartGameCalendar.VisibleDate = startGameDate.Date;
                StopGameCalendar.SelectedDate = endGameDate.Date;
                StopGameCalendar.VisibleDate = endGameDate.Date;

                StartGameTimeTextbox.Text = string.Format("{0:D2}:{1:D2}", startGameDate.Hour, startGameDate.Minute);
                StopGameTimeTextbox.Text = string.Format("{0:D2}:{1:D2}", endGameDate.Hour, endGameDate.Minute);
            }
        }

        protected void StopGameValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (StartGameCalendar.SelectedDate == null 
                || StartGameCalendar.SelectedDate == new DateTime(0001, 1, 1, 0, 0, 0)
                ||StopGameCalendar.SelectedDate == null 
                || StopGameCalendar.SelectedDate == new DateTime(0001, 1, 1, 0, 0, 0)
                || StartGameCalendar.SelectedDate>=StopGameCalendar.SelectedDate)// not click any date
                args.IsValid = false;
            else
                args.IsValid = true;
        }
        protected void ChangeDateButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var gamePropertyService = GetBuisnessService<IGamePropertyService>();

                var startDate = StartGameCalendar.SelectedDate;
                var startTimeArray = StartGameTimeTextbox.Text.Split(':');

                startDate = new DateTime(
                    startDate.Year,
                    startDate.Month,
                    startDate.Day,
                    int.Parse(startTimeArray[0]),
                    int.Parse(startTimeArray[1]),
                    0);

                gamePropertyService.ChangeGameStartDate(startDate);

                var stopDate = StopGameCalendar.SelectedDate;
                var stopTimeArray = StopGameTimeTextbox.Text.Split(':');

                stopDate = new DateTime(
                    stopDate.Year,
                    stopDate.Month,
                    stopDate.Day,
                    int.Parse(stopTimeArray[0]),
                    int.Parse(stopTimeArray[1]),
                    0);

                gamePropertyService.ChangeGameStopDate(stopDate);

                Response.Redirect("Calendar.aspx");
            }
        }

    }
}