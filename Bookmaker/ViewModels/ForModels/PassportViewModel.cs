using Bookmaker.Models;
using Bookmaker.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Bookmaker.ViewModels
{
    [Table("Passports")]
    class PassportViewModel : ViewModelBase, IDataErrorInfo
    {
        private Passport Passport;
        public PassportViewModel()
        {
            this.Passport = new Passport(null, null, null, null, null);
        }

        public string Id
        {
            get { return Passport.Id; }
            set
            {
                Passport.Id = value;
                OnPropertyChanged("Id");
            }
        }
        public string FirstName
        {
            get { return Passport.FirstName; }
            set
            {
                Passport.FirstName = value;
                OnPropertyChanged("FirstName");
            }
        }
        public string LastName
        {
            get { return Passport.LastName; }
            set
            {
                Passport.LastName = value;
                OnPropertyChanged("LastName");
            }
        }
        public string Patronymic
        {
            get { return Passport.Patronymic; }
            set
            {
                Passport.Patronymic = value;
                OnPropertyChanged("Patronymic");
            }
        }
        public byte[] Photo
        {
            get { return Passport.Photo; }
            set
            {
                Passport.Photo = value;
                OnPropertyChanged("Photo");
            }
        }

        #region LoadPassportPhotoCommand

        private Command.RelayCommand loadPassportPhotoCommand;
        public ICommand LoadPassportPhotoCommand
        {
            get
            {
                if (loadPassportPhotoCommand == null)
                {
                    loadPassportPhotoCommand = new Command.RelayCommand(LoadPassportPhoto);
                }
                return loadPassportPhotoCommand;
            }
        }

        private void LoadPassportPhoto(object obj)
        {
            this.Photo = ServiceManager.CallService("LoadPhoto", null) as byte[];
        }
        #endregion

        #region IDataErrorInfo Members
        public string Error => null;
        public string this[string columnName]
        {
            get
            {
                string result = null;

                switch (columnName)
                {
                    case "Id":
                        result = this.IdValidation();
                        break;

                    case "FirstName":
                        result = this.FirstNameValidation();
                        break;

                    case "LastName":
                        result = this.LastNameValidation();
                        break;

                    case "Patronymic":
                        result = this.PatronymicValidation();
                        break;

                    case "Photo":
                        result = this.PhotoValidation();
                        break;

                    default:
                        break;
                }

                return result;
            }
        }

        #endregion

        #region ValidationMethods

        public void Reset()
        {
            this.Id = default(string);
            this.FirstName = default(string);
            this.LastName = default(string);
            this.Patronymic = default(string);
            this.Photo = default(byte[]);
        }

        public bool IsValid()
        {
            var IdValid = this.IdValidation();
            var FirstNameValid = this.FirstNameValidation();
            var LastNameValid = this.LastNameValidation();
            var PatronymicValid = this.PatronymicValidation();
            var PhotoValid = this.PhotoValidation();

            var result = IdValid == null && FirstNameValid == null && LastNameValid == null
                && PatronymicValid == null && PhotoValid == null;
            return result;
        }
        private string IdValidation()
        {
            string result = null;
            Regex rule = new Regex(@"^[a-zA-Z0-9]{10}$");

            if (string.IsNullOrEmpty(this.Id))
                result = "Please enter a Passport Id";
            else if (!rule.IsMatch(this.Id))
                result = "Incorrect Id";

            return result;
        }
        private string FirstNameValidation()
        {
            string result = null;
            Regex rule = new Regex(@"^[a-zA-Z]{2,32}$|^[а-яА-Я]{2,32}$");
            
            if (string.IsNullOrEmpty(this.FirstName))
                result = "Please enter a First Name";
            else if (!rule.IsMatch(this.FirstName))
                result = "Incorrect First Name";

            return result;
        }
        private string LastNameValidation()
        {
            string result = null;
            Regex rule = new Regex(@"^[a-zA-Z]{2,32}$|^[а-яА-Я]{2,32}$");
            
            if (string.IsNullOrEmpty(this.LastName))
                result = "Please enter a Last Name";
            else if (!rule.IsMatch(this.LastName))
                result = "Incorrect Last Name";
            
            return result;
        }
        private string PatronymicValidation()
        {
            string result = null;
            Regex rule = new Regex(@"^[a-zA-Z]{2,32}$|^[а-яА-Я]{2,32}$");
            
            if (string.IsNullOrEmpty(this.Patronymic))
                result = "Please enter a Patronymic";
            else if (!rule.IsMatch(this.Patronymic))
                result = "Incorrect Patronymic";
            
            return result;
        }
        private string PhotoValidation()
        {
            string result = null;
            
            if (Photo == null)
                result = "Please load a Photo";
            
            return result;
        }

        #endregion
    }
}
