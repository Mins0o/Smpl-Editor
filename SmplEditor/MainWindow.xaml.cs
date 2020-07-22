using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmplEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var a = new SmplLoader();
            a.LoadSmpl();
        }
    }

    public class SmplLoader
    {
        public Song[] LoadSmpl()
        {
            OpenFileDialog openSmpls = new OpenFileDialog();
            openSmpls.Multiselect = true;
            openSmpls.ShowDialog();
            Song[] result = new Song[0];
            foreach (string openedSmpl in openSmpls.FileNames)
            {
                Song[] readInList = JsonSerializer.Deserialize<Smpl>(File.ReadAllText(openedSmpl)).members;
                Song[] temp = new Song[result.Length];
                result.CopyTo(temp,0);
                result = new Song[temp.Length + readInList.Length];
                temp.CopyTo(result, 0);
                readInList.CopyTo(result, temp.Length);
            }
            return result;
        }
    }
}
