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

namespace CookieClickerPlayer
{
    public partial class Form1 : Form
    {
        static bool stopstart = false;
        static bool pause = false;
        private Thread thread;
        public Form1()
        {
            InitializeComponent();
            
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            Thread t;
            if (!stopstart)
            {
                stopstart = true;
                button1.Text = "Stop";
                t = new Thread(T =>
                {
                    RunCookieClickerThread();
                })
                { IsBackground = true };
                t.Start();
            }
            else
            {
                stopstart = false;
                button1.Text = "Start";
                //t.Abort();
            }
        }

        static async Task RunCookieClickerThread()
        {
            ChromeDriver cd = new ChromeDriver();
            cd.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(1);
            cd.Url = "https://orteil.dashnet.org/cookieclicker/";
            try
            {
                //load saved data.
                if (System.IO.File.Exists(System.IO.Directory.GetCurrentDirectory() + "\\CookieSaveGame.txt"))
                {
                    Thread.Sleep(2000);
                    cd.FindElementById("prefsButton").Click();
                    //cd.FindElementByLinkText("Import save");
                    cd.ExecuteScript("Game.ImportSave();");
                    cd.FindElementById("textareaPrompt").SendKeys(System.IO.File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + "\\CookieSaveGame.txt"));
                    //cd.FindElementById("promptOption0").Click();
                    cd.ExecuteScript("Game.ImportSaveCode(l('textareaPrompt').value);Game.ClosePrompt();");
                    //cd.FindElementByCssSelector("div[class='X").Click();
                    cd.ExecuteScript("Game.ShowMenu();");
                }
                while (stopstart)
                {//Check for upgrades
                    var activeupgrade = cd.FindElementsByCssSelector("div[class='crate upgrade enabled']");
                    var activeBuildings = cd.FindElementsByCssSelector("div[class='product unlocked enabled']");
                    var activeShimmers = cd.FindElementsByClassName("shimmer");
                    
                    if(activeShimmers.Count > 0)
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
                    else if (activeBuildings.Count > 0)//Check for building able to be bought
                    {
                        try
                        {
                            activeBuildings.First().Click();
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
                    while(pause)
                    {
                        Thread.Sleep(500);
                    }
                }
                cd.FindElementById("prefsButton").Click();
                //cd.FindElementByLinkText("Export save");
                cd.ExecuteScript("Game.ExportSave()");
                string savedata = cd.FindElementById("textareaPrompt").Text;
                cd.Quit();
                System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + "\\CookieSaveGame.txt", savedata);
                
            }
            catch( Exception ex)
            {
                cd.FindElementById("prefsButton").Click();
                //cd.FindElementByLinkText("Export save");
                cd.ExecuteScript("Game.ExportSave()");
                string savedata = cd.FindElementById("textareaPrompt").Text;
                cd.Quit();
                System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + "\\CookieSaveGame.txt", savedata);
                MessageBox.Show(ex.Message + " " + ex.InnerException);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pause = !pause;
        }
    }
}
