using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookmaker.Services
{
    class LoadPhotoService : IService
    {
        public object Execute(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "jpg";
            openFileDialog.Filter =
                "JPG files (*.jpg)|*.jpg|BMP files (*.bmp)|*.bmp|PNG files (*.png)|*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                byte[] photo = File.ReadAllBytes(openFileDialog.FileName);
                return photo;
            }
            return false;
        }
    }
}
