﻿using System.Windows;
using MessageBox = System.Windows.MessageBox;
using System.Threading;
using System.Drawing;
using BotTibia.Actions.Print;
using System;
using System.Windows.Input;
using System.Linq;
using System.Diagnostics;
using BotTibia.Classes;
using BotTibia.Actions.AHK;
using BotTibia.Actions.Heal;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using BotTibia.Persistencia;
using System.Collections.Generic;
using BotTibia.Actions.Cavebot;

namespace BotTibia
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
             
            Global._caminho = Environment.CurrentDirectory;
            try
            {
                var ahk = new AhkFunctions();
                ahk.LoadScripts();
            }catch(Exception e)
            {
                MessageBox.Show("Ocorreu um erro ao carregar as funções AHK.\n" +
                                " Por favor verifique se o AHK está instalado e se as bibliotecas AHK estão na pasta do Bot.");
                MessageBox.Show("Erro: "+e.Message);
                Environment.Exit(1);
            }
            InitializeComponent();
        }
        #region Configuracao na selecao de personagem
        private void ConfigureBot()
        {
            CavebotCheckBox.IsEnabled = false;
            HealcheckBox.IsEnabled = false;
            ParalizecheckBox.IsEnabled = false;
            //inicialização dos dados
            AtualizaVariaveisGlobais();

            Bitmap tela = CapturaTela.CaptureWindow(Global._tibiaProcessName);
            Global._tela.X = 0;
            Global._tela.Y = 0;
            Global._tela.Width = tela.Width;
            Global._tela.Height = tela.Height;

            Global._andarDoMap = null;
            var count = 0;
            while (count <= 15)
            {
                Global._andarDoMap = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._tela,
                                                                    Global._caminho + "\\Images\\MapAndar\\floor-" +
                                                                    count.ToString() + ".png");
                if (Global._andarDoMap != null)
                {
                    break;
                }
                count++;
            }
            if (Global._andarDoMap == null)
                throw new Exception("Não foi possivel identificar a barra dos andares do personagem.");

            //Captura dados da Character Window
            count = 0;
            Global._mainWindow = null;
            while (Global._mainWindow == null)
            {
                Global._mainWindow = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._tela,
                                                                          Global._caminho + $"\\Images\\Global\\Configs\\characterWindow_{count}.png");
                count++;
                if (count >= 3)
                    break;
            }
            if(Global._mainWindow == null) 
                throw new Exception("Não foi possivel identificar a window do personagem.");

            //Captura Dados do mini map
            Global._miniMap = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._tela, 
                                                                   Global._caminho + $"\\Images\\Global\\Configs\\miniMap.png");

            if (Global._miniMap == null)
                throw new Exception("Não foi possivel identificar o Mini Map do personagem.");
            Global._miniMap.X += 3;
            Global._miniMap.Y += 3;
            Global._miniMap.Width -= 7;
            Global._miniMap.Height -= 7;
            //Captura Dados da vida
            var coordenadasCoracao = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._tela.X / 2,
                                                                                                0, Global._tela.Width,
                                                                                   Global._tela.Height, Global._caminho + "\\Images\\Global\\Configs\\coracao.png");
            if (coordenadasCoracao == null)
                throw new Exception("Não foi possivel identificar a life do personagem.");

            Global._vida.SetCoordenadasPorImagemDoCoracao(coordenadasCoracao);
            Global._vida.CalculaPixelsDoHeal(int.Parse(TerceiroHealPercent.Text),
                                                int.Parse(SegundoHealPercent.Text),
                                                    int.Parse(PrimeiroHealPercent.Text));

            //Captura Dados da mana
            var coordenadasRaio = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._tela.X / 2,
                                                                                                0, Global._tela.Width,
                                                                                                Global._tela.Height, Global._caminho + "\\Images\\Global\\Configs\\raio.png");
            if (coordenadasCoracao == null)
                throw new Exception("Não foi possivel identificar a life do personagem.");

            Global._mana.SetCoordenadasPorImagemDoRaio(coordenadasRaio);
            Global._mana.CalculaPixelsDoHeal(int.Parse(ManaHealPercent.Text));

            //Captura Dados da paralize
            var coordenadasStatusBar = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._tela.X / 2,
                                                            0, Global._tela.Width,
                                                            Global._tela.Height, Global._caminho + "\\Images\\Global\\Configs\\statusBar.png");
            if (coordenadasStatusBar == null)
                throw new Exception("Não foi possivel identificar a status bar do personagem.");

            Global._status.Coordenadas = coordenadasStatusBar;


            CavebotCheckBox.IsEnabled = true;
            HealcheckBox.IsEnabled = true;
            ParalizecheckBox.IsEnabled = true;
            MessageBox.Show("Bot inciado com sucesso!");
        }
        #endregion
        #region Healling
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
        #endregion
        #region Select Character
        private void ClientComboBox_SelectedIndexChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(ClientComboBox.SelectedItem as string))
                {
                    Global._tibiaProcessName = ClientComboBox.SelectedItem as string;
                    ConfigureBot();
                }
                else
                {             
                    Global._tibiaProcessName = "";
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                Global._tibiaProcessName = "";
                ClientComboBox.SelectedValue = "";
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
        #endregion
        #region MenuSuperior
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Displays a SaveFileDialog so the user can save the Image
                // assigned to Button2.
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "*.xml|";
                saveFileDialog.Title = "Save script";

                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // If the file name is not an empty string open it for saving.
                    if (saveFileDialog.FileName != "")
                    {
                        // Saves the Image via a FileStream created by the OpenFile method.
                        SaveToXml(saveFileDialog.FileName);
                    }
                }
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new OpenFileDialog()
                {
                    FileName = "Select a text file",
                    Filter = "Xml files (*.xml)|*.xml",
                    Title = "Open xml file"
                };

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // If the file name is not an empty string open it for saving.
                    if (openFileDialog.FileName != "")
                    {
                        LoadByXml(openFileDialog.FileName);

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        #endregion
        #region Save Script
        public void SaveToXml(string file)
        {
            var script = new ConfigScripts()
            {
                Primeiro = new Heal() { Percent = PrimeiroHealPercent.Text, Hotkey = PrimeiroHealHotkey.Text },
                Segundo = new Heal() { Percent = SegundoHealPercent.Text, Hotkey = SegundoHealHotkey.Text },
                Terceiro = new Heal() { Percent = TerceiroHealPercent.Text, Hotkey = TerceiroHealHotkey.Text },
                ManaHeal = new Heal() { Percent = ManaHealPercent.Text, Hotkey = ManaHealHotkey.Text },
                ParaHeal = ParalizeHealHotkey.Text,
                Firetimer = int.Parse(FireTimer.Text),
            };
            // Create a new XmlSerializer instance with the type of the test class
            XmlSerializer SerializerObj = new XmlSerializer(typeof(ConfigScripts));

            // Create a new file stream to write the serialized object to a file
            using (TextWriter WriteFileStream = new StreamWriter(@file))
            {
                SerializerObj.Serialize(WriteFileStream, script);

                // Cleanup
                WriteFileStream.Close();
            }
        }
        #endregion
        #region Load script
        public void LoadByXml(string file)
        {
            XmlSerializer SerializerObj = new XmlSerializer(typeof(ConfigScripts));
            ConfigScripts LoadedConfigs;
            // Create a new file stream for reading the XML file
            using (FileStream ReadFileStream = new FileStream(@file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {

                // Load the object saved above by using the Deserialize function
                LoadedConfigs = (ConfigScripts)SerializerObj.Deserialize(ReadFileStream);

                // Cleanup
                ReadFileStream.Close();
            }
            //Life
            PrimeiroHealPercent.Text = LoadedConfigs.Primeiro.Percent;
            PrimeiroHealHotkey.Text = LoadedConfigs.Primeiro.Hotkey;
            SegundoHealPercent.Text = LoadedConfigs.Segundo.Percent;
            SegundoHealHotkey.Text = LoadedConfigs.Segundo.Hotkey;
            TerceiroHealPercent.Text = LoadedConfigs.Terceiro.Percent;
            TerceiroHealHotkey.Text = LoadedConfigs.Terceiro.Hotkey;
            //Mana
            ManaHealPercent.Text = LoadedConfigs.ManaHeal.Percent;
            ManaHealHotkey.Text = LoadedConfigs.ManaHeal.Hotkey;
            //Para
            ParalizeHealHotkey.Text = LoadedConfigs.ParaHeal;
            //Firetimer
            FireTimer.Text = LoadedConfigs.Firetimer.ToString();

            AtualizaVariaveisGlobais();
        }
        #endregion
        #region Atualizacao de variaveis
        public void AtualizaVariaveisGlobais()
        {
            Global._vida.HighHeal.Key = PrimeiroHealHotkey.Text;
            Global._vida.MediumHeal.Key = SegundoHealHotkey.Text;
            Global._vida.LowHeal.Key = TerceiroHealHotkey.Text;
            Global._mana.ManaHeal.Key = ManaHealHotkey.Text;
            Global._status.ParaKey = ParalizeHealHotkey.Text;
            Global._fireTimer = int.Parse(FireTimer.Text);
        }
        #endregion

        private void AtivarCaveBot(object sender, RoutedEventArgs e)
        {
            try
            {
                Global._threadCavebot = new Thread(() => Cavebot.Hunting());
                Global._threadCavebot.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void DesativarCaveBot(object sender, RoutedEventArgs e)
        {
            try
            {
                Global._threadCavebot.Interrupt();
                Global._threadCavebot.Abort();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
