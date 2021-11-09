using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Assignment2
{
    public class Articles
    {
        public string Title { get; set; }
        public DateTime DateTime { get; set; }
        public string WebPage { get; set; }
    }

    public class WebPage
    {
        public string Webpage { get; set; }
        public string URL { get; set; }
    }
    public partial class MainWindow : Window
    {
        private Thickness spacing = new Thickness(5);
        private HttpClient http = new HttpClient();
        private List<XDocument> webbpageContentList = new List<XDocument>();
        private List<WebPage> webPagesList = new List<WebPage>();
        private List<Articles> articlesList = new List<Articles>();
        // We will need these as instance variables to access in event handlers.
        private TextBox addFeedTextBox;
        private Button addFeedButton;
        private ComboBox selectFeedComboBox;
        private Button loadArticlesButton;
        private StackPanel articlePanel;

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            // Window options
            Title = "Feed Reader";
            Width = 800;
            Height = 400;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Scrolling
            var root = new ScrollViewer();
            root.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content = root;

            // Main grid
            var grid = new Grid();
            root.Content = grid;
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var addFeedLabel = new Label
            {
                Content = "Feed URL:",
                Margin = spacing
            };
            grid.Children.Add(addFeedLabel);

            addFeedTextBox = new TextBox
            {
                Margin = spacing,
                Padding = spacing
            };
            grid.Children.Add(addFeedTextBox);
            Grid.SetColumn(addFeedTextBox, 1);

            addFeedButton = new Button
            {
                Content = "Add Feed",
                Margin = spacing,
                Padding = spacing
            };
            grid.Children.Add(addFeedButton);
            Grid.SetColumn(addFeedButton, 2);

            addFeedButton.Click += addFeedButton_ClickAsync;

            var selectFeedLabel = new Label
            {
                Content = "Select Feed:",
                Margin = spacing
            };
            grid.Children.Add(selectFeedLabel);
            Grid.SetRow(selectFeedLabel, 1);

            selectFeedComboBox = new ComboBox
            {
                Margin = spacing,
                Padding = spacing,
                IsEditable = false
            };
            grid.Children.Add(selectFeedComboBox);
            Grid.SetRow(selectFeedComboBox, 1);
            Grid.SetColumn(selectFeedComboBox, 1);

            loadArticlesButton = new Button
            {
                Content = "Load Articles",
                Margin = spacing,
                Padding = spacing,
            };
            grid.Children.Add(loadArticlesButton);
            Grid.SetRow(loadArticlesButton, 1);
            Grid.SetColumn(loadArticlesButton, 2);

            loadArticlesButton.Click += loadArticlesButton_ClickAsync;


            articlePanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = spacing
            };
            grid.Children.Add(articlePanel);
            Grid.SetRow(articlePanel, 2);
            Grid.SetColumnSpan(articlePanel, 3);

            selectFeedComboBox.Items.Add("All feeds");
        }


        //TODO
        //[x] Set so the same webbpage cant be added twice
        //[x] Make addFeedButton_ClickAsync Not clickable when awaiting
        //[x] Make loadArticlesButton_ClickAsync Not clickable when awaiting



        //Takes URL and Sends to LoadDocumentAsync() and finds the Wed sites Titel and url using WebPageTitle()
        public async void addFeedButton_ClickAsync(Object sender, EventArgs e)
        {
            var url = addFeedTextBox.Text;
            try
            {
                addFeedButton.IsEnabled = false;
                var webPageURL = await Task.Run( () => LoadDocumentAsync(url));
                addFeedButton.IsEnabled = true;
                addFeedTextBox.Text = "";   

                IEnumerable<string> webbpageTitle = webPageURL.Descendants("channel")
                    .Select(x => (string)x.Element("title"));

                var title = webbpageTitle.First();

                //Check to prevent duplicates # Not needed!
                var check = webPagesList.Select(w => w).Where(t=>t.Webpage == title);
                if (check.Count() > 0)
                {
                    webPagesList.Remove(check.First());
                    selectFeedComboBox.Items.Remove(title);
                }

                var webPage = new WebPage
                {
                    URL = url,
                    Webpage = title
                };
             
                webPagesList.Add(webPage);
                selectFeedComboBox.Items.Add(title);
            }
            catch
            {
                MessageBox.Show($"{url}\n Not a valid Http");
            }
        }

        //Load Webbpage and saves title and url to objekt WebPage
        public void WebPageTitle(XDocument page)
        {
            IEnumerable<string> webbpageTitle = page.Descendants("channel")
                .Select(x => (string)x.Element("title"));

            IEnumerable<string> webbpageURL = page.Descendants("channel")
                .Select(x => (string)x.Element("link"));

            var webbpage = new WebPage
            {
                URL = webbpageURL.ToString(),
                Webpage = webbpageTitle.ToString()
            };

            webPagesList.Add(webbpage);
            selectFeedComboBox.Items.Add(webbpageTitle.ToString());
        }

        //Load article from selectedFeedComboBox and prints results
        public async void loadArticlesButton_ClickAsync(Object sender, EventArgs e)
        {
            loadArticlesButton.IsEnabled = false;
            var activArticlesList = new List <Articles>();
            //Select All Feeds
            if (selectFeedComboBox.Text == "All feeds")
            {
                var articleOrded = articlesList.Select(a => a).OrderBy(a => a.DateTime);
                var pageURL = webPagesList.Select(p => p.URL);
                var valu = pageURL.Select(LoadDocumentAsync);
                var page = await Task.WhenAll(valu);

                foreach (var p in page)
                {
                    var list = WebbpageContent(p);
                    foreach(var l in list)
                    {
                        activArticlesList.Add(l);
                    }
                }
            }
            //Select One Feed
            else
            {
                var articleOrded = articlesList.Select(a => a).OrderBy(a => a.DateTime);
                var pageURL = webPagesList.Select(p => p).Where(p => p.Webpage == selectFeedComboBox.Text).First();
                var page = await LoadDocumentAsync(pageURL.URL.ToString());

                var list = WebbpageContent(page);
                foreach(var l in list)
                {
                    activArticlesList.Add(l);
                }
            }

            articlePanel.Children.Clear();

            var orderdArticalList = activArticlesList.OrderBy(a => a.DateTime).ToArray();

            for (int i = 0; i < activArticlesList.Count(); i++)
            {
                var articlePlaceholder = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = spacing
                };
                articlePanel.Children.Add(articlePlaceholder);

                var articleTitle = new TextBlock
                {
                    Text = $"{orderdArticalList[i].DateTime} - {orderdArticalList[i].Title}",
                    FontWeight = FontWeights.Bold,
                    TextTrimming = TextTrimming.CharacterEllipsis
                };
                articlePlaceholder.Children.Add(articleTitle);

                var articleWebsite = new TextBlock
                {
                    Text = $"{orderdArticalList[i].WebPage}"
                };
                articlePlaceholder.Children.Add(articleWebsite);
            }
            loadArticlesButton.IsEnabled = true;
        }

        //Gets selected values and creates Articles objects and returns a list of objects
        public List<Articles> WebbpageContent(XDocument page)
        {
            List<Articles> list = new List<Articles>();

            IEnumerable<string> webbpage = page.Descendants("channel")
                .Select(x => (string)x.Element("title"));

            var webbpageTitle = webbpage.First();

            IEnumerable<string> articals = page.Descendants("item")
                .Select(x => (string)x.Element("title")).Take(5);

            var date = page.Descendants("item")
                .Select(x => (string)x.Element("pubDate")).Take(5);

            for (var i = 0; i < articals.Count(); i++)
            {
                var xdate = date.ElementAt(i).ToString();
                var ydate = Convert.ToDateTime(xdate.Remove(25));

                var artical = new Articles
                {
                    Title = articals.ElementAt(i).ToString(),
                    DateTime = ydate,
                    WebPage = webbpageTitle
                };

                list.Add(artical);
            }

            return list;
        }

        private async Task<XDocument> LoadDocumentAsync(string url)
        {
            // This is just to simulate a slow/large data transfer and make testing easier.
            // Remove it if you want to.
            
            
            await Task.Delay(1000);
            var response = await http.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();
            var feed = XDocument.Load(stream);
            return feed;
            
        }
    }
}
