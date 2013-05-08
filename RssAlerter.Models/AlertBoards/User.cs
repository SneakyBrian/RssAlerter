using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RssAlerter.Models.AlertSources;

namespace RssAlerter.Models.AlertBoards
{
    public class User
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string MembershipId { get; set; }

        public IList<string> AlertBoardIds { get; set; }

        public IList<string> RssFeedIds { get; set; }

        public User()
        {
            AlertBoardIds = new List<string>();
            RssFeedIds = new List<string>();
            Id = "alertboard/user/"; // db assigns id
        }
    }
}
