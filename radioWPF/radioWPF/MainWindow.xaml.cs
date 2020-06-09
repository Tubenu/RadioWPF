using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Refit;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Globalization;

namespace radioWPF
{

    public partial class MainWindow : Window
    {
        public List<Data_Structure> Stations { get; set; }


        public MainWindow()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            var provider = RestService.For<IRadioBrowserApi>("http://all.api.radio-browser.info");
            DataContext = this;
            InitializeComponent();
            var broken_stations = provider.GetBrokenStations().Result;
            var count = broken_stations.Count();
            Task.Run(async () =>
            {
                var result = await provider.GetByCountry(100, "Poland");
                this.Stations = result;
            }).Wait();
            foreach (var item in this.Stations)
            {

                if (!broken_stations.Exists(data => data.url_resolved == item.url_resolved))
                {
                    station_list.Items.Add(item.name);
                }



            }

            station_list.SelectedIndex = 4;
            PickStation();
            //task_runner();


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            PickStation();
            //task_runner();
            radio.Play();


        }
        void change_radiobutton()
        {
            radio.Stop();
            TurnOff_buttons();          
            task_runner();
            //radio.Play();
            //TurnOn_buttons();



        }
        void PickStation()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                var value = (string)this.station_list.SelectedValue;
                radio.Source = new Uri(this.Stations.FirstOrDefault(x => x.name == value)?.url_resolved ?? "http://stream.srg-ssr.ch/m/drs1/mp3_128", UriKind.RelativeOrAbsolute);
              
            }));

        }

        void task_runner()
        {
            var task =  Task.Run(() =>
            {
                
                PickStation();
                Thread.Sleep(3200);
            });
            var awaiter = task.GetAwaiter();
            awaiter.OnCompleted(() =>
            {
                radio.Play();

                 TurnOn_buttons();
            });

            
             Task.WhenAll(task);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            radio.Stop();

        }
        void TurnOff_buttons()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                play_button.IsChecked = false;
                stop_button.IsChecked = true;
                previous_station.IsEnabled = false;
                next_station.IsEnabled = false;
                random_station.IsEnabled = false;
            }));

        }

        void TurnOn_buttons()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                previous_station.IsEnabled = true;
                next_station.IsEnabled = true;
                random_station.IsEnabled = true;
                play_button.IsChecked = true;
            }));


        }

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            volume_percentage.Text = (volume.Value * 100) + "%".ToString();
            radio.Volume = (double)volume.Value;


        }

        private void next_station_Click(object sender, RoutedEventArgs e)
        {

            station_list.SelectedIndex += 1;
            change_radiobutton();

        }

        private void previous_station_Click(object sender, RoutedEventArgs e)
        {

            station_list.SelectedIndex -= 1;
            change_radiobutton();

        }

        private void random_station_Click(object sender, RoutedEventArgs e)
        {

            Random dupa = new Random();
            station_list.SelectedIndex = dupa.Next(0, 99);
            change_radiobutton();

        }

        private void drop_down_opened(object sender, EventArgs e)
        {
            
            stop_button.IsChecked = false;
            play_button.IsChecked = false;
            
        }


        private void dropdownclosed(object sender, EventArgs e)
        {
            // PickStation();
            // radio.Play();
            task_runner();
            play_button.IsChecked = true;
        }
    }
}
