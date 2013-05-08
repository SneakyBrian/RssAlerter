using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RssAlerter.Models.AlertBoards;
using RssAlerter.Models.AlertSources;

namespace RssAlerter.Mvc.Web.Models
{
    public class AlertBoardViewModel
    {
        public string AlertBoardId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string AlertBoardUserId { get; set; }

        public string[] RssFeedAlertSourceIds { get; set; }

        public MultiSelectList RssFeedAlertSourceList { get; set; }

        public AlertBoardViewModel() { }

        /// <summary>
        /// creates a view model from the specified model
        /// </summary>
        /// <param name="alertBoard"></param>
        public AlertBoardViewModel(AlertBoard alertBoard, IEnumerable<RssFeedAlertSource> rssFeeds) 
        {
            AlertBoardId = alertBoard.Id;
            Name = alertBoard.Name;
            Description = alertBoard.Description;
            AlertBoardUserId = alertBoard.UserId;
            RssFeedAlertSourceIds = alertBoard.RssFeedIds.ToArray();
            RssFeedAlertSourceList = new MultiSelectList(rssFeeds, "Id", "FeedUriString", RssFeedAlertSourceIds);
        }

        /// <summary>
        /// Creates a view model for the specified user
        /// </summary>
        /// <param name="user"></param>
        public AlertBoardViewModel(User user, IEnumerable<RssFeedAlertSource> rssFeeds) 
        {
            AlertBoardUserId = user.Id;
            RssFeedAlertSourceList = new MultiSelectList(rssFeeds, "Id", "FeedUriString");
        }


        /// <summary>
        /// set the model from the specified viewmodel
        /// </summary>
        /// <param name="alertBoard"></param>
        /// <returns>the modified model</returns>
        public AlertBoard SetAlertBoard(AlertBoard alertBoard)
        {
            if (alertBoard.Id != this.AlertBoardId)
                throw new ArgumentException("Alert Board does not match");

            if (alertBoard.UserId != this.AlertBoardUserId)
                throw new ArgumentException("Alert Board User does not match");

            alertBoard.Name = this.Name;
            alertBoard.Description = this.Description;
            
            alertBoard.RssFeedIds.Clear();

            foreach (var sourceId in this.RssFeedAlertSourceIds)
            {
                alertBoard.RssFeedIds.Add(sourceId);
            }

            return alertBoard;
        }

        /// <summary>
        /// Converts the viewmodel to a new alert board model
        /// </summary>
        /// <returns></returns>
        public AlertBoard ToAlertBoard(User user)
        {
            return SetAlertBoard(new AlertBoard
            {
                Id = this.AlertBoardId,
                UserId = user.Id,
                RssFeedIds = new List<string>()
            });
        }
    }
}