using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookmaker.Models
{
    class Passport
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public byte[] Photo { get; set; }

        public Passport(string id, string firstName, string lastName, string patronymic, byte[] Photo)
        {
            this.Id = id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Patronymic = patronymic;
            this.Photo = Photo;
        }
    }
}
