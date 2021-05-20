using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using MQTTnet;
using MQTTnet.Client.Options;
using System.Globalization;

namespace MQTTProducer
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            ToggleForm(true);
        }

        private MqttFactory factory;
        private MQTTnet.Client.IMqttClient mqttClient;
        private string broker, port, clientId, topic;

        private bool keepRunning = false;

        private void ToggleForm(bool showhide)
        {
            txt_broker.IsVisible = showhide;
            txt_port.IsVisible = showhide;
            txt_clientid.IsVisible = showhide;
            txt_topic.IsVisible = showhide;

            lbl_broker.IsVisible = showhide;
            lbl_clientid.IsVisible = showhide;
            lbl_port.IsVisible = showhide;
            lbl_topic.IsVisible = showhide;

            btn_run.IsVisible = showhide;
            btn_stop1.IsVisible = !showhide;
            btn_stop2.IsVisible = !showhide;
        }

        private void btn_stopClicked(object sender, EventArgs e)
        {
            keepRunning = false;
            output.Text = output.Text + "\n\nSTOPPED";

            ToggleForm(true);
        }

        private async void btn_runClicked(object sender, EventArgs e)
        {
            broker = txt_broker.Text;
            port = txt_port.Text;
            clientId = txt_clientid.Text;
            topic = txt_topic.Text;

            ToggleForm(false);

            output.Text = "Sending to: " + broker;            

            keepRunning = true;

            while (keepRunning)
            {
                await Sender();
                await Task.Delay(1000);
            }
        }

        private async Task Sender()
        {
            factory = new MqttFactory();
            mqttClient = factory.CreateMqttClient();

            Random rnd = new Random();
            int reading = rnd.Next(1000);

            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",  CultureInfo.InvariantCulture);
            string payload = reading;

            output.Text = output.Text + "\n\n" + payload;

            var options = new MqttClientOptionsBuilder()
                .WithClientId(clientId)
                .WithTcpServer(broker)
                .Build();
            await mqttClient.ConnectAsync(options, CancellationToken.None);

            Console.WriteLine("Sending message...");
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .Build();

            await mqttClient.PublishAsync(message, CancellationToken.None);
        }
    }
}
