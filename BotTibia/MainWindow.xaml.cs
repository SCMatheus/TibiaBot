using System.Windows;
using MessageBox = System.Windows.MessageBox;
using System.Windows.Forms;
using BotTibia.Elementos;
using System.Threading;
using System.Drawing;
using BotTibia.Acoes;
using System;
using System.Windows.Input;
using System.Net.NetworkInformation;
using System.Linq;

namespace BotTibia
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        public static class Global
        {
            public static VidaBar _vida = new VidaBar();
            public static ManaBar _mana = new ManaBar();
            public static PersonagemStatus _status = new PersonagemStatus();
            public static int _fireTimer { get; set; }
            public static Thread _threadHeal { get; set; }
            public static string _telaPrincipal { get; set; }
        }

        public MainWindow()
        {
            String firstMacAddress = NetworkInterface
                                        .GetAllNetworkInterfaces()
                                        .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                                        .Select(nic => nic.GetPhysicalAddress().ToString())
                                        .FirstOrDefault();
            if(firstMacAddress != "B42E9904CFAA" && firstMacAddress != "40490FFE36E9" && firstMacAddress != "F48E38E15B8F")
            {
                Environment.Exit(1);
            }
            InitializeComponent();
        }

        private void OnClickEvent(object sender, RoutedEventArgs e)
        {
            try
            {

                //inicialização dos dados
                Global._vida.HighHeal.Key = PrimeiroHealHotkey.Text;
                Global._vida.MediumHeal.Key = SegundoHealHotkey.Text;
                Global._vida.LowHeal.Key = TerceiroHealHotkey.Text;
                Global._mana.ManaHeal.Key = ManaHealHotkey.Text;
                Global._status.ParaKey = ParalizeHealHotkey.Text;
                Global._fireTimer = int.Parse(FireTimer.Text);

                Bitmap tela = CapturaTela.CapturaDeTela();
                this.WindowState = (WindowState)FormWindowState.Minimized;
                Thread.Sleep(1000);
                var telaTibia = false;
                Global._telaPrincipal = PegaTelaPrincipal.GetActiveWindowTitle();
                if (Global._telaPrincipal != null) {
                    telaTibia = Global._telaPrincipal.Contains("Tibia");
                }
                if (telaTibia) {
                    var corte = CapturaTela.CortaTela(tela);

                    //Captura Dados da vida
                    Global._vida.Coordenadas = PegaElementosDaTela.PegaElementos(corte, "vida");
                    Global._vida.CalculaPixelsDoHeal(int.Parse(TerceiroHealPercent.Text), 
                                                        int.Parse(SegundoHealPercent.Text), 
                                                            int.Parse(PrimeiroHealPercent.Text));
                    Global._vida.pixel = tela.GetPixel(Global._vida.HighHeal.X, Global._vida.HighHeal.Y);

                    //Captura Dados da mana
                    Global._mana.Coordenadas = PegaElementosDaTela.PegaElementos(corte, "mana");
                    Global._mana.CalculaPixelsDoHeal(int.Parse(ManaHealPercent.Text));
                    Global._mana.pixel = tela.GetPixel(Global._mana.ManaHeal.X, Global._mana.ManaHeal.Y);

                    //Captura Dados da mana
                    var aux = PegaElementosDaTela.PegaElementos(corte, "statusBar");
                    Global._status.Coordenadas.X = aux.X - 118;
                    Global._status.Coordenadas.Y = aux.Y;
                    Global._status.Coordenadas.Height = aux.Height;


                    this.WindowState = (WindowState)FormWindowState.Maximized;
                    HealcheckBox.IsEnabled = true;
                    ParalizecheckBox.IsEnabled = true;
                    MessageBox.Show("Bot inciado com sucesso!");
                }
            }catch(Exception ex)
            {
                this.WindowState = (WindowState)FormWindowState.Maximized;
                MessageBox.Show(ex.Message);
            }
        }

        private void SetaHotkeyPrimeiroHeal(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                Global._vida.HighHeal.Key = SetaHotkey(e.Key, PrimeiroHealHotkey);
            }
            catch (Exception ex)
            {
                Global._vida.HighHeal.Key = "F1";
                MessageBox.Show(ex.Message);
            }
        }

        private void SetaPercentPrimeiroHeal(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                SetaValorDePorcentagem(PrimeiroHealPercent, Global._vida.CalculaPixelsDoHealHigh);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SetaHotkeySegundoHeal(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                Global._vida.MediumHeal.Key = SetaHotkey(e.Key, SegundoHealHotkey);
            }
            catch (Exception ex)
            {
                Global._vida.MediumHeal.Key = "F1";
                MessageBox.Show(ex.Message);
            }
        }

        private void SetaPercentSegundoHeal(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                SetaValorDePorcentagem(SegundoHealPercent, Global._vida.CalculaPixelsDoHealMedium);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SetaHotkeyTerceiroHeal(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                Global._vida.LowHeal.Key = SetaHotkey(e.Key, TerceiroHealHotkey);
            }
            catch (Exception ex)
            {
                Global._vida.LowHeal.Key = "F1";
                MessageBox.Show(ex.Message);
            }
        }

        private void SetaPercentTerceiroHeal(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                SetaValorDePorcentagem(TerceiroHealPercent, Global._vida.CalculaPixelsDoHealLow);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SetaHotkeyManaHeal(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                Global._mana.ManaHeal.Key = SetaHotkey(e.Key, ManaHealHotkey);
            }
            catch (Exception ex)
            {
                Global._mana.ManaHeal.Key = "F1";
                MessageBox.Show(ex.Message);
            }
        }

        private void SetaPercentManaHeal(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                SetaValorDePorcentagem(ManaHealPercent, Global._mana.CalculaPixelsDoHeal);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AtivarCura(object sender, RoutedEventArgs e)
        {
            try
            {
                Global._threadHeal = new Thread(() => Healer.Healar(Global._vida, Global._mana, Global._status, Global._fireTimer, Global._telaPrincipal));
                FireTimer.IsEnabled = false;
                Global._threadHeal.Start();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DesativarCura(object sender, RoutedEventArgs e)
        {
            try
            {
                FireTimer.IsEnabled = true;
                Global._threadHeal.Abort();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DesativarParalizeCura(object sender, RoutedEventArgs e)
        {
            try
            {
                Global._status.ParaStatus = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AtivarParalizeCura(object sender, RoutedEventArgs e)
        {
            try
            {
                Global._status.ParaStatus = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SetaParalizeManaHeal(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                Global._status.ParaKey = SetaHotkey(e.Key, ParalizeHealHotkey);
            }
            catch (Exception ex)
            {
                Global._status.ParaKey = "F1";
                MessageBox.Show(ex.Message);
            }
        }

        private void FireTimer_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try { 
                int resultado;
                if (int.TryParse(FireTimer.Text, out resultado))
                {
                    Global._fireTimer = resultado;
                }
                else
                {
                    throw new Exception("Por favor digite um valor valido!");
                }
            }
            catch (Exception ex)
            {
                FireTimer.Text = "";
                MessageBox.Show(ex.Message);
            }
        }

        public void SetaValorDePorcentagem(System.Windows.Controls.TextBox textBox, Action<int> CalculoDePixel)
        {

            if (textBox.Text == "")
            {
                return;
            }
            else
            {

                int resultado;
                if (int.TryParse(textBox.Text, out resultado))
                {
                    if (resultado >= 1 && resultado < 100)
                    {
                        CalculoDePixel(resultado);
                    }
                    else
                    {
                        textBox.Text = "";
                        throw new Exception("Por favor digite um valor valido! \n" +
                                                "O valor deve ser um numero entre 1 e 99!");
                    }
                }
                else
                {
                    textBox.Text = "";
                    throw new Exception("Por favor digite um valor valido! \n" +
                                            "O valor deve ser um numero entre 1 e 99!");
                }
            }
        }

        public string SetaHotkey(Key e, System.Windows.Controls.TextBox textBox)
        {
            if (e >= Key.F1 && e <= Key.F12)
            {
                textBox.Text = e.ToString();
                return textBox.Text;
            }
            else
            {
                textBox.Text = "";
                throw new Exception("Por favor digite uma Hotkey válida");
            }
        }
    }
}
