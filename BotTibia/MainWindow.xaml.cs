﻿using BotTibia.Actions.AHK;
using BotTibia.Actions.Cavebot;
using BotTibia.Actions.Heal;
using BotTibia.Actions.Print;
using BotTibia.Actions.Target;
using BotTibia.Classes;
using BotTibia.Enums;
using BotTibia.Persistencia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml.Serialization;
using MessageBox = System.Windows.MessageBox;
using Point = System.Drawing.Point;

namespace BotTibia {
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window {
        private readonly Dictionary<EnumSuportedClients, string> _suportedClients = 
            new Dictionary<EnumSuportedClients, string>() { 
                { EnumSuportedClients.Global, "Tibia - " }, 
                { EnumSuportedClients.ArchLight, "Legacy - " } 
            };

        public MainWindow()
        {
            Global._path = Environment.CurrentDirectory;
            try
            {
                AhkFunctions.LoadScripts();
                
                Enum.GetValues(typeof(EnumMarks))
                    .Cast<EnumMarks>()
                    .ToList()
                    .ForEach(prop =>
                        Global._marksMap.Add(prop, System.Drawing.Image.FromFile(Global._path + $"\\Images\\Marks\\{prop.ToString().ToLower()}.png")));
            } catch(Exception e)
            {
                MessageBox.Show("Ocorreu um erro ao carregar as funções AHK.\n" +
                                " Por favor verifique se o AHK está instalado e se as bibliotecas AHK estão na pasta do Bot.");
                MessageBox.Show("Erro: "+e.Message);
                Environment.Exit(1);
            }
            Global._backpacks.Add(new Backpack() { Tipo = EnumTipoBackpack.Main, Bp = EnumBackpacks.none, Coordenadas = null });
            Global._backpacks.Add(new Backpack() { Tipo = EnumTipoBackpack.Supply, Bp = EnumBackpacks.none, Coordenadas = null });
            Global._backpacks.Add(new Backpack() { Tipo = EnumTipoBackpack.Loot, Bp = EnumBackpacks.none, Coordenadas = null });
            Global._backpacks.Add(new Backpack() { Tipo = EnumTipoBackpack.Gold, Bp = EnumBackpacks.none, Coordenadas = null });
            Global._backpacks.Add(new Backpack() { Tipo = EnumTipoBackpack.Ammo, Bp = EnumBackpacks.none, Coordenadas = null });
            InitializeComponent();
        }
        #region Configuracao na selecao de personagem
        private void ConfigureBot(IProgress<int> progress)
        {
            Dispatcher.Invoke(() => {
                CavebotCheckBox.IsEnabled = false;
                HealcheckBox.IsEnabled = false;
                ParalizecheckBox.IsEnabled = false;
                TargetCheckBox.IsEnabled = false;
                LootCheckBox.IsEnabled = false;
                //inicialização dos dados
                AtualizaVariaveisGlobais();
            });

            Bitmap tela = CapturaTela.CaptureWindow(Global._tibiaProcessName);
            Global._tela.X = 0;
            Global._tela.Y = 0;
            Global._tela.Width = tela.Width;
            Global._tela.Height = tela.Height;

            //Captura Dados do mini map
            Global._miniMap = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._tela,
                                                                        Global._path + $"\\Images\\{Global.SelectedClient}\\Configs\\miniMap.png");

            if (Global._miniMap == null)
                throw new Exception("Não foi possivel identificar o Mini Map do personagem.");

            Dispatcher.Invoke(() => {
                progress.Report(30);
            });

            Global._miniMap.X += 3;
            Global._miniMap.Y += 3;
            Global._miniMap.Width -= 7;
            Global._miniMap.Height -= 7;

            if (Global.SelectedClient == EnumSuportedClients.ArchLight) {
                Global._battle = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._tela,
                                                            Global._path + $"\\Images\\{Global.SelectedClient}\\Configs\\battle.png");
                Dispatcher.Invoke(() => {
                    CavebotCheckBox.IsEnabled = true;
                    TargetCheckBox.IsEnabled = true;
                    progress.Report(100);
                });
                return;
            }

            Global._andarDoMap = null;
            List<int> andares = new List<int>() { 7, 6, 8, 5, 9, 4, 10, 3, 11, 2, 12, 1, 13, 0, 14 };
            foreach (var item in andares) {
                Global._andarDoMap = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._tela,
                                                                    Global._path + "\\Images\\MapAndar\\floor-" +
                                                                    item.ToString() + ".png");
                if (Global._andarDoMap != null) {
                    break;
                }
            }
            if (Global._andarDoMap == null)
                throw new Exception("Não foi possivel identificar a barra dos andares do personagem.");


            Dispatcher.Invoke(() => {
                progress.Report(10);
            });

            //Captura dados da Character Window
            var count = 0;
            Global._mainWindow = null;
            while (Global._mainWindow == null) {
                Global._mainWindow = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._tela,
                                                                            Global._path + $"\\Images\\Global\\Configs\\characterWindow_{count}.png", 25);
                count++;
                if (count >= 3)
                    break;
            }
            if (Global._mainWindow == null)
                throw new Exception("Não foi possivel identificar a window do personagem.");

            //Captura Dados da vida
            var coordenadasCoracao = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._tela, Global._path + "\\Images\\Global\\Configs\\coracao.png");
            if (coordenadasCoracao == null)
                throw new Exception("Não foi possivel identificar a life do personagem.");

            var cap = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._tela, Global._path + "\\Images\\Global\\Configs\\cap.png", 30);
            if(cap == null)
                throw new Exception("Não foi possivel identificar a cap do personagem.");

            Global._cap = new CoordenadasDeElementos() { X = cap.X, Y = cap.Y + cap.Height, Width = cap.Width, Height = cap.Height };

            Dispatcher.Invoke((Action)(() =>
            {
                progress.Report(50);
                Global._vida.SetCoordenadasPorImagemDoCoracao(coordenadasCoracao);
                Global._vida.CalculaPixelsDoHeal(int.Parse(TerceiroHealPercent.Text),
                                                    int.Parse(SegundoHealPercent.Text),
                                                        int.Parse(PrimeiroHealPercent.Text));
            }));

            //Captura Dados da mana
            var coordenadasRaio = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._tela.X / 2,
                                                                                                0, Global._tela.Width,
                                                                                                Global._tela.Height, Global._path + "\\Images\\Global\\Configs\\raio.png");
            if (coordenadasCoracao == null)
                throw new Exception("Não foi possivel identificar a life do personagem.");

            //Calcula posição do personagem
            Global._coordenadaDoPersonagem = PegaElementosDaTela.PegaPosicaoDoPersonagem();

            Dispatcher.Invoke((Action)(() =>
            {
                progress.Report(65);
            }));
            Global._mana.SetCoordenadasPorImagemDoRaio(coordenadasRaio);
            Dispatcher.Invoke((Action)(() =>
            {
                Global._mana.CalculaPixelsDoHeal(int.Parse(ManaHealPercent.Text));
            }));

            //Captura Dados da paralize
            var coordenadasStatusBar = PegaElementosDaTela.PegaElementosAhk(Global._tibiaProcessName, Global._tela.X / 2,
                                                            0, Global._tela.Width,
                                                            Global._tela.Height, Global._path + "\\Images\\Global\\Configs\\statusBar.png");
            if (coordenadasStatusBar == null)
                throw new Exception("Não foi possivel identificar a status bar do personagem.");

            Dispatcher.Invoke((Action)(() =>
            {
                progress.Report(80);
            }));
            Targeting.AjustaBattle();
            Global._status.Coordenadas = coordenadasStatusBar;

            Dispatcher.Invoke((Action)(() =>
            {
                CavebotCheckBox.IsEnabled = true;
                HealcheckBox.IsEnabled = true;
                ParalizecheckBox.IsEnabled = true;
                TargetCheckBox.IsEnabled = true;
                LootCheckBox.IsEnabled = true;

                progress.Report(100);
            }));
            //MessageBox.Show("Bot inciado com sucesso!");

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
                Global._threadHeal.IsBackground = true;
                Global._threadHeal.Start();
            }
            catch (ThreadAbortException ex)
            {

            }
            catch (Exception ex)
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

        public string SetaHotkeyNullable(Key e, System.Windows.Controls.TextBox textBox) {
            if (e >= Key.F1 && e <= Key.F12) {
                textBox.Text = e.ToString();
                return textBox.Text;
            } else if (e == Key.Delete) {
                textBox.Text = "";
                return null;
            } else {
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

        private async void ClientComboBox_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                LabelCalibrado.Content = "";
                ProgressConfig.Value = 0;
                if (!string.IsNullOrWhiteSpace(ClientComboBox.SelectedItem as string))
                {
                    Global._tibiaProcessName = ClientComboBox.SelectedItem as string;
                    var progress = new Progress<int>(value =>
                    {
                        ProgressConfig.Value = value;
                    });

                    var selected = _suportedClients.FirstOrDefault(sc => Global._tibiaProcessName.Contains(sc.Value));
                    Global.SelectedClient = selected.Key;

                    await Task.Run(() => ConfigureBot(progress));
                    LabelCalibrado.Content = "Calibrado!";
                }
                else
                {
                    LabelCalibrado.Content = "";
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
            var electedPages = _suportedClients.Values.SelectMany(sc =>
                    processlist
                        .Where(pl => pl.MainWindowTitle.StartsWith(sc))
                        .Select(pl => pl.MainWindowTitle)
                ).ToArray();

            foreach (var item in electedPages)
                ClientComboBox.Items.Add(item);
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
                Waypoints = Cavebot.Waypoints,
                VariaveisGlobais = Global._variaveisGlobais,
                Backpacks = Global._backpacks,
                Drops = Global._dropItens,
                Loots = Global._moveLootItens
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
            //Cavebot
            Cavebot.Waypoints = LoadedConfigs.Waypoints;
            GridView.ItemsSource = Cavebot.Waypoints;
            GridView.UpdateLayout();
            //Variaveis Globais
            Global._variaveisGlobais = LoadedConfigs.VariaveisGlobais;
            SetaVariaveisGlobaisNoTextBox();
            // Backpacks
            Global._backpacks = LoadedConfigs.Backpacks;
            AtualizaBpsNaView();
            //drops
            Global._dropItens = LoadedConfigs.Drops;
            Global._moveLootItens = LoadedConfigs.Loots;
            var drops = "";
            if (LoadedConfigs.Drops.Count > 0) {
                LoadedConfigs.Drops.ForEach(drop => drops += drop + ',');
                drops.Remove(drops.Length - 1, 1);
            }
            var loots = "";
            if (LoadedConfigs.Loots.Count > 0)
            {
                LoadedConfigs.Loots.ForEach(loot => loots += loot + ',');
                loots.Remove(loots.Length - 1, 1);
            }

            MoveItensTextBox.Text = loots;
            DropItensTextBox.Text = drops;

            AtualizaVariaveisGlobais();
        }
        private void AtualizaBpsNaView()
        {
            Global._backpacks.ForEach( backpack => { 
                if(backpack.Tipo == EnumTipoBackpack.Main)
                {
                    MainBP.SelectedItem = backpack.Bp;
                }
                else if(backpack.Tipo == EnumTipoBackpack.Loot)
                {
                    LootBP.SelectedItem = backpack.Bp;
                }
                else if (backpack.Tipo == EnumTipoBackpack.Gold)
                {
                    GoldBP.SelectedItem = backpack.Bp;
                }
                else if (backpack.Tipo == EnumTipoBackpack.Supply)
                {
                    SupplyBP.SelectedItem = backpack.Bp;
                }
                else
                {
                    AmmoBP.SelectedItem = backpack.Bp;
                }
            });
        }

        private void SetaVariaveisGlobaisNoTextBox()
        {
            string variaveis = "";
            foreach(var item in Global._variaveisGlobais)
            {
                variaveis += $"{item.Chave}={item.Valor};\r\n";
            }
            variaveisGlobais.Text = variaveis;
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
        #region Cavebot
        private void AtivarCaveBot(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Global.SelectedClient == EnumSuportedClients.Global) {
                    var andar = Cavebot.PegaAndarDoMap();
                    Global._ultimaCoordenadaDoPersonagem = PegaElementosDaTela
                                                           .PegaCoordenadasDoPersonagem(Global._tibiaProcessName,
                                                                                        Global._miniMap, andar);
                    if (Global._ultimaCoordenadaDoPersonagem == null || !(Cavebot.Waypoints.Count > 0)) {
                        MessageBox.Show("Não foi possível ativar o cavebot!");
                        CavebotCheckBox.IsChecked = false;
                        return;
                    }
                }
                if (Global._threadCavebot == null)
                {
                    Global._threadCavebot = new Thread(() =>
                    {
                        Hunting();
                    });
                    Global._threadCavebot.IsBackground = true;
                    Global._threadCavebot.Start();
                }
                else if (!Global._threadCavebot.IsAlive)
                {
                    Global._threadCavebot = new Thread(() =>
                    {
                        Hunting();
                    });
                    Global._threadCavebot.IsBackground = true;
                    Global._threadCavebot.Start();
                }
                GridView.IsEnabled = false;
                Global._isCavebot = true;
            }
            catch (Exception ex)
            {
                Global._threadCavebot.Interrupt();
                Global._threadCavebot.Abort();
                Global._threadCavebot = null;
                MessageBox.Show(ex.Message);
            }

        }
        private void DesativarCaveBot(object sender, RoutedEventArgs e)
        {
            try
            {
                Global._isCavebot = false;
                GridView.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void Hunting()
        {
            try
            {
                while (true)
                {

                    if(Global._isTarget)
                        Targeting.AttackAndLooting();
                    if (Global._isCavebot)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            if (GridView.HasItems)
                            {
                                GridView.UpdateLayout();
                                GridView.ScrollIntoView(GridView.Items[Cavebot.Index]);
                                ((DataGridRow)GridView.ItemContainerGenerator.ContainerFromIndex(Cavebot.Index)).IsSelected = true;
                            }
                        });
                        Cavebot.ExecutaWaypoint();
                    }
                    if (!(Global._isCavebot || Global._isTarget))
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (ThreadAbortException ex)
            {

            }
            catch (Exception ex)
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    MessageBox.Show(ex.Message);
                    GridView.IsEnabled = true;
                }));
            }
        }
        #endregion
        #region Target
        private void AtivarTarget(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Global._threadCavebot == null)
                {
                    Global._threadCavebot = new Thread(() =>
                    {
                        Hunting();
                    });
                    Global._threadCavebot.IsBackground = true;
                    Global._threadCavebot.Start();
                } else if (!Global._threadCavebot.IsAlive)
                {
                    Global._threadCavebot = new Thread(() =>
                    {
                        Hunting();
                    });
                    Global._threadCavebot.IsBackground = true;
                    Global._threadCavebot.Start();
                }
                Global._isTarget = true;
                TargetCheckBox.IsChecked = true;
            }
            catch (Exception ex)
            {
                Global._threadCavebot.Interrupt();
                Global._threadCavebot.Abort();
                Global._threadCavebot = null;
                MessageBox.Show(ex.Message);
            }
        }
        private void DesativarTarget(object sender, RoutedEventArgs e)
        {
            try
            {
                Global._isTarget = false;
                TargetCheckBox.IsChecked = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        #endregion
        #region Loot
        private void AtivarLoot(object sender, RoutedEventArgs e)
        {
            try
            {
                Global._isLoot = true;
                LootCheckBox.IsChecked = true;
            }
            catch (Exception ex)
            { 
                MessageBox.Show(ex.Message);
            }
        }
        private void DesativarLoot(object sender, RoutedEventArgs e)
        {
            try
            {
                Global._isLoot = false;
                LootCheckBox.IsChecked = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        #endregion
        #region number input validate
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private int? SetaValorDoRange(System.Windows.Controls.TextBox textBox, int maxRange)
        {
            if (textBox.Text == "")
            {
                return null;
            }
            else
            {

                if (int.TryParse(textBox.Text, out int resultado)) {
                    if (!(resultado >= 0 && resultado < maxRange + 1)) {
                        textBox.Text = "";
                        throw new Exception("Por favor digite um valor valido! \n" +
                                                "O valor deve ser um numero entre 0 e " + maxRange.ToString() + "!");
                    }
                } else {
                    textBox.Text = "";
                    throw new Exception("Por favor digite um valor valido! \n" +
                                            "O valor deve ser um numero entre 0 e e " + maxRange.ToString() + "!");
                }
                return resultado;
            }
        }

        private void SetaRangeX(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                SetaValorDoRange(textBoxRange1, 9);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SetaRangeY(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                SetaValorDoRange(textBoxRange2, 9);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion
        #region Button Waypoints
        private void ButtonNode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var waypoint = CriaWaypoint(Cavebot.Waypoints.Count() + 1, EnumWaypoints.Node
                                            , new Range(){X = int.Parse(textBoxRange1.Text),
                                            Y = int.Parse(textBoxRange1.Text)}, EnumAction.Empty, "");
                Cavebot.AddWaypoint(waypoint);
                GridView.ItemsSource = null;
                GridView.ItemsSource = Cavebot.Waypoints;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ButtonMark_Click(object sender, RoutedEventArgs e) {
            try {
                var waypoint = new Waypoint() { Mark = EnumMarks.Checked, Type = EnumWaypoints.Mark };
                Cavebot.AddWaypoint(waypoint);
                GridView.ItemsSource = null;
                GridView.ItemsSource = Cavebot.Waypoints;
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private Waypoint CriaWaypoint(int index, EnumWaypoints type, Range range, EnumAction typeAction, string parametros)
        {
            var waypoint = new Waypoint() {
                Index = index,
                Type = type,
                Range = range,
                TypeAction = typeAction,
                Parametros = parametros
            };
            if (Global.SelectedClient == EnumSuportedClients.Global) {
                var andar = Cavebot.PegaAndarDoMap();
                var coordenada = PegaElementosDaTela
                                       .PegaCoordenadasDoPersonagem(Global._tibiaProcessName,
                                                                    Global._miniMap, andar);
                if (coordenada == null) {
                    throw new Exception("Não foi possível encontrar a coordenada do personagem");

                }
                var direcao = DirecaoDoWaypoint();
                waypoint.Coordenada = new Coordenada() {
                    X = coordenada.X + direcao.X,
                    Y = coordenada.Y + direcao.Y,
                    Z = coordenada.Z
                };
            }
            radioButtonC.IsChecked = true;
            return waypoint;
        }
        private void ButtonStand_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var waypoint = CriaWaypoint(Cavebot.Waypoints.Count() + 1, EnumWaypoints.Stand, new Range() { X=1, Y=1}, EnumAction.Empty, "");
                Cavebot.AddWaypoint(waypoint);
                GridView.ItemsSource = null;
                GridView.ItemsSource = Cavebot.Waypoints;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonAction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var waypoint = CriaWaypoint(Cavebot.Waypoints.Count() + 1, EnumWaypoints.Action, new Range() { X = 1, Y = 1 }, EnumAction.Empty, "");
                Cavebot.AddWaypoint(waypoint);
                GridView.ItemsSource = null;
                GridView.ItemsSource = Cavebot.Waypoints;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
        #region Direção do waypoint
        private Point DirecaoDoWaypoint()
        {
            if (radioButtonC.IsChecked == true)
            {
                return new Point(0,0);
            }else if (radioButtonNW.IsChecked == true)
            {
                return new Point(-1, -1);
            }
            else if (radioButtonN.IsChecked == true)
            {
                return new Point(0, -1);
            }
            else if (radioButtonNE.IsChecked == true)
            {
                return new Point(1, -1);
            }
            else if (radioButtonW.IsChecked == true)
            {
                return new Point(-1, 0);
            }
            else if (radioButtonE.IsChecked == true)
            {
                return new Point(1, 0);
            }
            else if (radioButtonSW.IsChecked == true)
            {
                return new Point(-1, 1);
            }
            else if (radioButtonS.IsChecked == true)
            {
                return new Point(0, 1);
            }
            else if (radioButtonSE.IsChecked == true)
            {
                return new Point(1, 1);
            }
            return new Point(0,0);
        }
        #endregion
        #region ListView Waypoints
        private void DataGridItem_MouseClickEvent(object sender, MouseButtonEventArgs e)
        {
            Cavebot.Index = ((Waypoint)((DataGridRow)sender).Item).Index - 1;
            ((DataGridRow)sender).IsSelected = true;
        }

        private void DeletaWaypoint(object sender, RoutedEventArgs e)
        {
            try {

                var num = GridView.SelectedIndex;
                GridView.ItemsSource = null;
                if (Cavebot.Waypoints.Count != 0 && num >= 0)
                {
                    this.GridView.Items.Clear();
                    Cavebot.Waypoints.RemoveAt(num);
                    Cavebot.AtualizaIndex();
                    if (Cavebot.Waypoints.Count > 0)
                    {//Delete the selected line of the listview
                        GridView.ItemsSource = Cavebot.Waypoints;
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
}

        private void LimpaListaDeWaypoints(object sender, RoutedEventArgs e)
        {
            try
            {
                GridView.ItemsSource = null;
                Cavebot.Waypoints.Clear();
                this.GridView.Items.Clear();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ButtonUpListItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Cavebot.Waypoints.Count > 0)
                {
                    var index = (int)((Waypoint)GridView.SelectedItem).Index - 1;
                    if (index > 0 && GridView.SelectedItem != null)
                    {
                        GridView.ItemsSource = null;
                        var element1 = Cavebot.Waypoints.ElementAt(index);
                        var element2 = Cavebot.Waypoints.ElementAt(index - 1);
                        Cavebot.Waypoints[index] = element2;
                        Cavebot.Waypoints[index - 1] = element1;
                        Cavebot.AtualizaIndex();
                        GridView.ItemsSource = Cavebot.Waypoints;
                        GridView.UpdateLayout();
                        ((DataGridRow)GridView.ItemContainerGenerator.ContainerFromIndex(index-1)).IsSelected = true;
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ButtonDownListItem_Click(object sender, RoutedEventArgs e)
        {
            try { 
                if (Cavebot.Waypoints.Count > 0 && GridView.SelectedItem != null)
                {
                    var index = (int)((Waypoint)GridView.SelectedItem).Index - 1;
                    if (index < Cavebot.Waypoints.Count - 1)
                    {
                        GridView.ItemsSource = null;
                        var element1 = Cavebot.Waypoints.ElementAt(index);
                        var element2 = Cavebot.Waypoints.ElementAt(index + 1);
                        Cavebot.Waypoints[index] = element2;
                        Cavebot.Waypoints[index + 1] = element1;
                        Cavebot.AtualizaIndex();
                        GridView.ItemsSource = Cavebot.Waypoints;
                        GridView.UpdateLayout();
                        ((DataGridRow)GridView.ItemContainerGenerator.ContainerFromIndex(index + 1)).IsSelected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SomeDataMarkGridComboBoxChanged(object sender, SelectionChangedEventArgs e) {
            var index = ((Waypoint)GridView.SelectedItem).Index - 1;
            index = index < 0 ? 0 : index;
            Waypoint waypoint;
            var selectedMark = Enum.TryParse<EnumMarks>(e.AddedItems[0].ToString(), out var mark) ? mark : EnumMarks.Checked;
            var selected = ((Waypoint)GridView.SelectedItem);
            if (selected.Type == EnumWaypoints.Mark && selected.Mark != selectedMark) {
                GridView.ItemsSource = null;
                waypoint = Cavebot.Waypoints.ElementAt(index < 0 ? 0 : index);
                Cavebot.Waypoints[index] = waypoint;
                waypoint.Mark = selectedMark;
                Cavebot.AtualizaIndex();
                GridView.ItemsSource = Cavebot.Waypoints;
                GridView.UpdateLayout();
                ((DataGridRow)GridView.ItemContainerGenerator.ContainerFromIndex(index)).IsSelected = true;
            }
        }

        private void SomeDataGridComboBoxChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = (int)((Waypoint)GridView.SelectedItem).Index - 1;
            Waypoint waypoint;
            if (((Waypoint)GridView.SelectedItem).Type == EnumWaypoints.Action)
            {
                switch (System.Enum.Parse(typeof(EnumAction), e.AddedItems[0].ToString()))
                {
                    case EnumAction.Label:
                        GridView.ItemsSource = null;
                        waypoint = Cavebot.Waypoints.ElementAt(index);
                        waypoint.Parametros = "Digite o nome da Label";
                        Cavebot.Waypoints[index] = waypoint;
                        waypoint.TypeAction = EnumAction.Label;
                        Cavebot.AtualizaIndex();
                        GridView.ItemsSource = Cavebot.Waypoints;
                        GridView.UpdateLayout();
                        ((DataGridRow)GridView.ItemContainerGenerator.ContainerFromIndex(index)).IsSelected = true;
                        break;
                    case EnumAction.Use:
                        GridView.ItemsSource = null;
                        waypoint = Cavebot.Waypoints.ElementAt(index);
                        waypoint.Parametros = "Direção";
                        waypoint.TypeAction = EnumAction.Use;
                        Cavebot.Waypoints[index] = waypoint;
                        Cavebot.AtualizaIndex();
                        GridView.ItemsSource = Cavebot.Waypoints;
                        GridView.UpdateLayout();
                        ((DataGridRow)GridView.ItemContainerGenerator.ContainerFromIndex(index)).IsSelected = true;
                        break;
                    case EnumAction.UseItem:
                        GridView.ItemsSource = null;
                        waypoint = Cavebot.Waypoints.ElementAt(index);
                        waypoint.Parametros = "Key,Direção";
                        waypoint.TypeAction = EnumAction.UseItem;
                        Cavebot.Waypoints[index] = waypoint;
                        Cavebot.AtualizaIndex();
                        GridView.ItemsSource = Cavebot.Waypoints;
                        GridView.UpdateLayout();
                        ((DataGridRow)GridView.ItemContainerGenerator.ContainerFromIndex(index)).IsSelected = true;
                        break;
                    case EnumAction.Wait:
                        GridView.ItemsSource = null;
                        waypoint = Cavebot.Waypoints.ElementAt(index);
                        waypoint.Parametros = "Tempo em milissegundos";
                        waypoint.TypeAction = EnumAction.Wait;
                        Cavebot.Waypoints[index] = waypoint;
                        Cavebot.AtualizaIndex();
                        GridView.ItemsSource = Cavebot.Waypoints;
                        GridView.UpdateLayout();
                        ((DataGridRow)GridView.ItemContainerGenerator.ContainerFromIndex(index)).IsSelected = true;
                        break;
                    case EnumAction.TurnTo:
                        GridView.ItemsSource = null;
                        waypoint = Cavebot.Waypoints.ElementAt(index);
                        waypoint.Parametros = "Direção";
                        Cavebot.Waypoints[index] = waypoint;
                        waypoint.TypeAction = EnumAction.TurnTo;
                        Cavebot.AtualizaIndex();
                        GridView.ItemsSource = Cavebot.Waypoints;
                        GridView.UpdateLayout();
                        ((DataGridRow)GridView.ItemContainerGenerator.ContainerFromIndex(index)).IsSelected = true;
                        break;
                    case EnumAction.Say:
                        GridView.ItemsSource = null;
                        waypoint = Cavebot.Waypoints.ElementAt(index);
                        waypoint.Parametros = "Digite o que quiser falar";
                        waypoint.TypeAction = EnumAction.Say;
                        Cavebot.Waypoints[index] = waypoint;
                        Cavebot.AtualizaIndex();
                        GridView.ItemsSource = Cavebot.Waypoints;
                        GridView.UpdateLayout();
                        ((DataGridRow)GridView.ItemContainerGenerator.ContainerFromIndex(index)).IsSelected = true;
                        break;
                }
            }
        }
        #endregion

        #region Tools
        private void AtualizaVariaveisGlobais(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(variaveisGlobais.Text))
                {
                    Global._variaveisGlobais.Clear();
                    var texto = variaveisGlobais.Text;
                    texto = texto.Replace(" ", "");
                    texto = texto.Replace("\r\n", "");
                    var variaveis = texto.Split(';');
                    string[] variavel;
                    foreach (var item in variaveis)
                    {
                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            variavel = item.Split('=');
                            if (!Global._variaveisGlobais.Any(x => x.Chave == variavel[0].ToLower()) && variavel.Length == 2)
                            {
                                Global._variaveisGlobais.Add(new Variavel() {Chave = variavel[0].ToLower(), Valor = variavel[1].ToLower() });
                            }
                            else
                            {
                                MessageBox.Show("Existe alguma inconsistência nas variáveis");
                                Global._variaveisGlobais.Clear();
                                return;
                            }
                        }
                    }
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void MainBP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(e.AddedItems[0].ToString()) && e.AddedItems[0].ToString() != "none")
            {
                Global._backpacks.FirstOrDefault(x => x.Tipo == EnumTipoBackpack.Main).Bp = (EnumBackpacks)e.AddedItems[0];
            }
            else
            {
                MainBP.SelectedIndex = 0;
            }
            SupplyBP.SelectedIndex = 0;
            LootBP.SelectedIndex = 0;
            GoldBP.SelectedIndex = 0;
            AmmoBP.SelectedIndex = 0;
        }
        private void SupplyBP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var mainbp = MainBP.SelectedItem == null ? EnumBackpacks.none : (EnumBackpacks)MainBP.SelectedItem;
            var lootbp = LootBP.SelectedItem == null ? EnumBackpacks.none : (EnumBackpacks)LootBP.SelectedItem;
            var goldbp = GoldBP.SelectedItem == null ? EnumBackpacks.none : (EnumBackpacks)GoldBP.SelectedItem;
            var ammobp = AmmoBP.SelectedItem == null ? EnumBackpacks.none : (EnumBackpacks)AmmoBP.SelectedItem;

            if (!string.IsNullOrWhiteSpace(e.AddedItems[0].ToString()) 
                && e.AddedItems[0].ToString() != "none" 
                && mainbp != EnumBackpacks.none)
            {
                if ( mainbp != (EnumBackpacks)e.AddedItems[0]
                    && lootbp != (EnumBackpacks)e.AddedItems[0]
                    && goldbp != (EnumBackpacks)e.AddedItems[0]
                    && ammobp != (EnumBackpacks)e.AddedItems[0])
                {
                    Global._backpacks.FirstOrDefault(x => x.Tipo == EnumTipoBackpack.Supply).Bp = (EnumBackpacks)e.AddedItems[0];
                }
                else
                {
                    MessageBox.Show("Você não pode repetir as Backpacks!");
                    SupplyBP.SelectedIndex = 0;
                }
            }
            else
            {
                SupplyBP.SelectedIndex = 0;
            }
        }
        private void LootBP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var mainbp = MainBP.SelectedItem == null ? EnumBackpacks.none : (EnumBackpacks)MainBP.SelectedItem;
            var supplybp = SupplyBP.SelectedItem == null ? EnumBackpacks.none : (EnumBackpacks)SupplyBP.SelectedItem;
            var goldbp = GoldBP.SelectedItem == null ? EnumBackpacks.none : (EnumBackpacks)GoldBP.SelectedItem;
            var ammobp = AmmoBP.SelectedItem == null ? EnumBackpacks.none : (EnumBackpacks)AmmoBP.SelectedItem;
            if (!string.IsNullOrWhiteSpace(e.AddedItems[0].ToString())
                && e.AddedItems[0].ToString() != "none"
                && mainbp != EnumBackpacks.none)
            {

                if (supplybp != (EnumBackpacks)e.AddedItems[0]
                    && mainbp != (EnumBackpacks)e.AddedItems[0]
                    && goldbp != (EnumBackpacks)e.AddedItems[0]
                    && ammobp != (EnumBackpacks)e.AddedItems[0])
                {
                    Global._backpacks.FirstOrDefault(x => x.Tipo == EnumTipoBackpack.Loot).Bp = (EnumBackpacks)e.AddedItems[0];
                }
                else
                {
                    MessageBox.Show("Você não pode repetir as Backpacks!");
                    LootBP.SelectedIndex = 0;
                }
            }
            else
            {
                LootBP.SelectedIndex = 0;
            }
        }
        private void GoldBP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var mainbp = MainBP.SelectedItem == null ? EnumBackpacks.none : (EnumBackpacks)MainBP.SelectedItem;
            var supplybp = SupplyBP.SelectedItem == null ? EnumBackpacks.none : (EnumBackpacks)SupplyBP.SelectedItem;
            var lootbp = LootBP.SelectedItem == null ? EnumBackpacks.none : (EnumBackpacks)LootBP.SelectedItem;
            var ammobp = AmmoBP.SelectedItem == null ? EnumBackpacks.none : (EnumBackpacks)AmmoBP.SelectedItem;
            if (!string.IsNullOrWhiteSpace(e.AddedItems[0].ToString())
                && e.AddedItems[0].ToString() != "none"
                && mainbp != EnumBackpacks.none)
            {
                if (mainbp != (EnumBackpacks)e.AddedItems[0]
                    && supplybp != (EnumBackpacks)e.AddedItems[0]
                    && lootbp != (EnumBackpacks)e.AddedItems[0]
                    && ammobp != (EnumBackpacks)e.AddedItems[0])
                {
                    Global._backpacks.FirstOrDefault(x => x.Tipo == EnumTipoBackpack.Gold).Bp = (EnumBackpacks)e.AddedItems[0];
                }
                else
                {
                    MessageBox.Show("Você não pode repetir as Backpacks!");
                    GoldBP.SelectedIndex = 0;
                }
            }
            else
            {
                GoldBP.SelectedIndex = 0;
            }
        }
        private void AmmoBP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var mainbp = MainBP.SelectedItem == null ? EnumBackpacks.none : (EnumBackpacks)MainBP.SelectedItem;
            var supplybp = SupplyBP.SelectedItem == null ? EnumBackpacks.none : (EnumBackpacks)SupplyBP.SelectedItem;
            var lootbp = LootBP.SelectedItem == null ? EnumBackpacks.none : (EnumBackpacks)LootBP.SelectedItem;
            var goldbp = GoldBP.SelectedItem == null ? EnumBackpacks.none : (EnumBackpacks)GoldBP.SelectedItem;
            if (!string.IsNullOrWhiteSpace(e.AddedItems[0].ToString())
                && e.AddedItems[0].ToString() != "none"
                && mainbp != EnumBackpacks.none)
            {
                if (supplybp != (EnumBackpacks)e.AddedItems[0]
                    && mainbp != (EnumBackpacks)e.AddedItems[0]
                    && lootbp != (EnumBackpacks)e.AddedItems[0]
                    && goldbp != (EnumBackpacks)e.AddedItems[0])
                {
                    Global._backpacks.FirstOrDefault(x => x.Tipo == EnumTipoBackpack.Ammo).Bp = (EnumBackpacks)e.AddedItems[0];
                }
                else
                {
                    MessageBox.Show("Você não pode repetir as Backpacks!");
                    AmmoBP.SelectedIndex = 0;
                }

            }
            else
            {
                AmmoBP.SelectedIndex = 0;
            }
        }
        #endregion

        private void OpenBackpacksbutton_Click(object sender, RoutedEventArgs e)
        {
            CavebotAction.InvokeMethod(new Func<bool>(CavebotAction.CloseAllBackpacks));
            CavebotAction.InvokeMethod(new Func<bool>(CavebotAction.OpenAllBackpacks));
        }

        private void SaveDropMoveButton_Click(object sender, RoutedEventArgs e)
        {
            Global._moveLootItens.Clear();
            Global._dropItens.Clear();
            var moveItens = MoveItensTextBox.Text.Replace(" ", "").ToLower().Split(',');
            foreach (var item in moveItens) {
                if (!string.IsNullOrWhiteSpace(item))
                    Global._moveLootItens.Add(item);
            }
            var dropItens = DropItensTextBox.Text.Replace(" ", "").ToLower().Split(',');
            foreach (var item in dropItens)
            {
                if(!string.IsNullOrWhiteSpace(item))
                    Global._dropItens.Add(item);
            }
        }
        #region Food
        private void FoodCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Global._isEatFood = true;
        }
        private void FoodCheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            Global._isEatFood = false;
        }
        private void SetaHotkeyFood(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                Global._foodKey = SetaHotkey(e.Key, FoodHotkey);
            }
            catch (Exception ex)
            {
                Global._vida.HighHeal.Key = "F1";
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        private void SetSpellHotkey(object sender, System.Windows.Input.KeyEventArgs e) {
            switch (((FrameworkElement)sender).Name) {
                case "PrimeiraSpellHotkey":
                    if (Targeting.Combos[0] == null)
                        Targeting.Combos[0] = new Targeting.Combo() { Hotkey = SetaHotkeyNullable(e.Key, PrimeiraSpellHotkey) };
                    else
                        Targeting.Combos[0].Hotkey = SetaHotkeyNullable(e.Key, PrimeiraSpellHotkey);
                    break;
                case "SegundaSpellHotkey":
                    if (Targeting.Combos[1] == null)
                        Targeting.Combos[1] = new Targeting.Combo() { Hotkey = SetaHotkeyNullable(e.Key, SegundaSpellHotkey) };
                    else
                        Targeting.Combos[1].Hotkey = SetaHotkeyNullable(e.Key, SegundaSpellHotkey);
                    break;
                case "TerceiraSpellHotkey":
                    if (Targeting.Combos[2] == null)
                        Targeting.Combos[2] = new Targeting.Combo() { Hotkey = SetaHotkeyNullable(e.Key, TerceiraSpellHotkey) };
                    else
                        Targeting.Combos[2].Hotkey = SetaHotkeyNullable(e.Key, TerceiraSpellHotkey);
                    break;
                case "QuartaSpellHotkey":
                    if (Targeting.Combos[3] == null)
                        Targeting.Combos[3] = new Targeting.Combo() { Hotkey = SetaHotkeyNullable(e.Key, QuartaSpellHotkey) };
                    else
                        Targeting.Combos[3].Hotkey = SetaHotkeyNullable(e.Key, QuartaSpellHotkey);
                    break;
                case "PrimeiraSupportSpellHotkey":
                    if (Targeting.Combos[0] == null)
                        Targeting.Combos[0] = new Targeting.Combo() { Hotkey = SetaHotkeyNullable(e.Key, PrimeiraSupportSpellHotkey) };
                    else
                        Targeting.Combos[0].Hotkey = SetaHotkeyNullable(e.Key, PrimeiraSupportSpellHotkey);
                    break;
                case "SegundaSupportSpellHotkey":
                    if (Targeting.Combos[1] == null)
                        Targeting.Combos[1] = new Targeting.Combo() { Hotkey = SetaHotkeyNullable(e.Key, SegundaSupportSpellHotkey) };
                    else
                        Targeting.Combos[1].Hotkey = SetaHotkeyNullable(e.Key, SegundaSupportSpellHotkey);
                    break;
                case "TerceiraSupportSpellHotkey":
                    if (Targeting.Combos[2] == null)
                        Targeting.Combos[2] = new Targeting.Combo() { Hotkey = SetaHotkeyNullable(e.Key, TerceiraSupportSpellHotkey) };
                    else
                        Targeting.Combos[2].Hotkey = SetaHotkeyNullable(e.Key, TerceiraSupportSpellHotkey);
                    break;
                case "QuartaSupportSpellHotkey":
                    if (Targeting.Combos[3] == null)
                        Targeting.Combos[3] = new Targeting.Combo() { Hotkey = SetaHotkeyNullable(e.Key, QuartaSupportSpellHotkey) };
                    else
                        Targeting.Combos[3].Hotkey = SetaHotkeyNullable(e.Key, QuartaSupportSpellHotkey);
                    break;
            }
        }

        private void SetSpellCooldownTime(object sender, TextChangedEventArgs e) {
            switch (((FrameworkElement)sender).Name) {
                case "PrimeiraSpellCooldownTime":
                    if (Targeting.Combos[0] == null)
                        Targeting.Combos[0] = new Targeting.Combo() { Cooldown = SetaValorDoRange(PrimeiraSpellCooldownTime, 20) };
                    else
                        Targeting.Combos[0].Cooldown = SetaValorDoRange(PrimeiraSpellCooldownTime, 20);
                    break;
                case "SegundaSpellCooldownTime":
                    if (Targeting.Combos[1] == null)
                        Targeting.Combos[1] = new Targeting.Combo() { Cooldown = SetaValorDoRange(SegundaSpellCooldownTime, 20) };
                    else
                        Targeting.Combos[1].Cooldown = SetaValorDoRange(SegundaSpellCooldownTime, 20);
                    break;
                case "TerceiraSpellCooldownTime":
                    if (Targeting.Combos[2] == null)
                        Targeting.Combos[2] = new Targeting.Combo() { Cooldown = SetaValorDoRange(TerceiraSpellCooldownTime, 20) };
                    else
                        Targeting.Combos[2].Cooldown = SetaValorDoRange(TerceiraSpellCooldownTime, 20);
                    break;
                case "QuartaSpellCooldownTime":
                    if (Targeting.Combos[3] == null)
                        Targeting.Combos[3] = new Targeting.Combo() { Cooldown = SetaValorDoRange(QuartaSpellCooldownTime, 20) };
                    else
                        Targeting.Combos[3].Cooldown = SetaValorDoRange(QuartaSpellCooldownTime, 20);
                    break;
                case "PrimeiraSupportSpellCooldownTime":
                    if (Targeting.Combos[0] == null)
                        Targeting.Combos[0] = new Targeting.Combo() { Cooldown = SetaValorDoRange(PrimeiraSupportSpellCooldownTime, 20) };
                    else
                        Targeting.Combos[0].Cooldown = SetaValorDoRange(PrimeiraSupportSpellCooldownTime, 20);
                    break;
                case "SegundaSupportSpellCooldownTime":
                    if (Targeting.Combos[1] == null)
                        Targeting.Combos[1] = new Targeting.Combo() { Cooldown = SetaValorDoRange(SegundaSupportSpellCooldownTime, 20) };
                    else
                        Targeting.Combos[1].Cooldown = SetaValorDoRange(SegundaSupportSpellCooldownTime, 20);
                    break;
                case "TerceiraSupportSpellCooldownTime":
                    if (Targeting.Combos[2] == null)
                        Targeting.Combos[2] = new Targeting.Combo() { Cooldown = SetaValorDoRange(TerceiraSupportSpellCooldownTime, 20) };
                    else
                        Targeting.Combos[2].Cooldown = SetaValorDoRange(TerceiraSupportSpellCooldownTime, 20);
                    break;
                case "QuartaSupportSpellCooldownTime":
                    if (Targeting.Combos[3] == null)
                        Targeting.Combos[3] = new Targeting.Combo() { Cooldown = SetaValorDoRange(QuartaSupportSpellCooldownTime, 20) };
                    else
                        Targeting.Combos[3].Cooldown = SetaValorDoRange(QuartaSupportSpellCooldownTime, 20);
                    break;
            }
        }

        private void SetSpellTempoParaProxima(object sender, TextChangedEventArgs e) {
            switch (((FrameworkElement)sender).Name) {
                case "PrimeiraSpellTempoParaProxima":
                    if (Targeting.Combos[0] == null)
                        Targeting.Combos[0] = new Targeting.Combo() { TimeToNextSpell = SetaValorDoRange(PrimeiraSpellTempoParaProxima, 20) };
                    else
                        Targeting.Combos[0].TimeToNextSpell = SetaValorDoRange(PrimeiraSpellTempoParaProxima, 20);
                    break;
                case "SegundaSpellTempoParaProxima":
                    if (Targeting.Combos[1] == null)
                        Targeting.Combos[1] = new Targeting.Combo() { TimeToNextSpell = SetaValorDoRange(SegundaSpellTempoParaProxima, 20) };
                    else
                        Targeting.Combos[1].TimeToNextSpell = SetaValorDoRange(SegundaSpellTempoParaProxima, 20);
                    break;
                case "TerceiraSpellTempoParaProxima":
                    if (Targeting.Combos[2] == null)
                        Targeting.Combos[2] = new Targeting.Combo() { TimeToNextSpell = SetaValorDoRange(TerceiraSpellTempoParaProxima, 20) };
                    else
                        Targeting.Combos[2].TimeToNextSpell = SetaValorDoRange(TerceiraSpellTempoParaProxima, 20);
                    break;
                case "QuartaSpellTempoParaProxima":
                    if (Targeting.Combos[3] == null)
                        Targeting.Combos[3] = new Targeting.Combo() { TimeToNextSpell = SetaValorDoRange(QuartaSpellTempoParaProxima, 20) };
                    else
                        Targeting.Combos[3].TimeToNextSpell = SetaValorDoRange(QuartaSpellTempoParaProxima, 20);
                    break;
            }
        }
    }
}
