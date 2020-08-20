using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bookmaker.Models
{
    class Team
    {
        public string TeamName { get; set; }
        public KindOfSport KindOfSport { get; set; }
        public byte[] Logotype { get; set; }
        public Team(string teamName, KindOfSport kindOfSport)
        {
            var info = Application.GetResourceStream(new Uri("pack://application:,,,/Resources/Icons/NoLogo.png"));
            var memoryStream = new MemoryStream();
            info.Stream.CopyTo(memoryStream);

            this.TeamName = teamName;
            this.KindOfSport = kindOfSport;
            this.Logotype = memoryStream.ToArray();
        }
    }
}
