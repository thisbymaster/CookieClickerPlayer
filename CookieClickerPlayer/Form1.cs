using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace CookieClickerPlayer
{
    public partial class Form1 : Form
    {
        static bool stopstart = false;
        static bool pause = false;
        private Thread thread;
        public static ChromeDriver cd;
        public Form1()
        {
            InitializeComponent();
            new DriverManager().SetUpDriver(new ChromeConfig() {  });
            cd = new ChromeDriver();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            //Thread t;
            if (!stopstart)
            {
                stopstart = true;
                button1.Text = "Stop";
                thread = new Thread(T =>
                {
                    RunCookieClickerThread();
                })
                { IsBackground = true };
                thread.Start();
            }
            else
            {
                stopstart = false;
                Thread.Sleep(1000);
                button1.Text = "Start";
                //t.Abort();
                //cd.Quit();
                //t.Suspend();
                //thread.Abort();
                
            }
        }

        static async Task RunCookieClickerThread()
        {
            
            cd.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(1);
            cd.Url = "https://orteil.dashnet.org/cookieclicker/";
            cd.Manage().Window.Maximize();
            Thread.Sleep(4000);
            try
            {
                
                cd.FindElementById("langSelect-EN").Click();
                //load saved data.
                if (System.IO.File.Exists(System.IO.Directory.GetCurrentDirectory() + "\\CookieSaveGame.txt"))
                {
                    Thread.Sleep(2000);
                    cd.FindElementById("prefsButton").Click();
                    Thread.Sleep(2000);
                    //cd.FindElementByLinkText("Import save");
                    cd.ExecuteScript("Game.ImportSave();");
                    Thread.Sleep(2000);
                    cd.FindElementById("textareaPrompt").SendKeys(System.IO.File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + "\\CookieSaveGame.txt"));
                    Thread.Sleep(2000);
                    //cd.FindElementById("promptOption0").Click();
                    cd.ExecuteScript("Game.ImportSaveCode(l('textareaPrompt').value);Game.ClosePrompt();");
                    //cd.FindElementByCssSelector("div[class='X").Click();
                    cd.ExecuteScript("Game.ShowMenu();");
                }
                while (stopstart)
                {//Check for upgrades
                    try {
                        var activeupgrade = cd.FindElementById("upgrades").FindElements(By.CssSelector("div[class='crate upgrade enabled']"));
                        var activeBuildings = cd.FindElementsByCssSelector("div[class='product unlocked enabled']");
                        var inactiveBuildings = cd.FindElementsByCssSelector("div[class='product unlocked disabled']");
                        var activeShimmers = cd.FindElementsByClassName("shimmer");
                        var CanCastgrimoire = cd.FindElementById("grimoireSpell1").GetAttribute("class") == "grimoireSpell titleFont ready";

                        if (activeShimmers.Count > 0)
                        {
                            try
                            {
                                activeShimmers.First().Click();
                            }
                            catch (Exception ex)
                            {
                                //if selecting shimmer failed.
                            }
                        }
                        else if (activeupgrade.Count > 0)
                        {
                            try
                            {
                                activeupgrade.First().Click();
                            }
                            catch (Exception) { }

                        }
                        else if (activeBuildings.Count() > 0 && inactiveBuildings.Count() <= 2)// && activeBuildings.Any(T => T.GetAttribute("id") == "product" + FindBestBuilding(cd, activeBuildings.ToList()).ToString()))//Check for building able to be bought
                        {
                            try
                            {
                                //activeBuildings.First(T => T.GetAttribute("id") == "product" + FindBestBuilding(cd, activeBuildings.ToList()).ToString()).Click();
                                activeBuildings.Last().Click();
                            }
                            catch (Exception) { }
                        }
                        else if(CanCastgrimoire)
                        {
                            try
                            {
                                cd.FindElementById("grimoireSpell1").Click();
                            }
                            catch (Exception) { }
                        }
                        else
                        {
                            try {
                                cd.FindElementById("bigCookie").Click(); //Default to clicking the big cookie.
                            }
                            catch (Exception) { }
                        }
                        while (pause)
                        {
                            
                                Thread.Sleep(500);
                        }
                    } catch (Exception ex)
                    {
                        //if selecting shimmer failed.
                    }
                }
                 
                cd.FindElementById("prefsButton").Click();
                    //cd.FindElementByLinkText("Export save");
                    Thread.Sleep(500);
                    cd.ExecuteScript("Game.ExportSave()");
                    Thread.Sleep(500);
                    string savedata = cd.FindElementById("textareaPrompt").Text;
                    cd.Quit();
                    System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + "\\CookieSaveGame.txt", savedata);
                
        }
            catch( Exception ex)
            {
                cd.FindElementById("prefsButton").Click();
                //cd.FindElementByLinkText("Export save");
                Thread.Sleep(500);
                cd.ExecuteScript("Game.ExportSave()");
                Thread.Sleep(500);
                string savedata = cd.FindElementById("textareaPrompt").Text;
                cd.Quit();
                System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + "\\CookieSaveGame.txt", savedata);
                MessageBox.Show(ex.Message + " " + ex.InnerException);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pause = !pause;
            if (button2.Text == "Pause")
            {
                button2.Text = "UnPause";
            }else if(button2.Text == "UnPause")
            {
                button2.Text = "Pause";
            }
        }
        private static int FindBestBuilding(ChromeDriver cd,List<IWebElement> activeBuildings )
        {
            List<Building> buildings = new List<Building>();
            //for(int x=0;x <= 16;x++)
            foreach(var b in activeBuildings)
            {
                
                try
                {
                    int x = int.Parse(b.GetAttribute("id").Remove(0, 7));
                    buildings.Add(new Building(x, cd.ExecuteScript("return Game.ObjectsById[" + x.ToString() + "].tooltip()").ToString()));
                }
                catch (Exception ex)
                {
                    //if selecting shimmer failed.
                }
            }
            buildings.RemoveAll(T => T.percent == 0 || T.percent < 1);
            if (buildings.Count > 0)
            {
                buildings.OrderByDescending(T => T.Weight);
                return buildings.Last().index;
            }
            else
            {
                return 0;
            }
        }
        
    }

}
