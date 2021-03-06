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
using System.Diagnostics;
using BotTibia.Classes;
using System.Management;
using System.Collections.Generic;

namespace BotTibia
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
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

                Process[] processlist = Process.GetProcesses();

                Bitmap tela = CapturaTela.CaptureWindow(Global._tibiaProcessName);
                this.WindowState = (WindowState)FormWindowState.Minimized;

                var corte = CapturaTela.CortaTela(tela);

                //Captura Dados da vida
                Global._vida.SetCoordenadasPorImagemDoCoracao(PegaElementosDaTela.PegaElementos(corte, "coracao"));
                Global._vida.CalculaPixelsDoHeal(int.Parse(TerceiroHealPercent.Text), 
                                                    int.Parse(SegundoHealPercent.Text), 
                                                        int.Parse(PrimeiroHealPercent.Text));
                Global._vida.pixel = tela.GetPixel(Global._vida.LowHeal.X, Global._vida.LowHeal.Y);

                //Captura Dados da mana
                Global._mana.SetCoordenadasPorImagemDoRaio(PegaElementosDaTela.PegaElementos(corte, "raio"));
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
                Global._threadHeal = new Thread(() => Healer.Healar(Global._vida, Global._mana, Global._status, Global._fireTimer, Global._tibiaProcessName));
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
                Global._threadHeal.Interrupt();
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

        private void EhEk_Checked(object sender, RoutedEventArgs e)
        {
            TerceiroHeal.Content = "Potion:";
            Global._status.EhEk = true;
        }
        private void EhEk_Unchecked(object sender, RoutedEventArgs e)
        {
            TerceiroHeal.Content = "Terceiro:";
            Global._status.EhEk = false;
        }
        private void ClientComboBox_SelectedIndexChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ClientComboBox.SelectedItem as string))
            {
                Global._tibiaProcessName = ClientComboBox.SelectedItem as string;
            }
            else
            {
                Global._tibiaProcessName = "";
            }
        }

        private void ClientComboBox_DropDownOpened(object sender, EventArgs e)
        {
            ClientComboBox.Items.Clear();
            Process[] processlist = Process.GetProcesses();
            processlist.ToList().ForEach(process => 
                { 
                    if(process.MainWindowTitle.StartsWith("Tibia - "))
                    {
                        ClientComboBox.Items.Add(process.MainWindowTitle);
                    }
                });
        }
    }
}
