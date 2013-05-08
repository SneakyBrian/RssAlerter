using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RssAlerter.Models.AlertSources;
using System.ComponentModel.DataAnnotations;

namespace RssAlerter.Models.AlertBoards
{
    public class AlertBoard
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// List of alert sources for this board
        /// </summary>
        public IList<string> RssFeedIds { get; set; }

        public AlertBoard()
        {
            RssFeedIds = new List<string>();
            Id = "alertboard/"; // db assigns id
        }

    }
}
