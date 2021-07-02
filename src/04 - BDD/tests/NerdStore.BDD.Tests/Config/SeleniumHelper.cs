using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace NerdStore.BDD.Tests.Config
{
    public class SeleniumHelper:IDisposable
    {
        public IWebDriver WebDriver;
        public readonly ConfigurationHelper Configuration;
        public WebDriverWait Wait;

        public SeleniumHelper(Browser browser, ConfigurationHelper configuration, bool headless = true)
        {
            Configuration = configuration;
            WebDriver = WebDriverFactory.CreateWebDriver(browser, Configuration.WebDrivers, headless);
            WebDriver.Manage().Window.Maximize();
            Wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(30));
        }

        public string ObterUrl()
        {
            return WebDriver.Url;
        }

        public void IrParaUrl(string url)
        {
            WebDriver.Navigate().GoToUrl(url);
        }

        public void ClicarLinkPorTexto(string linkText)
        {
           WebDriver.FindElement(By.LinkText(linkText))?.Click();

           var link = Wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText(linkText)));
           link.Click();
        }


        public void Dispose()
        {
            WebDriver?.Dispose();
        }
    }
}