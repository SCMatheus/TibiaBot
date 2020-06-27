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
            if(firstMacAddress != "B42E9904CFAA")
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
                Global._vida.MediumHeal.Key = SegundoHealPercent.Text;
                Global._vida.LowHeal.Key = TerceiroHealHotkey.Text;
                Global._mana.ManaHeal.Key = ManaHealHotkey.Text;
                Global._fireTimer = 300;

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
                    Global._mana.CalculaPixelsDoHeal(50);
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
                if (e.Key >= Key.F1 && e.Key <= Key.F12)
                {
                    PrimeiroHealHotkey.Text = e.Key.ToString();
                    Global._vida.HighHeal.Key = PrimeiroHealHotkey.Text;
                }
                else
                {
                    PrimeiroHealHotkey.Text = "";
                    MessageBox.Show("Por favor digite uma Hotkey válida");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SetaPercentPrimeiroHeal(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if(PrimeiroHealPercent.Text == "")
                {
                    return;
                }
                int resultado;
                if (int.TryParse(PrimeiroHealPercent.Text, out resultado))
                {
                    if (resultado >= 1 && resultado < 99)
                        if (int.Parse(PrimeiroHealPercent.Text) >= 1 && int.Parse(PrimeiroHealPercent.Text) <= 99)
                        {
                            Global._vida.CalculaPixelsDoHealHigh(resultado);
                        }
                        else
                        {
                            throw new Exception("Por favor digite um valor valido! \n" +
                                                    "O valor deve ser um numero entre 1 e 99!");
                        }
                }
                else
                {
                    throw new Exception("Por favor digite um valor valido! \n" +
                                            "O valor deve ser um numero entre 1 e 99!");
                }
            }
            catch (Exception ex)
            {
                PrimeiroHealPercent.Text = "";
                MessageBox.Show(ex.Message);
            }
        }

        private void SetaHotkeySegundoHeal(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key >= Key.F1 && e.Key <= Key.F12)
                {
                    SegundoHealHotkey.Text = e.Key.ToString();
                    Global._vida.MediumHeal.Key = SegundoHealHotkey.Text;
                }
                else
                {
                    SegundoHealHotkey.Text = "";
                    MessageBox.Show("Por favor digite uma Hotkey válida");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SetaPercentSegundoHeal(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try 
            {
                if (SegundoHealPercent.Text == "")
                {
                    return;
                }
                int resultado;
                if (int.TryParse(SegundoHealPercent.Text, out resultado))
                {
                    if (resultado >= 1 && resultado < 99)
                    {
                        Global._vida.CalculaPixelsDoHealMedium(resultado);
                    }
                    else
                    {
                        throw new Exception("Por favor digite um valor valido! \n" +
                                                "O valor deve ser um numero entre 1 e 99!");
                    }
                }
                else
                {
                    throw new Exception("Por favor digite um valor valido! \n" +
                                            "O valor deve ser um numero entre 1 e 99!");
                }
            }
            catch (Exception ex)
            {
                SegundoHealPercent.Text = "";
                MessageBox.Show(ex.Message);
            }
        }

        private void SetaHotkeyTerceiroHeal(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key >= Key.F1 && e.Key <= Key.F12)
                {
                    TerceiroHealHotkey.Text = e.Key.ToString();
                    Global._vida.LowHeal.Key = TerceiroHealHotkey.Text;
                }
                else
                {
                    TerceiroHealHotkey.Text = "";
                    MessageBox.Show("Por favor digite uma Hotkey válida");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SetaPercentTerceiroHeal(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try 
            {
                if (TerceiroHealPercent.Text == "")
                {
                    return;
                }
                int resultado;
                if (int.TryParse(TerceiroHealPercent.Text, out resultado))
                {
                    if (resultado >= 1 && resultado < 99)
                    {
                        Global._vida.CalculaPixelsDoHealLow(resultado);
                    }
                    else
                    {
                        throw new Exception("Por favor digite um valor valido! \n" +
                                                "O valor deve ser um numero entre 1 e 99!");
                    }
                }
                else
                {
                    throw new Exception("Por favor digite um valor valido! \n" +
                                            "O valor deve ser um numero entre 1 e 99!");
                }
            }
            catch (Exception ex)
            {
                TerceiroHealPercent.Text = "";
                MessageBox.Show(ex.Message);
            }
}

        private void SetaHotkeyManaHeal(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key >= Key.F1 && e.Key <= Key.F12)
                {
                    ManaHealHotkey.Text = e.Key.ToString();
                    Global._vida.LowHeal.Key = ManaHealHotkey.Text;
                }
                else
                {
                    ManaHealHotkey.Text = "";
                    MessageBox.Show("Por favor digite uma Hotkey válida");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SetaPercentManaHeal(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (ManaHealPercent.Text == "")
                {
                    return;
                }
                int resultado;
                if (int.TryParse(ManaHealPercent.Text, out resultado))
                {
                    if (resultado >= 1 && resultado < 99)
                    {
                        Global._mana.CalculaPixelsDoHeal(resultado);
                    }
                    else
                    {
                        throw new Exception("Por favor digite um valor valido! \n" +
                                                "O valor deve ser um numero entre 1 e 99!");
                    }
                }
                else
                {
                    throw new Exception("Por favor digite um valor valido! \n" +
                                            "O valor deve ser um numero entre 1 e 99!");
                }
            }
            catch (Exception ex)
            {
                ManaHealPercent.Text = "";
                MessageBox.Show(ex.Message);
            }
        }

        private void AtivarCura(object sender, RoutedEventArgs e)
        {
            try
            {
                Global._threadHeal = new Thread(() => Healer.Healar(Global._vida, Global._mana, Global._status, Global._fireTimer, Global._telaPrincipal));
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
    }
}
