using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bookmaker.Models
{
    class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
        public bool IsAdministrator { get; set; }
        public bool IsBlocked { get; set; }
        public string CardId { get; set; }
        public decimal Cash { get; set; }
        public DateTime RegisterDate { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public byte[] Avatar { get; set; }
        public User(int id, string userName, bool isAdministrator, string cardId, decimal cash, string phoneNumber, string email)
        {

            var info = Application.GetResourceStream(new Uri("pack://application:,,,/Resources/Icons/StandartAvatar.png"));
            var memoryStream = new MemoryStream();
            info.Stream.CopyTo(memoryStream);

            this.Id = id;
            this.UserName = userName;
            this.IsAdministrator = isAdministrator;
            this.IsBlocked = false;
            this.CardId = cardId;
            this.Cash = cash;
            this.PhoneNumber = phoneNumber;
            this.Email = email;
            this.Avatar = memoryStream.ToArray();
        }
    }
}
