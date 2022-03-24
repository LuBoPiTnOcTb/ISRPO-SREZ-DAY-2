using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.Word;
using Window = System.Windows.Window;
using Microsoft.Win32;

namespace SalaryShop
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Sale> Sales = new List<Sale>();
        public class Sale
        {
            public DateTime DateSale { get; set; }
            public Client Client { get; set; }
            public List<Telephone> Telephones { get; set; }
        }
        public class Client
        {
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string Patronymic { get; set; }
        }
        public class Telephone
        {
            public int Articul { get; set; }
            public string NameTelephone { get; set; }
            public string Category { get; set; }
            public decimal Cost { get; set; }
            public int Count { get; set; }
            public string Manufacturer { get; set; }
        }
        public MainWindow()
        {
            InitializeComponent();
            photograf.Visibility = Visibility.Hidden;
            //WebClient getClient = new WebClient();
            //getClient.Encoding = Encoding.UTF8;
            //string data = getClient.DownloadString("https://localhost:7100/api/Sale/Get?datestart=10%2E02%2E2020&dateend=11%2E02%2E2020");
            //Sales = JsonConvert.DeserializeObject<List<Sale>>(data);
            //Dg1.ItemsSource = Sales;
        }

        private void btnPut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dpStart.SelectedDate >= dpEnd.SelectedDate)
                {
                    MessageBox.Show("Дата начала не должна быть больше даты окончания");
                }else
                {
                    WebClient getClient = new WebClient();
                    getClient.Encoding = Encoding.UTF8;
                    string startDate = $@"{dpStart.SelectedDate.Value.Month}.{dpStart.SelectedDate.Value.Day}.{dpStart.SelectedDate.Value.Year}";
                    string endDate = $@"{dpEnd.SelectedDate.Value.Month}.{dpEnd.SelectedDate.Value.Day}.{dpEnd.SelectedDate.Value.Year}";
                    string data = getClient.DownloadString($@"https://localhost:7100/api/Sale/Get?dateStart={startDate}&dateEnd={endDate}");
                    Sales = JsonConvert.DeserializeObject<List<Sale>>(data);
                    Dg1.ItemsSource = Sales;
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnWordChek_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Dg1.SelectedItem != null)
                {
                    var per = Dg1.SelectedItem.ToString();
                    Sale selected1 = Dg1.SelectedItem as Sale;
                    Client selected2 = Dg1.SelectedItem as Client;
                    Telephone selected3 = Dg1.SelectedItem as Telephone;
                    SaveFileDialog sfd = new SaveFileDialog();
                    string source = $@"{Directory.GetCurrentDirectory()}\товарный чек.doc";
                    word.Application app = new word.Application();
                    word.Document doc = app.Documents.Open(source);
                    word.Bookmarks wB = doc.Bookmarks;
                    doc.Activate();
                    try
                    {
                        if (sfd.ShowDialog() == false)
                        {
                            doc.Close();
                            doc = null;
                            app.Quit();
                            return;
                        }
                        wB["Номер чека"].Range.Text = selected1.ToString();
                        wB["Артикул"].Range.Text = selected3.Articul.ToString();
                        //wB["Дата"].Range.Text = selected1.Telephones.Find().DateSale.ToShortDateString();
                        wB["Цена"].Range.Text = selected3.Cost.ToString();
                        wB["Сумма"].Range.Text = selected3.ToString();
                        wB["НаименованиеТовара"].Range.Text = selected3.NameTelephone.ToString();
                        wB["Колво"].Range.Text = selected3.Count.ToString();
                        wB["ЕдИзм"].Range.Text = selected3.Manufacturer.ToString();
                        doc.SaveAs2(sfd.FileName);
                        doc.Close();
                        doc = null;
                        app.Quit();
                        MessageBox.Show("Файл успешно создан");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        doc.Close();
                        doc = null;
                        app.Quit();
                    }
                }
                else throw new Exception("Запись не выбрана,выберите запись и повторите попытку!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnExcelChek_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cbGraf_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Dg1.ItemsSource == null)
            {
                MessageBox.Show("График нельзя отобразить потому что нету данных в таблице");
                
            }else
            if (cbGraf.ItemsSource != null) {
                if (cbGraf.SelectedIndex == 0)
                {

                    SpGraf.Reset();
                    List<String> manufacturer = new List<String>();
                    List<Double> count = new List<double>();

                    Dictionary<string, double> data = new Dictionary<string, double>();

                    foreach (Sale sale in Dg1.ItemsSource)
                    {
                        foreach (Telephone phone in sale.Telephones)
                        {
                            if (manufacturer.Contains(phone.Manufacturer))
                            {
                                count[manufacturer.IndexOf(phone.Manufacturer)] += 1;
                            }
                            else
                            {
                                manufacturer.Add(phone.Manufacturer);
                                count.Add(0);
                            }
                        }
                    }
                    double[] _count = count.ToArray();
                    string[] _manufacturer = manufacturer.ToArray();
                    var pie = SpGraf.Plot.AddPie(_count);
                    pie.SliceLabels = _manufacturer;
                    pie.ShowPercentages = true;
                    pie.ShowLabels = true;
                    SpGraf.Refresh();
                }

                else if (cbGraf.SelectedIndex == 1)
                {
                    photograf.Visibility = Visibility.Hidden;
                    SpGraf.Reset();


                    double[] xs = { 1, 2, 3, 4 };
                    double[] ys = { 1, 4, 9, 16 };

                    var scatterList = SpGraf.Plot.AddScatterList();
                    scatterList.AddRange(xs, ys);
                    scatterList.Add(5, 25);

                    SpGraf.Refresh();
                }
            }         
        }
    } 
}
